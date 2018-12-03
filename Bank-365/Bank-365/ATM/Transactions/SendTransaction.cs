using System;
using Bank_365.ATM.Transactions.ServiceClasses;

namespace Bank_365.ATM.Transactions
{
  public class SendTransaction : Transaction
  {
    private double _amount;
    private string _receiver;
    private TransactionResultData _result;
    private string _key;

    public SendTransaction(string user, double amount, string receiver, string key) : base(user, TransactionType.Send)
    {
      if (amount <= 0)
      {
        throw new TransactionDeniedException();
      }
      _amount = amount;
      _receiver = receiver;
      _key = key;
    }

    public override bool Do()
    {
      var receiver = DataBase.Users[_receiver];
      if (DataBase.Users[UserId].WithdrawMoney(_amount))
      {
        receiver.AddMoney(_amount);
        _result = new TransactionResultData(true);
        Console.WriteLine("Money sent to " + receiver.GetCardNumber() + ". Amount: " + _amount);
        DataBase.Users[UserId].AddTransaction(_key, _result);
        return true;
      }
      _result = new TransactionResultData(false, TransactionDeniedReason.NotEnoughMoney);
      Console.WriteLine("Not enough money on card.");
      DataBase.Users[UserId].AddTransaction(_key, _result);
      return false;


    }
  }
}
