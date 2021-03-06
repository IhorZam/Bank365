﻿using System;
using System.Collections.Generic;
using Bank_365.ATM.Transactions;
using Bank_365.ATM.Transactions.ServiceClasses;
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

    public CreditInfo GetCreditInfo()
    {
      return _user.GetCreditInfo();
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

    public int GetPasswordAttempts()
    {
      return _user.GetPasswordAttempts();
    }

    public void SetPasswordAttempts(int i)
    {
      _user.SetPasswordAttempts(i);
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

    public void SetCreditInfo(CreditInfo info)
    {
      _user.SetCreditInfo(info);
    }

    public Dictionary<string, TransactionResultData> GetTransactionHistory()
    {
      return _user.History;
    }


    public void AddTransaction(string key, TransactionResultData data)
    {
      _user.AddTransactionToHistory(key, data);
    }


    [JsonObject(MemberSerialization.Fields)]
    public class AtmUser
    {
      private string _cardNumber;

      private string _cardPassword;

      private int _passwordAttempts;

      private double _creditLimit;

      private double _money;

      private bool _blocked = false;

      private CreditInfo _creditInfo;

      private Dictionary<string, TransactionResultData> _history;

      public double CreditLimit => _creditLimit;

      public Dictionary<string, TransactionResultData> History => _history;

      public AtmUser(string cardNumber, string cardPassword)
      {
        _money = 0;
        _cardPassword = cardPassword;
        _cardNumber = cardNumber;
        _creditLimit = 0;
        _passwordAttempts = 3;
        _history = new Dictionary<string, TransactionResultData>();
        _creditInfo = new CreditInfo()
        {
          Amount = 0,
          Percent = 0,
          Time = 0
        };
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

      public void AddTransactionToHistory(string number, TransactionResultData data)
      {
        _history[number] = data;
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

      internal int GetPasswordAttempts()
      {
        return _passwordAttempts;
      }

      internal void SetPasswordAttempts(int i)
      {
        _passwordAttempts = i;
      }

      internal CreditInfo GetCreditInfo()
      {
        return _creditInfo;
      }

      internal void SetCreditInfo(CreditInfo info)
      {
        _creditInfo = new CreditInfo()
        {
          Amount = info.Amount, Percent = info.Percent, Time = info.Time
        };
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
      string transactionHistory = "";
      int i = 1;
      foreach (var element in GetTransactionHistory())
      {
        transactionHistory += "\n" + element.Value.DateTime + " : Type? " + element.Value.Type + ". Done? " + element.Value.Done + ". Reason? " + element.Value.Reason + ". Change? " +
                             (element.Value.Reached ? "+" : "-") + element.Value.Amount + ". ";
      }
      CreditInfo creditInfo = GetCreditInfo();
      string info = "";
      info += "----------------------------------" + "\n";
      info += "Card number: " + GetCardNumber() + "\n";
      info += "Password: " + GetPassword() + "\n";
      info += "Password attempts left: " + GetPasswordAttempts() + "\n";
      info += "Money: " + GetMoneyAmount() + "\n";
      info += "Is blocked: " + GetBlockedStatus() + "\n";
      info += "Credit info: " + "Amount: " + creditInfo.Amount + ", Percent: " + creditInfo.Percent +
              ", Time: " + creditInfo.Time + "\n";
      info += "Credit limit: " + CreditLimit() + "\n";
      info += "Transaction history: " + transactionHistory + "\n";
      info += "----------------------------------";
      return info;
    }
  }
}