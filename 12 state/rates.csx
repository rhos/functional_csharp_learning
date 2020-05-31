#r "../../functional-csharp-code/LaYumba.Functional/bin/Debug/netstandard1.6/LaYumba.Functional.dll"
#r "nuget: System.Collections.Immutable, 1.7.1"

using LaYumba.Functional;
using static LaYumba.Functional.F;

using System.Collections.Immutable;
using System.Net.Http;

using Rates = System.Collections.Immutable.ImmutableDictionary<string, decimal>;

static class Yahoo
{
    public static decimal GetRate(string ccyPair)
    {
        WriteLine($"fetching rate...");
        // var uri = $"http://finance.yahoo.com/d/quotes.csv?f=l1&s={ccyPair}=X";
        // var request = new HttpClient().GetStringAsync(uri);
        // return decimal.Parse(request.Result.Trim());
        return decimal.Parse("0.923123");
    }

    public static Try<decimal> TryGetRate(string ccyPair)
        => () => GetRate(ccyPair);
}

public static void Run()
    => MainRec("Enter a currency pair like 'EURUSD', or 'q' to quit"
        , Rates.Empty);

static void MainRec(string message, Rates cache)
{
    var input = ReadLine().ToUpper();
    if (input == "Q") return;
    GetRate(Yahoo.TryGetRate, input, cache).Run()
        .Match(
            ex => MainRec($"Error: {ex.Message}", cache),
            result => MainRec(result.Rate.ToString(), result.NewState));
}

static Try<(decimal Rate, Rates NewState)> GetRate
    (Func<string, Try<decimal>> getRate, string ccyPair, Rates cache)
{
    if (cache.ContainsKey(ccyPair))
        return Try(() => (cache[ccyPair], cache));
    else 
        return getRate(ccyPair)
            .Map(rate => (rate, cache.Add(ccyPair, rate)));
}

Run();