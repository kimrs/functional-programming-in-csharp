using FluentAssertions;
using LaYumba.Functional;
using static LaYumba.Functional.F;
using static System.Math;

namespace Functional_Programming_in_CSharp._8_functional_error_handling;

public class Tests
{
    public static class Extensions
    {
    }

    static Either<string, double> Calc(double x, double y)
    {
        if (y == 0) return "y cannot be 0";
        if (x != 0 && Sign(x) != Sign(y))
            return "x / y cannot be negative";
        
        return Sqrt(x / y);
    }
    
    [Fact]
    public void CalcLeftValueYIs0()
        => Calc(3, 0)
            .Should()
            .Be((Either<string, double>) Left("y cannot be 0"));
    
    [Fact]
    public void CalcLeftValueOperandIsNegatve()
        => Calc(-3, 3)
            .Should()
            .Be((Either<string, double>) Left("x / y cannot be negative"));
    
    [Fact]
    public void CalcRightValue()
        => Calc(-3, -3)
            .Should()
            .Be(Right(1));
    
    
}