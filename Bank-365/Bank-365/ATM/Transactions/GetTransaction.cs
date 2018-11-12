using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_365.ATM.Transactions
{
    public class GetTransaction : Transaction
    {
        public GetTransaction(string user, int amount)  : base(user, TransactionType.Get)
        {
        }

    }
}
