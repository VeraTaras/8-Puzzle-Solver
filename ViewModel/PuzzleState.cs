using System;

namespace Puzzle8.ViewModel
{
    public class PuzzleState
    {
        public int[,] Board { get; private set; } 
        public int G { get; private set; } // Path cost from the start state
        public int H { get; private set; } // Heuristic cost (Manhattan distance)
        public int F => G + H; // Total cost, F = G + H
        public PuzzleState Parent { get; private set; } // Reference to the parent state

        public PuzzleState(int[,] board, PuzzleState parent = null)
        {
            Board = board;
            Parent = parent;
            G = parent?.G + 1 ?? 0; // If no parent, G = 0; otherwise, G = parent's G + 1
            H = CalculateHeuristic(); // Calculate the heuristic for this state
        }

        private int CalculateHeuristic()
        {
            int h = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (Board[i, j] == 0) continue; // Skip the empty cell
                    int targetX = (Board[i, j] - 1) / 3; // Target row for the current tile
                    int targetY = (Board[i, j] - 1) % 3; // Target column for the current tile
                    h += Math.Abs(targetX - i) + Math.Abs(targetY - j); // Add Manhattan distance
                }
            }
            return h; // Return the total heuristic value
        }
    }
}
