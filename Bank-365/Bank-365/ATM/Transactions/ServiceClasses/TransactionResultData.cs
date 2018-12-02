namespace Bank_365.ATM.Transactions.ServiceClasses
{
  public class TransactionResultData
  {
    private bool _done;
    private TransactionDeniedReason _reason;

    public bool Done => _done;

    public TransactionDeniedReason Reason => _reason;

    public TransactionResultData(bool done, TransactionDeniedReason reason=TransactionDeniedReason.DoNotDenied)
    {
      _done = done;
      _reason = reason;
    }
  }
}
