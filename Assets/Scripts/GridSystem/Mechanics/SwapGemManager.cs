using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class SwapGemManager : MonoBehaviour
{
    private void OnEnable()
    {
        Match3Events.SwapGems += (Vector2Int gridPosA, Vector2Int gridPosB, GridSystem2D<GridObject<Gem>> grid, float swapGemSpeed, Action callback) 
        => StartCoroutine(SwapGems(gridPosA, gridPosB, grid, swapGemSpeed, callback));
    }

    private void OnDisable()
    {
        Match3Events.SwapGems -= (Vector2Int gridPosA, Vector2Int gridPosB, GridSystem2D<GridObject<Gem>> grid, float swapGemSpeed, Action callback) 
        => StartCoroutine(SwapGems(gridPosA, gridPosB, grid, swapGemSpeed, callback));
    }

    IEnumerator SwapGems(Vector2Int gridPosA, Vector2Int gridPosB, GridSystem2D<GridObject<Gem>> grid, float swapGemSpeed, Action callback)
    {
        var gridObjectA = grid.GetValue(gridPosA.x, gridPosA.y);
        var gridObjectB = grid.GetValue(gridPosB.x, gridPosB.y);
        //Debug.Log("gird object: " + _grid);
        gridObjectA.GetValue().transform
        .DOLocalMove(grid.GetWorldPositionCenter(gridPosB.x, gridPosB.y), swapGemSpeed).SetEase(Ease.InQuad);

        gridObjectB.GetValue().transform
        .DOLocalMove(grid.GetWorldPositionCenter(gridPosA.x, gridPosA.y), swapGemSpeed).SetEase(Ease.InQuad);

        grid.SetValue(gridPosA.x, gridPosA.y, gridObjectB);
        grid.SetValue(gridPosB.x, gridPosB.y, gridObjectA);

        yield return new WaitForSeconds(swapGemSpeed);
        callback?.Invoke();
    }
}
