using System.Reflection;
using Bank_365.ATM.Transactions;

namespace Bank_365.ATM
{
  public class TransactionController
  {



    public Transaction CreateNewTransaction(UserProxy.AtmUser user, int amount, UserProxy.AtmUser receiver)
    {
      return new SendTransaction(user, amount, receiver);

    }

    public Transaction CreateNewTransaction(UserProxy.AtmUser user, UserProxy.CreditInfo creditInfo)
    {
      return new CreditTransaction(user, creditInfo);
    }

    public Transaction CreateNewTransaction(UserProxy.AtmUser user, int amount)
    {
      return new GetTransaction(user, amount);
    }


  }
}

