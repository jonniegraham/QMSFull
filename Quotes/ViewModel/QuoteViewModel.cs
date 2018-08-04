using System.Threading.Tasks;
using System.Windows.Input;
using DataAccess;
using ModelWrapper;
using QMSStyles.Control;
using Quotes.Model;
using TakeoffWrapper;
using Utilities;

namespace Quotes.ViewModel
{
    public class QuotesViewModel : Observable
    {
        private Takeoff takeoff;

        public QuotesViewModel(Takeoff takeoff)
        {
            this.takeoff = takeoff;
            GetAllQuotes();
        }

        private void GetAllQuotes()
        {
            Task.Factory.StartNew(async () =>
            {
                Quotes = await Data.Instance().Quotes().GetQuotesAsync<QuoteImp>("bp");
            });
        }

        #region Simple Properties
        private string _quoteSearchTerm;
        public string QuoteSearchTerm
        {
            get => _quoteSearchTerm;
            set
            {
                if (Equals(_quoteSearchTerm, value)) return;
                _quoteSearchTerm = value;
                OnPropertyChanged(nameof(QuoteSearchTerm));
            }
        }
        #endregion

        #region Collection Properties
        private ChangeTrackingCollection<QuoteImp> _quotes;
        public ChangeTrackingCollection<QuoteImp> Quotes
        {
            get => _quotes;
            set
            {
                if (Equals(_quotes, value)) return;
                _quotes = value;
                OnPropertyChanged(nameof(Quotes));
            }
        }
        #endregion

        #region Complex Properties
        private QuoteImp _selectedQuote;

        public QuoteImp SelectedQuote
        {
            get => _selectedQuote;
            set
            {
                if (Equals(_selectedQuote, value)) return;
                _selectedQuote = value;
                OnPropertyChanged(nameof(SelectedQuote));
            }
        }
        #endregion

        #region ICommands
        public RelayCommand SaveCommand => new RelayCommand(unusedParam =>
        {
            Quotes.AcceptChanges();
        }, unusedParam => CanSave);

        public RelayCommand ResetCommand => new RelayCommand(unusedParam =>
        {
            Quotes.RejectChanges();
        }, unusedParam => CanReset);

        public RelayCommand ResetSearchCommand => new RelayCommand(quoteSearchTextBox =>
        {
            (quoteSearchTextBox as SearchTextBox)?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            QuoteSearchTerm = string.Empty;
            GetAllQuotes();
        }, o => CanCancelSearch);

        public RelayCommand QuoteSearchCommand => new RelayCommand(o =>
        {
            Task.Factory.StartNew(async () =>
            {
                Quotes = await Data.Instance().Quotes().SearchQuotesByKeywordAsync<QuoteImp>(QuoteSearchTerm);
            });
        }, o => CanSearch);

        #endregion

        #region Predicates
        public bool CanSearch => !string.IsNullOrEmpty(QuoteSearchTerm);
        public bool CanCancelSearch => !string.IsNullOrEmpty(QuoteSearchTerm);
        public bool CanSave => Quotes != null && Quotes.IsChanged;
        public bool CanReset => Quotes != null && Quotes.IsChanged;
        #endregion
    }
}
