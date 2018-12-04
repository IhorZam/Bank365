using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bank_365.ATM.Transactions.ServiceClasses;

namespace Bank_365.ATM
{
  public partial class ContextWindow : Form
  {
    public ContextWindow()
    {
      InitializeComponent();
    }

    private UserProxy _currentUser = null;

    private bool _loggedIn = false;

    private int _passwordAttempts;

    private TransactionController _transactionController = new TransactionController();

    private static string _dictPath = "DataBase.txt";

    private static ContextWindow atm = new ContextWindow();

    private Menus currentMenu;

    private enum Menus
    {
      LoginMenu, MainMenu, RecheckPass, SendMoney, WithdrawMoney, GetCredit
    }

    private int[] creditTariffs =
    {
      0, 20, 19, 18, 17, 16, 15, 14, 13
    };

    public UserProxy CurrentUser
    {
      get { return _currentUser; }
      set { _currentUser = value; }
    }    


    public static void Main(string[] args)
    {

      atm._transactionController.Start();

      Thread CWThread = new Thread(RunAtm);
      CWThread.Start();

      Thread DevThread = new Thread(RunConsole);
      DevThread.Start();      

    }

    private static void RunConsole()
    {
      DevMenu();
    }

    private static void RunAtm()
    {
      Application.Run(atm);
    }

    private void ContextWindow_Load(object sender, EventArgs e)
    {
      atm.Initialize();
    }

    private void Initialize()
    {
      DataBase.CreateDict(_dictPath);
      DisablePanel(aPasswordPanel);
      DisablePanel(aCardNumberPanel);
      DisablePanel(aReceiverCardRequestPanel);
      DisablePanel(aAmountOfMoneyRequestPanel);
      LoginMenu();
    }

    private void EnablePanel(Panel panel)
    {
      panel.Enabled = true;
      panel.Visible = true;

      foreach (Control control in panel.Controls)
      {
        control.Enabled = true;
        control.Visible = true;
      }
    }

    private void DisablePanel(Panel panel)
    {
      panel.Enabled = false;
      panel.Visible = false;

      foreach (Control control in panel.Controls)
      {
        control.Enabled = false;
        control.Visible = false;
      }
    }

    private void RedrawWindow()
    {
      aInfoLabel.Text = "";
      switch (currentMenu)
      {
        case Menus.LoginMenu:
          {
            aLabelMenuName.Text = "Login menu";
            ClearSidepanelsButtons();
            EnablePanel(aCardNumberPanel);
            break;
          }
        case Menus.MainMenu:
          {
            aLabelMenuName.Text = "Main menu";
            ClearSidepanelsButtons();

            aButton2Label.Text = "Display your credit info";
            aButton3Label.Text = "Display amount of money";
            aButton4Label.Text = "Withdraw money";
            aButton7Label.Text = "Get credit";
            aButton8Label.Text = "Send money";

            aButton2.Enabled = true;
            aButton3.Enabled = true;
            aButton4.Enabled = true;
            aButton7.Enabled = true;
            aButton8.Enabled = true;
            aButton2.Click += (sender, args) => DisplayCreditInfo();
            aButton3.Click += (sender, args) => DisplayAmountOfMoney();
            aButton4.Click += (sender, args) => WithdrawMoney();
            aButton7.Click += (sender, args) => GetCreditMenu();
            aButton8.Click += (sender, args) => SendMoney();

            break;
          }
        case Menus.RecheckPass:
          {
            aLabelMenuName.Text = "Re-enter password";

            ClearSidepanelsButtons();

            EnablePanel(aPasswordPanel);

            break;
          }
        case Menus.SendMoney:
          {
            aLabelMenuName.Text = "Send money transaction";
            ClearSidepanelsButtons();
            EnablePanel(aReceiverCardRequestPanel);
            aAmountOfMoneyRequestTextBox.Mask = "000000";            
            break;
          }
        case Menus.WithdrawMoney:
          {
            aLabelMenuName.Text = "Money withdrawal";
            ClearSidepanelsButtons();
            EnablePanel(aAmountOfMoneyRequestPanel);
            aAmountOfMoneyRequestTextBox.Mask = "0000";
            break;
          }
        case Menus.GetCredit:
        {
          aLabelMenuName.Text = "Credit menu";
          ClearSidepanelsButtons();
          EnablePanel(aAmountOfMoneyRequestPanel);
          aAmountOfMoneyRequestTextBox.Mask = "000000";

            aButton1Label.Text = "6 months \n" + creditTariffs[1] + "%";
          aButton2Label.Text = "12 months \n" + creditTariffs[2] + "%";
          aButton3Label.Text = "18 months \n" + creditTariffs[3] + "%";
          aButton4Label.Text = "24 months \n" + creditTariffs[4] + "%";
          aButton5Label.Text = "30 months \n" + creditTariffs[5] + "%";
          aButton6Label.Text = "36 months \n" + creditTariffs[6] + "%";
          aButton7Label.Text = "42 months \n" + creditTariffs[7] + "%";
          aButton8Label.Text = "48 months \n" + creditTariffs[8] + "%";

            foreach (Button button in panel1.Controls)
          {
            button.Enabled = true;
          }
          foreach (Button button in panel2.Controls)
          {
            button.Enabled = true;
          }

          aButton1.Click += (sender, args) => GetCredit(1);
          aButton2.Click += (sender, args) => GetCredit(2);
          aButton3.Click += (sender, args) => GetCredit(3);
          aButton4.Click += (sender, args) => GetCredit(4);
          aButton5.Click += (sender, args) => GetCredit(5);
          aButton6.Click += (sender, args) => GetCredit(6);
          aButton7.Click += (sender, args) => GetCredit(7);
          aButton8.Click += (sender, args) => GetCredit(8);
            break;
        }
      }
    }

