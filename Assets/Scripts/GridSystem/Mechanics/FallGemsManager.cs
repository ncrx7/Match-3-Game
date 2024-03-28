using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class FallGemsManager : MonoBehaviour
{
    private void OnEnable()
    {
        Match3Events.FallGems += (int width, int height, GridSystem2D<GridObject<Gem>> grid, float fallGemDelay, Action callback) 
        => StartCoroutine(HandleFallGems(width, height, grid, fallGemDelay, callback));
    }

    private void OnDisable()
    {
        Match3Events.FallGems -= (int width, int height, GridSystem2D<GridObject<Gem>> grid, float fallGemDelay, Action callback) 
        => StartCoroutine(HandleFallGems(width, height, grid, fallGemDelay, callback));
    }

    IEnumerator HandleFallGems(int width, int height, GridSystem2D<GridObject<Gem>> grid, float fallGemDelay, Action callback)
    {
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                if (grid.GetValue(x, y) == null)
                {
                    for (var i = y + 1; i < height; i++)
                    {
                        if (grid.GetValue(x, i) != null)
                        {
                            var gem = grid.GetValue(x, i).GetValue();
                            grid.SetValue(x, y, grid.GetValue(x, i));
                            grid.SetValue(x, i, null);
                            gem.transform
                            .DOLocalMove(grid.GetWorldPositionCenter(x, y), 0.5f).SetEase(Ease.InQuad);
                            AudioManager.Instance.PlayWooshSound();
                            yield return new WaitForSeconds(fallGemDelay);
                            break;
                        }
                    }
                }
            }
        }

        callback?.Invoke();
    }
}
