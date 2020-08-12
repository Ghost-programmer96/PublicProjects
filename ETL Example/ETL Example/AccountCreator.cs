using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ETL_Example
{
    public class AccountCreator
    {
        private static readonly String CONNECTION_STRING = "Server=homeshare;Database=ETL;Trusted_Connection=true;";
        private static readonly Random RANDOM = new Random();
        private static String TIMESTAMP
        {
            get
            {
                return DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss");
            }
        }

        public int Run()
        {
            // to keep memory and CPU usage low, keep a hard limit of 1 million accounts at a time
            Int32 numAccountsToGenerate = ReturnRandomInt32(1, 1000000001);
            while (numAccountsToGenerate != 0)
            {
                CreateAndLoadAccounts(ref numAccountsToGenerate, 250000);
            }

            return 0;
        }

        public void CreateAndLoadAccounts(ref Int32 numAccountsToGenerate, Int32 maxAccountsToGenerate)
        {
            using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
            using (SqlCommand cmd = new SqlCommand("", conn))
            using (SqlBulkCopy sbc = new SqlBulkCopy(conn))
            using (DataTable dt = new DataTable())
            {
                dt.Columns.Add("AccountNumber", typeof(String));
                dt.Columns.Add("InternalSystemId", typeof(Int32));
                dt.Columns.Add("PatientFirstName", typeof(String));
                dt.Columns.Add("PatientLastName", typeof(String));
                dt.Columns.Add("GuarantorFirstName", typeof(String));
                dt.Columns.Add("GuarantorLastName", typeof(String));
                dt.Columns.Add("InitialBalance", typeof(Decimal));
                dt.Columns.Add("Adjustments", typeof(Decimal));
                dt.Columns.Add("Charges", typeof(Decimal));

                sbc.BulkCopyTimeout = 60;
                sbc.DestinationTableName = "Accounts";
                sbc.ColumnMappings.Add("AccountNumber", "AccountNumber");
                sbc.ColumnMappings.Add("InternalSystemId", "InternalSystemId");
                sbc.ColumnMappings.Add("PatientFirstName", "PatientFirstName");
                sbc.ColumnMappings.Add("PatientLastName", "PatientLastName");
                sbc.ColumnMappings.Add("GuarantorFirstName", "GuarantorFirstName");
                sbc.ColumnMappings.Add("GuarantorLastName", "GuarantorLastName");
                sbc.ColumnMappings.Add("InitialBalance", "InitialBalance");
                sbc.ColumnMappings.Add("Adjustments", "Adjustments");
                sbc.ColumnMappings.Add("Charges", "Charges");

                Int32 numAccounts = (numAccountsToGenerate > maxAccountsToGenerate ? maxAccountsToGenerate : numAccountsToGenerate);

                LogWriteLine($"Creating {FormatCommas(numAccounts)} accounts...");
                ReturnRandomAccounts(numAccounts)
                    .ForEach(x =>
                    {
                        dt.Rows.Add(x.ToArray());
                    });

                LogWriteLine("Writing the accounts to the server...");
                conn.Open();
                sbc.WriteToServer(dt);
                conn.Close();

                LogWriteLine("Clearing DataTable rows from memory...");
                sbc.Close();
                dt.Dispose();

                numAccountsToGenerate -= numAccounts;

                LogWriteLine($"Accounts remaining: {FormatCommas(numAccountsToGenerate)}");
            }
        }

        public List<Account> ReturnRandomAccounts(Int32 accountsToGenerateCount)
        {
            List<Account> accounts = new List<Account>();

            for (int x = 0; x < accountsToGenerateCount; x++)
            {
                accounts.Add(ReturnRandomAccount());
            }

            return accounts;
        }

        public Account ReturnRandomAccount()
        {
            return new Account()
            {
                AccountNumber = ReturnRandomInt32(100000, 1000001).ToString(),
                InternalSystemId = ReturnRandomInt32(1, 6),
                PatientFirstName = ReturnRandomString(),
                PatientLastName = ReturnRandomString(),
                GuarantorFirstName = ReturnRandomString(),
                GuarantorLastName = ReturnRandomString(),
                InitialBalance = ReturnRandomDecimal(),
                Adjustments = ReturnRandomDecimal(),
                Charges = ReturnRandomDecimal()
            };
        }

        public Int32 ReturnRandomInt32(Int32 exclusiveMax)
        {
            return RANDOM.Next(exclusiveMax);
        }

        public Int32 ReturnRandomInt32(Int32 inclusiveMin, Int32 exclusiveMax)
        {
            return RANDOM.Next(inclusiveMin, exclusiveMax);
        }

        public Decimal ReturnRandomDecimal()
        {
            Int32 integralPartCount = ReturnRandomInt32(1, 7);
            Int32 fractionalPartCount = ReturnRandomInt32(1, 3);

            String sign = (ReturnRandomInt32(2) == 1 ? "-" : String.Empty);
            String integralPart = String.Empty;
            String fractionalPart = String.Empty;

            for (int x = 0; x < integralPartCount; x++)
            {
                integralPart = String.Concat(integralPart, ReturnRandomInt32(1, 10).ToString());
            }
            for (int x = 0; x < fractionalPartCount; x++)
            {
                fractionalPart = String.Concat(fractionalPart, ReturnRandomInt32(1, 10).ToString());
            }

            return Convert.ToDecimal($"{sign}{integralPart}.{fractionalPart}");
        }

        public String ReturnRandomString()
        {
            Int32 charactersInString = ReturnRandomInt32(4, 11);
            String returnString = String.Empty;
            Boolean upperCaseCharacter;

            for (int x = 0; x < charactersInString; x++)
            {
                upperCaseCharacter = (ReturnRandomInt32(2) == 1);

                Int32 asciiCharacterValue = ReturnRandomInt32(65, 91);

                if (!upperCaseCharacter)
                {
                    asciiCharacterValue += 32;
                }

                returnString = String.Concat(returnString, (char)asciiCharacterValue);
            }

            return returnString;
        }

        public String FormatCommas(Int32 value)
        {
            return value.ToString("###,###,###,###");
        }

        public void LogWriteLine(string message)
        {
            Console.WriteLine($"{TIMESTAMP} - {message}");
        }
    }
}
