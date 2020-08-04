using System;
using System.Collections.Generic;

namespace PatternDemo
{
  public interface IShape
  {
    void Draw();
  }

  public class Square : IShape
  {
    public void Draw() => Console.WriteLine("Basic square");
  }

  public class Rectangle : IShape
  {
    public void Draw() => Console.WriteLine("Basic rectangle");
  }

  public class RoundedSquare : IShape
  {
    public void Draw() => Console.WriteLine("Rounded square");
  }

  public class RoundedRectangle : IShape
  {
    public void Draw() => Console.WriteLine("Rounded rectangle");
  }

  public enum Shape
  {
    Square,
    Rectangle
  }

  public abstract class ShapeFactory
  {
    public abstract IShape Create(Shape shape);
  }

  public class BasicShapeFactory : ShapeFactory
  {
    public override IShape Create(Shape shape)
    {
      switch (shape) 
      {
        case Shape.Square:
          return new Square();
        case Shape.Rectangle:
          return new Rectangle();
        default:
          throw new ArgumentOutOfRangeException(
            nameof(shape), shape, null);
      }
    }
  }

  public class RoundedShapeFactory : ShapeFactory
  {
    public override IShape Create(Shape shape)
    {
      switch (shape)
      {
        case Shape.Square:
          return new RoundedSquare();
        case Shape.Rectangle:
          return new RoundedRectangle();
        default:
          throw new ArgumentOutOfRangeException(nameof(shape), shape, null);
      }
    }
  }
  
  public class Demo
  {
    public static ShapeFactory GetFactory(bool rounded)
    {
      if (rounded)
        return new RoundedShapeFactory();
      else
        return new BasicShapeFactory();
    }
    
    public static void Main()
    {
      var basic = GetFactory(false);
      var basicRectangle = basic.Create(Shape.Rectangle);
      basicRectangle.Draw();

      var roundedSquare = GetFactory(true).Create(Shape.Square);
      roundedSquare.Draw();
    }
  }
}