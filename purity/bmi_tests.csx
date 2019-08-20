#r "nuget:FluentAssertions, 4.19.4"
#load "nuget:ScriptUnit, 0.2.0"
#load "bmi.csx"

using FluentAssertions;
using static ScriptUnit;
return await AddTestsFrom<BmiTests>().Execute();

public class BmiTests
{
    [Arguments(1.80, 77, 23.77)]
    [Arguments(1.60, 77, 30.08)]
    public void TestCalcBmi(double height, double weight, double expected)
        => CalcBmi(height, weight).Should().BeApproximately(expected, 2);

    [Arguments(23.77, BmiRange.Healthy)]
    [Arguments(30.08, BmiRange.Overweight)]
    public void TestCalcBmiRange(double bmi, BmiRange range)
        => CalcBmiRange(bmi).ShouldBeEquivalentTo(range);

    [Arguments(1.80, 77, BmiRange.Healthy)]
    [Arguments(1.60, 77, BmiRange.Overweight)]
    public void TestRun(double height, double weight, BmiRange expected)
    {
        var result = default(BmiRange);
        Func<string, double> read = s => s == "height" ? height : weight;
        Action<BmiRange> write = r => result = r;

        Run(read, write);
        result.ShouldBeEquivalentTo(expected);
    }
}