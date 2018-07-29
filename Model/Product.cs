using System;

namespace Model
{
    public class Product : ModelBase
    {
        public string Sku
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public string SUnit
        {
            get; set;
        }

        public double Retail
        {
            get; set;
        }

        public double? Discount
        {
            get; set;
        }

        public double? Cost
        {
            get; set;
        }

        public DateTime? Date
        {
            get; set;
        }

        public string MUnit
        {
            get; set;
        }

        public double MattFactor
        {
            get; set;
        }

        public double? Waste
        {
            get; set;
        }

        public double? Cover
        {
            get; set;
        }

        public string Notes
        {
            get; set;
        }

        public double? LRate
        {
            get; set;
        }

        public double? RoundFactor
        {
            get; set;
        }

        public double? Retail16
        {
            get; set;
        }

        public int CategoryId
        {
            get; set;
        }
    }
}
