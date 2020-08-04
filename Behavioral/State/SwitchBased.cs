using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetDesignPatternDemos.Behavioral.State
{
  enum State
  {
    Locked,
    Failed,
    Unlocked
  }

  public class SwitchBasedDemo
  {
    static void Main(string[] args)
    {
      string code = "1234";
      var data = new Queue<int>(new[] {1, 2, 3, 4});
      var state = State.Locked;
      var entry = new StringBuilder();

      while (true)
      {
        switch (state)
        {
          case State.Locked:
            var value = data.Dequeue();
            Console.WriteLine(value);
            entry.Append(
              //Console.ReadKey().KeyChar
              value
            );

            if (entry.ToString() == code)
            {
              state = State.Unlocked;
              break;
            }
            
            if (!code.StartsWith(entry.ToString()))
            {
              // the code is blatantly wrong
              state = State.Failed;
            }
            break;
          case State.Failed:
            Console.WriteLine("FAILED");
            return;
            //break;
          case State.Unlocked:
            //Console.CursorLeft = 0;
            Console.WriteLine("UNLOCKED");
            return;
        }
      }
    }
  }
}