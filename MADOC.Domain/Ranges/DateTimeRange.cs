namespace MADOC.Domain.Ranges
{
    public class DateTimeRange
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public DateTimeRange(){}

        public DateTimeRange(DateTime from, DateTime to)
        {
            this.From = from;
            this.To =to;
        }
    }
}
