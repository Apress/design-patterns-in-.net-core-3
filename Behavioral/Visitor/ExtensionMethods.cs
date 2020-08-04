using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using static System.Console;

namespace DotNetDesignPatternDemos.Behavioral.Visitor.ExtensionMethods
{
  public abstract class Expression
  {
  }

  public class DoubleExpression : Expression
  {
    public double Value;

    public DoubleExpression(double value)
    {
      Value = value;
    }
  }

  public class AdditionExpression : Expression
  {
    public Expression Left;
    public Expression Right;

    public AdditionExpression(Expression left, Expression right)
    {
      Left = left;
      Right = right;
    }
  }

  public static class ExpressionPrinter
  {
    private static readonly Dictionary<Type, MethodInfo> methods
      = new Dictionary<Type, MethodInfo>();

    static ExpressionPrinter()
    {
      var a = typeof(Expression).Assembly;
      var classes = a.GetTypes()
        .Where(t => t.IsSubclassOf(typeof(Expression)));
      var printMethods = typeof(ExpressionPrinter).GetMethods();
      foreach (var c in classes)
      {
        // find extension method that takes this class
        var pm = printMethods.FirstOrDefault(m =>
          m.Name.Equals(nameof(Print)) &&
          m.GetParameters()?[0]?.ParameterType == c);
        
        methods.Add(c, pm);
      }
    }
    
    public static void Print(this Expression e, StringBuilder sb)
    {
      methods[e.GetType()].Invoke(null, new object[] {e, sb});
      // switch (e)
      // {
      //   case DoubleExpression de: 
      //     de.Print(sb);
      //     break;
      //   case AdditionExpression ae: 
      //     ae.Print(sb);
      //     break;
      //   // and so on
      // }
    }
    
    public static void Print(this DoubleExpression de, StringBuilder sb)
    {
      sb.Append(de.Value);
    }
    
    public static void Print(this AdditionExpression ae, StringBuilder sb)
    {
      sb.Append("(");
      ae.Left.Print(sb);
      sb.Append("+");
      ae.Right.Print(sb);
      sb.Append(")");
    }
  }

  public class Demo
  {
    private static void Main(string[] args)
    {
      var e = new AdditionExpression(
        left: new DoubleExpression(1),
        right: new AdditionExpression(
          left: new DoubleExpression(2),
          right: new DoubleExpression(3)));
      var sb = new StringBuilder();
      e.Print(sb);
      WriteLine(sb);

      // what is more likely: new type or new operation
    }
  }
}