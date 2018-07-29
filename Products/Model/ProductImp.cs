using System.Runtime.InteropServices;
using Model;
using ModelWrapper;

namespace Products.Model
{
    [ComVisible(false)]
    public class ProductImp : ProductWapper
    {
        public ProductImp(Product model) : base(model) { }
    }
}
