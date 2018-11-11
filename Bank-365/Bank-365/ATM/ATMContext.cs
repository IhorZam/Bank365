using System.Collections.Generic;

namespace Bank_365.ATM
{
    public class AtmContext
    {
        private UserProxy _currentUser;

        private Dictionary<string, UserProxy> _users;

        private string _dictPath = "C:\\Users\\admin\\Dropbox\\CPP\\Bank365-master\\Bank-365\\Bank-365\\DictInfo.xml";

        public UserProxy CurrentUser
        {
            get { return _currentUser; }
            set { _currentUser = value; }
        }

        public static void Main(string[] args)
        {
        }

        public void Initialize()
        {

        }
    }
}
