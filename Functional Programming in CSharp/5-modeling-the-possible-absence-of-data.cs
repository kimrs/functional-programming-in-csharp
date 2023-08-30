using System.Collections.Specialized;
using FluentAssertions;
using Unit = System.ValueTuple;
using static Functional_Programming_in_CSharp._5_modeling_the_possible_absence_of_data.F;

namespace Functional_Programming_in_CSharp._5_modeling_the_possible_absence_of_data;

internal static class Extensions
{
    public static R Match<T, R>(this Option<T> opt, Func<R> None, Func<T, R> Some)
        => opt switch
        {
            None<T> => None(),
            Some<T>(var t) => Some(t),
            _ => throw new ArgumentException("Option must be None or Some")
        };
}

public struct NoneType { }

public abstract record Option<T>
{
    public static implicit operator Option<T>(NoneType _)
        => new None<T>();

    public static implicit operator Option<T>(T value)
        => value is null ? None : Some(value);

    // public static readonly NoneType None = default;
}

public record None<T> : Option<T> { }

public record Some<T>
    : Option<T>
{
    private T Value { get; }

    public Some(T value)
        => Value = value ?? throw new ArgumentNullException();

    public void Deconstruct(out T value)
        => value = Value;
}

public static class F
{
    public static readonly NoneType None = default;
    public static Option<T> Some<T>(T t) => new Some<T>(t);
}

public static class Int
{
    public static Option<int> Parse(string s)
        => int.TryParse(s, out var result)
            ? Some(result)
            : None;
}

public static class CollectionExtension
{
    public static Option<string> Lookup(this NameValueCollection collection, string key)
        => collection[key]; // works because of implicit operator

    public static Option<T> Lookup<K, T>(this IDictionary<K, T> dictionary, K key)
        => dictionary.TryGetValue(key, out T value) ? Some(value) : None;
}

public class Tests
{
    [Fact]
    public void GreetNoneTest()
        => Greet(None).Should().Be("Sorry, who?");
    
    [Fact]
    public void GreetSomeTest()
        => Greet(Some("John")).Should().Be("Hello, John");

    [Fact]
    public void ParseLegalIntTest()
        => Int.Parse("42").Should().Be(Some(42));

    [Fact]
    public void ParseIllegalIntTest()
        => Int.Parse("Hello").Should().Be((Option<string>) None);
    
    [Fact]
    public void LookupEmptyNameValueCollection()
        => new NameValueCollection().Lookup("green").Should().Be((Option<string>) None);
    
    [Fact]
    public void LookupNameValueCollection()
        => new NameValueCollection { {"green", "green value"}}
            .Lookup("green").Should().Be(Some("green value"));

    [Fact]
    public void LookupEmptyDictionary()
        => new Dictionary<int, string>().Lookup(42).Should().Be((Option<string>) None);
    
    [Fact]
    public void LookupDictionary()
        => new Dictionary<int, string> { {42, "blue"}}.Lookup(42).Should().Be(Some("blue"));

    private static string Greet(Option<string> greetee)
        => greetee.Match(
            () => "Sorry, who?",
            name => $"Hello, {name}"
        );
}