    private void ClearSidepanelsButtons()
    {
      foreach (Button button in panel1.Controls)
      {
        RemoveClickEvent(button);
        button.Enabled = false;
      }
      foreach (Button button in panel2.Controls)
      {
        RemoveClickEvent(button);
        button.Enabled = false;
      }
      foreach (Label label in aLeftButtonsDisplayPanel.Controls)
      {
        label.Text = "";
      }
      foreach (Label label in aRightButtonsDisplayPanel.Controls)
      {
        label.Text = "";
      }
    }

    private void RemoveClickEvent(Button b)
    {
      FieldInfo f1 = typeof(Control).GetField("EventClick",
        BindingFlags.Static | BindingFlags.NonPublic);
      object obj = f1.GetValue(b);
      PropertyInfo pi = b.GetType().GetProperty("Events",
        BindingFlags.NonPublic | BindingFlags.Instance);
      EventHandlerList list = (EventHandlerList)pi.GetValue(b, null);
      list.RemoveHandler(obj, list[obj]);
    }

    public static void UpdateDatabaseFile()
    {
      File.WriteAllText(_dictPath, JsonConvert.SerializeObject(DataBase.Users));
    }


    private void aInputButtonEnter_Click(object sender, EventArgs e)
    {
      aInfoLabel.Text = "";
      if (currentMenu == Menus.LoginMenu)
      {
        string inputCardNumber = null;
        string inputCardPassword = null;

        if (aCardNumberPanel.Enabled == true)
        {
          inputCardNumber = aCardNumberTextBox.Text;
          if (!ValidateInputCardNumber(inputCardNumber))
          {
            aInfoLabel.Text = "Wrong card number";
            aCardNumberTextBox.Text = "";
            inputCardNumber = null;
            return;
          }
          else
          {
            if (DataBase.Users.ContainsKey(inputCardNumber))
            {
              CurrentUser = DataBase.Users[inputCardNumber];
              _passwordAttempts = CurrentUser.GetPasswordAttempts();
              if (CurrentUser.GetBlockedStatus())
              {
                aInfoLabel.Text = "This card is blocked.";
                inputCardNumber = null;
                aCardNumberTextBox.Text = "";
                return;
              }
              DisablePanel(aCardNumberPanel);
              EnablePanel(aPasswordPanel);
              return;
            }
            else
            {
              aInfoLabel.Text = "Card number doesn't exist or wrong input. Try again.";
              inputCardNumber = null;
              aCardNumberTextBox.Text = "";
              return;
            }
          }
        }
        else if (aPasswordPanel.Enabled == true)
        {
          inputCardPassword = aPasswordTextBox.Text;
          if (!ValidateInputCardPassword(inputCardPassword))
          {
            aInfoLabel.Text = "Wrong password format. Try again.";
            aPasswordTextBox.Text = "";
            inputCardPassword = null;
            return;
          }
          else
          {
            inputCardPassword = aPasswordTextBox.Text;
            if (!DataBase.ValidateUser(CurrentUser, inputCardPassword))
            {
              if (--_passwordAttempts == 0)
              {
                CurrentUser.SetPasswordAttempts(_passwordAttempts);
                DataBase.BlockUser(CurrentUser);
                aInfoLabel.Text = "Wrong password. Your card is blocked.";
                DisablePanel(aPasswordPanel);
                EnablePanel(aCardNumberPanel);
                inputCardPassword = null;
                CurrentUser = null;
                return;
              }
              else
              {
                CurrentUser.SetPasswordAttempts(_passwordAttempts);
                aInfoLabel.Text = "Wrong password. " + _passwordAttempts + " attempts left.";
                inputCardPassword = null;
                aPasswordTextBox.Text = "";
                return;
              }
            }
            else
            {
              DisablePanel(aCardNumberPanel);
              DisablePanel(aPasswordPanel);
              CurrentUser.SetPasswordAttempts(3);
              atm._loggedIn = true;
              aInfoLabel.Text = "You have logged in. Welcome.";
              MainMenu();
              return;
            }
          }
        }
      }
      else if (currentMenu == Menus.SendMoney)
      {
        string inputCardNumber;
        double inputMoneyAmount;

        if (aReceiverCardRequestPanel.Enabled)
        {
          inputCardNumber = aReceiverCardNumberTextBox.Text;
          if (!ValidateInputCardNumber(inputCardNumber))
          {
            aInfoLabel.Text = "Wrong card number";
            aReceiverCardNumberTextBox.Text = "";
            inputCardNumber = null;
            return;
          }
          else
          {
            if (DataBase.Users.ContainsKey(inputCardNumber))
            {
              UserProxy receiver = DataBase.Users[inputCardNumber];
              if (receiver.GetBlockedStatus())
              {
                aInfoLabel.Text = "Receiver's card is blocked.";
                inputCardNumber = null;
                aReceiverCardNumberTextBox.Text = "";
                return;
              }
              DisablePanel(aReceiverCardRequestPanel);
              EnablePanel(aAmountOfMoneyRequestPanel);
              return;
            }
            else
            {
              aInfoLabel.Text = "Card number doesn't exist or wrong input. Try again.";
              inputCardNumber = null;
              aCardNumberTextBox.Text = "";
              return;
            }
          }
        }
        else if (aAmountOfMoneyRequestPanel.Enabled)
        {
          inputCardNumber = aReceiverCardNumberTextBox.Text;
          inputMoneyAmount = double.Parse(aAmountOfMoneyRequestTextBox.Text);
          string key = _transactionController.CreateNewTransaction(CurrentUser.GetCardNumber(), inputMoneyAmount, inputCardNumber);
          aAmountOfMoneyRequestTextBox.Text = "";
          aReceiverCardNumberTextBox.Text = "";
          DisablePanel(aAmountOfMoneyRequestPanel);
          aInfoLabel.Text = "Your transaction request is processing...";
          TransactionResultData result = null;
          while (result == null)
          {
            try
            {
              result = CurrentUser.GetTransactionHistory()[key];
            }
            catch (KeyNotFoundException)
            {
              continue;
            }
          }
          if (result.Done)
          {
            aInfoLabel.Text = "Transaction successful.";
          }
          else
          {
            aInfoLabel.Text =
              "Transaction unsuccessful. Reason: " + ((result.Reason == TransactionDeniedReason.NotEnoughMoney)
                ? "Not enough money" : "You can't send money to yourself");
          }
          return;
        }
        else
        {
          currentMenu = Menus.MainMenu;
          RedrawWindow();          
          return;
        }
      }
      else if (currentMenu == Menus.RecheckPass)
      {
        string inputCardPassword;

        if (aPasswordPanel.Enabled == true)
        {
          inputCardPassword = aPasswordTextBox.Text;
          if (!ValidateInputCardPassword(inputCardPassword))
          {
            aInfoLabel.Text = "Wrong password format. Try again.";
            aPasswordTextBox.Text = "";
            inputCardPassword = null;
            return;
          }
          else
          {
            inputCardPassword = aPasswordTextBox.Text;
            if (!DataBase.ValidateUser(CurrentUser, inputCardPassword))
            {
              if (--_passwordAttempts == 0)
              {
                CurrentUser.SetPasswordAttempts(_passwordAttempts);
                DataBase.BlockUser(CurrentUser);
                aInfoLabel.Text = "Wrong password. Your card is blocked.";
                DisablePanel(aPasswordPanel);
                EnablePanel(aCardNumberPanel);
                inputCardPassword = null;
                CurrentUser = null;
                return;
              }
              else
              {
                CurrentUser.SetPasswordAttempts(_passwordAttempts);
                aInfoLabel.Text = "Wrong password. " + _passwordAttempts + " attempts left.";
                inputCardPassword = null;
                aPasswordTextBox.Text = "";
                return;
              }
            }
            else
            {
              DisablePanel(aCardNumberPanel);
              DisablePanel(aPasswordPanel);
              CurrentUser.SetPasswordAttempts(3);
              atm._loggedIn = true;
              MainMenu();
              return;
            }
          }
        }
      }
      else if (currentMenu == Menus.WithdrawMoney)
      {
        int inputMoneyAmount;        
        if (aAmountOfMoneyRequestPanel.Enabled)
        {
          try
          {
            inputMoneyAmount = int.Parse(aAmountOfMoneyRequestTextBox.Text);
          }
          catch (FormatException)
          {
            return;
          }                    
          string key = _transactionController.CreateNewTransaction(CurrentUser.GetCardNumber(), inputMoneyAmount);
          aAmountOfMoneyRequestTextBox.Text = "";
          aReceiverCardNumberTextBox.Text = "";
          TransactionResultData result = null;
          while (result == null)
          {
            try
            {
              result = CurrentUser.GetTransactionHistory()[key];
            }
            catch (KeyNotFoundException)
            {
              continue;
            }            
          }
          DisablePanel(aAmountOfMoneyRequestPanel);
          if (result.Done)
          {
            aInfoLabel.Text = "Take your money";
            GiveBanknotes(inputMoneyAmount);
          }
          else
          {
            aInfoLabel.Text =
              "Transaction unsuccessful. Reason: " + ((result.Reason == TransactionDeniedReason.NotEnoughMoney)
                ? "Not enough money" : "Unknown");
          }
          return;
        }
        else
        {
          currentMenu = Menus.MainMenu;
          RedrawWindow();
          return;
        }
      }
      else if (currentMenu == Menus.GetCredit)
      {
        if (aAmountOfMoneyRequestPanel.Enabled == false)
        {
          currentMenu = Menus.MainMenu;
          RedrawWindow();
          return;
        }
      }
    }

