using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Functional_Programming_in_CSharp._2_higher_order_functions.Extensions;
using static System.Linq.Enumerable; 

namespace Functional_Programming_in_CSharp._2_higher_order_functions;

public static class Extensions
{
    public static Func<T2, T1, R> SwapArgs<T1, T2, R>(this Func<T1, T2, R> f)
        => (t2, t1) => f(t1, t2);

    public static Func<int, bool> IsMod(int n)
        => i => i % n == 0;
}

public class Tests
{
    // 2.2.2
    [Fact]
    public void AdapterFunctionTest()
    {
        var divide = (int x, int y) => x / y;
        var divideArgsSwaped = divide.SwapArgs();
        var five = divideArgsSwaped(2, 10);

        five.Should().Be(5);
    }

    // 2.2.3
    [Fact]
    public void FunctionFactoryTest()
    {
        var mod2 = Range(1, 10).Where(IsMod(2));
        var mod3 = Range(1, 10).Where(IsMod(3));
        mod2.Should().BeEquivalentTo(new [] {2, 4, 6, 8, 10});
        mod3.Should().BeEquivalentTo(new[] {3, 6, 9});
    }
}

