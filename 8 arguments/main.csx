#r "../../functional-csharp-code/LaYumba.Functional/bin/Debug/netstandard1.6/LaYumba.Functional.dll"
#r "nuget: xunit , 2.4.1"
#r "nuget: FsCheck.Xunit , 2.14.0"

using FsCheck.Xunit;
using Xunit;
using LaYumba.Functional;
using static LaYumba.Functional.F;

Func<int, int> @double = i => i * 2;
Some(3).Map(@double);//.ForEach(WriteLine);

Func<int, Func<int, int>> multiplyA = x => y => x * y;
var multBy3A = Some(3).Map(multiplyA);

Func<int, int, int> multiply = (x, y) => x * y;
var multBy3 = Some(3).Map(multiply);

public static Option<R> Apply<T, R>
    (this Option<Func<T, R>> optF, Option<T> optT)
    => optF.Match(
        () => None,
        (f) => optT.Match(
            () => None,
            (t) => Some(f(t))));

public static Option<Func<T2, R>> Apply<T1, T2, R>
    (this Option<Func<T1, T2, R>> optF, Option<T1> optT)
    => Apply(optF.Map(F.Curry), optT);

[Property]
void ApplicativeLawHolds(int a, int b)
{
    var first = Some(multiply)
        .Apply(Some(a))
        .Apply(Some(b));
    var second = Some(a)
        .Map(multiply)
        .Apply(Some(b));
    Assert.Equal(first, second);
}

Option<int> MultiplicationWithBind(string strX, string strY)
    => Int.Parse(strX)
        .Bind(x => Int.Parse(strY)
            .Bind<int, int>(y => multiply(x, y)));

Func<double, Option<double>> safeSqrt = d
    => d < 0 ? None : Some(Math.Sqrt(d));
    
void AssociativityHolds(Option<string> m)
    => Assert.Equal(
        m.Bind(Double.Parse).Bind(safeSqrt),
        m.Bind(x => Double.Parse(x).Bind(safeSqrt))
        );

public static Either<E, R> Apply<T, R, E>
    (this Either<E, Func<T, R>> eitherF, Either<E, T> eitherT)
    => eitherF.Match<Either<E, R>>(
        (l) => Left(l),
        (f) => eitherT.Match<Either<E, R>>(
            (l) => Left(l),
            (t) => Right(f(t))));

public static Either<E, Func<T2, R>> Apply<T1, T2, R, E>
    (this Either<E, Func<T1, T2, R>> eitherF, Either<E, T1> eitherT)
    => Apply(eitherF.Map(F.Curry), eitherT);

void MultiplyEither(int a, int b)
{
    var eitherMultiply = Right(multiply);
    Either<string, Func<int, int, int>> test = eitherMultiply;
    var res = test.Apply(Right(a)).Apply(b);
}
MultiplyEither(2, 3);

public static Either<L, R> Select<L, T, R>(this Either<L, T> @this
   , Func<T, R> map) => @this.Map(map);


public static Either<L, RR> SelectMany<L, T, R, RR>(this Either<L, T> eitherF
   , Func<T, Either<L, R>> bind, Func<T, R, RR> project)
   => eitherF.Match(
      Left: l => Left(l),
      Right: t =>
         bind(t).Match<Either<L, RR>>(
            Left: l => Left(l),
            Right: r => project(t, r)));