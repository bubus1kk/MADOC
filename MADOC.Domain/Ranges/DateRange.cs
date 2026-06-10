namespace MADOC.Domain.Ranges
{
    public class DateRange
    {
        public DateOnly From { get; set; }
        public DateOnly To { get; set; }

        public DateRange() { }

        public DateRange(DateOnly from, DateOnly to)
        {
            this.From = from;
            this.To = to;
        }
    }
}
