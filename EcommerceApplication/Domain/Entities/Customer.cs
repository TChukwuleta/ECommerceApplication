using EcommerceApplication.Domain.Enums;

namespace EcommerceApplication.Domain.Entities
{
    public class Customer : User
    {
        public int Id { get; set; }
        public UserState State { get; set; }
        public string StateDesc { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; }
    }
}
