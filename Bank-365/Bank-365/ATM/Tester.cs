using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bank_365.ATM
{
  [JsonObject(MemberSerialization.Fields)]
  public class Tester
  {
    private int Var1;
    private double Var2;
    private string Var3;
    

    public Tester(int value)
    {
      Var1 = value;
      Var2 = Var1 + 1.0;
      Var3 = "" + (Var2 + 1);
    }

    override public string ToString()
    {
      return "Tester values: " + Var1 + ", " + Var2 + ", " + Var3;
    }
  }
}
