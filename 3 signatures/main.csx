using System.Collections.Generic;
using System.Collections.Specialized;
using Unit = System.ValueTuple;
enum Risk { Low, Medium, High }
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
}
Risk CalculateRiskProfile(Age age)
    => (age < 60) ? Risk.Low : Risk.Medium;

public static Func<Unit> ToFunc(this Action action)
    => () => { action(); return default(Unit); };
public static Func<T, Unit> ToFunc<T>(this Action<T> action)
    => (t) => { action(t); return default(Unit); };

public static void Test()
{
    try
    {
        var empty = new NameValueCollection();
        var green = empty["green"];
        Console.WriteLine("green!");
        var alsoEmpty = new Dictionary<string, string>();
        var blue = alsoEmpty["blue"];
        Console.WriteLine("blue!");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.GetType().Name);
    }
}