using System;
using System.Collections.Generic;
using Bank_365.ATM.Transactions;
using Newtonsoft.Json;

namespace Bank_365.ATM
{
  [JsonObject(MemberSerialization.Fields)]
  public class UserProxy
  {
    private AtmUser _user;

    public override string ToString()
    {
      return GetUserInfo();
    }

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

    public void AddMoney(double amount)
    {
      _user.AddMoney(amount);
    }

    public void SetCreditLimit(double limit)
    {
      _user.SetCreditLimit(limit);
    }

    public string GetPassword()
    {
      return _user.GetPassword();
    }

    public string GetCardNumber()
    {
      return _user.GetCardNumber();
    }

    public void SetBlockedStatus(bool blocked)
    {
      _user.SetBlockedStatus(blocked);
    }

    public bool GetBlockedStatus()
    {
      return _user.GetBlockedStatus();
    }

    public double GetMoneyAmount()
    {
      return _user.GetMoneyAmount();
    }



    [JsonObject(MemberSerialization.Fields)]
    public class AtmUser
    {
      private string _cardNumber;

      private string _cardPassword;

      private double _creditLimit;

      private double _money;

      private bool _blocked = false;

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

      internal string GetPassword()
      {
        return _cardPassword;
      }

      internal string GetCardNumber()
      {
        return _cardNumber;
      }

      internal void SetBlockedStatus(bool blocked)
      {
        _blocked = blocked;
      }

      internal bool GetBlockedStatus()
      {
        return _blocked;
      }

      internal double GetMoneyAmount()
      {
        return _money;
      }

      internal void AddMoney(double amount)
      {
        _money += amount;
      }
    }

    public struct CreditInfo
    {
      public double Amount;

      // Time in number of month
      public int Time;
      public double Percent;
    }

    internal string GetUserInfo()
    {
      string info = "";
      info += "Card number: " + GetCardNumber() + "\n";
      info += "Password: " + GetPassword() + "\n";
      info += "Money: " + GetMoneyAmount() + "\n";
      info += "Is blocked: " + GetBlockedStatus() + "\n"; s
      info += "Credit limit: " + CreditLimit() + "\n";
      return info;
    }
  }
}