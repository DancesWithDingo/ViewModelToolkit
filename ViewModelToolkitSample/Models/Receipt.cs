using System.Diagnostics;

namespace ViewModelToolkitSample.Models;

public class Transaction : IEquatable<Transaction>
{
    public Transaction() { }
    public Transaction(Guid transactionId) => TransactionId = transactionId;

    public Guid TransactionId { get; } = Guid.NewGuid();
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }


    public bool Equals(Transaction other) =>
        other is not null &&
        TransactionId.Equals(other.TransactionId) &&
        TransactionDate == other.TransactionDate &&
        Amount == other.Amount &&
        Description == other.Description;

    public override bool Equals(object obj) => Equals(obj as Transaction);
    public override int GetHashCode() => HashCode.Combine(TransactionId, TransactionDate, Amount, Description);
    public static bool operator ==(Transaction left, Transaction right) => EqualityComparer<Transaction>.Default.Equals(left, right);
    public static bool operator !=(Transaction left, Transaction right) => !(left == right);
}
