using System;
using Model;

namespace ModelWrapper
{
    public class ProductWapper : ModelWrapper<Product>
    {
        public ProductWapper(Product model) : base(model) {}

        #region Simple Properties
        public int Id
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public string Sku
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string Description
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string SUnit
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public double Retail
        {
            get => GetValue<double>();
            set => SetValue(value);
        }

        public double? Discount
        {
            get => GetValue<double?>();
            set => SetValue(value);
        }

        public double? Cost
        {
            get => GetValue<double?>();
            set => SetValue(value);
        }

        public DateTime? Date
        {
            get => GetValue<DateTime?>();
            set => SetValue(value);
        }

        public string MUnit
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public double MattFactor
        {
            get => GetValue<double>();
            set => SetValue(value);
        }

        public double? Waste
        {
            get => GetValue<double?>();
            set => SetValue(value);
        }

        public double? Cover
        {
            get => GetValue<double?>();
            set => SetValue(value);
        }

        public string Notes
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public double? LRate
        {
            get => GetValue<double?>();
            set => SetValue(value);
        }

        public double? RoundFactor
        {
            get => GetValue<double?>();
            set => SetValue(value);
        }

        public double? Retail16
        {
            get => GetValue<double?>();
            set => SetValue(value);
        }

        public int CategoryId
        {
            get => GetValue<int>();
            set => SetValue(value);
        }
        #endregion

        #region Change Tracking
        public bool IdIsChanged => GetIsChanged(nameof(Id));
        public bool SkuIsChanged => GetIsChanged(nameof(Sku));
        public bool DescriptionIsChanged => GetIsChanged(nameof(Description));
        public bool SUnitIsChanged => GetIsChanged(nameof(SUnit));
        public bool RetailIsChanged => GetIsChanged(nameof(Retail));
        public bool DiscountIsChanged => GetIsChanged(nameof(Discount));
        public bool CostIsChanged => GetIsChanged(nameof(Cost));
        public bool DateIsChanged => GetIsChanged(nameof(Date));
        public bool MUnitIsChanged => GetIsChanged(nameof(MUnit));
        public bool MattFactorIsChanged => GetIsChanged(nameof(MattFactor));
        public bool WasteIsChanged => GetIsChanged(nameof(Waste));
        public bool CoverIsChanged => GetIsChanged(nameof(Cover));
        public bool NotesIsChanged => GetIsChanged(nameof(Notes));
        public bool LRateIsChanged => GetIsChanged(nameof(LRate));
        public bool RoundFactorIsChanged => GetIsChanged(nameof(RoundFactor));
        public bool Retail16IsChanged => GetIsChanged(nameof(Retail16));
        public bool CategoryIdIsChanged => GetIsChanged(nameof(CategoryId));
        #endregion
    }
}
