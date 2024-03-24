using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Utils.Extensions;

public class Match3 : MonoBehaviour
{
    [SerializeField] int width = 8;
    [SerializeField] int height = 8;
    [SerializeField] float cellSize = 1f;
    [SerializeField] Vector3 originPosition = Vector3.zero;
    [SerializeField] bool debug = true;

    [SerializeField] Gem gemPrefab;
    [SerializeField] GemType[] gemTypes;

    GridSystem2D<GridObject<Gem>> grid;

    [SerializeField] InputReader inputReader;

    Vector2Int selectedGem = Vector2Int.one * -1;

    private void Start()
    {
        InitializeGrid();
        inputReader.Fire += OnSelectGem;
    }

    private void OnDestroy()
    {
        inputReader.Fire -= OnSelectGem;
    }

    void OnSelectGem()
    {
        var gridPos = grid.GetXY(Camera.main.ScreenToWorldPoint(inputReader.Selected));

        Debug.Log("gridpos: " + gridPos);
        if (!IsValidPosition(gridPos) || IsEmptyPosition(gridPos)) return;

        if (selectedGem == gridPos)
        {
            DeselectGem();
        }
        else if (selectedGem == Vector2Int.one * -1)
        {
            SelectGem(gridPos);
        }
        else
        {
            StartCoroutine(RunGameLoop(selectedGem, gridPos)); // TODO: Bu işlem bitene kadar mouse inputuna izin verme flag tanımla
        }
    }

    IEnumerator RunGameLoop(Vector2Int gridPosA, Vector2Int gridPosB)
    {
        StartCoroutine(SwapGems(gridPosA, gridPosB));

        List<Vector2Int> matches = FindMatches();
        //TODO: eşleşme bulunmazsa can eksiltme.

        yield return StartCoroutine(HandleExplodeGems(matches));

        yield return StartCoroutine(HandleFallGems());

        yield return StartCoroutine(HandleFillEmptySlots());

        DeselectGem();

        yield return null;
    }

    IEnumerator HandleExplodeGems(List<Vector2Int> matches)
    {
        //sfx
        foreach (var match in matches)
        {
            var gem = grid.GetValue(match.x, match.y).GetValue(); // null check
            grid.SetValue(match.x, match.y, null);

            //explode vfx

            gem.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f, 1, 0.5f);

            yield return new WaitForSeconds(0.1f);
            gem.DestroyGem();
            //Destroy(gem.gameObject);
        }
    }

    IEnumerator HandleFallGems()
    {
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                if(grid.GetValue(x, y) == null)
                {
                    for (var i = y + 1; i < height; i++)
                    {
                        if(grid.GetValue(x, i) != null)
                        {
                            var gem = grid.GetValue(x, i).GetValue();
                            grid.SetValue(x, y, grid.GetValue(x, i));
                            grid.SetValue(x, i, null);
                            gem.transform
                            .DOLocalMove(grid.GetWorldPositionCenter(x, y), 0.5f).SetEase(Ease.InQuad);
                            //sfx woosh sound
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
        for (var x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(grid.GetValue(x,y) == null)
                {
                    CreateGem(x, y);
                    //sfx
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
    }

    List<Vector2Int> FindMatches()
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

        return new List<Vector2Int> (matches);
    }

    IEnumerator SwapGems(Vector2Int gridPosA, Vector2Int gridPosB)
    {
        var gridObjectA = grid.GetValue(gridPosA.x, gridPosA.y);
        var gridObjectB = grid.GetValue(gridPosB.x, gridPosB.y);
        Debug.Log("gird object: " + grid);
        gridObjectA.GetValue().transform
        .DOLocalMove(grid.GetWorldPositionCenter(gridPosB.x, gridPosB.y), 0.5f).SetEase(Ease.InQuad);

        gridObjectB.GetValue().transform
        .DOLocalMove(grid.GetWorldPositionCenter(gridPosA.x, gridPosA.y), 0.5f).SetEase(Ease.InQuad);

        grid.SetValue(gridPosA.x, gridPosA.y, gridObjectB);
        grid.SetValue(gridPosB.x, gridPosB.y, gridObjectA);

        yield return new WaitForSeconds(0.5f);
    }

    private void InitializeGrid()
    {
        grid = GridSystem2D<GridObject<Gem>>.VerticalGrid(width, height, cellSize, originPosition, debug);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CreateGem(x, y);
            }
        }
    }

    private void CreateGem(int x, int y)
    {
        Gem gem = Instantiate(gemPrefab, grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform);
        Debug.Log("gem position: " + grid.GetWorldPositionCenter(x, y));
        gem.SetGemType(gemTypes[Random.Range(0, gemTypes.Length)]);
        var gridObject = new GridObject<Gem>(grid, x, y);
        gridObject.SetValue(gem); // Initiliaize gem to grid object
        grid.SetValue(x, y, gridObject); // Init grid object to grid matrice

    }

    void DeselectGem() => selectedGem = new Vector2Int(-1, -1);

    void SelectGem(Vector2Int gridPos) => selectedGem = gridPos;

    bool IsValidPosition(Vector2 gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < width && gridPosition.y >= 0 && gridPosition.y < height;
    }

    bool IsEmptyPosition(Vector2Int gridPosition) => grid.GetValue(gridPosition.x, gridPosition.y) == null;
}
