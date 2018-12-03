using System;
using Bank_365.ATM.Transactions.ServiceClasses;

namespace Bank_365.ATM.Transactions
{
  public class GetTransaction : Transaction
  {
    private int _amount;
    private TransactionResultData _result;
    private string _key;

    public GetTransaction(string user, int amount, string key) : base(user, TransactionType.Get)
    {

      if (amount <= 0)
      {
        throw new TransactionDeniedException();
      }
      _amount = amount;
      _key = key;
    }

    public override bool Do()
    {
      if (DataBase.Users[UserId].WithdrawMoney(_amount))
      {
        _result = new TransactionResultData(true, DateTime.Now, _amount, false);
        Console.WriteLine("Money withdrawed. Amount: " + _amount);
        DataBase.Users[UserId].AddTransaction(_key, _result);
        return true;
      }
      _result = new TransactionResultData(false, DateTime.Now, _amount, false, TransactionDeniedReason.NotEnoughMoney);
      Console.WriteLine("Not enough money on card.");
      DataBase.Users[UserId].AddTransaction(_key, _result);
      return false;

    }

  }
}
