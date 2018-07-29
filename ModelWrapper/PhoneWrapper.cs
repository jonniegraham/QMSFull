using Model;

namespace ModelWrapper
{
    public class PhoneWrapper : ModelWrapper<Phone>
    {
        public PhoneWrapper(Phone model) : base(model) { }

        #region Simple Properties
        public int Id
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public string WorkPhone
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string CellPhone
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string AfterHours
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        #endregion

        #region Change Tracking
        public bool IdIsChanged => GetIsChanged(nameof(Id));
        public bool WorkPhoneIsChanged => GetIsChanged(nameof(WorkPhone));
        public bool CellPhoneIsChanged => GetIsChanged(nameof(CellPhone));
        public bool AfterHoursIsChanged => GetIsChanged(nameof(AfterHours));
        #endregion
    }
}
