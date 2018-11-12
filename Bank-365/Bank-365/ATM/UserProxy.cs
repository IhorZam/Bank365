using System;
using System.Collections.Generic;
using Bank_365.ATM.Transactions;

namespace Bank_365.ATM
{
  public class UserProxy
  {
    private AtmUser _user;

    private TransactionController _transactionController;

    private List<Transaction> _userTransactions;

    public UserProxy(string cardNumber, string cardPassword)
    {
      _user = new AtmUser(cardNumber, cardPassword);

    }

    public void Update()
    {
      foreach (var transaction in _userTransactions)
      {
        if (!transaction.Do())
          throw new TransactionDeniedException();
        if (transaction.Type == TransactionType.Credit)
        {
          CreditTransaction aux = (CreditTransaction)transaction;
          if (aux.CreditPayed)
            _userTransactions.Remove(transaction);
        }
        else
        {
          _userTransactions.Remove(transaction);
        }
      }
    }

    public bool ValidateUser(string password)
    {
      return _user.ValidatePassword(password);
    }

    public void CreateTransaction(TransactionType type, int amount, ref AtmUser receiver)
    {
      ref var userRef = ref _user;
      switch (type)
      {
        case TransactionType.Send:
          _userTransactions.Add(_transactionController.CreateNewTransaction(userRef, amount, receiver));
          break;
        case TransactionType.Get:
          _userTransactions.Add(_transactionController.CreateNewTransaction(userRef, amount));
          break;

      }

    }

    public void GetCredit(CreditInfo creditInfo)
    {
      _userTransactions.Add(_transactionController.CreateNewTransaction(_user, creditInfo));
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


      public void SetCreditLimit(int newLimit)
      {
        _creditLimit = newLimit;
      }

      public bool ValidatePassword(string password)
      {
        if (password == _cardPassword)
          return true;
        return false;
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