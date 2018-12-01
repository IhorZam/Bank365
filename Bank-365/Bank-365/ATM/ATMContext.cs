using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using Console = System.Console;

namespace Bank_365.ATM
{
  public class AtmContext
  {

    private UserProxy _currentUser = null;

    private bool _loggedIn = false;

    private TransactionController _transactionController = new TransactionController();

    private static string _dictPath = "DataBase.txt";


    public UserProxy CurrentUser
    {
      get { return _currentUser; }
      set { _currentUser = value; }
    }

    public static void Main(string[] args)
    {
      
      AtmContext atm = new AtmContext();
      Thread TCThread = atm._transactionController.thread;
      TCThread.Start();

      while (true)
      {
        atm.Initialize();
        DevMenu();

        while (true)
        {
          atm._loggedIn = false;

          while (!atm._loggedIn)
            atm._loggedIn = atm.LoginMenu();

          if (atm.CurrentUser == null)
          {
            break;
          }
          else          
          {
            atm.MainMenu();
          }

          UpdateDatabaseFile();
        }
      }
    }

    public void Initialize()
    {
      DataBase.CreateDict(_dictPath);
    }

    private static void UpdateDatabaseFile()
    {
      File.WriteAllText(_dictPath, JsonConvert.SerializeObject(DataBase.Users));
    }

    private void MainMenu()
    {
      while (true)
      {
        if (!_loggedIn)
          return;

        Console.WriteLine("User menu.");
        Console.WriteLine("What do you want to do?");
        Console.WriteLine("0 - Exit");
        Console.WriteLine("1 - Display amount of money");
        Console.WriteLine("2 - Withdraw money");
        Console.WriteLine("3 - Send money");
        Console.WriteLine("4 - Get credit(not implemented)");
        Console.WriteLine("5 - Pay credit(???)(not implemented)");
        ConsoleKeyInfo choice = Console.ReadKey(true);
        switch (choice.KeyChar)
        {
          case '0':
            _loggedIn = false;
            return;
          case '1':
            DisplayAmountOfMoney();
            RecheckPassword();
            break;
          case '2':
            WithdrawMoney();
            RecheckPassword();
            break;
          case '3':
            SendMoney();
            RecheckPassword();
            break;
          case '4':
            GetCredit();
            RecheckPassword();
            break;
          case '5':
            PayCredit();
            RecheckPassword();
            break;
          default:
            continue;
        }
      }      
    }

    private void RecheckPassword()
    {
      _loggedIn = false;
      string inputCardPassword = null;
      int attempts = 3;
      for (int i = 0; i < attempts; i++)
      {
        Console.WriteLine("Type in your card password: ");
        while (!ValidateInputCardPassword(inputCardPassword))
          inputCardPassword = Console.ReadLine();

        if (!DataBase.ValidateUser(CurrentUser, inputCardPassword))
        {
          if (--attempts == 0)
          {
            DataBase.BlockUser(CurrentUser);
            Console.WriteLine("Wrong password. Your card is blocked.");
            return;
          }

          Console.WriteLine("Wrong password." + attempts + " attempts left.");
          inputCardPassword = null;
        }
        else
        {
          _loggedIn = true;
          return;
        }
      }
    }

    private void PayCredit()
    {
      throw new NotImplementedException();
    }

    private void GetCredit()
    {
      throw new NotImplementedException();
      
    }

    private void SendMoney()
    {
      int amount;
      string receiver;
      while (true)
      {
        try
        {
          Console.WriteLine("Choose amount of money: ");
          amount = int.Parse(Console.ReadLine());
          break;
        }
        catch (Exception e)
        {
          Console.WriteLine("Wrong input. Try again.");
          continue;
        }
      }
      while (true)
      {
        try
        {
          Console.WriteLine("Type in card number of receiver: ");
          receiver = Console.ReadLine();
          if (!ValidateInputCardNumber(receiver))
            continue;          
          break;
        }
        catch (Exception)
        {
          Console.WriteLine("Wrong input. Try again.");
          continue;
        }
      }

      _transactionController.CreateNewTransaction(CurrentUser.GetCardNumber(), amount, receiver, out bool result);      
    }

    private void WithdrawMoney()
    {      
      int amount;
      while (true)
      {
        try
        {
          Console.WriteLine("Choose amount of money: ");
          amount = int.Parse(Console.ReadLine());
          break;
        }
        catch (Exception e)
        {
          Console.WriteLine("Wrong input. Try again.");
          continue;
        }
      }
      _transactionController.CreateNewTransaction(CurrentUser.GetCardNumber(), amount);
    }

    private void GiveBanknotes(int amount)
    {
      throw new NotImplementedException();
    }

    private void DisplayAmountOfMoney()
    {
      Console.Write("Current amount of money on this card: ");
      Console.WriteLine(CurrentUser.GetMoneyAmount());
      Console.WriteLine("(Press any key to continue...)");
      Console.ReadKey(true);
    }

