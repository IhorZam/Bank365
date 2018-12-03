
using Bank_365.ATM.Transactions.ServiceClasses;

namespace Bank_365.ATM.Transactions
{
  public abstract class Transaction
  {
    private TransactionType _type;

    private string _userId;

    public TransactionType Type => _type;

    public string UserId
    {
      get { return _userId;  }
      set { _userId = value; }
    }

    public Transaction(string user, TransactionType type)
    {
      _userId = user;
      _type = type;
    }

    public virtual bool Do()
    {
      return true;
    }

  }
}