    private void aInputButtonClear_Click(object sender, EventArgs e)
    {
      switch (currentMenu)
      {
        case Menus.LoginMenu:
        {
          aCardNumberTextBox.Text = "";
          aPasswordTextBox.Text = "";
          break;
        }
        case Menus.SendMoney:
        {
          aReceiverCardNumberTextBox.Text = "";
          aAmountOfMoneyRequestTextBox.Text = "";
          break;
        }
        case Menus.RecheckPass:
        {
          aPasswordTextBox.Text = "";
          break;
        }
        case Menus.WithdrawMoney:
        {
          aAmountOfMoneyRequestTextBox.Text = "";
          break;
        }
        case Menus.GetCredit:
        {
          aAmountOfMoneyRequestTextBox.Text = "";
          break;
          }
      }
    }

    private void aInputButtonCancel_Click(object sender, EventArgs e)
    {
      switch (currentMenu)
      {
        case Menus.LoginMenu:
          {
            aCardNumberTextBox.Text = "";
            aPasswordTextBox.Text = "";
            DisablePanel(aPasswordPanel);
            DisablePanel(aCardNumberPanel);
            LoginMenu();
            break;
          }
        case Menus.MainMenu:
          {
            aCardNumberTextBox.Text = "";
            aPasswordTextBox.Text = "";
            LoginMenu();
            break;
          }
        case Menus.RecheckPass:
          {
            _loggedIn = false;
            aCardNumberTextBox.Text = "";
            aPasswordTextBox.Text = "";
            LoginMenu();
            break;
          }
        case Menus.SendMoney:
          {
            aReceiverCardNumberTextBox.Text = "";
            aAmountOfMoneyRequestTextBox.Text = "";
            DisablePanel(aReceiverCardRequestPanel);
            DisablePanel(aAmountOfMoneyRequestPanel);
            currentMenu = Menus.MainMenu;
            RedrawWindow();
            break;
          }
        case Menus.WithdrawMoney:
        {
          aAmountOfMoneyRequestTextBox.Text = "";
          DisablePanel(aAmountOfMoneyRequestPanel);
          currentMenu = Menus.MainMenu;
          RedrawWindow();
          break;
          }
        case Menus.GetCredit:
        {
          aAmountOfMoneyRequestTextBox.Text = "";
          DisablePanel(aAmountOfMoneyRequestPanel);
          currentMenu = Menus.MainMenu;
          RedrawWindow();
          break;
        }
      }
    }

