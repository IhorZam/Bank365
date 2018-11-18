using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_365.ATM.Transactions
{
  public class GetTransaction : Transaction
  {
    private string _user;
    private int _amount;

    public GetTransaction(string user, int amount) : base(user, TransactionType.Get)
    {
      _user = user;
      _amount = amount;
    }

    public override bool Do()
    {
      if(DataBase.Users[_user].WithdrawMoney(_amount))
        Console.WriteLine("Money withdrawed. Amount: " + _amount);
      else
      {
        Console.WriteLine("Not enough money on card.");
        return false;
      }
      return true;
    }

  }
}
