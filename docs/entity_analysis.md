# Event Photography System - Entity Model Analysis
## Derived from Design System & Architecture Documents

---

## Base Entity (Abstract)

All entities in the system inherit from a base entity providing core audit fields:

```
BaseEntity
├── Id (UUID v7)
├── CreatedAt (DateTime)
├── CreatedBy (UserId?)
├── ModifiedAt (DateTime)
├── ModifiedBy (UserId?)
├── IsActive (bool)
├── DeletedAt (DateTime?)
```

**Purpose:** Provides consistent audit trail, soft delete capability, and temporal tracking across all entities.

**Key Behaviors:**
- Soft delete via `DeletedAt` (not physical deletion)
- `IsActive` controls general state (independent from deletion)
- UUID v7 provides chronologically sortable IDs

---

## Domain: Identity

### 1. User (Authentication Entity)

**Purpose:** Authentication and authorization only. Does not contain profile data.

**Fields:**
- Id (inherited from BaseEntity)
- Email (unique, case-insensitive)
- PasswordHash (bcrypt/Argon2)
- Role (enum: Admin | Consumer | Studio)
- LastLoginAt (DateTime?)
- EmailVerified (bool)
- EmailVerificationToken (string?)
- PasswordResetToken (string?)
- PasswordResetExpiry (DateTime?)
- TwoFactorEnabled (bool)
- TwoFactorSecret (string?)

**Relationships:**
- One-to-One → ConsumerProfile (if Role = Consumer)
- One-to-One → StudioProfile (if Role = Studio)

**Notes:**
- Role is immutable after creation
- No user switching between Consumer/Studio roles
- Password reset tokens expire after 1 hour
- Tokens are single-use

---

## Domain: Consumer

### 2. ConsumerProfile

**Purpose:** Consumer-specific profile data separate from authentication.

**Fields:**
- Id (inherited)
- UserId (FK to User, unique)
- FirstName (string)
- LastName (string)
- PhoneNumber (string?)
- ProfileImageUrl (string?)
- Bio (string?, 500 chars max)
- PreferredTimezone (string)
- NotificationPreferences (owned entity)

**Relationships:**
- One-to-One ← User
- One-to-Many → Event
- One-to-Many → Inquiry
- One-to-Many → Booking
- One-to-Many → Review

**Notes:**
- Separated from User for clean architecture
- Profile data does not affect authentication
- Can be extended without impacting auth flow

---

## Domain: Studio

### 3. StudioProfile

**Purpose:** Business entity representing photography studio.

**Fields:**
- Id (inherited)
- UserId (FK to User, unique)
- StudioName (string, required, unique)
- Description (string, 100-500 chars)
- ContactEmail (string, required)
- PhoneNumber (string?)
- WebsiteUrl (string?)
- CoverImageUrl (string, required)
- ProfileImageUrl (string, required)
- ServiceRadius (enum: 25km | 50km | 100km | Statewide | Nationwide)
- StartingPrice (decimal?)
- AverageRating (decimal, computed)
- TotalReviews (int, computed)
- TotalCompletedProjects (int, computed)
- IsSuspended (bool)
- SuspensionInfo (owned entity?)

**Relationships:**
- One-to-One ← User
- One-to-One → StudioLocation (owned)
- One-to-Many → StudioTag
- One-to-Many → ServiceOffering
- One-to-Many → Employee
- One-to-Many → PortfolioImage
- One-to-Many → Inquiry
- One-to-Many → Booking
- One-to-Many ← Review
- One-to-Many → SuspensionHistory

**Notes:**
- Studio-centric architecture (solo photographer = studio with 1 employee)
- StudioName immutable after creation
- Suspension is separate from deletion
- Location changes snapshot existing bookings

### 4. StudioLocation (Owned Entity)

**Purpose:** Geographic location of studio for geo-aware discovery.

