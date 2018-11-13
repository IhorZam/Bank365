using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Schema;
using Newtonsoft.Json.Linq;

namespace Bank_365.ATM
{
  public class AtmContext
  {
    private UserProxy _currentUser = null;

    private TransactionController _transactionController;

    private string _dictPath = "C:\\Users\\admin\\Dropbox\\CPP\\Bank365-master\\Bank-365\\Bank-365\\DictInfo.txt";
    

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



      //AtmContext atm = new AtmContext();

      //while (true)
      //{
      //  // atm.Initialize();
      //  // тд тп

      //  File.WriteAllText(atm._dictPath, JsonConvert.SerializeObject(DataBase.Users));
      //}
    }

    public void Initialize()
    {
     
      DataBase.CreateDict(_dictPath);

      //-------------------------------------------

      string inputCardNumber = null;
      string inputCardPassword = null;
    // XmlNode currentCard = null;

    //CardNumberRequest:
    //  Console.WriteLine("Type in your card number: ");
    //  inputCardNumber = Console.ReadLine();
    //  while (!ValidateInputCardNumber(inputCardNumber))
    //    inputCardNumber = Console.ReadLine();


      //if (_users.ContainsKey(inputCardNumber))
      //  CurrentUser = _users[inputCardNumber];
      /*
      foreach (XmlNode node in usersData.DocumentElement.ChildNodes)
      {
          if (inputCardNumber == node.Name)
              currentCard = node;
      }
      */

      /*
      if (currentCard == null)
      {
          Console.WriteLine("Wrong card number. Try again.");
          goto CardNumberRequest;
      }
      */

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
    //  int attempts = 3;
    //  Console.WriteLine("Type in your card password: ");
    //  inputCardPassword = Console.ReadLine();
    //  while (!ValidateInputCardPassword(inputCardPassword))
    //    inputCardPassword = Console.ReadLine();


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

      //if (!CurrentUser.ValidateUser(inputCardPassword))
      //{
      //  /*
      //  if (--attempts == 0)
      //  {
      //      CurrentUser.BlockUser();
      //      Console.WriteLine("Wrong password. Your card is blocked.");
      //      goto CardNumberRequest;
      //  }
      //  */

      //  Console.WriteLine("Wrong password." + attempts + "attempts left.");
      //  goto PasswordRequest;
      //}

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

    private bool ValidateInputCardPassword(string inputCardPassword)
    {
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

    private bool ValidateInputCardNumber(string inputCardNumber)
    {
      inputCardNumber.Replace(@"\s+", "");
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
