using System;
using System.Collections.Generic;
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
    
    private static XmlDocument usersData;

    public static Dictionary<string, UserProxy> Users
    {
      get { return _users; }
    }

    public static void CreateDict(string path)
    {
      usersData = new XmlDocument();
      usersData.Load(path);

      _users = new Dictionary<string, UserProxy>();

      foreach (XmlNode node in usersData.DocumentElement.ChildNodes)
      {
        _users.Add(node.Name, new UserProxy(node.Name, node.Attributes["password"].InnerText));
      }
    }
  }
}
