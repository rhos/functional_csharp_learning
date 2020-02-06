#r "../../functional-csharp-code/LaYumba.Functional/bin/Debug/netstandard1.6/LaYumba.Functional.dll"
// #load"../2 purity/boc.csx"
// #load"../9 data/main.csx"

using LaYumba.Functional;
using static LaYumba.Functional.F;

using CurrencyCode = System.String;

public enum AccountStatus
{
    Requested, Active, Frozen, Dormant, Closed
}

public abstract class Event
{
    public Guid EntityId { get; }
    public DateTime Timestamp { get; }
}
public sealed class CreatedAccount : Event
{
    public CurrencyCode Currency { get; }
}
public sealed class DepositedCash : Event
{
    public decimal Amount { get; }
    public Guid BranchId { get; }
}
public sealed class DebitedTransfer : Event
{
    public string Beneficiary { get; }
    public string Iban { get; }
    public string Bic { get; }
    public decimal DebitedAmount { get; }
    public string Reference { get; }
}
public sealed class FrozeAccount : Event
{

}

public sealed class AccountState
{
    public AccountStatus Status { get; }
    public CurrencyCode Currency { get; }
    public decimal Balance { get; }
    public decimal AllowedOverdraft { get; }

    public AccountState(CurrencyCode Currency
        , AccountStatus Status = AccountStatus.Requested
        , decimal AllowedOverdraft = 0
        , decimal Balance = 0)
    {
        this.Status = Status;
        this.Currency = Currency;
        this.AllowedOverdraft = AllowedOverdraft;
        this.Balance = Balance;
    }

    public AccountState WithStatus(AccountStatus newStatus)
        => new AccountState(
            Status: newStatus,
            Currency: this.Currency,
            AllowedOverdraft: this.AllowedOverdraft,
            Balance: this.Balance
            );

    public AccountState Credit(decimal amount)
        => new AccountState(
            Balance: this.Balance + amount,
            Currency: this.Currency,
            Status: this.Status,
            AllowedOverdraft: this.AllowedOverdraft
            );

    public AccountState Debit(decimal amount)
        => new AccountState(
            Balance: this.Balance - amount,
            Currency: this.Currency,
            Status: this.Status,
            AllowedOverdraft: this.AllowedOverdraft
            );
}

// enum Ripeness { Green, Yellow, Brown }
// abstract class Reward { }
// class Peanut : Reward { }
// class Banana : Reward { public Ripeness Ripeness; }

// string Describe(Reward reward)
//     => new Pattern<string>
//     {
//         (Peanut _) => "It's a peanut",
//         (Banana b) => $"It's a {b.Ripeness} banana"
//     }
//     .Default("It's a reward I don't know or care about")
//     .Match(reward);

public static AccountState Create(CreatedAccount evt)
    => new AccountState(
        Currency: evt.Currency,
        Status: AccountStatus.Active);

public static AccountState Apply(this AccountState account, Event evt)
    => new Pattern
        {
            (DepositedCash e) => account.Credit(e.Amount),
            (DebitedTransfer e) => account.Debit(e.DebitedAmount),
            (FrozeAccount _) => account.WithStatus(AccountStatus.Frozen),
        }
        .Match(evt);

public static Option<AccountState> From(IEnumerable<Event> history)
    => history.Match(
        Empty: () => None,
        Otherwise: (created, otherEvents) => Some(
            otherEvents.Aggregate(
                seed: Create((CreatedAccount)created),
                func: (state, evt) => state.Apply(evt))));