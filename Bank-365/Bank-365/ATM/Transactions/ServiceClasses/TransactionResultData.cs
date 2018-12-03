using System;
using System.Data;

namespace Bank_365.ATM.Transactions.ServiceClasses
{
  public class TransactionResultData
  {
    private bool _done;
    private TransactionDeniedReason _reason;
    private DateTime _dateTime;
    private double _amount;
    private bool _reached;

    public bool Done => _done;
    public TransactionDeniedReason Reason => _reason;
    public DateTime DateTime => _dateTime;
    public double Amount => _amount;
    public bool Reached => _reached;

    public TransactionResultData(bool done, DateTime dateTime, double amount, bool reached,
                                TransactionDeniedReason reason = TransactionDeniedReason.DoNotDenied)
    {
      _done = done;
      _reason = reason;
      _dateTime = dateTime;
      _amount = amount;
      _reached = reached;
    }
  }
}
