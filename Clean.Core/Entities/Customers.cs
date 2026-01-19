using System.ComponentModel.DataAnnotations;

namespace Clean.Core.Entities
{
    public class Customers
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int IdOfCustomer { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhonNumber { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Mikud { get; set; }
        public int NumberFlat { get; set; }

       

    }
}
