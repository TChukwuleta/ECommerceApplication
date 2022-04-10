using EcommerceApplication.Domain.Enums;

namespace EcommerceApplication.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public int ItemDataId { get; set; }
        public int Quantity { get; set; }
        public int AmountToBePaid { get; set; }
        public bool Paid { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DateShipped { get; set; }
        public string UserId { get; set; }
    }
}
