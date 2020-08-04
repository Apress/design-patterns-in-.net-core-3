using System;
using System.Dynamic;
using DotNetDesignPatternDemos.Annotations;
using ImpromptuInterface;
using static System.Console;

namespace DotNetDesignPatternDemos.Behavioral.NullObject
{
  public interface ILog
  {
    void Info(string msg);
    void Warn(string msg);
  }

  class ConsoleLog : ILog
  {
    public void Info(string msg)
    {
      WriteLine(msg);
    }
  
    public void Warn(string msg)
    {
      WriteLine("WARNING: " + msg);
    }
  }

  class OptionalLog: ILog
  {
    private ILog impl;

    public OptionalLog(ILog impl)
    {
      this.impl = impl;
    }

    public void Info(string msg)
    {
      impl?.Info(msg);
    }

    public void Warn(string msg)
    {
      impl?.Warn(msg);
    }
  }

  public class BankAccount
  {
    private readonly ILog log;
    private int balance;

    private const ILog NoLogging = null; 

    public BankAccount([CanBeNull] ILog log = NoLogging)
    {
      this.log = new OptionalLog(log);
    }

    public void Deposit(int amount)
    {
      balance += amount;
      // check for null everywhere
      log?.Info($"Deposited ${amount}, balance is now {balance}");
    }

    public void Withdraw(int amount)
    {
      if (balance >= amount)
      {
        balance -= amount;
        log?.Info($"Withdrew ${amount}, we have ${balance} left");
      }
      else
      {
        log?.Warn($"Could not withdraw ${amount} because " +
                  $"balance is only ${balance}");
      }
    }
  }

  public sealed class NullLog : ILog
  {
    public void Info(string msg) { }
    public void Warn(string msg) { }
  }

  public class Null<T> : DynamicObject where T:class
  {
    public static T Instance
    {
      get
      {
        if (!typeof(T).IsInterface)
          throw new ArgumentException("I must be an interface type");
    
        return new Null<T>().ActLike<T>();
      }
    }
  
    public override bool TryInvokeMember(InvokeMemberBinder binder, 
      object[] args, out object result)
    {
      var name = binder.Name;
      result = Activator.CreateInstance(binder.ReturnType);
      return true;
    }
  }

  public class Demo
  {
    static void Main()
    {
      //var log = new ConsoleLog();
      //ILog log = null;
      var log = new NullLog();
      var ba = new BankAccount(log);
      ba.Deposit(100);
      ba.Withdraw(200);
    }
  }
}