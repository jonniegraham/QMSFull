using Model;

namespace ModelWrapper
{
    public class AddressWrapper : ModelWrapper<Address>
    {
        public AddressWrapper(Address model) : base(model) { }

        #region Simple Properties
        public int Id
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public string Number
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string Street
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string StreetType
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string PostCode
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string Town
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        #endregion

        #region Change Tracking
        public bool IdIsChanged => GetIsChanged(nameof(Id));
        public bool NumberIsChanged => GetIsChanged(nameof(Number));
        public bool StreetIsChanged => GetIsChanged(nameof(Street));
        public bool StreetTypeIsChanged => GetIsChanged(nameof(StreetType));
        public bool PostCodeIsChanged => GetIsChanged(nameof(PostCode));
        public bool TownIsChanged => GetIsChanged(nameof(Town));
        #endregion
    }
}
