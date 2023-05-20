using System.Diagnostics;

namespace ViewModelToolkitSample.Models;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Person : IEquatable<Person>
{
    public Person() { }
    public Person(string firstName, string lastName, DateTime birthDate) {
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();
    public string SortName {
        get {
            string comma = ((LastName + FirstName)?.Any() ?? false) ? "," : "";
            return $"{LastName}{comma} {FirstName}".Trim();
        }
    }

    public bool Equals(Person other) {
        return other is not null &&
               FirstName == other.FirstName &&
               LastName == other.LastName &&
               BirthDate == other.BirthDate;
    }

    public override bool Equals(object obj) => Equals(obj as Person);
    public override int GetHashCode() => HashCode.Combine(FirstName, LastName, BirthDate);
    public static bool operator ==(Person left, Person right) => EqualityComparer<Person>.Default.Equals(left, right);
    public static bool operator !=(Person left, Person right) => !(left == right);

    string GetDebuggerDisplay() => $"Person => {LastName}, {FirstName}: {BirthDate:d}";
}
