#r "../../functional-csharp-code/LaYumba.Functional/bin/Debug/netstandard1.6/LaYumba.Functional.dll"
#r "nuget: Dapper, 2.0.30"
#r "nuget: System.Data.SqlClient, 4.7.0"
#load"../2 purity/boc.csx"

using LaYumba.Functional;
using static LaYumba.Functional.F;
using System;
using System.Data;
using System.Data.SqlClient;
using Dapper;

using Name = System.String;
using Greeting = System.String;
using PersonalizedGreeting = System.String;

Func<Greeting, Name, PersonalizedGreeting> greet
    = (gr, name) => $"{gr}, {name}";

Name[] names = { "Tristan", "Ivan" };
names.Map(g => greet("Hello", g)).ForEach(WriteLine);

Func<Greeting, Func<Name, PersonalizedGreeting>> greetWith
    = gr => name => $"{gr}, {name}";

var helloGreet = greetWith("Hello");

public static Func<T2, R> Apply<T1, T2, R>
    (this Func<T1, T2, R> f, T1 t1)
        => t2 => f(t1, t2);

public static Func<T2, T3, R> Apply<T1, T2, T3, R>
    (this Func<T1, T2, T3, R> f, T1 t1)
        => (t2, t3) => f(t1, t2, t3);

var heyGreet = greet.Apply("hey");

public class TypeInference_Delegate
{
    string separator = "! ";
    // 1. field
    Func<Greeting, Name, PersonalizedGreeting> GreeterField
        = (gr, name) => $"{gr}, {name}";
    // 2. property
    Func<Greeting, Name, PersonalizedGreeting> GreeterProperty
        => (gr, name) => $"{gr}{separator}{name}";
    // 3. factory
    Func<Greeting, T, PersonalizedGreeting> GreeterFactory<T>()
        => (gr, t) => $"{gr}{separator}{t}";
}

public static Func<T1, Func<T2, R>> Curry<T1, T2, R>
    (this Func<T1, T2, R> func)
    => t1 => t2 => func(t1, t2);
public static Func<T1, Func<T2, Func<T3, R>>> Curry<T1, T2, T3, R>
    (this Func<T1, T2, T3, R> func)
    => t1 => t2 => t3 => func(t1, t2, t3);

public class ConnectionString
{
    string Value { get; }
    public ConnectionString(string value) { Value = value; }
    public static implicit operator string(ConnectionString c)
        => c.Value;
    public static implicit operator ConnectionString(string s)
        => new ConnectionString(s);
    public override string ToString() => Value;
}

public class SqlTemplate
{
    string Value { get; }
    public SqlTemplate(string value) { Value = value; }
    public static implicit operator string(SqlTemplate c)
        => c.Value;
    public static implicit operator SqlTemplate(string s)
        => new SqlTemplate(s);
    public override string ToString() => Value;
}

public static R Connect<R>(string connString, Func<IDbConnection, R> f)
{
    using (var conn = new SqlConnection(connString))
    {
        conn.Open();
        return f(conn);
    }
}

public static Func<SqlTemplate, object, IEnumerable<T>> Query<T>(this ConnectionString connString)
    => (sql, param)
    => Connect(connString, conn => conn.Query<T>(sql, param));

ConnectionString connString = "connection-string";
SqlTemplate sel = "SELECT * FROM EMPLOYEES"
    , sqlById = $"{sel} WHERE ID = @Id"
    , sqlByName = $"{sel} WHERE LASTNAME = @LastName";

class Employee
{

}
// (SqlTemplate, object)  IEnumerable<Employee>
var queryEmployees = connString.Query<Employee>();
// object  IEnumerable<Employee>
var queryById = queryEmployees.Apply(sqlById);
// object  IEnumerable<Employee>
var queryByLastName = queryEmployees.Apply(sqlByName);
// Guid  Option<Employee>#load"../purity/boc.csx"
Option<Employee> lookupEmployee(Guid id)
    => queryById(new { Id = id }).FirstOrDefault();
// string  IEnumerable<Employee>
IEnumerable<Employee> findEmployeesByLastName(string lastName)
    => queryByLastName(new { LastName = lastName });


public sealed class TransferDateIsPast : Error
{
    public override string Message { get; }
    = "Transfer date cannot be in the past";
}

public static class Errors
{
    public static TransferDateIsPast TransferDateIsPast
       => new TransferDateIsPast();
}

public delegate Validation<T> Validator<T>(T t);
public static Validator<MakeTransfer> DateNotPast(Func<DateTime> clock)
    =>
        cmd => cmd.Date.Date < clock().Date
            ? Errors.TransferDateIsPast
            : Valid(cmd);

public static Validator<T> FailFast<T>(IEnumerable<Validator<T>> validators)
    =>
        t => validators.Aggregate(Valid(t), (acc, validator) => acc.Bind(_ => validator(t)));

public static Validator<T> HarvestErrors<T>
    (IEnumerable<Validator<T>> validators)
    =>
        t =>
        {
            var errors = validators
                .Map(validate => validate(t))
                .Bind(v => v.Match(
                    Invalid: errs => Some(errs),
                    Valid: _ => None))
                .ToList();
            return errors.Count == 0
                ? Valid(t)
                : Invalid(errors.Flatten());
        };

// 1. 

public static Func<int, int, int> Remainder = (dividend, divisor)
   => dividend - ((dividend / divisor) * divisor);

// ApplyR : (((T1, T2) -> R), T2) -> T1 -> R
// ApplyR : (T1 -> T2 -> R) -> T2 -> T1 -> R (curried form)
static Func<T1, R> ApplyR<T1, T2, R>(this Func<T1, T2, R> func, T2 t2)
    => t1 => func(t1, t2);

static Func<int, int> RemainderWhenDividingBy5 = Remainder.ApplyR(5);

static Func<T1, T2, R> ApplyR<T1, T2, T3, R>(this Func<T1, T2, T3, R> func, T3 t3)
    => (t1, t2) => func(t1, t2, t3);

// 2. 

enum NumberType { Mobile, Home, Office }

class CountryCode
{
    string Value { get; }
    public CountryCode(string value) { Value = value; }
    public static implicit operator string(CountryCode c) => c.Value;
    public static implicit operator CountryCode(string s) => new CountryCode(s);
    public override string ToString() => Value;
}

class PhoneNumber
{
    public NumberType Type { get; }
    public CountryCode Country { get; }
    public string Number { get; }

    public PhoneNumber(NumberType type, CountryCode country, string number)
    {
        Type = type;
        Country = country;
        Number = number;
    }
}

static Func<CountryCode, NumberType, string, PhoneNumber> CreatePhoneNumber 
    = (country, type, number) => new PhoneNumber(type, country, number);

static Func<NumberType, string, PhoneNumber>
    CreateUkNumber = CreatePhoneNumber.Apply((CountryCode)"uk");

static Func<string, PhoneNumber>
    CreateUkMobileNumber = CreateUkNumber.Apply(NumberType.Mobile);


// 3. 

enum Level { Debug, Info, Error }

delegate void Log(Level level, string message);

static Log consoleLogger = (Level level, string message)
   => Console.WriteLine($"[{level}]: {message}");

static void Debug(this Log log, string message)
   => log(Level.Debug, message);

static void Info(this Log log, string message)
   => log(Level.Info, message);

static void Error(this Log log, string message)
   => log(Level.Error, message);

public static void _main()
   => ConsumeLog(consoleLogger);

static void ConsumeLog(Log log)
   => log.Info("this is an info message");