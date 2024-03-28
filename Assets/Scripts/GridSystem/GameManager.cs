using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Utils.Extensions;


public class GameManager : MonoBehaviour
{
    [SerializeField] int _width = 8;
    [SerializeField] int _height = 8;
    [SerializeField] float _cellSize = 1f;
    [SerializeField] Vector3 _originPosition = Vector3.zero;
    [SerializeField] bool _debug = true;

    [SerializeField] Gem _gemPrefab;
    [SerializeField] GemType[] _gemTypes;
    [SerializeField] GameObject _explosion;

    GridSystem2D<GridObject<Gem>> _grid;

    [SerializeField] InputReader inputReader;

    [Header("Game mechanic delay speed")]
    [SerializeField] private float _swapGemSpeed;
    [SerializeField] private float _explodeGemSpeed;
    [SerializeField] private float _fallGemDelay;
    [SerializeField] private float _fillEmptySlotDelay;

    Vector2Int selectedGem = Vector2Int.one * -1;

    private void Start()
    {
        InitializeGrid();
        inputReader.Fire += OnSelectGem; // when mouse 1 clicked
    }

    private void OnDestroy()
    {
        inputReader.Fire -= OnSelectGem;
    }

    // when mouse 1 clicked
    void OnSelectGem()
    {
        var gridPos = _grid.GetXY(Camera.main.ScreenToWorldPoint(inputReader.Selected)); //TODO: make inputreader class singleton

        //Debug.Log("gridpos: " + gridPos);
        if (!IsValidPosition(gridPos) || IsEmptyPosition(gridPos)) return;

        if (selectedGem == gridPos) //when clicked to same grid object two times.
        {
            DeselectGem();
            AudioManager.Instance.PlayDeselectSound();
        }
        else if (selectedGem == Vector2Int.one * -1) // If there is no selected grid object.
        {
            SelectGem(gridPos);
            AudioManager.Instance.PlayClickSound();
        }
        else
        {
            StartCoroutine(HandleGameActions(selectedGem, gridPos)); // TODO: Bu işlem bitene kadar mouse inputuna izin verme flag tanımla
        }
    }

    IEnumerator HandleGameActions(Vector2Int gridPosA, Vector2Int gridPosB)
    {
        List<Vector2Int> matches = new List<Vector2Int>();
        //TODO: eşleşme bulunmazsa can eksilt.
        //TODO: skor hesapla.

        //TODO: Optimize için match yoksa alt kısımı çalıştırma (deselect hariç o çalışsın)

        #region sequential callback
        Action fillEmptySlotsCallback = null;
        Action fallGemsCallback = () => Match3Events.FillEmptySlots?.Invoke(_width, _height, _grid, _gemTypes, _gemPrefab , _fillEmptySlotDelay, fillEmptySlotsCallback);
        Action explodeGemsCallback = () => Match3Events.FallGems?.Invoke(_width, _height, _grid, _fallGemDelay, fallGemsCallback);
        Action<List<Vector2Int>> returnMatchesCallback = (List<Vector2Int> matchesUpdated) => Match3Events.ExplodeGems?.Invoke(matchesUpdated, _grid, _explodeGemSpeed, explodeGemsCallback); //burda kaldın explode içinde matches güncellenmiyor
        Action findMatchesCallback = () => matches = Match3Events.ReturnMatches?.Invoke();
        Action swapGemsCallback = () => Match3Events.FindMatches?.Invoke(_width, _height, _grid, matches, findMatchesCallback, returnMatchesCallback);
        #endregion

        Match3Events.SwapGems?.Invoke(gridPosA, gridPosB, _grid, _swapGemSpeed, swapGemsCallback);
        //Match3Events.ExplodeGems?.Invoke(matches, _grid, explodeGemsCallback);

        //seviyenin bitip bitmediğini kontrol et.

        DeselectGem();
        yield return null;
    }

    private void InitializeGrid()
    {
        _grid = GridSystem2D<GridObject<Gem>>.VerticalGrid(_width, _height, _cellSize, _originPosition, _debug);

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Match3Events.CreateGemObject?.Invoke(x, y, _grid, _gemTypes, _gemPrefab);
                //CreateGem(x, y);
            }
        }
    }

    void DeselectGem() => selectedGem = new Vector2Int(-1, -1);

    void SelectGem(Vector2Int gridPos) => selectedGem = gridPos;

    bool IsValidPosition(Vector2 gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < _width && gridPosition.y >= 0 && gridPosition.y < _height;
    }

    bool IsEmptyPosition(Vector2Int gridPosition) => _grid.GetValue(gridPosition.x, gridPosition.y) == null;
}
