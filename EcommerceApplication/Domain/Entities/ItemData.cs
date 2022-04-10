namespace EcommerceApplication.Domain.Entities
{
    public class ItemData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public bool AddToCart { get; set; } = false;
    }
}
