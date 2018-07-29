using System;

namespace Model
{
    public class Quote : ModelBase
    {
        public string FileNumber
        {
            get; set;
        }

        public string UserId
        {
            get; set;
        }

        public DateTime? Date
        {
            get; set;
        }

        public double? Amount
        {
            get; set;
        }

        public double? Cost
        {
            get; set;
        }

        public Client Client { get; set; }
    }
}
