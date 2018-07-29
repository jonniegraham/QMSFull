using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System;
using DataAccess;
using Model;
using Products.Model;
using QMSStyles.Control;
using TakeoffWrapper;
using Utilities;

namespace Products.ViewModel
{
    public class ProductsViewModel : Observable
    {
        public Takeoff Takeoff;

        public ProductsViewModel(Takeoff takeoff)
        {
            Takeoff = takeoff;
            Initialize();
        }

        private void Initialize()
        {
            GetCategories();
            PropertyChanged += ProductsViewModel_PropertyChanged;
        }

        private async void GetCategories()
        {
            await Task.Factory.StartNew(async () =>
            {
                _categoriesOriginal = Categories = await Data.Instance().Products().GetCategories<CategoryImp<ProductImp>, ProductImp>();
            });
        }

        private void ProductsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Equals(e.PropertyName, nameof(SelectedCategory)) && SelectedCategory != null && SelectedCategory.Products == null)
            {
                Task.Factory.StartNew(async () =>
                {
                    if(SelectedCategory.Id > 0)
                        SelectedCategory.Products =
                            await Data.Instance().Products().GetProductsByCategoryIdAsync<ProductImp>(SelectedCategory.Id);
                    else
                        SelectedCategory.Products =
                            await Data.Instance().Products().GetProductsBySearchTermAsync<ProductImp>(ProductSearchTerm);
                });
            }
        }

        #region Collection Properties
        private ObservableCollection<CategoryImp<ProductImp>> _categoriesOriginal;
        private ObservableCollection<CategoryImp<ProductImp>> _categories;
        public ObservableCollection<CategoryImp<ProductImp>> Categories
        {
            get => _categories;
            set
            {
                if (Equals(_categories, value)) return;
                _categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }
        #endregion

        #region Complex Properties
        private CategoryImp<ProductImp> _selectedCategory;
        public CategoryImp<ProductImp> SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (Equals(_selectedCategory, value)) return;
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }
        #endregion

        #region Search Terms
        private string _categorySearchTerm;
        public string CategorySearchTerm
        {
            get => _categorySearchTerm;
            set
            {
                if (Equals(_categorySearchTerm, value)) return;
                _categorySearchTerm = value;
                OnPropertyChanged(nameof(CategorySearchTerm));
            }
        }

        private string _productSearchTerm;
        public string ProductSearchTerm
        {
            get => _productSearchTerm;
            set
            {
                if (Equals(_productSearchTerm, value)) return;
                _productSearchTerm = value;
                OnPropertyChanged(nameof(ProductSearchTerm));
            }
        }
        #endregion

        #region ICommands
        public RelayCommand CategorySearchCommand => new RelayCommand(o =>
        {
            Categories = new ObservableCollection<CategoryImp<ProductImp>>(_categoriesOriginal.Where(c => c.ShortDescr.ToLower().Contains(CategorySearchTerm.ToLower()) || c.LongDescr.ToLower().Contains(CategorySearchTerm.ToLower())));
        }, o => true);

        public RelayCommand ProductSearchCommand => new RelayCommand(o =>
        {
            var newCategory = new CategoryImp<ProductImp>(new Category());
            Categories.Add(newCategory);
            newCategory.ShortDescr = "SEARCH:";
            newCategory.LongDescr = $"\"{ProductSearchTerm}\"";
            SelectedCategory = newCategory;
        }, o => true);

        public RelayCommand ClearSearchCommand => new RelayCommand(o =>
        {
            if (!(o is SearchTextBox searchTextBox)) return;

            if(Equals(searchTextBox.Name, "CategorySearch"))
            {
                Categories = _categoriesOriginal;
                CategorySearchTerm = string.Empty;
                searchTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
            }
            else
            {
                ProductSearchTerm = string.Empty;
                searchTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }, o => true);

        public RelayCommand SelectCommand => new RelayCommand(async o =>
        {
            if (Takeoff == null)
            {
                MessageBox.Show("_takeoff is null");
                return; // remove
            }

            if (!(o is ProductImp product))
            {
                MessageBox.Show("product is null");
                return; // remove
            }

            await Task.Factory.StartNew(async () =>
            {
                if (Takeoff == null)
                    throw new NullReferenceException("Takeoff has not been set.");
                // the '[30]' should not be there.
                Takeoff.Worksheet.Rows.Columns[1].Value = product.Sku;
                await Task.Delay(200);
                Takeoff.Worksheet.Rows.Columns[3].Value = product.Description;
                await Task.Delay(200);
                Takeoff.Worksheet.Rows.Columns[7].Value = product.SUnit;
            });

        }, o => true);
        #endregion
    }
}
