using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank_365.ATM.Transactions.ServiceClasses;

namespace Bank_365.ATM.Transactions
{
  public class CreditTransaction : Transaction
  {
    private UserProxy.CreditInfo _creditInfo;

    private double _amountLeft;

    private int _currentMonthNumeber;

    private int _monthLeft;

    private double _monthPay;

    private bool _creditPayed;

    private string _key;

    public bool CreditPayed => _creditPayed;


    public CreditTransaction(string user, UserProxy.CreditInfo creditInfo, string key) : base(user, TransactionType.Credit)
    {
      if (DataBase.Users[user].CreditLimit() < creditInfo.Amount)
      {
        throw new TransactionDeniedException();
      }
      _creditInfo = creditInfo;
      _amountLeft = creditInfo.Amount * creditInfo.Percent;
      _monthLeft = _creditInfo.Time;
      _monthPay = (_creditInfo.Amount * _creditInfo.Percent) / _creditInfo.Time;
      _key = key;
    }

    public override bool Do()
    {
      int month = DateTime.Today.Month;
      if (month != _currentMonthNumeber)
      {
        _currentMonthNumeber = month;
        if (DataBase.Users[base.UserId].WithdrawMoney(_monthPay))
        {
          _amountLeft -= _monthPay;
          if (--_monthLeft == 0)
          {
            _creditPayed = true;
          }
          DataBase.Users[base.UserId].AddTransaction(_key + _monthLeft, new TransactionResultData(true));
          return true;
        }
      }
      DataBase.Users[base.UserId].AddTransaction(_key + _monthLeft, new TransactionResultData(false, TransactionDeniedReason.NotEnoughMoney));
      return false;
    }
  }
}
