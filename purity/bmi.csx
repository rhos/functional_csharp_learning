using static System.Console;
using static System.Math;

public enum BmiRange { Underweight, Healthy, Overweight }

//IO ()
static void Run(Func<string, double> read, Action<BmiRange> write)
{
    var weight = read("weight");
    var height = read("height");

    var bmi = CalcBmiRange(CalcBmi(height, weight));
    write(bmi);
}
//IO
static double ReadDouble(string name)
{
    WriteLine($"{name}?:");
    return double.Parse(ReadLine());
}
//IO
private static void WriteBmiRange(BmiRange bmiRange)
    => WriteLine($"bmi: {bmiRange}");
static Func<double, double, double> CalcBmi =
    (double height, double weight) => weight / Pow(height, 2);

static Func<double, BmiRange> CalcBmiRange =
    (double bmi) =>
        bmi < 18.5
            ? BmiRange.Underweight
            : 25 <= bmi
                ? BmiRange.Overweight
                : BmiRange.Healthy;

