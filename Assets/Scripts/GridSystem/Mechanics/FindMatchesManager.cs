using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FindMatchesManager : MonoBehaviour
{
    List<Vector2Int> _matches = new();
    Action<List<Vector2Int>> _returnMatchesCallback;
    private void OnEnable()
    {
        Match3Events.FindMatches += FindMatches;
        Match3Events.ReturnMatches += ReturnMatches;
    }

    private void OnDisable()
    {
        Match3Events.FindMatches -= FindMatches;
        Match3Events.ReturnMatches -= ReturnMatches;
    }

    void FindMatches(int width, int height, GridSystem2D<GridObject<Gem>> grid, List<Vector2Int> matchesList, Action callback, Action<List<Vector2Int>> callbackReturn)
    {
        HashSet<Vector2Int> matches = new();

        //Horizontal Find
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width - 2; x++)
            {
                var gemA = grid.GetValue(x, y);
                var gemB = grid.GetValue(x + 1, y);
                var gemC = grid.GetValue(x + 2, y);

                if (gemA == null || gemB == null || gemC == null)
                {
                    continue;
                }

                if (gemA.GetValue().GetGemType() == gemB.GetValue().GetGemType() && gemB.GetValue().GetGemType() == gemC.GetValue().GetGemType())
                {
                    //CheckTargetGem(gemA.GetValue().GetGemType(), ref GameFinishTaskGenerator.Instance.Target);
                    matches.Add(new Vector2Int(x, y));
                    matches.Add(new Vector2Int(x + 1, y));
                    matches.Add(new Vector2Int(x + 2, y));
                }
            }
        }

        //Vertical Find
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height - 2; y++)
            {
                var gemA = grid.GetValue(x, y);
                var gemB = grid.GetValue(x, y + 1);
                var gemC = grid.GetValue(x, y + 2);

                if (gemA == null || gemB == null || gemC == null)
                {
                    continue;
                }

                if (gemA.GetValue().GetGemType() == gemB.GetValue().GetGemType() && gemB.GetValue().GetGemType() == gemC.GetValue().GetGemType())
                {
                    matches.Add(new Vector2Int(x, y));
                    matches.Add(new Vector2Int(x, y + 1));
                    matches.Add(new Vector2Int(x, y + 2));
                }
            }
        }

        if (matches.Count == 0)
        {
            AudioManager.Instance.PlayNoMatchSound();
        }
        else
        {
            AudioManager.Instance.PlayMatchSound();
        }
        
        CheckTargetGem(ref GameFinishTaskGenerator.Instance.Target, matches, grid);
        //Debug.Log("matches hash count: " + matches.Count);
        _matches = new List<Vector2Int>(matches);
        //Debug.Log("matcheslist count: " + _matches.Count);
        _returnMatchesCallback = callbackReturn;
        callback?.Invoke();
        //return new List<Vector2Int>(matches);
    }

    List<Vector2Int> ReturnMatches()
    {
        _returnMatchesCallback?.Invoke(_matches);
        //Debug.Log("matcheslist count from return: " + _matches.Count);
        return _matches;
    }

    private void CheckTargetGem(ref Dictionary<GemType, int> targetTasks, HashSet<Vector2Int> matches, GridSystem2D<GridObject<Gem>> grid)
    {
        foreach (var item in matches)
        {
            GemType gemTypeFromHash = grid.GetValue(item.x, item.y).GetValue().GetGemType();
            if(targetTasks.ContainsKey(gemTypeFromHash))
            {
                Debug.Log("gem task value for: " + gemTypeFromHash.name + "  :" + targetTasks[gemTypeFromHash]);
                targetTasks[gemTypeFromHash]--; 
                if(targetTasks[gemTypeFromHash] < 0) targetTasks[gemTypeFromHash] = 0;
                Debug.Log("gem task value after for: " + gemTypeFromHash.name + "  :" + targetTasks[gemTypeFromHash]);
            }
        }


/*         Debug.Log("gemtype: " + gemType.name);
        if (targetTasks.ContainsKey(gemType))
        {
            Debug.Log("gem task value for: " + gemType.name + "  :" + targetTasks[gemType]);
            targetTasks[gemType] -= 3;

            if(targetTasks[gemType] < 0) targetTasks[gemType] = 0;
            Debug.Log("gem task value after for: " + gemType.name + "  :" + targetTasks[gemType]);
        } */
    }
}