**Fields:**
- Latitude (decimal, required)
- Longitude (decimal, required)
- DisplayCity (string?) - for UX
- DisplayState (string?) - for UX
- DisplayLabel (string?) - for UX

**Relationship:**
- Owned by StudioProfile

**Notes:**
- Lat/Lng are canonical source of truth
- Display fields are metadata for UI/admin
- Used for distance calculations and geo-filtering

### 5. StudioTag

**Purpose:** Tag association for studio specialization and discovery matching.

**Fields:**
- Id (inherited)
- StudioId (FK)
- TagName (string, lowercase, single-word)
- IsCustomTag (bool)

**Relationships:**
- Many-to-One → StudioProfile

**Notes:**
- Tags are hashtag-style: #candid, #moody, #wedding
- Predefined tags + custom tags allowed
- Minimum 2 tags, maximum 5 recommended
- Used for tag-based discovery

### 6. ServiceOffering

**Purpose:** Services provided by studio (Photography, Video, Drone, etc.)

**Fields:**
- Id (inherited)
- StudioId (FK)
- ServiceType (string) - e.g., "Photography", "Video", "Drone", "Editing"

**Relationships:**
- Many-to-One → StudioProfile

**Notes:**
- Checkboxes on profile edit screen
- Studios can offer multiple services

### 7. Employee

**Purpose:** Internal studio team members (no platform authentication in MVP).

**Fields:**
- Id (inherited)
- StudioId (FK)
- Name (string, required)
- Email (string, required, unique within studio)
- PhoneNumber (string?)
- Role (enum: Owner | Employee)
- Specialty (string?) - e.g., "Main photographer", "Second shooter", "Drone operator", "Editor"

**Relationships:**
- Many-to-One → StudioProfile
- Many-to-Many → Booking (via EmployeeAssignment)

**Notes:**
- Employees are internal-only (no login in MVP)
- Work under Studio brand
- Owner role cannot be deleted
- Employees can be assigned to multiple projects
- At least 1 employee required per studio

### 8. PortfolioImage

**Purpose:** Studio's public marketplace portfolio images.

**Fields:**
- Id (inherited)
- StudioId (FK)
- ImageUrl (string, required)
- Title (string?, 100 chars max)
- Category (string?) - e.g., "Wedding", "Candid", "Venue"
- IsFeatured (bool)
- DisplayOrder (int)

**Relationships:**
- Many-to-One → StudioProfile

**Notes:**
- Max 50 images in MVP (extensible)
- JPEG/PNG only, max 10MB per image
- Featured images appear first on profile
- Reorderable via DisplayOrder

---

## Domain: Events

### 9. Event

**Purpose:** Consumer-created occasion that drives studio discovery and booking.

**Fields:**
- Id (inherited)
- ConsumerId (FK to ConsumerProfile)
- Title (string, required, 100 chars max)
- Category (enum or string) - Weddings, Birthdays, Corporate, etc.
- CustomCategory (string?) - if Category = "Other"
- EventDate (DateTime, required, future dates only)
- CoverageDuration (string) - e.g., "4 hours", "8 hours", "Full Day"
- Budget (decimal, required, minimum 1000 INR)
- Status (enum: Draft | Searching | InquirySent | Booked | InProgress | Completed | Cancelled)

**Relationships:**
- Many-to-One → ConsumerProfile
- One-to-One → EventLocation (owned)
- One-to-Many → EventTag
- One-to-Many → Inquiry
- One-to-One → Booking (optional, only if booked)

**Notes:**
- Event creation is mandatory before studio discovery
- Event location is geographic anchor for discovery
- Status transitions: Draft → Searching → InquirySent → Booked → InProgress → Completed
- Cannot be deleted after booking

### 10. EventLocation (Owned Entity)

**Purpose:** Event location for geo-aware studio discovery.

**Fields:**
- Latitude (decimal, required)
- Longitude (decimal, required)
- FormattedAddress (string)
- City (string?)
- State (string?)

