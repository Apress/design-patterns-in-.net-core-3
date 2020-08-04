namespace DotNetDesignPatternDemos.Command.Generated
{
  using System.Collections.Generic;

  public class Person
  {
    public long Id;
    public string Name;
  }

  public class Repository
  {
    public List<Person> Persons = new List<Person>();
  }

  public class CreatePersonCommand
  {
    public string Name { get; set; }
  }
}