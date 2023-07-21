using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoBogus;
using Xunit;

using static System.Linq.Enumerable;
using Range = System.Range;

namespace Functional_Programming_in_CSharp._3_why_function_purity_matters;

static class Extensions
{
    public static string ToSentenceCase(this string s)
        => s == string.Empty
        ? string.Empty
        : char.ToUpperInvariant(s[0]) + s.ToLower()[1..];
}

class ImpureListFormatter
{
    private int counter;
    
    string PrependCounter(string s) => $"{++counter}. {s}";

    public List<string> Format(List<string> list)
        => list
            .AsParallel()
            .Select(Extensions.ToSentenceCase)
            .Select(PrependCounter)
            .ToList();
}

public static class PureListFormatter
{
    public static List<string> Format(List<string> list)
        => list
            .Select(Extensions.ToSentenceCase)
            .Zip(Range(1, list.Count), (s, i) => $"{i}. {s}")
            .ToList();
}

public class Tests
{
    [Fact]
    public void ImpureListFormatterTest()
    {
        var listOfStrings = new AutoFaker<string>()
            .UseSeed(42)
            .Generate(9);
        var formatter = new ImpureListFormatter();
        var formatted = formatter.Format(listOfStrings);
        var order = formatted
            .Select(x => x.First().ToString())
            .Select(int.Parse);
        order.Should().BeInAscendingOrder();
    }
    
    [Fact]
    public void PureListFormatterTest()
    {
        var listOfStrings = new AutoFaker<string>()
            .UseSeed(42)
            .Generate(9);
        var formatted = PureListFormatter.Format(listOfStrings);
        var order = formatted
            .Select(x => x.First().ToString())
            .Select(int.Parse);
        order.Should().BeInAscendingOrder();
    }
}