**Relationship:**
- Owned by Event

**Notes:**
- Lat/Lng are canonical
- Used for distance-based studio filtering
- Must be within India (validated)

### 11. EventTag

**Purpose:** Consumer's photography style preferences for studio matching.

**Fields:**
- Id (inherited)
- EventId (FK)
- TagName (string, lowercase, single-word)
- IsCustomTag (bool)

**Relationships:**
- Many-to-One → Event

**Notes:**
- Minimum 1 tag, maximum 5 recommended (up to 10 allowed)
- Predefined + custom tags
- Used for tag-based studio matching
- Custom tags stored in DB for future reuse

---

## Domain: Bookings

### 12. Inquiry

**Purpose:** Consumer's request to studio for event coverage.

**Fields:**
- Id (inherited)
- EventId (FK to Event)
- ConsumerId (FK to ConsumerProfile)
- StudioId (FK to StudioProfile)
- MessageText (string) - consumer's inquiry message
- SpecialRequirements (string?)
- PreferredShots (string?)
- Status (enum: Pending | Reviewed | Accepted | Declined | Cancelled)
- StudioResponse (string?)
- DeclineReason (string?)
- RespondedAt (DateTime?)

**Relationships:**
- Many-to-One → Event
- Many-to-One → ConsumerProfile
- Many-to-One → StudioProfile
- One-to-One → Booking (if accepted)

**Notes:**
- Consumer can send inquiries to multiple studios
- Status: Pending (no response) → Reviewed (viewed) → Accepted/Declined
- Studio can decline with optional reason
- Accepted inquiry triggers booking workflow (payment required)
- Once event is booked, other inquiries auto-cancelled

### 13. Booking

**Purpose:** Confirmed engagement between consumer and studio.

**Fields:**
- Id (inherited)
- InquiryId (FK to Inquiry, unique)
- EventId (FK to Event, unique)
- ConsumerId (FK to ConsumerProfile)
- StudioId (FK to StudioProfile)
- Status (enum: Confirmed | InProgress | Delivered | Completed | Cancelled)
- TotalAmount (decimal)
- AdvanceAmount (decimal)
- PaymentStatus (enum: Pending | AdvancePaid | FullyPaid | Refunded)
- BookingConfirmedAt (DateTime)
- ExpectedDeliveryDate (DateTime?)
- ActualDeliveryDate (DateTime?)

**Relationships:**
- One-to-One ← Inquiry
- One-to-One ← Event
- Many-to-One → ConsumerProfile
- Many-to-One → StudioProfile
- One-to-One → ProjectWorkspace
- One-to-Many → EmployeeAssignment
- One-to-Many → Payment

**Notes:**
- Advance payment required for confirmation
- UPI-only in MVP (Razorpay)
- One booking per event
- Status transitions: Confirmed → InProgress → Delivered → Completed
- Cannot be cancelled after event date (studio penalty/refund logic TBD)

### 14. EmployeeAssignment

**Purpose:** Assigns studio employees to specific bookings/projects.

**Fields:**
- Id (inherited)
- BookingId (FK)
- EmployeeId (FK)
- AssignedRole (string?) - e.g., "Main photographer", "Second shooter"
- AssignedAt (DateTime)

**Relationships:**
- Many-to-One → Booking
- Many-to-One → Employee

**Notes:**
- Multiple employees can be assigned to one booking
- One employee can work on multiple bookings
- AssignedRole is optional free-text

---

## Domain: Payments

### 15. Payment

**Purpose:** Payment transaction record.

**Fields:**
- Id (inherited)
- BookingId (FK)
- Amount (decimal)
- PaymentMethod (enum: UPI)
- PaymentProvider (enum: Razorpay)
- TransactionId (string, external provider ID)
- Status (enum: Initiated | Success | Failed | Refunded)
- InitiatedAt (DateTime)
- CompletedAt (DateTime?)

