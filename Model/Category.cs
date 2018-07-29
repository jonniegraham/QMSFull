using System.Collections.Generic;

namespace Model
{
    public class Category : ModelBase
    {
        public string ShortDescr { get; set; }

        public string LongDescr { get; set; }

        List<Product> Products { get; set; }
    }
}
