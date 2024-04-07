using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class ExplodeGemManager : MonoBehaviour
{
    [SerializeField] GameObject _explosion;

    private void OnEnable()
    {
        Match3Events.ExplodeGems += (List<Vector2Int> matches, GridSystem2D<GridObject<Gem>> grid, float explodeGemSpeed, Action callback)
        => StartCoroutine(HandleExplodeGems(matches, grid, explodeGemSpeed, callback));
    }

    private void OnDisable()
    {
        Match3Events.ExplodeGems -= (List<Vector2Int> matches, GridSystem2D<GridObject<Gem>> grid, float explodeGemSpeed, Action callback)
        => StartCoroutine(HandleExplodeGems(matches, grid, explodeGemSpeed, callback));
    }

    public IEnumerator HandleExplodeGems(List<Vector2Int> matches, GridSystem2D<GridObject<Gem>> grid, float explodeGemSpeed, Action callback)
    {
        //AudioManager.Instance.PlayPopSound();
        //Debug.Log("grid: " + grid);
        //Debug.Log("matches count: " + matches.Count);
        //Debug.Log("matches count from explode callback : " + matches.Count);

        foreach (var match in matches)
        {
            var gem = grid.GetValue(match.x, match.y).GetValue(); // TODO: null check
            grid.SetValue(match.x, match.y, null);

            HandleExplodeFx(match, grid);
            AudioManager.Instance.PlayExplosionSound();

            gem.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f, 1, 0.5f);

            yield return new WaitForSeconds(explodeGemSpeed);
            
            gem.gameObject.SetActive(false);
            GameManager.Instance.Score++;
            Match3Events.UpdateScoreText?.Invoke(GameManager.Instance.Score);
            //gem.DestroyGem();
            //Destroy(gem.gameObject);
        }

        callback?.Invoke();
    }

    public void HandleExplodeFx(Vector2Int match, GridSystem2D<GridObject<Gem>> grid)
    {
        var explodeFx = Instantiate(_explosion, transform);
        explodeFx.transform.position = grid.GetWorldPositionCenter(match.x, match.y);
        Destroy(explodeFx, 3f);
    }
}
