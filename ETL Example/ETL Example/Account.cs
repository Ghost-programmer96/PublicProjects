using System;

namespace ETL_Example
{
    public class Account
    {
        public String AccountNumber { get; set; }
        public Int32 InternalSystemId { get; set; }
        public String PatientFirstName { get; set; }
        public String PatientLastName { get; set; }
        public String GuarantorFirstName { get; set; }
        public String GuarantorLastName { get; set; }
        public Decimal InitialBalance { get; set; }
        public Decimal Adjustments { get; set; }
        public Decimal Charges { get; set; }

        public Account() { }

        public object[] ToArray()
        {
            return new object[]
            {
                AccountNumber,
                InternalSystemId,
                PatientFirstName,
                PatientLastName,
                GuarantorFirstName,
                GuarantorLastName,
                InitialBalance,
                Adjustments,
                Charges
            };
        }
    }
}
