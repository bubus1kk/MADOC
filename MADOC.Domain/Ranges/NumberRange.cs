namespace MADOC.Domain.Ranges
{
    public class NumberRange
    {
        public decimal From { get; set; }

        public decimal To { get; set; }

        public NumberRange() { }

        public NumberRange(decimal from, decimal to)
        {
            this.From = from;
            this.To = to;
        }
    }
}
