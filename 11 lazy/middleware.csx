#r "../../functional-csharp-code/LaYumba.Functional/bin/Debug/netstandard1.6/LaYumba.Functional.dll"
#r "nuget: System.Data.SqlClient, 4.7.0"
using System;
using System.Data;
using System.Data.SqlClient;
using LaYumba.Functional;

using Unit = System.ValueTuple;

using static LaYumba.Functional.F;


public delegate dynamic Middleware<T>(Func<T, dynamic> cont);

public static Middleware<R> Bind<T,R>(this Middleware<T> mw, Func<T, Middleware<R>> f)
    => cont
        => mw(t => f(t)(cont));

public static Middleware<R> Map<T,R>(this Middleware<T> mw, Func<T,R> f)
    => cont
        => mw(t => cont(f(t)));

public class LogMessage { }

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
public static class ConnectionHelper
{
    public static R Connect<R>
        (string connString, Func<SqlConnection, R> func)
    {
        // using (var conn = new SqlConnection(connString))
        // {
        // conn.Open();
        var conn = new SqlConnection();
        return func(conn);
        // }
    }

    public static R Transact<R>
        (SqlConnection conn, Func<SqlTransaction, R> f)
    {
        R r = default(R);
        using (var tran = conn.BeginTransaction())
        {
        r = f(tran);
        tran.Commit();
        }
        return r;
    }
}

public class DbLogger
{
    Middleware<SqlConnection> Connect;
    Func<string, Middleware<Unit>> Time;

    public DbLogger(ConnectionString connString)
    {
        Connect = f => 
            { 
                //do some shit
                var res = ConnectionHelper.Connect(connString, f);
                //do some more shit
                return res;
            };

        Time = 
            op => 
                f => 
                {
                    var t = f(Unit());
                    return Unit();
                };

    }
    public void Log(LogMessage message)
    {
        // Connect( conn => "some value from db");
        // Connect.Bind(Time)(conn => "some value from db");
        Time("weop").Bind(w => Connect)(conn => "some value from db2");
    }
    //  (
    //     from conn in Connect
    //     select conn.Execute("sp_create_log", message
    //         , commandType: CommandType.StoredProcedure)
    //     ).Run();
}

var test = new DbLogger("some string");
test.Log(new LogMessage());
