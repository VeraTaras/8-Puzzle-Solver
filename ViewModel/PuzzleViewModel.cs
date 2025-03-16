using Puzzle8.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Puzzle8.ViewModel
{
    public class PuzzleViewModel : ObservableObject
    {
        #region Steps
        private int _bfsSteps;
        public int BFSSteps
        {
            get => _bfsSteps;
            set
            {
                _bfsSteps = value;
                OnPropertyChanged();
            }
        }

        private int _dfsSteps;
        public int DFSSteps
        {
            get => _dfsSteps;
            set
            {
                _dfsSteps = value;
                OnPropertyChanged();
            }
        }

        private int _heuristicSteps;
        public int HeuristicSteps
        {
            get => _heuristicSteps;
            set
            {
                _heuristicSteps = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Cells Initialize
        private int _cell0 = 1, _cell1 = 2, _cell2 = 3, _cell3 = 4, _cell4 = 5, _cell5 = 6, _cell6 = 7, _cell7 = 8, _cell8 = 0;

        public int Cell0 { get => _cell0; set => SetCellValue(ref _cell0, value); }
        public int Cell1 { get => _cell1; set => SetCellValue(ref _cell1, value); }
        public int Cell2 { get => _cell2; set => SetCellValue(ref _cell2, value); }
        public int Cell3 { get => _cell3; set => SetCellValue(ref _cell3, value); }
        public int Cell4 { get => _cell4; set => SetCellValue(ref _cell4, value); }
        public int Cell5 { get => _cell5; set => SetCellValue(ref _cell5, value); }
        public int Cell6 { get => _cell6; set => SetCellValue(ref _cell6, value); }
        public int Cell7 { get => _cell7; set => SetCellValue(ref _cell7, value); }
        public int Cell8 { get => _cell8; set => SetCellValue(ref _cell8, value); }
        #endregion

        #region Collections for displaying the solution
        public ObservableCollection<int> BFSBoard { get; } = new ObservableCollection<int>();
        public ObservableCollection<int> DFSBoard { get; } = new ObservableCollection<int>();
        public ObservableCollection<int> HeuristicBoard { get; } = new ObservableCollection<int>();
        #endregion

        #region Commands
        public ICommand ApplySequenceCommand { get; }
        public ICommand SolveBFSCommand { get; }
        public ICommand SolveDFSCommand { get; }
        public ICommand SolveHeuristicCommand { get; }
        #endregion

        public PuzzleViewModel()
        {
            #region InitializeComponent
            Cell0 = _cell0;
            Cell1 = _cell1;
            Cell2 = _cell2;
            Cell3 = _cell3;
            Cell4 = _cell4;
            Cell5 = _cell5;
            Cell6 = _cell6;
            Cell7 = _cell7;
            Cell8 = _cell8;

            ApplySequenceCommand = new RelayCommand(ApplySequence);
            SolveBFSCommand = new RelayCommand(SolveBFS);
            SolveDFSCommand = new RelayCommand(SolveDFS);
            SolveHeuristicCommand = new RelayCommand(SolveHeuristic);

            BFSBoard = new ObservableCollection<int>();
            DFSBoard = new ObservableCollection<int>();
            HeuristicBoard = new ObservableCollection<int>();

            InitializeBoard(BFSBoard);
            InitializeBoard(DFSBoard);
            InitializeBoard(HeuristicBoard);
            #endregion
        }

        #region Initialization methods
        private void InitializeBoard(ObservableCollection<int> board)
        {
            board.Clear();
            int[] correctSequence = { 1, 2, 3, 4, 5, 6, 7, 8, 0 };

            foreach (var cell in correctSequence)
            {
                board.Add(cell);
            }
        }

        private void SetCellValue(ref int cell, int value)
        {
            if (value >= 0 && value <= 8)
            {
                cell = value;
            }
            else
            {
                cell = 0;
            }
            OnPropertyChanged();
        }
        #endregion

        #region Actions
        private void ApplySequence()
        {
            var initialState = new[] { Cell0, Cell1, Cell2, Cell3, Cell4, Cell5, Cell6, Cell7, Cell8 };

            if (IsUniqueNumber(initialState))
            {
                UpdateBoard(BFSBoard, initialState);
                UpdateBoard(DFSBoard, initialState);
                UpdateBoard(HeuristicBoard, initialState);
            }
            else
            {
                MessageBox.Show("Numbers must be unique", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            UpdateBoard(BFSBoard, initialState);
            UpdateBoard(DFSBoard, initialState);
            UpdateBoard(HeuristicBoard, initialState);
        }

        private void SolveBFS()
        {
            var (solvedState, steps) = RunBFSAlgorithm();
            UpdateBoard(BFSBoard, solvedState);
            BFSSteps = steps;
        }

        private void SolveDFS()
        {
            var (solvedState, steps) = RunDFSAlgorithm();
            UpdateBoard(DFSBoard, solvedState);
            DFSSteps = steps;
        }

        private void SolveHeuristic()
        {
            var (solvedState, steps) = RunHeuristicAlgorithm();
            UpdateBoard(HeuristicBoard, solvedState);
            HeuristicSteps = steps;
        }

        private void UpdateBoard(ObservableCollection<int> board, int[] newState)
        {
            if (board == null || newState == null) return;

            board.Clear();
            foreach (var cell in newState)
            {
                board.Add(cell);
            }
        }
        #endregion

        #region Algorithms
        public (int[] Solution, int Steps) RunBFSAlgorithm()
        {
            // Get the initial state of the puzzle
            var initialState = GetCurrentState();

            // Initialize a queue for BFS and a set to track visited states
            var queue = new Queue<Node>();
            var visited = new HashSet<string>();

            // Create the root node (initial state) and add it to the queue and visited set
            var rootNode = new Node(initialState, null);
            queue.Enqueue(rootNode);
            visited.Add(initialState.ToString());

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();

                // Check if the current state is the goal state
                if (currentNode.State.IsGoalState())
                    return (currentNode.State.ToFlatArray(), currentNode.Steps);

                // Loop through all neighboring states of the current state
                foreach (var neighbor in currentNode.State.GetNeighbors())
                {
                    // If the neighbor hasn't been visited, add it to the queue and visited set
                    if (!visited.Contains(neighbor.ToString()))
                    {
                        queue.Enqueue(new Node(neighbor, currentNode));
                        visited.Add(neighbor.ToString());
                    }
                }
            }

            return (null, -1); // no solution is found
        }

        public (int[] Solution, int Steps) RunDFSAlgorithm()
        {
            // Get the initial state of the puzzle
            var initialState = GetCurrentState();

            // Initialize a stack for DFS and a set to track visited states
            var stack = new Stack<Node>();
            var visited = new HashSet<string>();

            // Create the root node (initial state) and add it to the stack and visited set
            var rootNode = new Node(initialState, null);
            stack.Push(rootNode);
            visited.Add(initialState.ToString());

            int maxDepth = 1000;

            while (stack.Count > 0)
            {
                var currentNode = stack.Pop();

                if (currentNode.Steps > maxDepth)
                    continue;

                // Check if the current state is the goal state
                if (currentNode.State.IsGoalState())
                    return (currentNode.State.ToFlatArray(), currentNode.Steps);

                foreach (var neighbor in currentNode.State.GetNeighbors())
                {
                    // If the neighbor hasn't been visited, add it to the stack and visited set
                    if (!visited.Contains(neighbor.ToString()))
                    {
                        stack.Push(new Node(neighbor, currentNode));
                        visited.Add(neighbor.ToString());
                    }
                }
            }

            return (null, -1); // no solution is found
        }



        private (int[] Solution, int Steps) RunHeuristicAlgorithm()
        {
            // Initialize the initial puzzle state with the given board configuration
            var initialState = new int[,] {
        { Cell0, Cell1, Cell2 },
        { Cell3, Cell4, Cell5 },
        { Cell6, Cell7, Cell8 }
    };

            // Create PuzzleState objects for the initial state and the goal state
            var initialPuzzleState = new PuzzleState(initialState);
            var goalState = new PuzzleState(new int[,] {
        { 1, 2, 3 },
        { 4, 5, 6 },
        { 7, 8, 0 }
    });

            // Initialize the open set with the initial state and the closed set to track visited states
            var openSet = new List<PuzzleState> { initialPuzzleState };
            var closedSet = new HashSet<string>();
            int steps = 0; 

            while (openSet.Count > 0)
            {
                // Select the state with the lowest heuristic score (F value)
                var currentState = openSet.OrderBy(s => s.F).First();

                // Check if the current state matches the goal state
                if (IsGoalState(currentState.Board, goalState.Board))
                {
                    return (FlattenBoard(currentState), steps);
                }

                // Move the current state from the open set to the closed set
                openSet.Remove(currentState);
                closedSet.Add(BoardToString(currentState.Board));

                // Generate possible moves from the current state
                foreach (var nextState in GetNeighbors(currentState))
                {
                    var stateString = BoardToString(nextState.Board);

                    // Skip if this state has already been processed
                    if (closedSet.Contains(stateString)) continue;

                    // Add the next state to the open set if it hasn't been visited yet
                    if (!openSet.Any(s => BoardToString(s.Board) == stateString))
                    {
                        openSet.Add(nextState);
                        steps++; // Increment step counter for each valid move
                    }
                }
            }

            return (null, -1); // no solution is found
        }
        #endregion

        #region Auxiliary methods
        private bool IsGoalState(int[,] current, int[,] goal)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (current[i, j] != goal[i, j])
                        return false;
                }
            }
            return true;
        }

        private List<PuzzleState> GetNeighbors(PuzzleState state)
        {
            var neighbors = new List<PuzzleState>();
            var zeroPos = FindZeroPosition(state.Board);

            var moves = new (int, int)[]
            {
        (-1, 0), // Up
        (1, 0),  // Down
        (0, -1), // Left
        (0, 1)   // Right
            };

            foreach (var move in moves)
            {
                var newZeroPos = (zeroPos.Item1 + move.Item1, zeroPos.Item2 + move.Item2);
                if (IsWithinBounds(newZeroPos))
                {
                    var newBoard = Swap(state.Board, zeroPos, newZeroPos);
                    neighbors.Add(new PuzzleState(newBoard, state));
                }
            }

            return neighbors;
        }

        private (int, int) FindZeroPosition(int[,] board)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == 0) return (i, j);
                }
            }
            return (-1, -1); // not correct
        }

        private bool IsWithinBounds((int, int) pos)
        {
            return pos.Item1 >= 0 && pos.Item1 < 3 && pos.Item2 >= 0 && pos.Item2 < 3;
        }

        private int[,] Swap(int[,] board, (int, int) zeroPos, (int, int) newPos)
        {
            var newBoard = (int[,])board.Clone();
            newBoard[zeroPos.Item1, zeroPos.Item2] = newBoard[newPos.Item1, newPos.Item2];
            newBoard[newPos.Item1, newPos.Item2] = 0;
            return newBoard;
        }

        // Converts a board into a string for hashing
        private string BoardToString(int[,] board)
        {
            return string.Join(",", board.Cast<int>());
        }

        // Converts the board to a one-dimensional array for output
        private int[] FlattenBoard(PuzzleState state)
        {
            return state.Board.Cast<int>().ToArray();
        }

        private PuzzleModel GetCurrentState()
        {
            int[,] initialState = {
            { Cell0, Cell1, Cell2 },
            { Cell3, Cell4, Cell5 },
            { Cell6, Cell7, Cell8 }
        };
            return new PuzzleModel(initialState);
        }
        private bool IsUniqueNumber(int[] numbers)
        {
            return numbers.Distinct().Count() == numbers.Length;
        }
        #endregion
    }
}
