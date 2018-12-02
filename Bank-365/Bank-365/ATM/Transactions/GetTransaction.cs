using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank_365.ATM.Transactions.ServiceClasses;

namespace Bank_365.ATM.Transactions
{
  public class GetTransaction : Transaction
  {
    private string _user;
    private int _amount;
    private TransactionResultData _result;

    public GetTransaction(string user, int amount, out TransactionResultData result) : base(user, TransactionType.Get)
    {
      _user = user;
      _amount = amount;
      result = _result;
    }

    public override bool Do()
    {
      if (DataBase.Users[_user].WithdrawMoney(_amount))
      {
        _result = new TransactionResultData(true);
        Console.WriteLine("Money withdrawed. Amount: " + _amount);
        return true;
      }
      _result = new TransactionResultData(false, TransactionDeniedReason.NotEnoughMoney);
      Console.WriteLine("Not enough money on card.");
      return false;

    }

  }
}
