namespace Model
{
    public class Contact : ModelBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Phone Phone { get; set; }
        public Address Address { get; set; }
    }
}
