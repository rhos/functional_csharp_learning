using System.Text.RegularExpressions;

public abstract class Command { }
public sealed class MakeTransfer : Command
{
    public Guid DebitedAccountId { get; set; }
    public string Beneficiary { get; set; }
    public string Iban { get; set; }
    public string Bic { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}
public interface IValidator<T>
{
    bool IsValid(T t);
}
public sealed class BicFormatValidator : IValidator<MakeTransfer>
{
    static readonly Regex regex = new Regex("^[A-Z]{6}[A-Z1-9]{5}$");
    public bool IsValid(MakeTransfer cmd)
        => regex.IsMatch(cmd.Bic);
}
public interface IDateTimeService
{
    DateTime UtcNow { get; }
}
public class DefaultDateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
}
public class DateNotPastValidator : IValidator<MakeTransfer>
{
    private readonly IDateTimeService clock;
    public DateNotPastValidator(IDateTimeService clock)
    {
        this.clock = clock;
    }
    public bool IsValid(MakeTransfer request)
        => clock.UtcNow.Date <= request.Date.Date;
}

public sealed class BicExistsValidator : IValidator<MakeTransfer>
{
    readonly Func<IEnumerable<string>> getValidCodes;
    public BicExistsValidator(Func<IEnumerable<string>> getValidCodes)
    {
        this.getValidCodes = getValidCodes;
    }
    public bool IsValid(MakeTransfer cmd)
        => getValidCodes().Contains(cmd.Bic);
}