**Relationships:**
- Many-to-One → Booking

**Notes:**
- UPI-only in MVP
- Razorpay integration
- Advance payment required for booking confirmation
- Multiple payments possible (advance + balance)

---

## Domain: Workspace

### 16. ProjectWorkspace

**Purpose:** Collaborative workspace for booked event workflow.

**Fields:**
- Id (inherited)
- BookingId (FK to Booking, unique)
- Status (enum: Active | Closed | Archived)
- CreatedAt (inherited)
- ClosedAt (DateTime?)

**Relationships:**
- One-to-One ← Booking
- One-to-Many → Gallery
- One-to-Many → WorkspaceComment

**Notes:**
- Created automatically after booking confirmation
- Shared access between consumer and studio
- Remains accessible after completion (archived, read-only)

### 17. Gallery

**Purpose:** Container for event photos with approval workflow.

**Fields:**
- Id (inherited)
- WorkspaceId (FK)
- GalleryName (string, required, 3-50 chars)
- Description (string?)
- Status (enum: Draft | ReadyForReview | UnderReview | ChangesRequested | Approved | Delivered)
- ImageCount (int, computed)
- SubmittedForReviewAt (DateTime?)
- ApprovedAt (DateTime?)
- DeliveredAt (DateTime?)

**Relationships:**
- Many-to-One → ProjectWorkspace
- One-to-Many → GalleryImage
- One-to-Many → GalleryComment

**Notes:**
- Multiple galleries per workspace
- Approval workflow: Draft → ReadyForReview → UnderReview → Approved/ChangesRequested
- Studio marks ReadyForReview → Consumer Approves/Requests Changes
- External delivery model (Google Drive, Dropbox, OneDrive)
- Platform does NOT host media in MVP

### 18. GalleryImage (Reference Entity)

**Purpose:** Reference to externally-hosted image with metadata.

**Fields:**
- Id (inherited)
- GalleryId (FK)
- ExternalUrl (string, required) - link to external provider
- ThumbnailUrl (string?)
- Title (string?)
- DisplayOrder (int)
- IsFavorite (bool) - consumer-only marking
- UploadedAt (DateTime)

**Relationships:**
- Many-to-One → Gallery
- One-to-Many → ImageComment

**Notes:**
- Platform stores metadata only, not the actual image
- ExternalUrl points to Google Drive/Dropbox/OneDrive
- Favorites visible to consumer only (not studio)
- Reorderable via DisplayOrder

### 19. WorkspaceComment

**Purpose:** Workflow-related comments in workspace.

**Fields:**
- Id (inherited)
- WorkspaceId (FK)
- AuthorId (FK to User)
- CommentText (string, required)
- CommentedAt (DateTime)

**Relationships:**
- Many-to-One → ProjectWorkspace
- Many-to-One → User

**Notes:**
- Workflow comments, not general chat
- Both consumer and studio can comment
- No realtime chat in MVP

### 20. GalleryComment

**Purpose:** Comments specific to a gallery.

**Fields:**
- Id (inherited)
- GalleryId (FK)
- AuthorId (FK to User)
- CommentText (string, required)
- CommentedAt (DateTime)

**Relationships:**
- Many-to-One → Gallery
- Many-to-One → User

**Notes:**
- Gallery-level comments (e.g., change requests)

### 21. ImageComment

**Purpose:** Comments specific to individual images.

**Fields:**
- Id (inherited)
- ImageId (FK to GalleryImage)
- AuthorId (FK to User)
- CommentText (string, required)
- CommentedAt (DateTime)

**Relationships:**
- Many-to-One → GalleryImage
- Many-to-One → User

**Notes:**
- Image-specific feedback

---

## Domain: Media (External Delivery)

### 22. ExternalMediaProvider

**Purpose:** Configuration for external media storage (Google Drive, Dropbox, OneDrive).

