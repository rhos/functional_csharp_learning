// using static System.Linq.Enumerable;
using static System.Linq.ParallelEnumerable;

var test = Enumerable.Zip(new[] { 1, 2, 3 }, new[] { "one", "two", "three" }, (num, str) => $"{str}:{num}");

var shoppingList = new List<string> { "coffee beans", "BANANAS", "Dates" };
public static string ToSentenceCase(this string s) => s.ToUpper()[0] + s.ToLower().Substring(1);
public static IEnumerable<string> Format(IEnumerable<string> strings)
    => strings.AsParallel()
        .Select(ToSentenceCase)
        .Zip(Range(1, strings.Count()), (s, i) => $"{i}. {s}");