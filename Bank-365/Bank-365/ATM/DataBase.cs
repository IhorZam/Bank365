using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace Bank_365.ATM
{
  public static class DataBase
  {

    private static Dictionary<string, UserProxy> _users = new Dictionary<string, UserProxy>();
    
    //private static Dictionary<string, UserProxy> usersData;

    public static Dictionary<string, UserProxy> Users
    {
      get { return _users; }
    }    

    public static void CreateDict(string path)
    {
      string dataBaseString = "";

      try
      {
        dataBaseString = File.ReadAllText(path);
      }
      catch (FileNotFoundException e)
      {
        File.WriteAllText(path, JsonConvert.SerializeObject(Users));
        _users = JsonConvert.DeserializeObject<Dictionary<string, UserProxy>>(dataBaseString);
      }

      _users = JsonConvert.DeserializeObject<Dictionary<string, UserProxy>>(dataBaseString);
    }

    public static void ClearDict(string path)
    {
      File.Delete(path);
      _users = new Dictionary<string, UserProxy>();
      File.WriteAllText(path, JsonConvert.SerializeObject(Users));
    }

    private static string getHashSha256(string text)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(text);
      SHA256Managed hashstring = new SHA256Managed();
      byte[] hash = hashstring.ComputeHash(bytes);
      string hashString = string.Empty;
      foreach (byte x in hash)
      {
        hashString += String.Format("{0:x2}", x);
      }
      return hashString;
    }

    public static void AddUser(string cardNumber, string password)
    {
      string passHash = getHashSha256(password);
      UserProxy newUser = new UserProxy(cardNumber, passHash);
      Users.Add(cardNumber, newUser);
    }

    public static bool ValidateUser(UserProxy user, string password)
    {
      string passHash = getHashSha256(password);
      return Users[user.GetCardNumber()].GetPassword().Equals(passHash);
    }

    public static void BlockUser(UserProxy user)
    {
      Users[user.GetCardNumber()].SetBlockedStatus(true);
    }
  }
}