**Fields:**
- Id (inherited)
- StudioId (FK)
- ProviderType (enum: GoogleDrive | Dropbox | OneDrive | Custom)
- AccessToken (string, encrypted)
- RefreshToken (string, encrypted)
- TokenExpiry (DateTime?)
- IsConnected (bool)

**Relationships:**
- Many-to-One → StudioProfile

**Notes:**
- Studios connect external providers
- OAuth-based connection
- Platform does not host media in MVP
- Tokens encrypted at rest

### 23. DeliveryLink

**Purpose:** External delivery link for final gallery delivery.

**Fields:**
- Id (inherited)
- GalleryId (FK)
- ExternalUrl (string, required)
- DeliveryMethod (enum: GoogleDrive | Dropbox | OneDrive | DirectDownload | Email | Custom)
- ExpiresAt (DateTime?)
- AccessCode (string?) - optional password protection

**Relationships:**
- Many-to-One → Gallery

**Notes:**
- Studio provides external link after approval
- Platform orchestrates workflow, not storage
- Link can expire (optional)

---

## Domain: Reviews

### 24. Review

**Purpose:** Consumer feedback on completed studio booking.

**Fields:**
- Id (inherited)
- BookingId (FK to Booking, unique)
- ConsumerId (FK to ConsumerProfile)
- StudioId (FK to StudioProfile)
- Rating (decimal, 1.0-5.0, 0.5 increments)
- ReviewText (string, 50-500 chars)
- IsVerified (bool) - always true in MVP (only verified consumers can review)
- IsVisible (bool) - admin can hide reviews
- SubmittedAt (DateTime)

**Relationships:**
- One-to-One ← Booking
- Many-to-One → ConsumerProfile
- Many-to-One → StudioProfile

**Notes:**
- Only verified, completed bookings can be reviewed
- One review per booking
- Rating: 1-5 stars (0.5 increments)
- Review text: 50-500 characters
- Only latest 5 reviews shown on profile (no pagination in MVP)
- Sorted by most recent only (no sorting/filtering)
- Studio profile shows average rating (computed)

---

## Domain: Suspension

### 25. SuspensionInfo (Owned Entity)

**Purpose:** Suspension state for Studio or Consumer (separate from deletion).

**Fields:**
- SuspendedAt (DateTime)
- SuspensionReason (string)
- SuspendedBy (FK to User) - Admin who suspended
- AutoLiftAt (DateTime?) - optional auto-lift time

**Relationship:**
- Owned by StudioProfile OR ConsumerProfile

**Notes:**
- Suspension ≠ Deletion
- Separate mechanism for account restrictions
- Hangfire-powered auto-lift
- Only on Studio and Consumer profiles (not on User)

### 26. SuspensionHistory

**Purpose:** Audit trail for suspension actions.

**Fields:**
- Id (inherited)
- TargetId (FK to StudioProfile OR ConsumerProfile)
- TargetType (enum: Studio | Consumer)
- Action (enum: Suspended | Lifted)
- Reason (string)
- ActionedBy (FK to User)
- ActionedAt (DateTime)

**Relationships:**
- Many-to-One → StudioProfile OR ConsumerProfile (polymorphic)
- Many-to-One → User (admin who performed action)

**Notes:**
- Audit table for all suspension actions
- Tracks who, when, why for compliance

---

## Domain: Admin

### 27. AdminAction

**Purpose:** Audit log for admin operations.

**Fields:**
- Id (inherited)
- AdminId (FK to User)
- Action (string) - e.g., "SuspendedStudio", "DeletedUser", "HidReview"
- TargetType (string) - e.g., "Studio", "Consumer", "Review"
- TargetId (UUID)
- Reason (string?)
- ActionedAt (DateTime)

**Relationships:**
- Many-to-One → User (admin)

**Notes:**
- Audit trail for all admin actions
- Compliance and accountability

---

## Additional Supporting Entities

### 28. NotificationPreferences (Owned Entity)

