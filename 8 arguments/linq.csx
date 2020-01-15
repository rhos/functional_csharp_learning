#r "../../functional-csharp-code/LaYumba.Functional/bin/Debug/netstandard1.6/LaYumba.Functional.dll"
using System.Linq;
using static System.Linq.Enumerable;
using LaYumba.Functional;
using static LaYumba.Functional.F;

var enumerableSelect = from x in Enumerable.Range(1, 4)
                       select x * 2;

var optionSelect = from x in Some(12)
                   select x * 2;

var noneSelect = from x in (Option<int>)None
                 select x * 2;

bool check = (from x in Some(1) select x * 2) == Some(1).Map(x => x * 2);

var chars = new[] { 'a', 'b', 'c' };
var ints = new[] { 2, 3 };
var comb = from c in chars
           from i in ints
           select (c, i);

var comb2 = chars
    .Bind(c => ints
        .Bind<int, (char, int)>(i => (c, i)));

var s1 = "1";
var s2 = "2";
// 1. using LINQ query
var linqRes = from a in Int.Parse(s1)
              from b in Int.Parse(s2)
              select a + b;
// 2. normal method invocation
var bindRes = Int.Parse(s1)
    .Bind(a => Int.Parse(s2)
        .Bind(b => Some(a + b)));
// 3. the method invocation that the LINQ query will be converted to
var convRes = Int.Parse(s1)
    .SelectMany(a => Int.Parse(s2)
        , (a, b) => a + b);
// 4. using Apply
var applyRes = Some(new Func<int, int, int>((a, b) => a + b))
    .Apply(Int.Parse(s1))
    .Apply(Int.Parse(s2));

    