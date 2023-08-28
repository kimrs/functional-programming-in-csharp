using FluentAssertions;
using Unit = System.ValueTuple;

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

    public static readonly NoneType None = default;
}

public struct None
{
    internal static readonly None Default = new None();
}

public record None<T> : Option<T>
{
    
}

public record Some<T>
    : Option<T>
{
    public void Deconstruct(out T o)
    {
        throw new NotImplementedException();
    }
}

public class Tests
{
    static readonly NoneType None;
    
    [Fact]
    public void GreetNoneTest()
    {
        var greeting = Greet(None);
        greeting.Should().Be("Sorry, who?");
    }

    private static string Greet(Option<string> greetee)
        => greetee.Match(
            () => "Sorry, who?",
            name => $"Hello, {name}"
        );
}