**Purpose:** User's notification preferences for email alerts.

**Fields:**
- InquiryNotifications (bool)
- InquiryFrequency (enum: Instant | Daily | Weekly)
- BookingNotifications (bool)
- BookingFrequency (enum: Instant | Daily | Weekly)
- GalleryApprovalNotifications (bool)
- PaymentNotifications (bool)
- SystemNotifications (bool)

**Relationship:**
- Owned by ConsumerProfile OR StudioProfile

**Notes:**
- Granular control over notification types and frequency
- Email-only in MVP (no in-app push notifications)

---

## Summary of Entity Count

**Total Core Entities: 28**

**By Domain:**
- Identity: 1 (User)
- Consumer: 1 (ConsumerProfile)
- Studio: 8 (StudioProfile, StudioLocation, StudioTag, ServiceOffering, Employee, PortfolioImage, ExternalMediaProvider, DeliveryLink)
- Events: 3 (Event, EventLocation, EventTag)
- Bookings: 3 (Inquiry, Booking, EmployeeAssignment)
- Payments: 1 (Payment)
- Workspace: 6 (ProjectWorkspace, Gallery, GalleryImage, WorkspaceComment, GalleryComment, ImageComment)
- Media: 0 (covered in Studio and Workspace domains)
- Reviews: 1 (Review)
- Suspension: 2 (SuspensionInfo, SuspensionHistory)
- Admin: 1 (AdminAction)
- Supporting: 1 (NotificationPreferences)

---

## Key Architectural Patterns

1. **Separation of Auth and Profile**: User (auth) vs ConsumerProfile/StudioProfile (data)
2. **Owned Entities**: Location, SuspensionInfo, NotificationPreferences (no separate IDs)
3. **Soft Delete**: All entities inherit DeletedAt from BaseEntity
4. **Suspension ≠ Deletion**: Separate SuspensionInfo entity
5. **External Media**: Platform orchestrates, doesn't host (Google Drive, Dropbox, OneDrive)
6. **Tag-Based Discovery**: StudioTag and EventTag for matching
7. **Geo-Aware Discovery**: Lat/Lng on StudioLocation and EventLocation
8. **Workflow-Centric**: Gallery approval workflow (Draft → ReadyForReview → Approved)
9. **Single Booking Per Event**: One-to-One relationship
10. **Verified Reviews Only**: Only completed bookings can be reviewed

---

## Entity Relationship Highlights

**User → Profile (1:1):**
- User (auth) → ConsumerProfile OR StudioProfile (data)
- Role determines which profile exists

**Event → Studio Discovery:**
- Event (with EventLocation, EventTags) → drives geo-aware + tag-based studio matching
- Event → Inquiry (many studios) → Booking (one studio)

**Booking → Workspace:**
- Booking → ProjectWorkspace → Gallery → GalleryImage (external references)
- Workflow: Upload → Review → Approve → Deliver

**Studio → Employees:**
- StudioProfile → Employee (many) → EmployeeAssignment → Booking
- No employee authentication in MVP

**External Media:**
- Platform stores metadata only
- GalleryImage.ExternalUrl → Google Drive/Dropbox/OneDrive
- DeliveryLink → final gallery delivery

---

## Notes for Implementation

1. **UUID v7 for all IDs** (chronologically sortable)
2. **Soft delete on all entities** (DeletedAt nullable)
3. **EF Core Global Query Filters** for IsActive and DeletedAt
4. **IsPubliclyVisible Specification**: IsActive = true AND DeletedAt = null AND IsSuspended = false
5. **Hangfire** for auto-lifting suspensions
6. **Clean Architecture**: Domain entities in single project with subdomain folders
7. **Discriminated Values**: Explicit per-entity fields in MVP (defer abstraction until rule of three)
8. **Rule of Three Before Abstracting**: Wait for pattern to recur 3x before creating shared abstractions
