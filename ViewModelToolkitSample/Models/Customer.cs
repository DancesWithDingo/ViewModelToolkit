using System.Diagnostics;

namespace ViewModelToolkitSample.Models;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Customer : Person, IEquatable<Customer>
{
    public Customer() { }
    public Customer(Guid accountId, string firstName, string lastName, DateTime birthDate, DateTime anniversaryDate = default)
            : base(firstName, lastName, birthDate) {
        AccountId = accountId;
        AnniversaryDate = anniversaryDate == default ? DateTime.Now : anniversaryDate;
    }

    public Guid AccountId { get; set; }
    public DateTime AnniversaryDate { get; set; }
    public int LoyaltyPoints { get; set; }

    public override bool Equals(object obj) => Equals(obj as Customer);

    public bool Equals(Customer other) {
        return other is not null &&
               base.Equals(other) &&
               AccountId.Equals(other.AccountId) &&
               FirstName == other.FirstName &&
               LastName == other.LastName &&
               BirthDate == other.BirthDate &&
               FullName == other.FullName &&
               SortName == other.SortName &&
               AnniversaryDate == other.AnniversaryDate &&
               LoyaltyPoints == other.LoyaltyPoints;
    }

    public override int GetHashCode() {
        HashCode hash = new HashCode();
        hash.Add(base.GetHashCode());
        hash.Add(AccountId);
        hash.Add(FirstName);
        hash.Add(LastName);
        hash.Add(BirthDate);
        hash.Add(FullName);
        hash.Add(SortName);
        hash.Add(AnniversaryDate);
        hash.Add(LoyaltyPoints);
        return hash.ToHashCode();
    }

    public static bool operator ==(Customer left, Customer right) => EqualityComparer<Customer>.Default.Equals(left, right);
    public static bool operator !=(Customer left, Customer right) => !(left == right);

    string GetDebuggerDisplay() =>
        $"Customer => {LastName ?? "null"}, {FirstName ?? "null"}: {BirthDate:d}, {AccountId}, {AnniversaryDate:d}, {LoyaltyPoints}";
}
