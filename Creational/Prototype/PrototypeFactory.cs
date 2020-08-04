using System;
using DotNetDesignPatternDemos.Creational.Prototype;
using NUnit.Framework.Internal.Commands;
using static System.Console;

namespace DotNetDesignPatternDemos.Creational.PrototypeFactory
{
  public class Address
  {
    public string StreetAddress, City;
    public int Suite;

    public Address(string streetAddress, string city, int suite)
    {
      StreetAddress = streetAddress;
      City = city;
      Suite = suite;
    }

    public Address(Address other)
    {
      StreetAddress = other.StreetAddress;
      City = other.City;
      Suite = other.Suite;
    }
  }

  public partial class Employee
  {
    public string Name;
    public Address Address;

    public Employee(string name, Address address)
    {
      Name = name ?? throw new ArgumentNullException(paramName: nameof(name));
      Address = address ?? throw new ArgumentNullException(paramName: nameof(address));
    }

    public Employee(Employee other)
    {
      Name = other.Name;
      Address = new Address(other.Address);
    }

    public override string ToString()
    {
      return $"{nameof(Name)}: {Name}, {nameof(Address)}: {Address}";
    }
    
    //partial class EmployeeFactory {}
  }

  public class EmployeeFactory
  {
    private static Employee main = 
      new Employee(null, new Address("123 East Dr", "London", 0));
    private static Employee aux = 
      new Employee(null, new Address("123B East Dr", "London", 0));
  
    private static Employee NewEmployee(Employee proto, string name, int suite)
    {
      var copy = proto.DeepCopy();
      copy.Name = name;
      copy.Address.Suite = suite;
      return copy;
    }
  
    public static Employee NewMainOfficeEmployee(string name, int suite) =>
      NewEmployee(main, name, suite);
  
    public static Employee NewAuxOfficeEmployee(string name, int suite) =>
      NewEmployee(aux, name, suite);
  }
  
  public class Demo
  {
    static Employee main = new Employee(null, new Address("123 East Dr", "London", 0));
    
    static void Main(string[] args)
    {
      var main = new Employee(null, new Address("123 East Dr", "London", 0));
      var aux = new Employee(null, new Address("123B East Dr", "London", 0));
      
      
      var john = new Employee("John", new Address("123 London Road", "London", 123));

      //var chris = john;
      var jane = new Employee(john);

      jane.Name = "Jane";
      
      WriteLine(john); // oop s, john is called chris
      WriteLine(jane);


    }
  }
}
