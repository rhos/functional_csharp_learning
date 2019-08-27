#r"../../functional-csharp-code/LaYumba.Functional/bin/Debug/netstandard1.6/LaYumba.Functional.dll"
using LaYumba.Functional;
using static LaYumba.Functional.F;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Collections.Generic;
Option<string> _ = None;
Option<string> john = Some("John");
string greet(Option<string> greetee)
    => greetee.Match(
        None: () => "Sorry, who?",
        Some: (name) => $"Hello, {name}");

greet(None); // => "Sorry, who?"
greet(Some("John")); // => "Hello, John"

public static Option<T> Parse<T>(this string s) where T : struct
   => System.Enum.TryParse(s, out T t) ? Some(t) : None;
public static Option<T> Lookup<T>(this IEnumerable<T> ts, Func<T, bool> pred)
{
    foreach (T t in ts) if (pred(t)) return Some(t);
    return None;
}

public class Email
{
    static readonly Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

    private string Value { get; }

    private Email(string value) => Value = value;

    public static Option<Email> Create(string s)
       => regex.IsMatch(s)
          ? Some(new Email(s))
          : None;

    public static implicit operator string(Email e)
       => e.Value;
}

public class AppConfig
{
    private NameValueCollection Source { get; }

    public AppConfig(NameValueCollection source)
    {
        this.Source = source;
    }

    public Option<T> Get<T>(string key)
       => Source[key] == null
          ? None
          : Some((T)Convert.ChangeType(Source[key], typeof(T)));

    public T Get<T>(string key, T defaultValue)
       => Get<T>(key).Match(
          () => defaultValue,
          (value) => value);
}