using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife_V4._0
{
    public static class Window
    {
        public static void WindowSize(int width, int height)
        {
            Console.SetWindowSize(Math.Min(2 * width + 30, Console.LargestWindowWidth), Math.Min(height + 35, Console.LargestWindowHeight));
            Console.SetWindowPosition(0, 0);
        }
    }
}
