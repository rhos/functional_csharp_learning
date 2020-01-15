#r "nuget:FluentAssertions, 4.19.4"
#load "nuget:ScriptUnit, 0.2.0"
#load "boc.csx"

using FluentAssertions;
using static ScriptUnit;
return await AddTestsFrom<BocTests>().Execute();

public class BocTests
{
    public void WhenTransferDateIsFuture_ThenValidationPasses()
    {
        var transfer = new MakeTransfer { Date = new DateTime(2020, 12, 12) };
        var validator = new DateNotPastValidator(new DefaultDateTimeService());
        var actual = validator.IsValid(transfer);
        actual.Should().BeTrue();
    }
    static DateTime presentDate = new DateTime(2016, 12, 12);
    private class FakeDateTimeService : IDateTimeService
    {
        public DateTime UtcNow => presentDate;
    }
    [Arguments(+1, true)]
    [Arguments(0, true)]
    [Arguments(-1, false)]
    public void WhenTransferDateIsPast_ThenValidatorFails(int offset, bool result)
    {
        var sut = new DateNotPastValidator(new FakeDateTimeService());
        var cmd = new MakeTransfer { Date = presentDate.AddDays(offset) };
        sut.IsValid(cmd).Should().Be(result);
    }
    static string[] validCodes = { "ABCDEFGJ123" };
    [Arguments("ABCDEFGJ123", true)]
    [Arguments("XXXXXXXXXXX", false)]
    public void WhenBicNotFound_ThenValidationFails(string bic, bool result)
        => new BicExistsValidator(() => validCodes)
            .IsValid(new MakeTransfer { Bic = bic })
            .Should().Be(result);
}