using System;
using Model;

namespace ModelWrapper
{
    public class ContactWrapper : ModelWrapper<Contact>
    {
        public ContactWrapper(Contact contact) : base(contact)
        {
            InitializeComplexProperties(contact);
        }

        #region Initialization
        private void InitializeComplexProperties(Contact contact)
        {
            if (contact.Phone == null)
                throw new ArgumentException("Phone cannot be null.");
            if (contact.Address == null)
                throw new ArgumentException("Address cannot be null.");

            Phone = new PhoneWrapper(contact.Phone);
            Address = new AddressWrapper(contact.Address);

            // Change tracking
            RegisterComplexOject(Phone);
            RegisterComplexOject(Address);
        }
        #endregion
        #region Simple Properties
        public int Id
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public string FirstName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string LastName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        #endregion

        #region Complex Properties
        private PhoneWrapper _phone;
        public PhoneWrapper Phone
        {
            get
            {
                return _phone;
            }
            set
            {
                if (Equals(_phone, value)) return;
                _phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }

        private AddressWrapper _address;
        public AddressWrapper Address
        {
            get
            {
                return _address;
            }
            set
            {
                if (Equals(_address, value)) return;
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }
        #endregion

        #region Change Tracking
        public bool IdIsChanged => GetIsChanged(nameof(Id));
        public bool FirstNameIsChanged => GetIsChanged(nameof(FirstName));
        public bool LastNameIsChanged => GetIsChanged(nameof(LastName));
        #endregion
    }
}
