using EcommerceApplication.Domain.Enums;

namespace EcommerceApplication.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } // First name + last name from application user
        public string Phone { get; set; }
        public string Address { get; set; }
        public UserState State { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; }
    }
}
