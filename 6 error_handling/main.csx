#r"../../functional-csharp-code/LaYumba.Functional/bin/Debug/netstandard1.6/LaYumba.Functional.dll"
#load"../2 purity/boc.csx"
using LaYumba.Functional;
using System.Text.RegularExpressions;
using static LaYumba.Functional.F;
using static System.Math;
using Unit = System.ValueTuple;

string Render(Either<string, double> val) =>
    val.Match(
        Left: l => $"Invalid value: {l}",
        Right: r => $"The result is: {r}");
var res1 = Render(Right(12d));
var res2 = Render(Left("oops"));

Either<string, double> Calc(double x, double y)
{
    if (y == 0) return "y cannot be 0";
    if (x != 0 && Sign(x) != Sign(y))
        return "x / y cannot be negative";
    return Sqrt(x / y);
}

public sealed class InvalidBic : Error
{
    public override string Message { get; }
    = "The beneficiary's BIC/SWIFT code is invalid";
}
public sealed class TransferDateIsPast : Error
{
    public override string Message { get; }
    = "Transfer date cannot be in the past";
}

public static class Errors
{
    public static InvalidBic InvalidBic
        => new InvalidBic();
    public static TransferDateIsPast TransferDateIsPast
       => new TransferDateIsPast();
}

public class BookTransferController
{
    DateTime now;
    Regex bicRegex = new Regex("[A-Z]{11}");
    Either<Error, Unit> Handle(MakeTransfer cmd)
    => Right(cmd)
        .Bind(ValidateBic)
        .Bind(ValidateDate)
        .Bind(Save);

    Either<Error, MakeTransfer> ValidateBic(MakeTransfer cmd)
    {
        if (!bicRegex.IsMatch(cmd.Bic))
            return Errors.InvalidBic;
        else return cmd;
    }
    Either<Error, MakeTransfer> ValidateDate(MakeTransfer cmd)
    {
        if (cmd.Date.Date <= now.Date)
            return Errors.TransferDateIsPast;
        else return cmd;
    }

    Either<Error, Unit> Save(MakeTransfer cmd) => Right(new Unit());
}

// 1.

static Option<R> ToOption<L, R>(this Either<L, R> @this)
    => @this.Match(Left: l => None, Right: r => Some(r));

static Either<L, R> ToEither<L, R>(this Option<R> @this, Func<L> left)
    => @this.Match<Either<L, R>>(() => left(), r => r);

// 2.

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
static Func<string, Option<Age>> parseAge = ageStr
   => Int.Parse(ageStr).Bind(Age.Of);

static Either<string, int> ParseIntVerbose(this string s)
   => Int.Parse(s).ToEither(() => $"'{s}' is not a valid representation of an int");

public static Option<RR> Bind<L, R, RR>(this Either<L, R> @this, Func<R, Option<RR>> func)
    => @this.Match(
       Left: _ => None,
       Right: r => func(r));

public static Option<RR> Bind<L, R, RR>(this Option<R> @this, Func<R, Either<L, RR>> func)
    => @this.Match(
       None: () => None,
       Some: v => func(v).ToOption());

static Func<string, Option<Age>> parseAge2 = s
   => s.ParseIntVerbose().Bind(Age.Of);

// 3. 

static Exceptional<T> Try<T>(Func<T> func)
{
    try { return func(); }
    catch (Exception ex) { return ex; }
}

// 4. 

static Either<L, R> Safely<L, R>(Func<R> func, Func<Exception, L> left)
{
    try { return func(); }
    catch (Exception ex) { return left(ex); }
}