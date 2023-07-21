using Functional_Programming_in_CSharp;
using static Functional_Programming_in_CSharp._2_Higher_order_functions;
using static System.Linq.Enumerable; 

// 2.2.2 Adapter function
var divide = (int x, int y) => x / y;
var divideSwapped = divide.SwapArgs();
var five = divideSwapped(2, 10);
Console.WriteLine(five);

// 2.2.3
var d = Range(1, 10).Where(IsMod(2));