#r "../../functional-csharp-code/LaYumba.Functional/bin/Debug/netstandard1.6/LaYumba.Functional.dll"
using System;
using System.Data;
using LaYumba.Functional;

using static LaYumba.Functional.F;


public delegate dynamic Middleware<T>(Func<T, dynamic> cont);

public static Middleware<R> Bind<T,R>(this Middleware<T> mw, Func<T, Middleware<R>> f)
    => cont
        => mw(t => f(t)(cont));

public static Middleware<R> Map<T,R>(this Middleware<T> mw, Func<T,R> f)
    => cont
        => mw(t => cont(f(t)));