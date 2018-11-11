﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_365.ATM.Transactions
{
    public class SendTransaction : Transaction
    {
        private UserProxy.AtmUser user;
        private int amount;

        public SendTransaction(UserProxy.AtmUser user, int amount)
        {
            this.user = user;
            this.amount = amount;
        }
    }
}