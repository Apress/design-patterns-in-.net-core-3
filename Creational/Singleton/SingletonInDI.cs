using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ImpromptuInterface;
using MoreLinq;
using NUnit.Framework;
using static System.Console;

namespace DotNetDesignPatternDemos.Creational.SingletonDI
{
  public class Database
  {
    private Database()
    {
    }

    public static Database Instance { get; } = new Database();
  }

  public class MyDatabase
  {
    private MyDatabase()
    {
      Console.WriteLine("Initializing database");
    }
    private static Lazy<MyDatabase> instance = 
      new Lazy<MyDatabase>(() => new MyDatabase());

    public static MyDatabase Instance => instance.Value;
  }
  
  public interface IDatabase
  {
    int GetPopulation(string name);
  }

  public class SingletonDatabase : IDatabase
  {
    private Dictionary<string, int> capitals;
    private static int instanceCount;
    public static int Count => instanceCount;

    private SingletonDatabase()
    {
      WriteLine("Initializing database");

      capitals = File.ReadAllLines(
        Path.Combine(
          new FileInfo(typeof(IDatabase).Assembly.Location)
            .DirectoryName, 
          "capitals.txt")
        )
        .Batch(2)
        .ToDictionary(
          list => list.ElementAt(0).Trim(),
          list => int.Parse(list.ElementAt(1)));
    }

    public int GetPopulation(string name)
    {
      return capitals[name];
    }

    // laziness + thread safety
    private static readonly Lazy<SingletonDatabase> instance 
      = new Lazy<SingletonDatabase>(() =>
    {
      instanceCount++;
      return new SingletonDatabase();
    });

    public static IDatabase Instance => instance.Value;
  }

  public class SingletonRecordFinder
  {
    public int TotalPopulation(IEnumerable<string> names)
    {
      return names.Sum(name => SingletonDatabase.Instance.GetPopulation(name));
    }
  }

  public class ConfigurableRecordFinder
  {
    private IDatabase database;
  
    public ConfigurableRecordFinder(IDatabase database)
    {
      this.database = database;
    }
  
    public int GetTotalPopulation(IEnumerable<string> names)
    {
      return names.Sum(name => database.GetPopulation(name));
    }
  }

  public class DummyDatabase : IDatabase
  {
    public int GetPopulation(string name)
    {
      return new Dictionary<string, int>
      {
        ["alpha"] = 1,
        ["beta"] = 2,
        ["gamma"] = 3
      }[name];
    }
  }

  public class OrdinaryDatabase : IDatabase
  {
    private readonly Dictionary<string, int> cities;

    public OrdinaryDatabase()
    {
      WriteLine("Initializing database");

      cities = File.ReadAllLines(
          Path.Combine(
            new FileInfo(typeof(IDatabase).Assembly.Location)
              .DirectoryName, 
            "capitals.txt")
        )
        .Batch(2)
        .ToDictionary(
          list => list.ElementAt(0).Trim(),
          list => int.Parse(list.ElementAt(1)));
    }

    public int GetPopulation(string name)
    {
      return cities[name];
    }
  }

  [TestFixture]
  public class SingletonTests
  {
    private IContainer container;
    
    [SetUp]
    public void SetUp()
    {
      var cb = new ContainerBuilder();
      cb.RegisterType<DummyDatabase>()
        .As<IDatabase>()
        .SingleInstance();
      cb.RegisterType<ConfigurableRecordFinder>();
      container = cb.Build();
    }
    
    [Test]
    public void IsSingletonTest()
    {
      var db = SingletonDatabase.Instance;
      var db2 = SingletonDatabase.Instance;
      Assert.That(db, Is.SameAs(db2));
      Assert.That(SingletonDatabase.Count, Is.EqualTo(1));
    }

    [Test]
    public void SingletonTotalPopulationTest()
    {
      // testing on a live database
      var rf = new SingletonRecordFinder();
      var names = new[] {"Seoul", "Mexico City"};
      int tp = rf.TotalPopulation(names);
      Assert.That(tp, Is.EqualTo(17500000 + 17400000));
    }

    [Test]
    public void DependentTotalPopulationTest()
    {
      var db = new DummyDatabase();
      var rf = new ConfigurableRecordFinder(db);
      Assert.That(
        rf.GetTotalPopulation(new[]{"alpha", "gamma"}),
        Is.EqualTo(4));
    }

    [Test]
    public void DIPopulationTest()
    {
      var rf = container.Resolve<ConfigurableRecordFinder>();
      Assert.That(
        rf.GetTotalPopulation(new[]{"alpha", "gamma"}),
        Is.EqualTo(4));
    }
  }
}
