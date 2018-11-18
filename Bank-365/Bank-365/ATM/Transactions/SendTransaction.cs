using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_365.ATM.Transactions
{
  public class SendTransaction : Transaction
  {
    private string _user;
    private int _amount;
    private UserProxy.AtmUser _receiver;

    public SendTransaction(string user, int amount, UserProxy.AtmUser receiver) : base(user, TransactionType.Send)
    {
      _user = user;
      _amount = amount;
      _receiver = receiver;
    }

    public override bool Do()
    {
      if (DataBase.Users[_user].WithdrawMoney(_amount))
      {
        _receiver.AddMoney(_amount);
        Console.WriteLine("Money sent to " + _receiver.GetCardNumber() + ". Amount: " + _amount);
      }
      else
      {
        Console.WriteLine("Not enough money on card.");
        return false;
      }
      return true;
    }
  }
}
