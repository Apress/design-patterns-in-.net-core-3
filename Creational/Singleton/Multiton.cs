using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DotNetDesignPatternDemos.Creational.Multiton
{
  
  // can only allow two components related to subsystems
  enum Subsystem
  {
    Main,
    Backup
  }

  class Printer
  {
    private Printer() { }

    public static Printer Get(Subsystem ss)
    {
      if (instances.ContainsKey(ss))
        return instances[ss];

      var instance = new Printer();
      instances[ss] = instance;
      return instance;
    }

    private static readonly Dictionary<Subsystem, Printer> instances
      = new Dictionary<Subsystem, Printer>();
  }

  class Demo
  {
    public static void Main(string[] args)
    {
      var primary = Printer.Get(Subsystem.Main);
      var backup = Printer.Get(Subsystem.Backup);

      var backupAgain = Printer.Get(Subsystem.Backup);

      Console.WriteLine(ReferenceEquals(backup, backupAgain));
    }
  }
}