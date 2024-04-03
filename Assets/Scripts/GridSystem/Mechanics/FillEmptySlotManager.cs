using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillEmptySlotManager : MonoBehaviour
{
    private void OnEnable()
    {
        Match3Events.FillEmptySlots += (int width, int height, GridSystem2D<GridObject<Gem>> grid, GemType[] gemTypes, int gemPoolId, float fillEmptySlotDelay, Action callback)
        => StartCoroutine(HandleFillEmptySlots(width, height, grid, gemTypes, gemPoolId, fillEmptySlotDelay, callback));
    }

    private void OnDisable()
    {
        Match3Events.FillEmptySlots -= (int width, int height, GridSystem2D<GridObject<Gem>> grid, GemType[] gemTypes, int gemPoolId, float fillEmptySlotDelay, Action callback)
        => StartCoroutine(HandleFillEmptySlots(width, height, grid, gemTypes, gemPoolId, fillEmptySlotDelay, callback));
    }

    IEnumerator HandleFillEmptySlots(int width, int height, GridSystem2D<GridObject<Gem>> grid, GemType[] gemTypes, int gemPoolId, float fillEmptySlotDelay, Action callback)
    {
        for (var x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid.GetValue(x, y) == null)
                {
                    Match3Events.CreateGemObject?.Invoke(x, y, grid, gemTypes, gemPoolId);
                    //CreateGem(x, y);
                    AudioManager.Instance.PlayPopSound();
                    yield return new WaitForSeconds(fillEmptySlotDelay);
                }
            }
        }

        callback?.Invoke();
    }
}
