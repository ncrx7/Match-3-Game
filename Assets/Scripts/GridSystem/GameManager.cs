using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using DG.Tweening;
using Services.Firebase.Database;
using UnityEngine;
using Utils.Extensions;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] public int SwapAmount {get; set;}
    public int Level {get; set;} = 5;
    public int Score {get; set;}
    public int HighScore {get; set;}
    public bool LevelPassed {get; set;}

    [Header("Grid Settings")]
    [SerializeField] int _width = 8;
    [SerializeField] int _height = 8;
    [SerializeField] float _cellSize = 1f;
    [SerializeField] Vector3 _originPosition = Vector3.zero;
    [SerializeField] bool _debug = true;

    [Header("Gem Settings")]
    [SerializeField] Gem _gemPrefab;
    [SerializeField] public GemType[] GemTypes;

    [Header("FX")]
    [SerializeField] GameObject _explosion;

    [Header("Game mechanic delay")]
    [SerializeField] private float _swapGemDelay;
    [SerializeField] private float _explodeGemDelay;
    [SerializeField] private float _fallGemDelay;
    [SerializeField] private float _fillEmptySlotDelay;


    GridSystem2D<GridObject<Gem>> _grid;
    private bool _isProcessing;
    Vector2Int selectedGem = Vector2Int.one * -1;
    private int _gemPoolId;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private async void Start()
    {
        _gemPoolId = ObjectPooler.CreatePool(_gemPrefab, 100);
        InitializeGridAndGems();
        InputReader.Instance.Fire += OnSelectGem; // when mouse 1 clicked
        Match3Events.RepeatGameActions += HandleGameActions;

        //TODO: SET LEVEL FROM DATABASE
        var result = await Database.GetLevel() ;
        Level = result.Item;
        Debug.Log("lv from gm:" + Level);
        Match3Events.UpdateLevelText?.Invoke(Level);

        Match3Events.CreateInitialTask?.Invoke(Level, GameFinishTaskGenerator.Instance.GetSortedGemWeights(GemTypes));
    }

    private void OnDestroy()
    {
        InputReader.Instance.Fire -= OnSelectGem;
        Match3Events.RepeatGameActions -= HandleGameActions;
    }
    
    // when mouse 1 clicked
    void OnSelectGem()
    {
        if (!_isProcessing && SwapAmount > 0)
        {
            var gridPos = _grid.GetXY(Camera.main.ScreenToWorldPoint(InputReader.Instance.Selected)); //TODO: make inputreader class singleton

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
            else if (!CheckSelectedGemDistance(selectedGem, gridPos)) //TODO: If distance between selected object and clicked object is greater than 1,1, run this state
            {
                DeselectGem();
                AudioManager.Instance.PlayDeselectSound();
            }
            else
            {
                SwapAmount--;
                Match3Events.UpdateSwapAmountText?.Invoke(SwapAmount);
                Debug.Log("swap amount: " + SwapAmount);

                HandleGameActions(selectedGem, gridPos); // TODO: Bu işlem bitene kadar mouse inputuna izin verme flag tanımla
            }
        }
    }

    void HandleGameActions(Vector2Int gridPosA, Vector2Int gridPosB) //TEKRARLAMA 1.çözüm: burayı void yapıp RepeatGameAction eventine abone etmek
    {
        _isProcessing = true;
        List<Vector2Int> matches = new List<Vector2Int>();
        //TODO: eşleşme bulunmazsa can eksilt.
        //TODO: skor hesapla.

        //TODO: Optimize için match yoksa alt kısımı çalıştırma (deselect hariç o çalışsın)

        #region sequential callback
        Action fillEmptySlotsCallback = delegate ()
        {
            _isProcessing = false;

            if (matches.Count == 0 && SwapAmount == 0 && !LevelPassed)
            {
                Match3Events.OnGameFinishedUnsuccessfully?.Invoke();
                return;
            } 

            if (matches.Count == 0) return;
            Debug.Log("mathces count above repeat: " + matches.Count);
            
            Match3Events.RepeatGameActions.Invoke(new Vector2Int(0, 0), new Vector2Int(0, 0)); //if there are matches after all process, repeat the procoess until no match is found
        };

        Action fallGemsCallback = () => Match3Events.FillEmptySlots?.Invoke(_width, _height, _grid, GemTypes, _gemPoolId, _fillEmptySlotDelay, fillEmptySlotsCallback);

        Action explodeGemsCallback = delegate ()
        {
            //matches.Clear();
            Match3Events.FallGems?.Invoke(_width, _height, _grid, _fallGemDelay, fallGemsCallback);
        };

        Action<List<Vector2Int>> returnMatchesCallback = delegate (List<Vector2Int> matchesUpdated)
        {
            matches = matchesUpdated;
            Match3Events.ExplodeGems?.Invoke(matchesUpdated, _grid, _explodeGemDelay, explodeGemsCallback); //burda kaldın explode içinde matches güncellenmiyor -- halledildi
        };

        Action findMatchesCallback = () => matches = Match3Events.ReturnMatches?.Invoke();

        Action swapGemsCallback = delegate ()
        {
            //_isProcessing = true;
            Match3Events.FindMatches?.Invoke(_width, _height, _grid, matches, findMatchesCallback, returnMatchesCallback);
        };
        #endregion

        Match3Events.SwapGems?.Invoke(gridPosA, gridPosB, _grid, _swapGemDelay, swapGemsCallback);

        //seviyenin bitip bitmediğini kontrol et.

        DeselectGem();
        //yield return null;
    }

    private void InitializeGridAndGems()
    {
        _grid = GridSystem2D<GridObject<Gem>>.VerticalGrid(_width, _height, _cellSize, _originPosition, _debug);

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Match3Events.CreateGemObject?.Invoke(x, y, _grid, GemTypes, _gemPoolId);
                //CreateGem(x, y);
            }
        }
    }

    private bool CheckSelectedGemDistance(Vector2Int selectedGem, Vector2Int gridPos)
    {
        if (selectedGem.x - gridPos.x == 0 && MathF.Abs(selectedGem.y - gridPos.y) == 1)
        {
            return true;
        }
        else if (MathF.Abs(selectedGem.x - gridPos.x) == 1 && selectedGem.y - gridPos.y == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void DeselectGem()
    {
        if (selectedGem != null) _grid.GetValue(selectedGem.x, selectedGem.y).GetValue().transform.DOPunchScale(Vector3.one * 0.4f, 0.3f, 1, 0.5f);
        selectedGem = new Vector2Int(-1, -1);
    }

    void SelectGem(Vector2Int gridPos) => selectedGem = gridPos;

    bool IsValidPosition(Vector2 gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < _width && gridPosition.y >= 0 && gridPosition.y < _height;
    }

    bool IsEmptyPosition(Vector2Int gridPosition) => _grid.GetValue(gridPosition.x, gridPosition.y) == null;
}
