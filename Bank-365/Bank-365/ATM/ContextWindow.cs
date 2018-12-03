using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
      LoginMenu, MainMenu, RecheckPass, SendMoney, WithdrawMoney
    }

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

            aButton3Label.Text = "Display amount of money";
            aButton4Label.Text = "Withdraw money";
            aButton8Label.Text = "Send money";

            aButton3.Enabled = true;
            aButton4.Enabled = true;
            aButton8.Enabled = true;
            aButton3.Click += (sender, args) => DisplayAmountOfMoney();
            aButton4.Click += (sender, args) => WithdrawMoney();
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
            break;
          }
        case Menus.WithdrawMoney:
          {
            aLabelMenuName.Text = "Money withdrawal";
            ClearSidepanelsButtons();
            EnablePanel(aAmountOfMoneyRequestPanel);
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
          _transactionController.CreateNewTransaction(CurrentUser.GetCardNumber(), inputMoneyAmount, inputCardNumber, out bool result);
          aAmountOfMoneyRequestTextBox.Text = "";
          aReceiverCardNumberTextBox.Text = "";
          DisablePanel(aAmountOfMoneyRequestPanel);
          aInfoLabel.Text = "Your transaction request is sent for processing.";
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
        //**************************************out result!***********************************************//
        if (aAmountOfMoneyRequestPanel.Enabled)
        {
          inputMoneyAmount = int.Parse(aAmountOfMoneyRequestTextBox.Text);
          _transactionController.CreateNewTransaction(CurrentUser.GetCardNumber(), inputMoneyAmount);
          aAmountOfMoneyRequestTextBox.Text = "";
          aReceiverCardNumberTextBox.Text = "";
          DisablePanel(aAmountOfMoneyRequestPanel);
          aInfoLabel.Text = "Your transaction request is sent for processing.";
          return;
        }
        else
        {
          currentMenu = Menus.MainMenu;
          RedrawWindow();
          return;
        }
      }
    }

    private void aInputButtonClear_Click(object sender, EventArgs e)
    {
      if (currentMenu == Menus.LoginMenu)
      {
        aCardNumberTextBox.Text = "";
        aPasswordTextBox.Text = "";
      }
      else if (currentMenu == Menus.SendMoney)
      {
        aReceiverCardNumberTextBox.Text = "";
        aAmountOfMoneyRequestTextBox.Text = "";
      }
      else if (currentMenu == Menus.RecheckPass)
      {
        aPasswordTextBox.Text = "";
      }
      else if (currentMenu == Menus.WithdrawMoney)
      {
        aAmountOfMoneyRequestTextBox.Text = "";
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
      throw new NotImplementedException();
    }

    private void DisplayAmountOfMoney()
    {
      aInfoLabel.Text = "";
      aInfoLabel.Text += "Current amount of money on this card: \n";
      aInfoLabel.Text += (CurrentUser.GetMoneyAmount());
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

    private void ClickInputButton(int v)
    {
      if (aPasswordPanel.Enabled == true)
        aPasswordTextBox.AppendText(v.ToString());
      if (aCardNumberPanel.Enabled == true)
        aCardNumberTextBox.AppendText(v.ToString());
      if (aReceiverCardRequestPanel.Enabled == true)
        aReceiverCardNumberTextBox.AppendText(v.ToString());
      if (aAmountOfMoneyRequestPanel.Enabled == true)
        aAmountOfMoneyRequestTextBox.AppendText(v.ToString());
    }

    private void aInputButton1_Click(object sender, EventArgs e)
    {
      ClickInputButton(1);
    }

    private void aInputButton2_Click(object sender, EventArgs e)
    {
      ClickInputButton(2);
    }

    private void aInputButton3_Click(object sender, EventArgs e)
    {
      ClickInputButton(3);
    }

    private void aInputButton4_Click(object sender, EventArgs e)
    {
      ClickInputButton(4);
    }

    private void aInputButton5_Click(object sender, EventArgs e)
    {
      ClickInputButton(5);
    }

    private void aInputButton6_Click(object sender, EventArgs e)
    {
      ClickInputButton(6);
    }

    private void aInputButton7_Click(object sender, EventArgs e)
    {
      ClickInputButton(7);
    }

    private void aInputButton8_Click(object sender, EventArgs e)
    {
      ClickInputButton(8);
    }

    private void aInputButton9_Click(object sender, EventArgs e)
    {
      ClickInputButton(9);
    }
  }
}
