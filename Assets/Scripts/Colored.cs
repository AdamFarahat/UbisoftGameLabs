using UnityEngine;

public enum Color
{
    Black = 0b000,
    Red = 0b000,
    Yellow = 0b010,
    Blue = 0b100,
    Orange = Red | Yellow,
    Purple = Red | Blue,
    Green = Yellow | Blue,
    White = 0b111
}

public class Colored : MonoBehaviour
{
    [SerializeField] private Color color;

    public bool MatchesColor(Color other)
    {
        return color == other;
    }

    public bool MatchesColor(Colored other)
    {
        return MatchesColor(other.color);
    }

    public bool OverlapsColor(Color other)
    {
        return (color & other) != Color.Black;
    }

    public bool OverlapsColor(Colored other)
    {
        return OverlapsColor(other.color);
    }
}
