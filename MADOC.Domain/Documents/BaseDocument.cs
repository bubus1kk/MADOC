namespace MADOC.Domain.Documents
{
    public abstract class BaseDocument
    {
        public Guid Id { get; protected set; }
        public DateTime CreationDate {  get; protected set; }

        protected BaseDocument()
        {
            this.Id = Guid.NewGuid();
            this.CreationDate = DateTime.Now;
        }
    }
}
