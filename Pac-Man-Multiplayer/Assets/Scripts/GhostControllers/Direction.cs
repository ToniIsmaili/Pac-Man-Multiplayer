using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public static class DirectionExtensions
{
    public static Direction ToDirection(this Vector2 vector2)
    {
        if (vector2 == Vector2.up)
            return Direction.Up;
        if (vector2 == Vector2.down)
            return Direction.Down;
        if (vector2 == Vector2.left)
            return Direction.Left;
        if (vector2 == Vector2.right)
            return Direction.Right;
        return Direction.Right;
    }

    public static Vector2 ToVector2(this Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Vector2.up;
            case Direction.Down:
                return Vector2.down;
            case Direction.Left:
                return Vector2.left;
            case Direction.Right:
                return Vector2.right;
            default:
                return Vector2.right;
        }
    }

    public static Direction TurnBack(this Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Direction.Down;
            case Direction.Down:
                return Direction.Up;
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
            default:
                return Direction.Right;
        }
    }
}