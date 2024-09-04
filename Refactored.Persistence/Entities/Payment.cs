namespace Refactored.Persistence.Entities
{
    public class Payment
    {
        public int Id { get; set; } //Added field

        public decimal Amount { get; set; }
        public string Reference { get; set; } = string.Empty;
    }
}