    private bool LoginMenu()
    {
      aInfoLabel.Text = "";
      CurrentUser = null;
      currentMenu = Menus.LoginMenu;

      RedrawWindow();

      return true;
    }



    private void MainMenu()
    {
      currentMenu = Menus.MainMenu;

      RedrawWindow();

      return;
    }

    private void RecheckPassword()
    {
      currentMenu = Menus.RecheckPass;

      RedrawWindow();

      return;
    }    

    private void GetCreditMenu()
    {
      if (CurrentUser.GetCreditInfo().Amount != 0)
      {
        aInfoLabel.Text = "You already have a credit!";
        return;
      }
       
      currentMenu = Menus.GetCredit;

      RedrawWindow();
      aInfoLabel.Text = "Type in amount of money (minimum - 1000, maximum - your credit limit) and choose desirable credit tariff.";

      return;
    }

    private void GetCredit(int v)
    {
      int inputCreditSize;
      if (aAmountOfMoneyRequestPanel.Enabled)
      {
        try
        {
          inputCreditSize = int.Parse(aAmountOfMoneyRequestTextBox.Text);
        }
        catch (FormatException)
        {
          return;
        }

        if (inputCreditSize < 1000)
        {
          aInfoLabel.Text = "Minimum money amount - 1000";
          aAmountOfMoneyRequestTextBox.Text = "";
          return;
        }
        else if (inputCreditSize > CurrentUser.CreditLimit())
        {
          aInfoLabel.Text = "Typed amount is higher than your credit limit. \n";
          aInfoLabel.Text += "(Your credit limit: " + CurrentUser.CreditLimit() + ")";
          aAmountOfMoneyRequestTextBox.Text = "";
          return;
        }
        else
        {
          aAmountOfMoneyRequestTextBox.Text = "";
          TransactionResultData result = null;
          ClearSidepanelsButtons();
          DisablePanel(aAmountOfMoneyRequestPanel);
          string key = _transactionController.CreateNewTransaction(CurrentUser.GetCardNumber(), new UserProxy.CreditInfo
          {
            Amount = inputCreditSize, Percent = creditTariffs[v], Time = v*6
          });          
          while (result == null)
          {
            try
            {
              result = CurrentUser.GetTransactionHistory()[key];
            }
            catch (KeyNotFoundException)
            {
              continue;
            }
          }                    
          if (result.Done)
          {
            aInfoLabel.Text = "You successfully took a credit. Money added to your account";            
          }
          else
          {
            aInfoLabel.Text =
              "Transaction unsuccessful. Reason: " + result.Reason;
          }
          return;
        }        
      }
      else
      {
        currentMenu = Menus.MainMenu;
        RedrawWindow();
        return;
      }
    }

