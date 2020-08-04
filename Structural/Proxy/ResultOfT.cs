using System;

namespace DotNetDesignPatternDemos.Structural.Proxy
{
  public class Result<T>
  {
    public readonly T Value;
    public readonly string? Error;

    public Result(T value)
    {
      Value = value;
    }

    public Result(string error)
    {
      Error = error;
    }

    public bool HasValue => Error == null;

    public static implicit operator bool(Result<T> result)
    {
      return result.HasValue;
    }
  }

  class EquationSolver
  {
    public static Result<(double x1, double x2)>
      SolveQuadratic(double a, double b, double c)
    {
      var disc = b * b - 4 * a * c;
      if (disc < 0)
        return new Result<(double x1, double x2)>(
          "Cannot process complex roots");
      
      var rootDisc = Math.Sqrt(disc);
      return new Result<(double x1, double x2)>((
        (-b + rootDisc) / (2 * a),
        (-b - rootDisc) / (2 * a)
      ));
    }
  }
}