namespace EcommerceApplication.Domain.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public int ItemDataId { get; set; }
        public int Amount { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
