using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bank_365.ATM.Transactions;
using Bank_365.ATM.Transactions.ServiceClasses;

namespace Bank_365.ATM
{
  public class TransactionController
  {
    private List<Transaction> _transactions;

    public Thread thread;

    public TransactionController()
    {
      _transactions = new List<Transaction>();  
      thread = new Thread(RunThread);
    }

    private void RunThread()
    {
      while (true)
      {
        try
        {
          Update();
        }
        catch (InvalidOperationException)
        {
          continue;
        }
      }
    }

    public void Update()
    {      
      foreach (var transaction in _transactions)
      {
        transaction.Do();
        if (transaction.Type == TransactionType.Credit)
        {
          CreditTransaction aux = (CreditTransaction) transaction;
          if (aux.CreditPayed)
            _transactions.Remove(transaction);
        }
        else
        {
          _transactions.Remove(transaction);
        }
      }
    }


    public void CreateNewTransaction(string user, int amount, string receiver, out TransactionResultData result)
    {
       _transactions.Add(new SendTransaction(user, amount, receiver, out result));
    }

    public void CreateNewTransaction(string user, UserProxy.CreditInfo creditInfo)
    {
      _transactions.Add(new CreditTransaction(user, creditInfo));
    }

    public void CreateNewTransaction(string user, int amount, out TransactionResultData result)
    {
      _transactions.Add(new GetTransaction(user, amount, out result));      
    }
  }
}

