using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using Bank_365.ATM.Transactions;
using Bank_365.ATM.Transactions.ServiceClasses;

namespace Bank_365.ATM
{
  public class TransactionController
  {
    private List<Transaction> _getTransactions;
    private List<Transaction> _sendTransactions;
    private List<Transaction> _creditTransactions;

    public Thread _getTransThread;
    public Thread _sendTransThread;
    public Thread _creditTransThread;

    public TransactionController()
    {
      _getTransactions = new List<Transaction>();
      _sendTransactions = new List<Transaction>();
      _creditTransactions = new List<Transaction>();
      
      _getTransThread = new Thread(RunGetThread);
      _sendTransThread = new Thread(RunSendThread);
      _creditTransThread = new Thread(RunCreditThread);
    }

    public void Start()
    {
      _getTransThread.Start();
      _sendTransThread.Start();
      _creditTransThread.Start();
    }

    private void RunGetThread()
    {
      while (true)
      {
        try
        {
          UpdateGetTransactions();
        }
        catch (InvalidOperationException)
        {
          continue;
        }
        catch (NullReferenceException)
        {
          continue;
        }
      }
    }

    private void RunSendThread()
    {
      while (true)
      {
        try
        {
          UpdateSendTransactions();
        }
        catch (InvalidOperationException)
        {
          continue;
        }
        catch (NullReferenceException)
        {
          continue;
        }
      }
    }

    private void RunCreditThread()
    {
      while (true)
      {
        try
        {
          UpdateCreditTransactions();
        }
        catch (InvalidOperationException)
        {
          continue;
        }
        catch (NullReferenceException)
        {
          continue;
        }
        finally
        {
          Thread.Sleep(10000);
        }
      }
    }

    public void UpdateGetTransactions()
    {      
      foreach (var transaction in _getTransactions)
      {
        transaction.Do();
        _getTransactions.Remove(transaction);
        ContextWindow.UpdateDatabaseFile();
      }
    }

    public void UpdateSendTransactions()
    {
      foreach (var transaction in _sendTransactions)
      {
        transaction.Do();
        _sendTransactions.Remove(transaction);
        ContextWindow.UpdateDatabaseFile();
      }
    }

    public void UpdateCreditTransactions()
    {
      foreach (var transaction in _creditTransactions)
      {
        transaction.Do();
        CreditTransaction aux = (CreditTransaction)transaction;
        if (aux.CreditPayed)
        {
          _creditTransactions.Remove(transaction);
        }          
        ContextWindow.UpdateDatabaseFile();        
      }
    }    

    public string CreateNewTransaction(string user, double amount, string receiver)
    {
      string key = user + amount + DataBase.Users[user].GetTransactionHistory().Count;
      _sendTransactions.Add(new SendTransaction(user, amount, receiver, key));
      return key;
    }

    public string CreateNewTransaction(string user, UserProxy.CreditInfo creditInfo)
    {
      string key = user + creditInfo.Amount + DataBase.Users[user].GetTransactionHistory().Count;
      _creditTransactions.Add(new CreditTransaction(user, creditInfo, key));
      return key;
    }

    public string CreateNewTransaction(string user, int amount)
    {
      string key = user + amount + DataBase.Users[user].GetTransactionHistory().Count;
      _getTransactions.Add(new GetTransaction(user, amount, key));
      return key;
    }
  }
}

