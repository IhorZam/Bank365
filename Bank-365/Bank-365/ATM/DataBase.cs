using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

      _users = JsonConvert.DeserializeObject<Dictionary<string, UserProxy>>(File.ReadAllText(path));

      //usersData = JsonConvert.DeserializeObject<Dictionary<string, UserProxy>>(File.ReadAllText(path));

      //_users = new Dictionary<string, UserProxy>();

      //foreach (KeyValuePair<string, UserProxy> user in usersData)
      //{
      //  _users.Add(user.Key, user.Value);
      //}
    }

    public static void DeleteDict(string path)
    {
      File.Delete(path);
    }

    public static void AddUser(string cardNumber, string password)
    {
      UserProxy newUser = new UserProxy(cardNumber, password);
      Users.Add(cardNumber, newUser);
    }

    public static bool ValidateUser(UserProxy user, string password)
    {
      //return user.GetPassword().Equals(password);
      return Users[user.GetCardNumber()].GetPassword().Equals(password);
    }

    public static void BlockUser(UserProxy user)
    {
      //user.SetBlockedStatus(true);
      Users[user.GetCardNumber()].SetBlockedStatus(true);
    }
  }
}
