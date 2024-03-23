using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            StartCoroutine(RunGameLoop(selectedGem, gridPos));
        }
    }

    IEnumerator RunGameLoop(Vector2Int gridPosA, Vector2Int gridPosB)
    {
        var gridObjectA = grid.GetValue(gridPosA.x, gridPosA.y);
        var gridObjectB = grid.GetValue(gridPosB.x, gridPosB.y);
        yield return null;
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
        gem.SetGemType(gemTypes[Random.Range(0, gemTypes.Length)]);
        var gridObject = new GridObject<Gem>(grid, x, y);
        gridObject.SetValue(gem);
        grid.SetValue(x, y, gridObject);

    }

    void DeselectGem() => selectedGem = new Vector2Int(-1, -1);
    void SelectGem(Vector2Int gridPos) => selectedGem = gridPos;

    //init grid

    //read player input and swap gems

    //start coroutine
    //swap animation
    //matches
    //make gems explode
    //replace empty slots
    //is game over
}
