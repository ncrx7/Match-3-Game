using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Utils.Extensions;

public class Match3 : MonoBehaviour
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
        var gridPos = _grid.GetXY(Camera.main.ScreenToWorldPoint(inputReader.Selected));

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
            StartCoroutine(HandleGameAction(selectedGem, gridPos)); // TODO: Bu işlem bitene kadar mouse inputuna izin verme flag tanımla
        }
    }

    IEnumerator HandleGameAction(Vector2Int gridPosA, Vector2Int gridPosB)
    {
        StartCoroutine(SwapGems(gridPosA, gridPosB));

        List<Vector2Int> matches = FindMatches();
        //TODO: eşleşme bulunmazsa can eksiltme.
        //TODO: skor hesapla.

        //TODO: Optimize için match yoksa alt kısımı çalıştırma (deselect hariç o çalışsın)
        yield return StartCoroutine(HandleExplodeGems(matches));

        yield return StartCoroutine(HandleFallGems());

        yield return StartCoroutine(HandleFillEmptySlots());

        //seviyenin bitip bitmediğini kontrol et.

        DeselectGem();
    }

    IEnumerator HandleExplodeGems(List<Vector2Int> matches)
    {
        //AudioManager.Instance.PlayPopSound();
        

        foreach (var match in matches)
        {
            var gem = _grid.GetValue(match.x, match.y).GetValue(); // TODO: null check
            _grid.SetValue(match.x, match.y, null);

            HandleExplodeFx(match);
            AudioManager.Instance.PlayExplosionSound();

            gem.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f, 1, 0.5f);

            yield return new WaitForSeconds(0.1f);
            gem.DestroyGem();
            //Destroy(gem.gameObject);
        }
    }

    IEnumerator HandleFallGems()
    {
        for (var x = 0; x < _width; x++)
        {
            for (var y = 0; y < _height; y++)
            {
                if (_grid.GetValue(x, y) == null)
                {
                    for (var i = y + 1; i < _height; i++)
                    {
                        if (_grid.GetValue(x, i) != null)
                        {
                            var gem = _grid.GetValue(x, i).GetValue();
                            _grid.SetValue(x, y, _grid.GetValue(x, i));
                            _grid.SetValue(x, i, null);
                            gem.transform
                            .DOLocalMove(_grid.GetWorldPositionCenter(x, y), 0.5f).SetEase(Ease.InQuad);
                            AudioManager.Instance.PlayWooshSound();
                            yield return new WaitForSeconds(0.1f);
                            break;
                        }
                    }
                }
            }
        }
    }

    IEnumerator HandleFillEmptySlots()
    {
        for (var x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_grid.GetValue(x, y) == null)
                {
                    CreateGem(x, y);
                    AudioManager.Instance.PlayPopSound();
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
    }

    List<Vector2Int> FindMatches()
    {
        HashSet<Vector2Int> matches = new();

        //Horizontal Find
        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width - 2; x++)
            {
                var gemA = _grid.GetValue(x, y);
                var gemB = _grid.GetValue(x + 1, y);
                var gemC = _grid.GetValue(x + 2, y);

                if (gemA == null || gemB == null || gemC == null)
                {
                    continue;
                }

                if (gemA.GetValue().GetGemType() == gemB.GetValue().GetGemType() && gemB.GetValue().GetGemType() == gemC.GetValue().GetGemType())
                {
                    matches.Add(new Vector2Int(x, y));
                    matches.Add(new Vector2Int(x + 1, y));
                    matches.Add(new Vector2Int(x + 2, y));
                }
            }
        }

        //Vertical Find
        for (var x = 0; x < _width; x++)
        {
            for (var y = 0; y < _height - 2; y++)
            {
                var gemA = _grid.GetValue(x, y);
                var gemB = _grid.GetValue(x, y + 1);
                var gemC = _grid.GetValue(x, y + 2);

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

        return new List<Vector2Int>(matches);
    }

    IEnumerator SwapGems(Vector2Int gridPosA, Vector2Int gridPosB)
    {
        var gridObjectA = _grid.GetValue(gridPosA.x, gridPosA.y);
        var gridObjectB = _grid.GetValue(gridPosB.x, gridPosB.y);
        Debug.Log("gird object: " + _grid);
        gridObjectA.GetValue().transform
        .DOLocalMove(_grid.GetWorldPositionCenter(gridPosB.x, gridPosB.y), 0.5f).SetEase(Ease.InQuad);

        gridObjectB.GetValue().transform
        .DOLocalMove(_grid.GetWorldPositionCenter(gridPosA.x, gridPosA.y), 0.5f).SetEase(Ease.InQuad);

        _grid.SetValue(gridPosA.x, gridPosA.y, gridObjectB);
        _grid.SetValue(gridPosB.x, gridPosB.y, gridObjectA);

        yield return new WaitForSeconds(0.5f);
    }

    private void InitializeGrid()
    {
        _grid = GridSystem2D<GridObject<Gem>>.VerticalGrid(_width, _height, _cellSize, _originPosition, _debug);

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                CreateGem(x, y);
            }
        }
    }

    private void CreateGem(int x, int y)
    {
        Gem gem = Instantiate(_gemPrefab, _grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform); //TODO: FARKLI PARENT
        //Debug.Log("gem position: " + grid.GetWorldPositionCenter(x, y));
        gem.SetGemType(_gemTypes[Random.Range(0, _gemTypes.Length)]); //TODO: Generate algoritması geliştirilmeli
        var gridObject = new GridObject<Gem>(_grid, x, y);
        gridObject.SetValue(gem); // Initiliaize gem to grid object
        _grid.SetValue(x, y, gridObject); // Init grid object to grid matrice

    }

    void DeselectGem() => selectedGem = new Vector2Int(-1, -1);

    void SelectGem(Vector2Int gridPos) => selectedGem = gridPos;

    bool IsValidPosition(Vector2 gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < _width && gridPosition.y >= 0 && gridPosition.y < _height;
    }

    bool IsEmptyPosition(Vector2Int gridPosition) => _grid.GetValue(gridPosition.x, gridPosition.y) == null;

    void HandleExplodeFx(Vector2Int match)
    {
        var explodeFx = Instantiate(_explosion, transform);
        explodeFx.transform.position = _grid.GetWorldPositionCenter(match.x, match.y);
        Destroy(explodeFx, 3f);
    }
}
