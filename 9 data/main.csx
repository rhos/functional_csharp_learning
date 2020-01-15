#r "../../functional-csharp-code/LaYumba.Functional/bin/Debug/netstandard1.6/LaYumba.Functional.dll"

using LaYumba.Functional;
using static LaYumba.Functional.F;
using System.Collections.Immutable;
using CurrencyCode = System.String;
using Transaction = System.Int16;

public enum AccountStatus
{
    Requested, Active, Frozen, Dormant, Closed
}
public sealed class AccountState
{
    public AccountStatus Status { get; }
    public CurrencyCode Currency { get; }
    public decimal AllowedOverdraft { get; }
    public IEnumerable<Transaction> Transactions { get; }

    public AccountState(CurrencyCode Currency
        , AccountStatus Status = AccountStatus.Requested
        , decimal AllowedOverdraft = 0
        , IEnumerable<Transaction> Transactions = null)
    {
        this.Status = Status;
        this.Currency = Currency;
        this.AllowedOverdraft = AllowedOverdraft;
        this.Transactions = ImmutableList.CreateRange
            (Transactions ?? Enumerable.Empty<Transaction>());
    }

    // public AccountState WithStatus(AccountStatus newStatus)
    //     => new AccountState(
    //         Status: newStatus,
    //         Currency: this.Currency,
    //         AllowedOverdraft: this.AllowedOverdraft,
    //         Transactions: this.Transactions
    //         );

    public AccountState With( AccountStatus? Status = null
                            , decimal? AllowedOverdraft = null
                            , CurrencyCode Currency = null)
        => new AccountState(
            Status: Status ?? this.Status,
            AllowedOverdraft: AllowedOverdraft ?? this.AllowedOverdraft,
            Currency: Currency ?? this.Currency,
            Transactions: this.Transactions);

    public AccountState Add(Transaction t)
        => new AccountState(
            Transactions: Transactions.Prepend(t),
            Currency: this.Currency,
            Status: this.Status,
            AllowedOverdraft: this.AllowedOverdraft
            );

    public AccountState Freeze()
        => this.With(Status: AccountStatus.Frozen);

    public AccountState RedFlag()
        => this.With(
            Status: AccountStatus.Frozen,
            AllowedOverdraft: 0m);
}

var oldState = new AccountState("EUR", AccountStatus.Active);
var newState = oldState.With(a => a.Status, AccountStatus.Frozen);
var testStatus = oldState.Status; // => AccountStatus.Active
testStatus = newState.Status; // => AccountStatus.Frozen
var testCurr = newState.Currency; // => "EUR"