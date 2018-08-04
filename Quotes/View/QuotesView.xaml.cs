using System.Windows;
using System.Windows.Input;
using Quotes.ViewModel;
using TakeoffWrapper;

namespace Quotes.View
{
    /// <inheritdoc>
    ///     <cref></cref>
    /// </inheritdoc>
    /// <summary>
    /// Interaction logic for QuotesManager2.xaml
    /// </summary>
    public partial class QuotesView
    {
        public QuotesView(ref Takeoff takeoff)
        {
            DataContext = new QuotesViewModel(takeoff);
            InitializeComponent();
        }

        private void MoveFocus(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(ClientName);
        }
    }
}
