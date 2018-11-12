using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_365.ATM.Transactions
{
    public class SendTransaction : Transaction
    {
        private int _amount;
        private UserProxy.AtmUser _receiver;

        public SendTransaction(string user, int amount,  UserProxy.AtmUser receiver) : base(user, TransactionType.Send)
        {
            _amount = amount;
            _receiver = receiver;
        }

        public override bool Do()
        {
            return true;
        }
    }
}