    private bool LoginMenu()
    {
      Console.WriteLine("Login menu.");

      string inputCardNumber = null;
      string inputCardPassword = null;

      while (true)
      {
        Console.WriteLine("Type in your card number (0 to cancel): ");
        while (!ValidateInputCardNumber(inputCardNumber) && inputCardNumber != "0")
          inputCardNumber = Console.ReadLine();

        if (inputCardNumber == "0")
        {
          CurrentUser = null;
          return true;
        }          

        if (DataBase.Users.ContainsKey(inputCardNumber))
          CurrentUser = DataBase.Users[inputCardNumber];

        if (CurrentUser == null)
        {
          Console.WriteLine("Card number doesn't exist. Try again.");
          inputCardNumber = null;
          continue;
        }

        if (CurrentUser.GetBlockedStatus())
        {
          Console.WriteLine("This card is blocked.");
          inputCardNumber = null;
          continue;
        }

        int attempts = 3;
        for (int i = 0; i < attempts; i++)
        {
          Console.WriteLine("Type in your card password: ");
          while (!ValidateInputCardPassword(inputCardPassword))
            inputCardPassword = Console.ReadLine();

          if (!DataBase.ValidateUser(CurrentUser, inputCardPassword))
          {
            if (--attempts == 0)
            {
              DataBase.BlockUser(CurrentUser);
              Console.WriteLine("Wrong password. Your card is blocked.");
              return false;
            }

            Console.WriteLine("Wrong password." + attempts + " attempts left.");
            inputCardPassword = null;
          }
          else
          {
            Console.WriteLine("You have logged in. Welcome.");
            return true;
          }
        }
      }
    }

    private static bool ValidateInputCardPassword(string inputCardPassword)
    {
      if (inputCardPassword == null)
        return false;

      if (inputCardPassword.Length != 4)
      {
        Console.WriteLine("Invalid length. Try again.");
        return false;
      }

      if (!inputCardPassword.All(char.IsDigit))
      {
        Console.WriteLine("Not a number. Try again.");
        return false;
      }

      return true;
    }

    private static bool ValidateInputCardNumber(string inputCardNumber)
    {
      if (inputCardNumber == null || inputCardNumber == "0")
        return false;

      inputCardNumber.Replace(" ", "");
      inputCardNumber.Replace("\t", "");
      if (inputCardNumber.Length != 16)
      {
        Console.WriteLine("Invalid length. Try again.");
        return false;
      }

      if (!inputCardNumber.All(char.IsDigit))
      {
        Console.WriteLine("Not a number. Try again.");
        return false;
      }

      return true;
    }

    #region DevMethods

    private static void DevMenu()
    {
      while (true)
      {
        Console.WriteLine("Developer menu.");
        Console.WriteLine("What do you want to do?");
        Console.WriteLine("0 - Skip and continue");
        Console.WriteLine("1 - Add new ATM User");
        Console.WriteLine("2 - Add money to ATM User");
        Console.WriteLine("3 - View users info");
        Console.WriteLine("4 - Clear DataBase file");
        Console.WriteLine("9 - Exit");
        ConsoleKeyInfo choice = Console.ReadKey(true);
        switch (choice.KeyChar)
        {
          case '0':
            return;
          case '1':
            AddNewATMUser();
            break;
          case '2':
            AddMoneyToATMUser();
            break;
          case '3':
            ViewUsersInfo();
            break;
          case '4':
            ClearDataBaseFile();
            break;
          case '9':
            Environment.Exit(0);
            break;
          default:
            continue;
        }
      }
    }

    private static void AddMoneyToATMUser()
    {
      if (DataBase.Users.Count != 0)
      {
        Console.WriteLine("List of existing cards: ");
        int i = 1;
        foreach (var user in DataBase.Users)
        {
          Console.WriteLine(i + ". " + user.Value.GetCardNumber());
        }
        while (true)
        {
          Console.WriteLine("Choose card. (0 to cancel)");
          int choice;
          if (int.TryParse(Console.ReadLine(), out choice))
          {
            if (choice == 0)
              return;
            if (choice <= DataBase.Users.Count)
            {
              while (true)
              {
                Console.WriteLine("Write money amount: ");
                double amount = 0;
                if (double.TryParse(Console.ReadLine(), out amount))
                {
                  if (amount == 0)
                    return;
                  DataBase.Users.ElementAt(choice - 1).Value.AddMoney(amount);
                  UpdateDatabaseFile();
                  return;
                }
                else
                {
                  Console.WriteLine("Not a number. Try again. (0 to cancel");
                  continue;
                }
              }
            }
          }
          else
          {
            Console.WriteLine("Not an integer. Try again.");
            continue;
          }
        }
      }
      else
      {
        Console.WriteLine("There are no existing users yet.");
        return;
      }
    }

    private static void AddNewATMUser()
    {
      string cardNumber = null;
      string cardPassword = null;
      Console.WriteLine("Creating new User...");
      Console.WriteLine("Write your card number: ");
      while (!ValidateInputCardNumber(cardNumber))
        cardNumber = Console.ReadLine();
      Console.WriteLine("Write your password: ");
      while (!ValidateInputCardPassword(cardPassword))
        cardPassword = Console.ReadLine();
      DataBase.AddUser(cardNumber, cardPassword);
      Console.WriteLine("User created!");
      UpdateDatabaseFile();
    }

    private static void ViewUsersInfo()
    {
      if (DataBase.Users.Count != 0)
      {
        Console.WriteLine("List of existing cards: ");
        int i = 1;
        foreach (var user in DataBase.Users)
        {
          Console.WriteLine(i + ". " + user.Value.GetCardNumber());
        }
        while (true)
        {
          Console.WriteLine("Choose card. (0 to cancel)");
          int choice;
          if (int.TryParse(Console.ReadLine(), out choice))
          {
            if (choice == 0)
              return;
            if (choice <= DataBase.Users.Count)
            {
              Console.WriteLine(DataBase.Users.ElementAt(choice - 1).Value.GetUserInfo());
            }
          }
          else
          {
            Console.WriteLine("Not an integer. Try again. (0 to cancel)");
            continue;
          }
        }
      }
      else
      {
        Console.WriteLine("There are no existing users yet.");
        return;
      }
    }

    private static void ClearDataBaseFile()
    {
      DataBase.ClearDict(_dictPath);
    }

    #endregion

  }
}
