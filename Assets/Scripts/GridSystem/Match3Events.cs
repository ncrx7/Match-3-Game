using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Match3Events
{
    public static Action<Action> LaunchTheGame;
    public static Action SetSwapAmount;
    public static Action<int, List<GemType>> CreateGameTask;
    public static Action<int, int, GridSystem2D<GridObject<Gem>>, GemType[], int> CreateGemObject;//
    public static Action<Vector2Int, Vector2Int, GridSystem2D<GridObject<Gem>>, float, Action> SwapGems;
    public static Action<int, int, GridSystem2D<GridObject<Gem>>, List<Vector2Int>, Action, Action<List<Vector2Int>>> FindMatches;
    public static Func<List<Vector2Int>> ReturnMatches;
    public static Action<List<Vector2Int>, GridSystem2D<GridObject<Gem>>, float, Action> ExplodeGems;
    public static Action<int, int, GridSystem2D<GridObject<Gem>>, float, Action> FallGems;
    public static Action<int, int, GridSystem2D<GridObject<Gem>>, GemType[], int, float, Action> FillEmptySlots;//
    public static Action<Vector2Int, Vector2Int> RepeatGameActions;

    //UI Event
    public static Action<int, TextMeshProUGUI> UpdateTaskGemRemainAmountText; 
    public static Action<int> UpdateScoreText;
    public static Action<int> UpdateSwapAmountText;
    public static Action<int> UpdateLevelText;

    public static Action<int, int> OnGameFinishedSuccessfully;
    public static Action OnGameFinishedUnsuccessfully;
}
