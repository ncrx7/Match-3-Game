using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3Events
{
    public static Action<int, int, GridSystem2D<GridObject<Gem>>, GemType[], Gem> CreateGemObject;
    public static Action<Vector2Int, Vector2Int, GridSystem2D<GridObject<Gem>>, float, Action> SwapGems;
    public static Action<int, int, GridSystem2D<GridObject<Gem>>, List<Vector2Int>, Action, Action<List<Vector2Int>>> FindMatches;
    public static Func<List<Vector2Int>> ReturnMatches;
    public static Action<List<Vector2Int>, GridSystem2D<GridObject<Gem>>, float, Action> ExplodeGems;
    public static Action<int, int, GridSystem2D<GridObject<Gem>>, float, Action> FallGems;
    public static Action<int, int, GridSystem2D<GridObject<Gem>>, GemType[], Gem, float, Action> FillEmptySlots;
    public static Action<Vector2Int, Vector2Int> RepeatGameActions;
}
