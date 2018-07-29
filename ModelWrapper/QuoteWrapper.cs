using System;
using Model;

namespace ModelWrapper
{
    public class QuoteWrapper : ModelWrapper<Quote>
    {
        public QuoteWrapper(Quote model) : base(model)
        {
            InitializeComplexProperties(model);
        }

        #region Initialization
        private void InitializeComplexProperties(Quote quote)
        {
            if (quote.Client == null)
                throw new ArgumentException("Client cannot be null.");

            Client = new ClientWrapper(quote.Client);

            // Change tracking
            RegisterComplexOject(Client);
        }
        #endregion

        #region SimpleProperties
        public int Id
        {
            get => GetValue<int>();
            set => SetValue(value);
        }
        public string FileNumber
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public string UserId
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public DateTime? Date
        {
            get => GetValue<DateTime?>();
            set => SetValue(value);
        }
        public double? Amount
        {
            get => GetValue<double>();
            set => SetValue(value);
        }
        public double? Cost
        {
            get => GetValue<double?>();
            set => SetValue(value);
        }
        #endregion

        #region Complex Properties
        private ClientWrapper _client;
        public ClientWrapper Client
        {
            get => _client;
            set
            {
                if (Equals(_client, value)) return;
                _client = value;
                OnPropertyChanged(nameof(Client));
            }
        }
        #endregion

        #region ChangeTracking
        public bool CostIsChanged => GetIsChanged(nameof(Cost));
        public bool AmountIsChanged => GetIsChanged(nameof(Amount));
        public bool DateIsChanged => GetIsChanged(nameof(Date));
        public bool UserIdIsChanged => GetIsChanged(nameof(UserId));
        public bool FileNumberIsChanged => GetIsChanged(nameof(FileNumber));
        public bool IdNumberIsChanged => GetIsChanged(nameof(Id));
        #endregion
    }
}
