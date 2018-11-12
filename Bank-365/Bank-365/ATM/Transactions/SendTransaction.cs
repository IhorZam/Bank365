using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_365.ATM.Transactions
{
    public class SendTransaction : Transaction
    {
        private UserProxy.AtmUser _user;
        private int _amount;
        private UserProxy.AtmUser _receiver;

        public SendTransaction(UserProxy.AtmUser user, int amount,  UserProxy.AtmUser receiver)
        {
            _user = user;
            _amount = amount;
            _receiver = receiver;
        }

        public override bool Do()
        {

        }
    }
}
