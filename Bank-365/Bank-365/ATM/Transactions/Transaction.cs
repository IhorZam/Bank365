using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_365.ATM.Transactions
{
    public abstract class Transaction
    {
        public TransactionType Type { get; internal set; }

        public virtual bool Do()
        {
            return true;
        } 

    }
}