    private void SendMoney()
    {
      aInfoLabel.Text = "";
      currentMenu = Menus.SendMoney;
      RedrawWindow();

      return;
    }

    private void WithdrawMoney()
    {
      currentMenu = Menus.WithdrawMoney;

      RedrawWindow();

      return;

      //int amount;
      //while (true)
      //{
      //  try
      //  {
      //    Console.WriteLine("Choose amount of money: ");
      //    amount = int.Parse(Console.ReadLine());
      //    break;
      //  }
      //  catch (Exception e)
      //  {
      //    Console.WriteLine("Wrong input. Try again.");
      //    continue;
      //  }
      //}
      //_transactionController.CreateNewTransaction(CurrentUser.GetCardNumber(), amount);
    }

    private void GiveBanknotes(int amount)
    {
      
    }    

    private void DisplayAmountOfMoney()
    {
      aInfoLabel.Text = "";
      aInfoLabel.Text += "Current amount of money on this card: \n";
      aInfoLabel.Text += (CurrentUser.GetMoneyAmount());
    }

    private void DisplayCreditInfo()
    {
      aInfoLabel.Text = "";
      UserProxy.CreditInfo info = CurrentUser.GetCreditInfo();
      aInfoLabel.Text += "Current credit: ";
      if (info.Amount == 0)
      {
        aInfoLabel.Text += "none \n";
      }
      else
      {
        aInfoLabel.Text += "\n Amount: " + info.Amount + "\n Percent: " + info.Percent + "\n Time: " + info.Time + "\n";
      }
      aInfoLabel.Text += "Credit limit: " + CurrentUser.CreditLimit();
    }

