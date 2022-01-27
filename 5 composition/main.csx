#r"../../functional-csharp-code/LaYumba.Functional/bin/Debug/netstandard1.6/LaYumba.Functional.dll"
using static LaYumba.Functional.F;
using LaYumba.Functional;
public interface IRepository<T>
{
    Option<T> Get(Guid id);
    void Save(Guid id, T t);
}
interface ISwiftService
{
    void Wire(MakeTransfer transfer, AccountState account);
}

public interface IValidator<T>
{
    bool IsValid(T t);
}

public class MakeTransfer
{
    public Guid DebitedAccountId { get; set; }
    public string Beneficiary { get; set; }
    public string Iban { get; set; }
    public string Bic { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}
public class AccountState
{
    public decimal Balance { get; }
    public AccountState(decimal balance) { Balance = balance; }
}
public static Option<AccountState> Debit
    (this AccountState acc, decimal amount)
        => (acc.Balance < amount)
            ? None
            : Some(new AccountState(acc.Balance - amount));

public class MakeTransferController
{
    IValidator<MakeTransfer> validator;
    IRepository<AccountState> accounts;
    ISwiftService swift;
    public void MakeTransfer(MakeTransfer transfer)
        => Some(transfer)
            //.Map(Normalize)
            .Where(validator.IsValid)
            .ForEach(Book);
            
    void Book(MakeTransfer transfer)
        => accounts.Get(transfer.DebitedAccountId)
            .Bind(account => account.Debit(transfer.Amount))
            .ForEach(account =>
            {
                accounts.Save(transfer.DebitedAccountId, account);
                swift.Wire(transfer, account);
            });
}

static Func<T1, R> Compose<T1, T2, R>(this Func<T2, R> g, Func<T1, T2> f)
   => x => g(f(x));