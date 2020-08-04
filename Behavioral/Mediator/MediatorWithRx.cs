using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using Autofac;
using DotNetDesignPatternDemos.Annotations;
using static System.Console;

namespace DotNetDesignPatternDemos.Behavioral.Mediator.WithRx
{
  [UsedImplicitly]
  public class EventBroker : IObservable<EventArgs>
  {
    private readonly List<Subscription> subscribers = new List<Subscription>();
    
    public IDisposable Subscribe(IObserver<EventArgs> subscriber)
    {
      Subscription sub = new Subscription(this, subscriber);
      if (subscribers.All(s => s.Subscriber != subscriber))
        subscribers.Add(sub);
      return sub;
    }

    private void Unsubscribe(IObserver<EventArgs> subscriber)
    {
      subscribers.RemoveAll(s => s.Subscriber == subscriber);
    }

    public void Publish<T>(T args) where T : EventArgs
    {
      foreach (var s in subscribers.ToArray())
        s.Subscriber.OnNext(args); // will call Unsubscribe() from here
    }
    
    private class Subscription : IDisposable
    {
      private readonly EventBroker broker;
      public IObserver<EventArgs> Subscriber { get; private set; }
      public Subscription(EventBroker broker, IObserver<EventArgs> subscriber)
      {
        this.broker = broker;
        Subscriber = subscriber;
      }
      public void Dispose()
      {
        broker.Unsubscribe(Subscriber);
      }
    }
  }

  class PlayerScoredEventArgs : EventArgs
  {
    public string PlayerName;
    public int GoalsScoredSoFar;

    public PlayerScoredEventArgs
      (string playerName, int goalsScoredSoFar)
    {
      PlayerName = playerName;
      GoalsScoredSoFar = goalsScoredSoFar;
    }
  }

  [UsedImplicitly]
  public class Player
  {
    public string Name;
    private int goalsScored;
    private EventBroker broker;

    public delegate Player Factory(string name);
    
    public Player(string name, EventBroker broker)
    {
      Name = name;
      this.broker = broker;
    }

    public void Score()
    {
      goalsScored++;
      var args = new PlayerScoredEventArgs(Name, goalsScored);
      broker.Publish(args);
    }
  }

  [UsedImplicitly]
  public class Coach
  {
    private IDisposable subscription;

    public Coach(EventBroker broker)
    {
      subscription = broker
        .OfType<PlayerScoredEventArgs>()
        .Skip(1)
        .Take(3)
        .Subscribe(args =>
          WriteLine($"Well done, {args.PlayerName}! ({args.GoalsScoredSoFar} goals)"));
    }
  }

  static class Program
  {
    public static void Main(string[] args)
    {
      var cb = new ContainerBuilder();
      cb.RegisterType<EventBroker>().SingleInstance();
      cb.RegisterType<Player>();
      cb.RegisterType<Coach>();

      var container = cb.Build();

      var pf = container.Resolve<Player.Factory>();
      var player = pf("John");

      var coach = container.Resolve<Coach>();
      
      player.Score();
      player.Score(); //
      player.Score(); //
      player.Score(); //
      player.Score();
    }
  }
}