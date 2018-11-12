using System;
using System.Collections.Generic;
using Bank_365.ATM.Transactions;

namespace Bank_365.ATM
{
  public class UserProxy
  {
    private AtmUser _user;

    public double CreditLimit()
    {
      return _user.CreditLimit;
    }
    
    public UserProxy(string cardNumber, string cardPassword)
    {
      _user = new AtmUser(cardNumber, cardPassword);

    }

    public bool WithdrawMoney(double amount)
    {
      return _user.WithdrawMoney(amount);
    }

    public void SetCreditLimit(double limit)
    {
      _user.SetCreditLimit(limit);
    }

    public class AtmUser
    {
      private string _cardNumber;

      private string _cardPassword;

      private double _creditLimit;

      private double _money;

      public double CreditLimit => _creditLimit;

      public AtmUser(string cardNumber, string cardPassword)
      {
        _money = 0;
        _cardPassword = cardPassword;
        _cardNumber = cardNumber;
        _creditLimit = 0;
      }

      internal bool WithdrawMoney(double amount)
      {
        if (_money - amount >= 0)
        {
          _money -= amount;
          return true;
        }

        return false;
      }


      public void SetCreditLimit(double newLimit)
      {
        _creditLimit = newLimit;
      }
      
    }

    public struct CreditInfo
    {
      public double Amount;
      // Time in number of month
      public int Time;
      public double Percent;
    }
  }


}