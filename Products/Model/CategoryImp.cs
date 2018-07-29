using System.Runtime.InteropServices;
using Model;
using ModelWrapper;

namespace Products.Model
{

    [ComVisible(false)]
    public class CategoryImp<T> : CategoryWrapper<T> where T : ProductWapper
    {
        public CategoryImp(Category model) : base(model) { }
    }
}
