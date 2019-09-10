#r"../../functional-csharp-code/LaYumba.Functional/bin/Debug/netstandard1.6/LaYumba.Functional.dll"
using static System.Console;
using static LaYumba.Functional.F;
using String = LaYumba.Functional.String;
using LaYumba.Functional;

Option<string> name = Some("Enrico");
name
    .Map(String.ToUpper)
    .ForEach(WriteLine);

IEnumerable<string> names = new[] { "Constance", "Albert" };
names
    .Map(String.ToUpper)
    .ForEach(WriteLine);