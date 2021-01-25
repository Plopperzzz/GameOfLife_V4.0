using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife_V4
{
    public class Cell
    {
        public State State { get; set; }
        public ConsoleColor Color { get; set; }
        public int Age { get; set; }
        public bool StateChanged { get; set; } // To keep track of what we draw

        public Cell()
        {
            State = State.DEAD;
            Color = ConsoleColor.White;
        }

        public Cell(ConsoleColor color, State state = State.DEAD)
        {
            Color = color;
            State = state;
        }

        public static void InitializeCellArray(Cell [,] cellArray)
        {
            int i = 0;
            for(int col = 0; col < cellArray.GetLength(0); ++col)

                for(int row = 0; row < cellArray.GetLength(1); ++row)

                    cellArray[col, row] = new Cell(ConsoleColor.White);
        }

        public void SetColor()
        {
            ConsoleColor oldColor = Color;
            switch (State)
            {
                case State.ALIVE:
                    switch (Age)
                    {
                        case int age when age < 2:
                            Color = ConsoleColor.White;
                            break;
                        case int age when age >= 2 && age < 4:
                            Color = ConsoleColor.DarkCyan;
                            break;
                        case int age when age >= 4 && age < 6:
                            Color = ConsoleColor.Magenta;
                            break;
                        case int age when age >= 6:
                            Color = ConsoleColor.DarkYellow;
                            break;
                    }
                    break;
                case State.SICK:
                    Color = ConsoleColor.DarkGray;
                    break;
                case State.ZOMBIE:
                    Color = ConsoleColor.Green;
                    break;
                case State.DEAD:
                    Color = ConsoleColor.DarkRed;
                    break;
            }
            if (oldColor == Color) StateChanged = !StateChanged;
        }
    }
}
