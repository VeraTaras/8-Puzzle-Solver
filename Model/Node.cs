using Puzzle8.Model;

public class Node
{
    public PuzzleModel State { get; }
    public Node Parent { get; }
    public int Steps { get; } // New property to count the steps

    public Node(PuzzleModel state, Node parent)
    {
        State = state;
        Parent = parent;
        Steps = parent != null ? parent.Steps + 1 : 0; // Increment steps if there's a parent
    }
}
