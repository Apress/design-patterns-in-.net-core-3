using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using DotNetDesignPatternDemos.Annotations;
using MoreLinq;

namespace DotNetDesignPatternDemos.Behavioral.Observer.PropertyDependencies
{
  public class PropertyNotificationSupport 
    : INotifyPropertyChanged, INotifyPropertyChanging
  {
    private readonly Dictionary<string, HashSet<string>> affectedBy
      = new Dictionary<string, HashSet<string>>();

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged
      ([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

      foreach (var affected in affectedBy.Keys)
        if (affectedBy[affected].Contains(propertyName))
          OnPropertyChanged(affected);
    }
    
    protected Func<T> property<T>(string name, Expression<Func<T>> expr)
    {
      Console.WriteLine($"Creating computed property for expression {expr}");

      var visitor = new MemberAccessVisitor(GetType());
      visitor.Visit(expr);

      if (visitor.PropertyNames.Any())
      {
        if (!affectedBy.ContainsKey(name))
          affectedBy.Add(name, new HashSet<string>());

        foreach (var propName in visitor.PropertyNames)
          if (propName != name)
            affectedBy[name].Add(propName);
      }

      return expr.Compile();
    }

    private class MemberAccessVisitor : ExpressionVisitor
    {
      private readonly Type declaringType;
      public readonly IList<string> PropertyNames = new List<string>();

      public MemberAccessVisitor(Type declaringType)
      {
        this.declaringType = declaringType;
      }

      public override Expression Visit(Expression expr)
      {
        if (expr != null && expr.NodeType == ExpressionType.MemberAccess)
        {
          var memberExpr = (MemberExpression)expr;
          if (memberExpr.Member.DeclaringType == declaringType)
          {
            PropertyNames.Add(memberExpr.Member.Name);
          }
        }

        return base.Visit(expr);
      }
    }

    public event PropertyChangingEventHandler PropertyChanging;

    protected virtual void OnPropertyChanging(
      [CallerMemberName] string propertyName = null)
    {
      PropertyChanging?.Invoke(this, 
        new PropertyChangingEventArgs(propertyName));
    }

    protected void setValue<T>(T value, ref T field, 
      [CallerMemberName] string propertyName = null)
    {
      if (value.Equals(field)) return;
      OnPropertyChanging(propertyName);
      field = value;
      OnPropertyChanged(propertyName);
    }
  }

  public class Person : PropertyNotificationSupport
  {
    private int age;

    public int Age
    {
      get => age;
      set => setValue(value, ref age);
    }

    public bool Citizen
    {
      get => citizen;
      set => setValue(value, ref citizen);
    }

    private readonly Func<bool> canVote;
    private bool citizen;
    public bool CanVote => canVote();

    public Person()
    {
      canVote = property(
        nameof(CanVote), 
        () => Citizen && Age >= 16);
    }
  }

  public class Demo
  {
    static void Main(string[] args)
    {
      var p = new Person();
      p.PropertyChanged += (sender, eventArgs) =>
      {
        Console.WriteLine($"{eventArgs.PropertyName} has changed");
      };
      p.Age = 15; // should not really affect CanVote :)
      p.Citizen = true;
    }
  }
}