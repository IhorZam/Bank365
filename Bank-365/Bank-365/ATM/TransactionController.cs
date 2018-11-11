using System.Reflection;
using Bank_365.ATM.Transactions;

namespace Bank_365.ATM
{
    public class TransactionController
    {



        public Transaction CreateNewTransaction(UserProxy.AtmUser user, TransactionType type, int amount)
        {
            switch (type)
            {
                case TransactionType.Send:
                    return new SendTransaction(user, amount);
                    break;
                case TransactionType.Credit:
                    return new CreditTransaction(user, amount);
                    break;
            }

            return null;
        }

        
    }
}

    