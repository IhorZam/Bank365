using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Bank_365.ATM.Transactions
{
  public abstract class Transaction
  {
    private TransactionType _type;

    private UserProxy.AtmUser _user;

    public TransactionType Type => _type;

    public UserProxy.AtmUser User
    {
      get { return _user;  }
      set { _user = value; }
    }

    public Transaction(UserProxy.AtmUser user, TransactionType type)
    {
      _user = user;
      _type = type;
    }

    public virtual bool Do()
    {
      return true;
    }

  }
}
