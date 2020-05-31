#r "../../functional-csharp-code/LaYumba.Functional/bin/Debug/netstandard1.6/LaYumba.Functional.dll"
#r "nuget: System.Collections.Immutable, 1.7.1"

using LaYumba.Functional;
using static LaYumba.Functional.F;

using System.Collections.Immutable;

public delegate (T Value, int Seed) Generator<T>(int seed);

public static T Run<T>(this Generator<T> gen, int seed)
    => gen(seed).Value;
public static T Run<T>(this Generator<T> gen)
    => gen(Environment.TickCount).Value;

public static Generator<int> NextInt = 
    (seed) =>
    {
        seed ^= seed >> 13;
        seed ^= seed << 18;
        int result = seed & 0x7fffffff;
        return (result, result);
    };

public static Generator<bool> NextBoolOld = (seed) =>
{
    var (i, newSeed) = NextInt(seed);
    return (i % 2 == 0, newSeed);
};

public static Generator<R> Map<T, R>
    (this Generator<T> gen, Func<T, R> f)
    => seed =>
        {
            var (t, newSeed) = gen(seed);
            return (f(t), newSeed);
        };

public static Generator<R> Bind<T, R>
    (this Generator<T> gen, Func<T, Generator<R>> f)
    => seed0 =>
        {
            var (t, seed1) = gen(seed0);
            return f(t)(seed1);
        };

public static Generator<bool> NextBool =>
    NextInt.Map(i => i%2 == 0);

public static Generator<char> NextChar =>
    NextInt.Map(i => (char)(i % (char.MaxValue + 1)));

public static Generator<(int, int)> PairOfInts =>
    NextInt.Bind(a => 
        NextInt.Map(b => (a,b)));

var pairTest = PairOfInts.Run();

public static Generator<Option<int>> OptionInt =>
    NextBool.Bind(some =>
        NextInt.Map(i => some ? Some(i) : None));

public static Generator<T> Return<T>(T value)
    => seed => (value, seed);

static Generator<IEnumerable<int>> Empty
    => Return(Enumerable.Empty<int>());

public static Generator<IEnumerable<int>> IntList
    => NextBool.Bind(empty => empty ? Empty : Empty);

static Generator<IEnumerable<int>> NonEmpty
    => NextInt.Bind(head => 
            IntList.Map(tail => List(head).Concat(tail)));


Console.WriteLine("done");