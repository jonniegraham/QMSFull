using System.Windows;
using Products.ViewModel;
using TakeoffWrapper;

namespace Products.View
{
    public partial class ProductsView : Window
    {
        public ProductsView(ref Takeoff takeoff)
        {
            DataContext = new ProductsViewModel(takeoff);
            InitializeComponent();
        }

        private void SelectionChanged(object sender, RoutedEventArgs e)
        {
            if(Categories?.SelectedItem != null && !Categories.IsKeyboardFocusWithin)
                Categories.ScrollIntoView(Categories.SelectedItem);
        }
    }
}
