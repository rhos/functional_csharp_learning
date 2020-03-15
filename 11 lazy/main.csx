#r "../../functional-csharp-code/LaYumba.Functional/bin/Debug/netstandard1.6/LaYumba.Functional.dll"
using LaYumba.Functional;
using static LaYumba.Functional.F;


var rand = new Random();

T Pick<T>(Func<T> l, Func<T> r)
    => rand.NextDouble() < 0.5 ? l() : r();

var pick = Pick(() => 1 + 2, () => 3 + 4);
Console.WriteLine(pick);

interface IRepository<T> 
{ 
    Option<T> Lookup(Guid id); 
}
class CachingRepository<T> : IRepository<T>
{
    IDictionary<Guid, T> cache;
    IRepository<T> db;
    public Option<T> Lookup(Guid id)
        // => cache.Lookup(id).OrElse(db.Lookup(id));
        => cache.Lookup(id).OrElse(() => db.Lookup(id));
}

public static Func<R> Map<T, R>(this Func<T> f, Func<T, R> g)
    => () => g(f());

public static Func<R> Bind<T, R>(this Func<T> f, Func<T, Func<R>> g)
    => () => g(f())();

public delegate Exceptional<T> Try<T>();
public static Exceptional<T> Run<T>(this Try<T> @try)
{
    try { return @try(); }
    catch (Exception ex) { return ex; }
}

Try<Uri> CreateUri(string uri) => () => new Uri(uri);
var test = CreateUri("test").Run();
// var test2 = CreateUri("test")();
public static Try<R> Bind<T, R>(this Try<T> @try, Func<T, Try<R>> g)
    => ()
        => @try.Run().Match(
            Exception: ex => ex,
            Success: t => g(t).Run());

public static Try<R> Map<T, R>(this Try<T> @try, Func<T, R> g)
    => ()
        => @try.Run().Match<Exceptional<R>>(
            Exception: ex => ex,
            Success: t => g(t));

Console.WriteLine("done");
