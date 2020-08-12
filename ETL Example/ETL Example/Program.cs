using System;

namespace ETL_Example
{
    public class Program
    {
        static void Main(String[] args)
        {
            AccountCreator app = new AccountCreator();

            int exitCode = app.Run();
            Environment.Exit(exitCode);
        }
    }
}
