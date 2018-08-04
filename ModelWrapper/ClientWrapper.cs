using System;
using Model;

namespace ModelWrapper
{
    public class ClientWrapper : ModelWrapper<Client>
    {
        public ClientWrapper(Client client) : base(client)
        {
            InitializeComplexProperties(client);
        }

        #region Initialization
        private void InitializeComplexProperties(Client client)
        {
            if (client.Discount == null)
                throw new ArgumentException("Discount cannot be null.");
            if (client.Contact == null)
                throw new ArgumentException("Contact cannot be null.");

            Discount = new DiscountWrapper(client.Discount);
            Contact = new ContactWrapper(client.Contact);

            // Change tracking
            RegisterComplexOject(Discount);
            RegisterComplexOject(Contact);
        }
        #endregion

        #region Simple Properties
        public int Id
        {
            get => GetValue<int>();
            set => SetValue(value);
        }
        public string Name
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        #endregion

        #region Complex Properties
        public DiscountWrapper _discount;
        public DiscountWrapper Discount
        {
            get => _discount;
            set
            {
                if (Equals(_discount, value)) return;
                _discount = value;
                OnPropertyChanged(nameof(Discount));
            }
        }

        private ContactWrapper _contact;
        public ContactWrapper Contact
        {
            get
            {
                return _contact;
            }
            set
            {
                if (Equals(_contact, value)) return;
                _contact = value;
                OnPropertyChanged(nameof(Contact));
            }
        }
        #endregion

        #region Change Tracking
        public bool IdIsChanged => GetIsChanged(nameof(Id));
        public bool NameIsChanged => GetIsChanged(nameof(Name));
        #endregion
    }
}
