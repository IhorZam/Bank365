using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bank_365.ATM.Transactions
{
  public class SendTransaction : Transaction
  {
    private string _user;
    private int _amount;
    private string _receiver;
    private bool _result;

    public SendTransaction(string user, int amount, string receiver, out bool result) : base(user, TransactionType.Send)
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
        _result = true;
        Console.WriteLine("Money sent to " + receiver.GetCardNumber() + ". Amount: " + _amount);
      }
      else
      {
        _result = false;
        Console.WriteLine("Not enough money on card.");
        return false;
      }
      return true;
    }
  }
}
