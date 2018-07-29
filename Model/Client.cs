namespace Model
{
    public class Client : ModelBase
    {
        public string Name
        {
            get; set;
        }

        public Discount Discount { get; set; }
        public Contact Contact { get; set; }
    }
}
