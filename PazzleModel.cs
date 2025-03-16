using System.Collections.Generic;

namespace Puzzle8.Model
{
    public class PuzzleModel
    {
        public int[,] State { get; private set; }
        public int Steps { get; private set; }

        public PuzzleModel(int[,] initialState, int steps = 0)
        {
            State = initialState;
            Steps = steps;
        }

        public bool IsGoalState()
        {
            int[,] GoalState = {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 0 }
        };

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (State[i, j] != GoalState[i, j])
                        return false;
            return true;
        }

        public List<PuzzleModel> GetNeighbors()
        {
            var neighbors = new List<PuzzleModel>();
            (int x, int y) = FindZeroPosition();

            var moves = new (int, int)[]
            {
            (-1, 0), // Up
            (1, 0),  // Down
            (0, -1), // Left
            (0, 1)   // Right
            };

            foreach (var (dx, dy) in moves)
            {
                int newX = x + dx;
                int newY = y + dy;

                if (newX >= 0 && newX < 3 && newY >= 0 && newY < 3)
                {
                    var newState = (int[,])State.Clone();
                    newState[x, y] = newState[newX, newY];
                    newState[newX, newY] = 0;
                    neighbors.Add(new PuzzleModel(newState, Steps + 1));
                }
            }
            return neighbors;
        }

        public int[] ToFlatArray()
        {
            int[] result = new int[9];
            int index = 0;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    result[index++] = State[i, j];
            return result;
        }


        public (int, int) FindZeroPosition()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (State[i, j] == 0)
                        return (i, j);
            return (-1, -1);
        }

        public override string ToString() 
        {
            var hashCode = 17;
            foreach (var cell in ToFlatArray())
            {
                hashCode = hashCode * 31 + cell;
            }
            return $"{hashCode}, Steps: {Steps}";
        }

    }
}

