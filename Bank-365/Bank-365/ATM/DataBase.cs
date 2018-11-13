using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace Bank_365.ATM
{
  public static class DataBase
  {

    private static Dictionary<string, UserProxy> _users;
    
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
  }
}
