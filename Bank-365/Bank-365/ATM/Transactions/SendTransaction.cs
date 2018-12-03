using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Bank_365.ATM.Transactions.ServiceClasses;

namespace Bank_365.ATM.Transactions
{
  public class SendTransaction : Transaction
  {
    private string _user;
    private double _amount;
    private string _receiver;
    private TransactionResultData _result;

    public SendTransaction(string user, int amount, string receiver, out TransactionResultData result) : base(user, TransactionType.Send)
    {
      _user = user;
      _amount = amount;
      _receiver = receiver;
      result = _result;
    }

    public override bool Do()
    {
      var receiver = DataBase.Users[_receiver];
      if (DataBase.Users[_user].WithdrawMoney(_amount))
      {
        receiver.AddMoney(_amount);
        _result = new TransactionResultData(true);
        Console.WriteLine("Money sent to " + receiver.GetCardNumber() + ". Amount: " + _amount);
        return true;
      }
      _result = new TransactionResultData(false, TransactionDeniedReason.NotEnoughMoney);
      Console.WriteLine("Not enough money on card.");
      return false;

    }
  }
}
