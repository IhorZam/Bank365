using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Xml;
using System.Xml.Schema;
using Newtonsoft.Json.Linq;
using Console = System.Console;

namespace Bank_365.ATM
{
  public class AtmContext
  {
    private UserProxy _currentUser = null;

    private TransactionController _transactionController;

    private static string _dictPath = "DataBase.txt";


    public UserProxy CurrentUser
    {
      get { return _currentUser; }
      set { _currentUser = value; }
    }

    public static void Main(string[] args)
    {
      // Tests.
      //Tester test1 = new Tester(1);
      //Tester test100 = new Tester(100);

      //Console.WriteLine(test1);
      //Console.WriteLine(test100);

      //Console.WriteLine(JsonConvert.SerializeObject(test1));
      //string serialized100 = JsonConvert.SerializeObject(test100);
      //System.Console.WriteLine(serialized100);

      //test1 = JsonConvert.DeserializeObject<Tester>(serialized100);


      //Console.WriteLine(JsonConvert.DeserializeObject<Tester>(serialized100));
      //Console.WriteLine(test1);

      //Dictionary<string, string> dic = new Dictionary<string, string>();
      //dic.Add("key1", "value1");
      //dic.Add("key2", "value2");

      //Console.WriteLine(JsonConvert.SerializeObject(dic));



      AtmContext atm = new AtmContext();

      DevMenu();

      while (true)
      {
        bool loggedIn = false;
        atm.Initialize();

        while (!loggedIn)
          loggedIn = atm.LoginMenu();

        atm.MainMenu();


        UpdateDatabaseFile();        
      }
    }

    private void MainMenu()
    {
      Console.ReadLine(); // pause
    }

    private static void UpdateDatabaseFile()
    {
      File.WriteAllText(_dictPath, JsonConvert.SerializeObject(DataBase.Users));
    }

    private static void DevMenu()
    {
      while (true)
      {
        Console.WriteLine("What do you want to do?");
        Console.WriteLine("0 - Skip and continue");
        Console.WriteLine("1 - Add new ATM User");
        Console.WriteLine("2 - Delete DataBase file");
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
            DataBase.DeleteDict(_dictPath);
            break;
          case '9':
            Environment.Exit(0);
            break;
          default:
            return;
        }
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

    private bool LoginMenu()
    {
      Console.WriteLine("Logging in...");

      string inputCardNumber = null;
      string inputCardPassword = null;

      while (true)
      {
        Console.WriteLine("Type in your card number: ");
        while (!ValidateInputCardNumber(inputCardNumber))
          inputCardNumber = Console.ReadLine();

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

            Console.WriteLine("Wrong password." + attempts + "attempts left.");
            inputCardPassword = null;
          }
        }
        Console.WriteLine("You have logged in. Welcome.");
        return true;
      }
    }

    public void Initialize()
    {

      DataBase.CreateDict(_dictPath);

      //-------------------------------------------


      

      //if (CurrentUser == null)
      //{
      //  Console.WriteLine("Wrong card number. Try again.");
      //  goto CardNumberRequest;
      //}

      /*
      if (currentCard.Attributes["blocked"].InnerText == "1")
      {
          Console.WriteLine("Card is blocked.");
          currentCard = null;
          inputCardNumber = null;
          goto CardNumberRequest;
      }
      */

      //PasswordRequest:



      /*if (inputCardPassword != currentCard.Attributes["cardPassword"].InnerText)
      {

          if (--attempts == 0)
          {                 
              BlockCard(currentCard);
              Console.WriteLine("Wrong password. Your card is blocked.");
              currentCard = null;
              inputCardNumber = null;
              goto CardNumberRequest;
          }



          Console.WriteLine("Wrong password." + attempts + "attempts left.");
          goto PasswordRequest;
      }*/

      return;
    }

    /*
    private void BlockCard(XmlNode currentUser)
    {
        XmlNode newUserData = currentUser;
        newUserData.Attributes["blocked"].Value = "1";
        usersData.DocumentElement.ReplaceChild(currentUser, newUserData);
        throw new NotImplementedException();
    }
    */

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
      if (inputCardNumber == null)
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
  }
}
