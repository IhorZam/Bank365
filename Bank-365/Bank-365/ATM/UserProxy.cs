using System.Collections.Generic;
using Bank_365.ATM.Transactions;

namespace Bank_365.ATM
{
    public class UserProxy
    {
        private AtmUser _user;

        private TransactionController _transactionController;

        private List<Transaction> _userTransactions;

        public UserProxy(string cardNumber, string cardPassword)
        {
            _user = new AtmUser(cardNumber, cardPassword);

        }

        public void Update()
        {
            foreach (var transaction in _userTransactions)
            {
                if (!transaction.Do())
                    throw new TransactionDeniedException();
                if (transaction.Type != TransactionType.Credit)
                {
                    _userTransactions.Remove(transaction);
                }
            }
        }

        public bool ValidateUser(string password)
        {
            return _user.ValidatePassword(password);
        }

        public void CreateTransaction(TransactionType type, int amount)
        {
            _userTransactions.Add(_transactionController.CreateNewTransaction(_user, type, amount));
        }

        public void GetCredit(int creditAmount)
        {
            _userTransactions.Add( _transactionController.CreateNewTransaction(_user, TransactionType.Credit, creditAmount));
        }

        public class AtmUser
        {
            private string _cardNumber;

            private string _cardPassword;

            private int _creditLimit;

            public AtmUser(string cardNumber, string cardPassword)
            {
                _cardPassword = cardPassword;
                _cardNumber = cardNumber;
                _creditLimit = 0;
            }

            public void SetCreditLimit(int newLimit)
            {
                _creditLimit = newLimit;
            }

            public bool ValidatePassword(string password)
            {
                if (password == _cardPassword)
                    return true;
                return false;
            }
            
        }
    }

    
}