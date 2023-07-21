using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functional_Programming_in_CSharp;

public static class _2_Higher_order_functions
{
    // Used in [placeholder]
    public static Func<T2, T1, R> SwapArgs<T1, T2, R>(this Func<T1, T2, R> f)
        => (t2, t1) => f(t1, t2);

    public static Func<int, bool> IsMod(int n)
        => i => i % n == 0;
}
