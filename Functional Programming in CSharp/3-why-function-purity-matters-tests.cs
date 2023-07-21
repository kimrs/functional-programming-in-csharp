using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Functional_Programming_in_CSharp;

static class StringExt
{
    public static string ToSentenceCase(this string s)
        => s == string.Empty
        ? string.Empty
        : char.ToUpperInvariant(s[0]) + s.ToLower()[1..];
}

public class _3_why_function_purity_matters_tests
{
    [Fact]
    public void ListFormatterTest()
    {
        var str = "HELLO WORLD";
        var res = str.ToSentenceCase();

        res.Should().Be("Hello world");
    }
}
