using System.Collections.Generic;
using System.Reflection;
using Bank_365.ATM.Transactions;

namespace Bank_365.ATM
{
  public class TransactionController
  {


    private List<Transaction> _transactions;

    public TransactionController()
    {
      _transactions = new List<Transaction>();
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


    public void CreateNewTransaction(string user, int amount, UserProxy.AtmUser receiver)
    {
       _transactions.Add(new SendTransaction(user, amount, receiver));

    }

    public void CreateNewTransaction(string user, UserProxy.CreditInfo creditInfo)
    {
      _transactions.Add(new CreditTransaction(user, creditInfo));
    }

    public void CreateNewTransaction(string user, int amount)
    {
      _transactions.Add(new GetTransaction(user, amount));
    }


  }
}

