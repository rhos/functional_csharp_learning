#r"../../functional-csharp-code/LaYumba.Functional/bin/Debug/netstandard1.6/LaYumba.Functional.dll"
using static System.Console;
using static LaYumba.Functional.F;
using String = LaYumba.Functional.String;
using LaYumba.Functional;

Option<string> name = Some("Enrico");
name
    .Map(String.ToUpper)
    .ForEach(WriteLine);

IEnumerable<string> names = new[] { "Constance", "Albert" };
names
    .Map(String.ToUpper)
    .ForEach(WriteLine);

public class Age
{
    private int Value { get; }
    public Age(int value)
    {
        if (!IsValid(value))
            throw new ArgumentException($"{value} is not a valid age");
        Value = value;
    }
    private static bool IsValid(int age)
        => 0 <= age && age < 120;
    public static bool operator <(Age l, Age r)
        => l.Value < r.Value;
    public static bool operator >(Age l, Age r)
        => l.Value > r.Value;
    public static bool operator <(Age l, int r)
        => l < new Age(r);
    public static bool operator >(Age l, int r)
        => l > new Age(r);

    public static Option<Age> Of(int age) => IsValid(age) ? Some(new Age(age)) : None;
    public override string ToString()
    {
        return Value.ToString();
    }
}

public static Option<int> Parse(string s)
{
    return int.TryParse(s, out int result)
        ? Some(result) : None;
}

string input;
Option<int> optI = Int.Parse(input);
Option<Option<Age>> ageOpt = optI.Map(i => Age.Of(i));
Func<string, Option<Age>> parseAge = s => Int.Parse(s).Bind(Age.Of);

public static class AskForValidAgeAndPrintFlatteringMessage
{
    public static void Main()
        => WriteLine($"Only {ReadAge()}! That's young!");

    static Age ReadAge()
        => ParseAge(Prompt("Please enter your age"))
            .Match(
                () => ReadAge(),
                (age) => age);

    static Option<Age> ParseAge(string s)
        => Int.Parse(s).Bind(Age.Of);

    static string Prompt(string prompt)
    {
        WriteLine(prompt);
        return ReadLine();
        //return "24";
    }
}

// public static Option<R> Bind<T, R>(this Option<T> optT, Func<T, Option<R>> f)
//     => optT.Match(
//         () => None,
//         (t) => f(t));
// public static Option<R> Map<T, R>(this Option<T> optT, Func<T, R> f)
//     => optT.Match(
//         () => None,
//         (t) => Some(f(t)));

public static IEnumerable<R> Bind<T, R>(this IEnumerable<T> ts, Func<T, IEnumerable<R>> f)
{
    foreach (T t in ts)
        foreach (R r in f(t))
            yield return r;
}

var neighbors = new[]
{
    new { Name = "John", Pets = new string[] {"Fluffy", "Thor"} },
    new { Name = "Tim", Pets = new string[] {} },
    new { Name = "Carl", Pets = new string[] {"Sybil"} },
};
IEnumerable<IEnumerable<string>> nested = neighbors.Map(n => n.Pets);
// => [["Fluffy", "Thor"], [], ["Sybil"]]
IEnumerable<string> flat = neighbors.Bind(n => n.Pets);
// => ["Fluffy", "Thor", "Sybil"]

class Subject
{
    public Option<Age> Age { get; set; }
}
IEnumerable<Subject> Population => new[]
{
    new Subject { Age = Age.Of(33) },
    new Subject { },
    new Subject { Age = Age.Of(37) },
};

public static IEnumerable<R> Bind<T, R>
    (this IEnumerable<T> list, Func<T, Option<R>> func)
        => list.Bind(t => func(t).AsEnumerable());
public static IEnumerable<R> Bind<T, R>
    (this Option<T> opt, Func<T, IEnumerable<R>> func)
        => opt.AsEnumerable().Bind(func);

var optionalAges = Population.Map(p => p.Age);
// => [Some(Age(33)), None, Some(Age(37))]
var statedAges = Population.Bind(p => p.Age);
// => [Age(33), Age(37)]
//var averageAge = statedAges.Map(age => age.Value).Average();
// => 35

/* Exercises */

// 1 Implement Map for ISet<T> and IDictionary<K, T>. 

// Map : ISet<T> -> (T -> R) -> ISet<R>
static ISet<R> Map<T, R>(this ISet<T> ts, Func<T, R> f)
{
    //var rs = new HashSet<R>();
    //foreach (var t in ts)
    //    rs.Add(f(t));
    //return rs;
    return ts.AsEnumerable().Map(f).ToHashSet();
}

// Map : IDictionary<K, T> -> (T -> R) -> IDictionary<K, R>
static IDictionary<K, R> Map<K, T, R>
   (this IDictionary<K, T> dict, Func<T, R> f)
{
    //var rs = new Dictionary<K, R>();
    //foreach (var pair in dict)
    //    rs[pair.Key] = f(pair.Value);
    //return rs;
    return dict.Keys.Zip(dict.Values.Map(f), (k, v) => new { k, v })
            .ToDictionary(x => x.k, x => x.v);
}
// 2 Implement Map for Option and IEnumerable in terms of Bind and Return.
public static Option<R> Map<T, R>(this Option<T> opt, Func<T, R> f)
   => opt.Bind(t => Some(f(t)));

public static IEnumerable<R> Map<T, R>(this IEnumerable<T> ts, Func<T, R> f)
   => ts.Bind(t => List(f(t)));

// 3 Use Bind and an Option-returning Lookup function (such as the one we defined
// in chapter 3) to implement GetWorkPermit, shown below. 

public struct WorkPermit
{
    public string Number { get; set; }
    public DateTime Expiry { get; set; }
}

public class Employee
{
    public string Id { get; set; }
    public Option<WorkPermit> WorkPermit { get; set; }

    public DateTime JoinedOn { get; }
    public Option<DateTime> LeftOn { get; }
}
static Option<WorkPermit> GetWorkPermit(Dictionary<string, Employee> employees, string employeeId)
   => employees.Lookup(employeeId).Bind(e => e.WorkPermit);


static Option<WorkPermit> GetValidWorkPermit(Dictionary<string, Employee> employees, string employeeId)
   => employees
      .Lookup(employeeId)
      .Bind(e => e.WorkPermit)
      .Where(HasExpired.Negate());

static Func<WorkPermit, bool> HasExpired => permit => permit.Expiry < DateTime.Now.Date;

// 4 Use Bind to implement AverageYearsWorkedAtTheCompany, shown below (only
// employees who have left should be included).

static double AverageYearsWorkedAtTheCompany(List<Employee> employees)
   => employees
      .Bind(e => e.LeftOn.Map(leftOn => YearsBetween(e.JoinedOn, leftOn)))
      .Average();

// a more elegant solution, which will become clear in Chapter 9
static double AverageYearsWorkedAtTheCompany_LINQ(List<Employee> employees)
   => (from e in employees
       from leftOn in e.LeftOn
       select YearsBetween(e.JoinedOn, leftOn)
      ).Average();

static double YearsBetween(DateTime start, DateTime end)
   => (end - start).Days / 365d;