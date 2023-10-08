using System.Collections.Immutable;
using FluentAssertions;
using Functional_Programming_in_CSharp._4_designing_function_signatures_and_types;
using Functional_Programming_in_CSharp._5_modeling_the_possible_absence_of_data;
using static Functional_Programming_in_CSharp._5_modeling_the_possible_absence_of_data.F;
using static System.Console;
using Unit = System.ValueTuple;


namespace Functional_Programming_in_CSharp._6_patterns_in_functional_programming;

internal static class Extensions
{
    public static IEnumerable<R> Map<T, R>(this IEnumerable<T> ts, Func<T, R> f)
        => ts.Select(f);

    public static Option<R> Map<T, R>(this Option<T> optT, Func<T, R> f)
        => optT.Match(
            () => None,
            (t) => Some(f(t))
        );

    public static Option<Unit> ForEach<T>(this Option<T> opt, Action<T> action)
        => Map(opt, action.ToFunc());
    public static IEnumerable<Unit> ForEach<T>(this IEnumerable<T> ts, Action<T> action)
       => ts.Map(action.ToFunc()).ToImmutableList();

    public static Option<R> Bind<T, R>(
        this Option<T> optT,
        Func<T, Option<R>> f
    )
        => optT.Match
        (
            () => None,
            Some: f
        );
}

public class Tests
{
    [Fact]
    public void MapOptionTest()
        => Some("John")
            .Map(name => $"Hello {name}")
            .ForEach(WriteLine);
    
    [Fact]
    public void MapIEnumerableTest()
        => new [] { "Constance", "Albert" }
            .Map(name => $"Hello {name}")
            .ForEach(WriteLine);

    [Fact]
    public void MapSomeAgeTest()
        => Int.Parse("42")
            .Map(Age.Create)
            .ToString()
            .Should()
            .Be("Some { Value = Some { Value = Functional_Programming_in_CSharp._5_modeling_the_possible_absence_of_data.Age } }");

    [Fact]
    public void BindSomeAgeTest()
        => Int.Parse("42")
            .Bind(Age.Create)
            .ToString()
            .Should()
            .Be("Some { Value = Functional_Programming_in_CSharp._5_modeling_the_possible_absence_of_data.Age }");
    
    
}
