using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife_V4
{
    public class World
    {
        public Cell[,] Grid { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Population { get; set; }
        public int Generation { get; set; }
        public int BorderThicknessWidth { get; set; }
        public int BorderThicknessHeight { get; set; }
        public bool Wrap { get; set; }

        public World() { }
        public World(int width, int height)
        {
            Width = width;
            Height = height;
            Population = 0;
            Generation = 0;
            Grid = new Cell[height, width];

            Cell.InitializeCellArray(Grid);
        }

        /// <World>
        /// Creates a new World with a grid in a random state
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="stateChance"></param>
        public World(int width, int height, int initWidth,int initHeight, int top, int left, params Tuple<State, double>[] stateChance)
        {
            Random rand = new Random();

            Width = width;
            Height = height;
            Population = 0;
            Generation = 0;
            Grid = new Cell[height, width];

            Cell.InitializeCellArray(Grid);

            for (int col = initHeight; col < Height-top; ++col)
                for (int row = initWidth; row < Width-left; ++row)
                {
                    double chance = rand.NextDouble();

                    // Calculating which state ech cell starts in
                    // if states s1, s2, s3 have probability p1, p2, p3 respectively
                    // then s1 happens if `chance - p1 < 0'
                    foreach (Tuple<State, double> stateProbability in stateChance)
                    {
                        chance = chance - stateProbability.Item2;
                        if (chance < 0)
                        {
                            Grid[col, row].State = stateProbability.Item1;
                            break;
                        }
                    }
                    if (Grid[col, row].State != State.DEAD)
                    {
                        ++Population;
                        Grid[col, row].StateChanged = true;
                    }
                    Grid[col, row].SetColor();
                }
        }

        public delegate void RuleCalculation(Cell[,] cellArray, int col, int row);

        
        /// <CountNeighboursWrap>
        /// Takes the coordinates of a cell and counts the living adjacent cells in a wrapped grid
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>
        /// The number of neighbours of a given cell.
        /// </returns>
        private int CountNeighboursWrap(int x, int y)
        {
            int neighbours = 0;

            // Check the 8 adjacent cells
            for (int row = -1; row <= 1; ++row)
            {
                for (int col = -1; col <= 1; ++col)
                {
                    if (col != 0 || row != 0)
                    {
                        // calculating the indecies for the surrounding cells
                        // mod Size to wrap around the grid
                        int newCol = (x + col + Width) % Width;
                        int newRow = (y + row + Height) % Height;
                        neighbours += Grid[newRow, newCol].State != State.DEAD ? 1 : 0;
                    }
                }
            }

            return neighbours; 
        }

        /// <CountNeighbours>
        /// Takes the coordinates of a cell and counts the living adjacent cells in a finite grid
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>
        /// The number of neighbours of a given cell
        /// </returns>
        private int CountNeighbours(int x, int y)
        {
            int neighbours = 0;

            for (int row = -1; row <= 1; ++row)
            {
                for (int col = -1; col <= 1; ++col)
                {
                    if (col + x >= 0 && col + x < Width && row + y >= 0 && row + y < Height)
                        if (col != 0 || row != 0)
                        {
                            if (Grid[row + y, col + x].State != State.DEAD)
                            {
                                ++neighbours;
                            }

                        }
                }
            }
            return neighbours;
        }

        public void Write()
        {
            if (Console.CursorVisible)
                Console.CursorVisible = false;

            for (int col = 0; col < Height; ++col)
            {

                Console.SetCursorPosition(BorderThicknessWidth, col + BorderThicknessHeight);

                for (int row = 0; row < Width ; ++row)
                {
                    if (Grid[col, row].StateChanged)// || Grid[col,row].State != State.DEAD)
                    {
                        Console.SetCursorPosition(2 * row + BorderThicknessWidth, col + BorderThicknessHeight);
                        Grid[col, row].SetColor();
                        if (Grid[col, row].State != State.DEAD)
                        {
                            Console.ForegroundColor = Grid[col, row].Color;
                            Console.Write("■ ");
                        }
                       // else if(Grid[col, row].State == State.DEAD)
                        //{
                       else
                            Console.Write("  ");
                       // }
                       // else
                            //Console.Write("X ");
                    }
                }
                Console.WriteLine();
            }

            // Set cursor position to write stats
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(0, Height + 2*BorderThicknessHeight);
            Console.Write($"Generation: {Generation}");
            Console.SetCursorPosition(25, Height + 2*BorderThicknessHeight);
            Console.Write($"Population: {Population}");
        }

        public Cell[,] Update()
        {
            Cell[,] retArray = new Cell[Height,Width];
            Cell.InitializeCellArray(retArray);

            Update(Rule1, retArray);

            return retArray;
        }
        public void Update(RuleCalculation rule, Cell [,] retArray)
        {

            ++Generation;
            for(int row = 0; row < Height; ++row)
            {
                for(int col = 0; col < Width; ++col)
                {
                    rule(retArray, col, row);
                    retArray[row, col].StateChanged = // Generation % 10 == 0 ?  true :
                         (Grid[row, col].State != State.DEAD) ^ (retArray[row, col].State != State.DEAD);
                    retArray[row, col].SetColor();
                }
            }
        }


        public void Rule1(Cell[,] retArray, int col, int row)
        {
            int neighbours = Wrap ? CountNeighboursWrap(col, row) : CountNeighbours(col, row);
            Random rand = new Random();

            // Will living cell die?
            if (Grid[row, col].State != State.DEAD)
                    {
                        if (neighbours == 2 || neighbours == 3)
                        {
                            retArray[row, col].State = Grid[row, col].State;
                            retArray[row, col].Age = Grid[row, col].Age + 1;
                            retArray[row, col].Color = Grid[row, col].Color;
                        }
                        else
                        {
                            retArray[row, col].State = State.DEAD;
                            --Population;
                        }
                    }

                    // is cell revived?
                    else if (neighbours == 3)
                    {
                        ++Population;
                        retArray[row, col].State = State.ALIVE;
                    }
                    else
                        retArray[row, col].State = State.DEAD;
        }
        public void Maze(Cell[,] retArray, int col, int row)
        {
            int neighbours = Wrap ? CountNeighboursWrap(col, row) : CountNeighbours(col, row);

            // Will living cell die?
            if (Grid[row, col].State != State.DEAD)
            {
                if (neighbours == 1 || neighbours == 2 || neighbours == 3 || neighbours == 4 || neighbours == 5)
                {
                    retArray[row, col].State = Grid[row, col].State;
                    retArray[row, col].Age = Grid[row, col].Age + 1;
                    retArray[row, col].Color = Grid[row, col].Color;
                }
                else
                {
                    retArray[row, col].State = State.DEAD;
                    --Population;
                }
            }

            // is cell revived?
            else if (neighbours == 3)
            {
                ++Population;
                retArray[row, col].State = State.ALIVE;
            }
            else
                retArray[row, col].State = State.DEAD;
        }
        public void Rule2(Cell[,] retArray, int col, int row)
        {
            int neighbours = Wrap ? CountNeighboursWrap(col, row) : CountNeighbours(col, row);
            Random rand = new Random();

            // Will living cell die?
            if (Grid[row, col].State != State.DEAD)
            {
                if (neighbours == 1 || neighbours == 2 || neighbours == 4) //|| neighbours == 8)
                {
                    retArray[row, col].State = Grid[row, col].State;
                    retArray[row, col].Age = Grid[row, col].Age + 1;
                    retArray[row, col].Color = Grid[row, col].Color;
                }
                else
                {
                    retArray[row, col].State = State.DEAD;
                    --Population;
                }
            }

            // is cell revived?
            else if (neighbours == 3 || neighbours == 5)// || neighbours == 7)
            {
                ++Population;
                retArray[row, col].State = State.ALIVE;
            }
            else if (retArray[row, col].Age > 15)
                retArray[row, col].State = State.SICK;
            else
                retArray[row, col].State = State.DEAD;
        }
        public void ZombieRule1(Cell[,] retArray, int col, int row)
        {
            int neighbours = Wrap ? CountNeighboursWrap(col, row) : CountNeighbours(col, row);
            int sick, zombies;
            Random rand = new Random();
            zombies = FindZombie(col, row, out sick);

            // Will living cell die?
            if (Grid[row, col].State != State.DEAD)
            {

                if (neighbours == 2 || neighbours == 3)
                {
                    retArray[row, col].State = Grid[row, col].State;
                    retArray[row, col].Age = Grid[row, col].Age + 1;
                    retArray[row, col].Color = Grid[row, col].Color;
                }
                else if (sick >= 1)
                {
                    retArray[row, col].State = State.SICK;
                }
                else if (Grid[row,col].State == State.SICK && Grid[row,col].Age >= 3)
                {
                    if (rand.NextDouble() <= 0.7)
                    {
                        retArray[row, col].State = State.ZOMBIE;
                    }
                    Console.Beep(600, 1);
                }
                else if(zombies == 2)
                {
                    retArray[row, col].State = State.ZOMBIE;
                    Console.Beep(600, 1);
                }
                else
                {
                    retArray[row, col].State = State.DEAD;
                    ++Population;
                }
            }

            // is cell revived?
            else if (neighbours == 3)
            {
                ++Population;
                retArray[row, col].State = State.ALIVE;
            }
            else if(zombies == 2)
            {
                retArray[row, col].State = State.ZOMBIE;
                Console.Beep(600, 1);

            }
            else
                retArray[row, col].State = State.DEAD;
        }
        public void ZombieRule2(Cell[,] retArray, int col, int row)
        {
            int neighbours = Wrap ? CountNeighboursWrap(col, row) : CountNeighbours(col, row);
            int sick, zombies;
            Random rand = new Random();
            zombies = FindZombie(col, row, out sick);

            // Will living cell die?
            if (Grid[row, col].State != State.DEAD)
            {

                if (neighbours == 2 || neighbours == 3)
                {
                    retArray[row, col].State = Grid[row, col].State;
                    retArray[row, col].Age = Grid[row, col].Age + 1;
                    retArray[row, col].Color = Grid[row, col].Color;
                }
                else if (sick >= 1)
                {
                    retArray[row, col].State = State.SICK;
                }
                else if (Grid[row, col].State == State.SICK && Grid[row, col].Age >= 3 && rand.NextDouble() <= 0.7)
                {

                    retArray[row, col].State = State.DEAD;

                    Console.Beep(600, 1);
                }
                else
                {
                    retArray[row, col].State = State.DEAD;
                    ++Population;
                }
            }

            // is cell revived?
            else if (neighbours == 3 || rand.NextDouble()>0.99)
            {
                if (rand.NextDouble() >= 0.5)
                {
                    retArray[row, col].State = State.ZOMBIE;

                }
                else
                {
                    ++Population;
                    retArray[row, col].State = State.ALIVE;
                }
            }
            else
                retArray[row, col].State = State.DEAD;
        }
        private int FindZombie(int x, int y, out int sick)
        {
            int zombies = 0;
            sick = 0;

            for (int row = -1; row <= 1; ++row)
            {
                for (int col = -1; col <= 1; ++col)
                {
                    if (col + x >= 0 && col + x < Width && row + y >= 0 && row + y < Height)
                        if (col != 0 || row != 0)
                        {
                            if (Grid[row + y, col + x].State != State.ZOMBIE)
                            {
                                ++zombies;
                            }
                            else if (Grid[row + y, col + x].State != State.SICK)
                            {
                                ++sick;
                            }

                        }
                }
            }
            return zombies;
        }
    }
}
