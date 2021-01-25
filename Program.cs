using GameOfLife_V4._0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameOfLife_V4
{
    class Program
    {
        public event EventHandler KeyPressedEvent;
        static void Main(string[] args)
        {
            int x, y;

            do
            {
                Console.Clear();
                Console.WriteLine("Current window size: {0} x {1} lines",Console.WindowWidth, Console.WindowHeight);
                Console.Write("Enter a width less than {0} and a height less than {1}: ", Console.LargestWindowWidth / 2, Console.LargestWindowHeight);
                string[] input = Console.ReadLine().Split(' ');
                (x, y) = (Convert.ToInt32(input[0]), Convert.ToInt32(input[1]));
                try
                {
                    Window.WindowSize(x, y);

                }
                catch (System.ArgumentOutOfRangeException)
                {
                }
            } while (x >= Console.LargestWindowWidth || y >= Console.LargestWindowHeight);

            World world = new World(x, y, 2, 2, 5, 5, new Tuple<State, 
                double>(State.DEAD, 0.75), new Tuple<State, double>(State.ALIVE, 0.25), new Tuple<State, double>(State.ZOMBIE, 0.000), new Tuple<State, double>(State.SICK, 0)) { BorderThicknessHeight = 2, BorderThicknessWidth = 2, Wrap=true };
            while (true)
            {
                world.Write();
                world.Grid = world.Update();
                Thread.Sleep(0);
            }

        }

        public static void KeyPress()
        {
            while (true)
            {
                var input = Console.ReadKey(true);
                try
                {
                    switch (input.Key)
                    {
                        case ConsoleKey.UpArrow:
                            Console.CursorTop -= 1;
                            break;
                        case ConsoleKey.DownArrow:
                            Console.CursorTop += 1;
                            break;
                        case ConsoleKey.LeftArrow:
                            Console.CursorLeft -= 1;
                            break;
                        case ConsoleKey.RightArrow:
                            Console.CursorLeft += 1;
                            break;
                        default:
                            if(input.Key != ConsoleKey.Escape)
                                Console.Write(input.KeyChar);
                            break;
                    }
                }
                catch(System.ArgumentOutOfRangeException)
                {

                }
                if (input.Key == ConsoleKey.Escape)
                   Environment.Exit(0);
            }
        }

    }
}
