namespace Lumora.Infrastructure.Repositories
{
    internal class ConsumerDetailsDto
    {
        private object id;
        private object fullName;
        private object value1;
        private object value2;
        private object priorBookings;
        private string v;

        public ConsumerDetailsDto(object id, object fullName, object value1, object value2, object priorBookings, string v)
        {
            this.id = id;
            this.fullName = fullName;
            this.value1 = value1;
            this.value2 = value2;
            this.priorBookings = priorBookings;
            this.v = v;
        }
    }
}