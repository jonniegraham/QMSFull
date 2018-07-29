using System.Windows;
using System.Windows.Input;

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
        public QuotesView()
        {
            InitializeComponent();
        }

        private void MoveFocus(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(ClientName);
        }
    }
}
