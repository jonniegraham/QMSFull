using Model;

namespace ModelWrapper
{
    public class CategoryWrapper<T> : ModelWrapper<Category> where T : ProductWapper
    {
        public CategoryWrapper(Category model) : base(model) { }

        #region Collection Properties

        private ChangeTrackingCollection<T> _products;
        public ChangeTrackingCollection<T> Products
        {
            get => _products;
            set
            {
                if (Equals(_products, value)) return;
                _products = value;
                OnPropertyChanged(nameof(Products));
            }
        }
        #endregion

        #region Simple Properties
        public int Id
        {
            get => GetValue<int>();
            set => SetValue(value);
        }
        public string ShortDescr
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public string LongDescr
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        #endregion

        #region Complex Properties
        private T _selectedProduct;
        public T SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (Equals(_selectedProduct, value)) return;
                _selectedProduct = value;
                OnPropertyChanged(nameof(SelectedProduct));
            }
        }
        #endregion
    }
}
