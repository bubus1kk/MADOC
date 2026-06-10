namespace MADOC.Domain.Ranges
{
    public class NumberRange
    {
        public double From { get; set; }

        public double To { get; set; }

        public NumberRange() { }

        public NumberRange(double from, double to)
        {
            this.From = from;
            this.To = to;
        }
    }
}
