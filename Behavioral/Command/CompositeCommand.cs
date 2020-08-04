using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DotNetDesignPatternDemos.Annotations;
using static System.Console;

namespace DotNetDesignPatternDemos.Behavioral.Command.Composite
{
  public class BankAccount
  {
    private int balance;
    private readonly int overdraftLimit = -500;

    public BankAccount(int balance = 0)
    {
      this.balance = balance;
    }

    public void Deposit(int amount)
    {
      balance += amount;
      Console.WriteLine($"Deposited ${amount}, balance is now {balance}");
    }

    public bool Withdraw(int amount)
    {
      if (balance - amount >= overdraftLimit)
      {
        balance -= amount;
        Console.WriteLine($"Withdrew ${amount}, balance is now {balance}");
        return true;
      }
      return false;
    }

    public override string ToString()
    {
      return $"{nameof(balance)}: {balance}";
    }
  }

  public interface ICommand
  {
    void Call();
    void Undo();
    bool Success { get; set; }
  }

  public class BankAccountCommand : ICommand
  {
    private readonly BankAccount account;

    public enum Action
    {
      Deposit, Withdraw
    }

    private readonly Action action;
    private readonly int amount;

    public BankAccountCommand(BankAccount account, Action action, int amount)
    {
      this.account = account;
      this.action = action;
      this.amount = amount;
    }

    public void Call()
    {
      switch (action)
      {
        case Action.Deposit:
          account.Deposit(amount);
          Success = true;
          break;
        case Action.Withdraw:
          Success = account.Withdraw(amount);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public void Undo()
    {
      if (!Success) return;
      switch (action)
      {
        case Action.Deposit:
          account.Withdraw(amount);
          break;
        case Action.Withdraw:
          account.Deposit(amount);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public bool Success { get; set; }
  }

  public class CompositeBankAccountCommand 
    : List<BankAccountCommand>, ICommand
  {
    public CompositeBankAccountCommand()
    {
    }

    public CompositeBankAccountCommand(
      [NotNull] IEnumerable<BankAccountCommand> collection) : base(collection)
    {
    }

    public virtual void Call()
    {
      Success = true;
      ForEach(cmd =>
      {
        cmd.Call();
        Success &= cmd.Success;
      });
    }

    public virtual void Undo()
    {
      foreach (var cmd in 
        ((IEnumerable<BankAccountCommand>)this).Reverse())
      {
        cmd.Undo();
      }
    }

    public bool Success { get; set; }
  }

  public class MoneyTransferCommand : CompositeBankAccountCommand
  {
    public MoneyTransferCommand(BankAccount from,
      BankAccount to, int amount)
    {
      AddRange(new[]
      {
        new BankAccountCommand(from,
          BankAccountCommand.Action.Withdraw, amount),
        new BankAccountCommand(to,
          BankAccountCommand.Action.Deposit, amount), 
      });
    }

    public override void Call()
    {
      BankAccountCommand last = null;
      foreach (var cmd in this)
      {
        if (last == null || last.Success)
        {
          cmd.Call();
          last = cmd;
        }
        else
        {
          cmd.Undo();
          break;
        }
      }
    }
  }
  
  class Demo
  {
    static void Main(string[] args)
    {
      // composite
      var ba = new BankAccount();
      var cmdDeposit = new BankAccountCommand(ba,
        BankAccountCommand.Action.Deposit, 100);
      var cmdWithdraw = new BankAccountCommand(ba,
        BankAccountCommand.Action.Withdraw, 1000);
      var composite = new CompositeBankAccountCommand(new[]{
        cmdDeposit, cmdWithdraw
      });
      
      composite.Call();
      Console.WriteLine(ba);
      
      composite.Undo();
      Console.WriteLine(ba);
      

      // money transfer
      var from = new BankAccount();
      from.Deposit(100);
      var to = new BankAccount();
      
      var mtc = new MoneyTransferCommand(from, to, 1000);
      mtc.Call();

      Console.WriteLine(from);
      Console.WriteLine(to);
      
      mtc.Undo();
      
      Console.WriteLine(from);
      Console.WriteLine(to);
    }
  }
}