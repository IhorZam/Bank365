using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_365.ATM.Transactions
{
    class CreditTransaction : Transaction
    {
        private UserProxy.AtmUser _user;
        private UserProxy.CreditInfo _creditInfo;

        public CreditTransaction(UserProxy.AtmUser user, UserProxy.CreditInfo creditInfo)
        {
            _user = user;
            _creditInfo = creditInfo;
        }
    }
}
