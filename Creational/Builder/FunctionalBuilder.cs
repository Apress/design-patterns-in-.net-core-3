using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetDesignPatternDemos.Creational.Builder
{
  public class Person
  {
    public string Name, Position;
  }

  public abstract class FunctionalBuilder<TSubject, TSelf>
    where TSelf: FunctionalBuilder<TSubject, TSelf> 
    where TSubject : new() 
  {
    private readonly List<Func<TSubject, TSubject>> actions 
      = new List<Func<TSubject, TSubject>>();
  
    public TSelf Do(Action<TSubject> action)
      => AddAction(action);

    private TSelf AddAction(Action<TSubject> action)
    { 
      actions.Add(p => { action(p); return p; }); 
      return (TSelf) this; 
    }
    public TSubject Build()
      => actions.Aggregate(new TSubject(), (p, f) => f(p));
  }
  
  public sealed class PersonBuilder
    : FunctionalBuilder<Person, PersonBuilder> 
  { 
    public PersonBuilder Called(string name) 
      => Do(p => p.Name = name); 
  }
  
  public static class PersonBuilderExtensions
  {
    public static PersonBuilder WorksAsA
      (this PersonBuilder builder, string position)
    {
      builder.Do(p => p.Position = position);
      return builder;
    }
  }
  
  public class FunctionalBuilder
  {
    public static void Main(string[] args)
    {
      var b = new PersonBuilder();
      var p = b.Called("Dmitri")
        .WorksAsA("Developer")
        .Build();
    }
  }
}