    private static bool ValidateInputCardPassword(string inputCardPassword)
    {
      if (inputCardPassword == null)
        return false;

      if (inputCardPassword.Length != 4)
      {
        Console.WriteLine("Invalid password length. Try again.");
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
      if (inputCardNumber == null)
        return false;

      inputCardNumber.Replace(" ", "");
      inputCardNumber.Replace("\t", "");
      inputCardNumber.Replace("-", "");
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
        Console.WriteLine("4 - Set credit limit of ATM User");
        Console.WriteLine("5 - Clear DataBase file");
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
            SetCreditLimit();
            break;
          case '5':
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

    private static void SetCreditLimit()
    {
      if (DataBase.Users.Count != 0)
      {
        Console.WriteLine("List of existing cards: ");
        int i = 1;
        foreach (var user in DataBase.Users)
        {
          Console.WriteLine(i++ + ". " + user.Value.GetCardNumber());
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
                Console.WriteLine("Write credit limit: ");
                double amount = 0;
                if (double.TryParse(Console.ReadLine(), out amount))
                {
                  if (amount == 0)
                    return;
                  DataBase.Users.ElementAt(choice - 1).Value.SetCreditLimit(amount);
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

    private static void AddMoneyToATMUser()
    {
      if (DataBase.Users.Count != 0)
      {
        Console.WriteLine("List of existing cards: ");
        int i = 1;
        foreach (var user in DataBase.Users)
        {
          Console.WriteLine(i++ + ". " + user.Value.GetCardNumber());
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
      int count = 0;
      try
      {
        count = DataBase.Users.Count;
      }
      catch (Exception)
      {
        Console.WriteLine("There are no existing users yet.");
        return;
      }      
      if (count != 0)
      {
        Console.WriteLine("List of existing cards: ");
        int i = 1;
        foreach (var user in DataBase.Users)
        {
          Console.WriteLine(i++ + ". " + user.Value.GetCardNumber());
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

    private void ClickInputButton(string v)
    {
      if (aPasswordPanel.Enabled == true)
        aPasswordTextBox.AppendText(v);
      if (aCardNumberPanel.Enabled == true)
        aCardNumberTextBox.AppendText(v);
      if (aReceiverCardRequestPanel.Enabled == true)
        aReceiverCardNumberTextBox.AppendText(v);
      if (aAmountOfMoneyRequestPanel.Enabled == true)
        aAmountOfMoneyRequestTextBox.AppendText(v);
    }

    private void aInputButton1_Click(object sender, EventArgs e)
    {
      ClickInputButton("1");
    }

    private void aInputButton2_Click(object sender, EventArgs e)
    {
      ClickInputButton("2");
    }

    private void aInputButton3_Click(object sender, EventArgs e)
    {
      ClickInputButton("3");
    }

    private void aInputButton4_Click(object sender, EventArgs e)
    {
      ClickInputButton("4");
    }

    private void aInputButton5_Click(object sender, EventArgs e)
    {
      ClickInputButton("5");
    }

    private void aInputButton6_Click(object sender, EventArgs e)
    {
      ClickInputButton("6");
    }

    private void aInputButton7_Click(object sender, EventArgs e)
    {
      ClickInputButton("7");
    }

    private void aInputButton8_Click(object sender, EventArgs e)
    {
      ClickInputButton("8");
    }

    private void aInputButton9_Click(object sender, EventArgs e)
    {
      ClickInputButton("9");
    }

    private void aInputButton0_Click(object sender, EventArgs e)
    {
      ClickInputButton("0");
    }

    private void aInputButtonDot_Click(object sender, EventArgs e)
    {
      ClickInputButton(".");
    }

    private void aInputButtonZeros_Click(object sender, EventArgs e)
    {
      ClickInputButton("00");
    }
  }
}
