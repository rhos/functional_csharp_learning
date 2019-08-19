using static System.Math;
using System.Linq;
public class Circle
{
    public Circle(double radius)
        => Radius = radius;
    public double Radius { get; }
    public double Circumference
        => PI * 2 * Radius;
    public double Area
    {
        get
        {
            double Square(double d) => Pow(d, 2);
            return PI * Square(Radius);
        }
    }
    public (double Circumference, double Area) Stats
        => (Circumference, Area);
}

public delegate int Comparison<in T>(T x, T y);
IEnumerable<int> ComparisonTest()
{
    var list = Enumerable.Range(1, 10).Select(i => i * 3).ToList();
    Comparison<int> alphabetically = (l, r)
        => l.ToString().CompareTo(r.ToString());
    //list.Sort(alphabetically);
    list.Sort((l, r) => l.ToString().CompareTo(r.ToString()));
    return list;
}

static Func<T2, T1, R> SwapArgs<T1, T2, R>(this Func<T1, T2, R> f)
    => (t2, t1) => f(t1, t2);

public class Person : IDisposable
{
    public void Feed()
    {
        Console.WriteLine("last meal");
    }
    public void Dispose()
    {
        Console.WriteLine("disposing person");
    }
}
public static R Using<TDisp, R>(TDisp disposable, Func<TDisp, R> f)
    where TDisp : IDisposable
{
    using (disposable) return f(disposable);
}

static R Using<TDisp, R>(Func<TDisp> createDisposable
    , Func<TDisp, R> func) where TDisp : IDisposable
{
    using (var disp = createDisposable()) return func(disp);
}
public static R InPerson<R>(Func<Person, R> f)
    => Using(new Person(), person => { return f(person); });

public static Func<T, bool> Negate<T>(Func<T, bool> pred) => t => !pred(t);

public static IEnumerable<int> QuickSort(IEnumerable<int> unsorted)
{
    var p = unsorted.First();
    var rest = unsorted.Skip(1);
    return rest.Where(val => val < p)
        .Append(p)
        .Concat(rest.Where(val => val >= p));
}

public static IEnumerable<int> QuickSort(IEnumerable<int> unsorted, Comparison<int> predicate)
{
    var p = unsorted.First();
    var rest = unsorted.Skip(1);
    return rest.Where(val => predicate(val, p) <= 0)
        .Append(p)
        .Concat(rest.Where(val => predicate(val, p) > 0));
}
// static List<int> QSort(this List<int> list)
//     => list.Match(
//         () => List<int>(),
//         (pivot, rest) => rest.Where(i => i <= pivot).ToList().QSort()
//             .Append(pivot)
//             .Concat(rest.Where(i => pivot < i).ToList().QSort())
//     ).ToList();