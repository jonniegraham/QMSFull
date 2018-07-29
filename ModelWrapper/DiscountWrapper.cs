using Model;

namespace ModelWrapper
{
    public class DiscountWrapper : ModelWrapper<Discount>
    {
        public DiscountWrapper(Discount model) : base(model) { }

        #region Simple Properties
        public int Id
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public string Code
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        #endregion

        #region Change Tracking
        public bool IdIsChanged => GetIsChanged(nameof(Id));
        public bool CodeIsChanged => GetIsChanged(nameof(Code));
        #endregion
    }
}
