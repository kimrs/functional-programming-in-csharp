using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoBogus;
using Xunit;

using static System.Linq.Enumerable;
using Range = System.Range;
using Unit = System.ValueTuple;

namespace Functional_Programming_in_CSharp._4_designing_function_signatures_and_types;

static class Extensions
{
    public static Func<Unit> ToFunc(this Action action)
        => () => { action(); return default; };
}

public static class Instrumentation
{
    public static void Time(string op, Action action)
        => Time(op, action.ToFunc());
    public static T Time<T>(string op, Func<T> f)
    {
        var sw = new Stopwatch();
        sw.Start();
        var t = f();
        sw.Stop();
        Console.WriteLine($"{op} took {sw.ElapsedMilliseconds}ms");
        return t;
    }
}

public static class Permutations
{
    public static void NumberOfPermutations()
    {
        var number = 0;
        const int r = 100;
        foreach (var i in Range(0, r))
        {
            foreach (var j in Range(0, r))
            {
                foreach (var k in Range(0, r))
                {
                    number++;
                }
            }
        }
    }
}


public class Tests
{
    [Fact]
    public void ImpureListFormatterTest()
    {
        Instrumentation.Time("test", Permutations.NumberOfPermutations);
    }
    
}
