namespace MADOC.Domain.Core.Ranges
{
    public class TimeRange
    {
        public TimeOnly From { get; set; }
        public TimeOnly To { get; set; }

        public TimeRange() { }

        public TimeRange(TimeOnly from, TimeOnly to)
        {
            this.From = from;
            this.To = to;
        }
    }
}
