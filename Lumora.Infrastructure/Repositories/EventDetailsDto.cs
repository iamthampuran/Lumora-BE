namespace Lumora.Infrastructure.Repositories
{
    internal class EventDetailsDto
    {
        private object value1;
        private object category;
        private object eventDate;
        private object locationName;
        private object duration;
        private object budget;
        private object value2;
        private object tags;

        public EventDetailsDto(object value1, object category, object eventDate, object locationName, object duration, object budget, object value2, object tags)
        {
            this.value1 = value1;
            this.category = category;
            this.eventDate = eventDate;
            this.locationName = locationName;
            this.duration = duration;
            this.budget = budget;
            this.value2 = value2;
            this.tags = tags;
        }
    }
}