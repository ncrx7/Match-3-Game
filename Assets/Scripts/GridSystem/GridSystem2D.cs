using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridSystem2D<T>
{
    readonly int _width;
    readonly int _height;
    readonly float _cellSize;
    readonly Vector3 _origin;
    readonly T[,] _gridArray;

    // creating for both vertical and horizontal grids
    readonly CoordinateConverter _coordinateConverter;

    public event Action<int, int, T> OnValueChangeEvent;

    //choose one In match3 launch horizontal or vertical to make grid. (factory methods)
    public static GridSystem2D<T> VerticalGrid(int width, int height, float cellSize, Vector3 origin, bool debug = false)
    {
        return new GridSystem2D<T>(width, height, cellSize, origin, new VerticalConverter(), debug);
    }

    public static GridSystem2D<T> HorizontalGrid(int width, int height, float cellSize, Vector3 origin, bool debug = false)
    {
        return new GridSystem2D<T>(width, height, cellSize, origin, new HorizontalConverter(), debug);
    }

    public GridSystem2D(int width, int height, float cellSize, Vector3 origin, CoordinateConverter coordinateConverter, bool debug)
    {
        this._width = width;
        this._height = height;
        this._cellSize = cellSize;
        this._origin = origin;
        this._coordinateConverter = coordinateConverter ?? new VerticalConverter();

        _gridArray = new T[width, height];

        if (debug)
        {
            DrawDebugLines();
        }
    }
    // methods for setting values from grid positions
    public void SetValue(Vector3 worldPosition, T value)
    {
        Vector2Int pos = _coordinateConverter.WorldToGrid(worldPosition, _cellSize, _origin);
        SetValue(pos.x, pos.y, value);
    }

    public void SetValue(int x, int y, T value)
    {
        if (IsValid(x, y))
        {
            _gridArray[x, y] = value;
            OnValueChangeEvent?.Invoke(x, y, value);
        }
    }
    // methods for getting values from grid positions
    public T GetValue(Vector3 worldPosition)
    {
        Vector2Int pos = GetXY(worldPosition);
        return GetValue(pos.x, pos.y);
    }

    public T GetValue(int x, int y)
    {
        return IsValid(x, y) ? _gridArray[x, y] : default(T);
    }

    bool IsValid(int x, int y) => x >= 0 && y >= 0 && x < _width && y < _height;

    // kordinat dönüştürücümüzün türüne göre göre çevirici fonksiyonlarımızı yazdık.
    public Vector2Int GetXY(Vector3 worldPosition) => _coordinateConverter.WorldToGrid(worldPosition, _cellSize, _origin);

    public Vector3 GetWorldPositionCenter(int x, int y) => _coordinateConverter.GridToWorldCenter(x, y, _cellSize, _origin);

    Vector3 GetWorldPosition(int x, int y)
    {
        Vector3 coordinate = _coordinateConverter.GridToWorld(x, y, _cellSize, _origin);
        return coordinate;
    }

    void DrawDebugLines()
    {
        const float duration = 100f;
        var parent = new GameObject("Debugging");

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                CreateWorldText(parent, x + "," + y, GetWorldPositionCenter(x, y), _coordinateConverter.Forward);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, duration);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, duration);
            }
        }

        Debug.DrawLine(GetWorldPosition(0, _height), GetWorldPosition(_width, _height), Color.white, duration);
        Debug.DrawLine(GetWorldPosition(_width, 0), GetWorldPosition(_width, _height), Color.white, duration);
    }

    private void CreateWorldText(GameObject parent, string text, Vector3 position, Vector3 dir,
            int fontSize = 2, Color color = default, TextAlignmentOptions textAnchor = TextAlignmentOptions.Center, int sortingOrder = 0)
    {
        GameObject gameObject = new GameObject("Debug Text_" + text, typeof(TextMeshPro));
        gameObject.transform.SetParent(parent.transform); //!!
        gameObject.transform.position = position;
        gameObject.transform.forward = dir;

        TextMeshPro textMeshPro = gameObject.GetComponent<TextMeshPro>();
        textMeshPro.text = text;
        textMeshPro.fontSize = fontSize;
        textMeshPro.color = color == default ? Color.white : color;
        textMeshPro.alignment = textAnchor;
        textMeshPro.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
    }

    public abstract class CoordinateConverter
    {
        public abstract Vector3 GridToWorld(int x, int y, float cellSize, Vector3 origin);
        public abstract Vector3 GridToWorldCenter(int x, int y, float cellSize, Vector3 origin);
        public abstract Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 origin);
        public abstract Vector3 Forward { get; }

    }

    //grid converter for vertical grids.
    public class VerticalConverter : CoordinateConverter
    {
        public override Vector3 Forward => Vector3.forward;

        public override Vector3 GridToWorld(int x, int y, float cellSize, Vector3 origin)
        {
            return new Vector3(x, y) * cellSize + origin; // x and y is coordinate of grid (x , y is not unity world position, just a quee of the grid)
        }

        public override Vector3 GridToWorldCenter(int x, int y, float cellSize, Vector3 origin)
        {
            return new Vector3(x * cellSize + cellSize * 0.5f, y * cellSize + cellSize * 0.5f) + origin;
        }

        public override Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 origin)
        {
            Vector3 gridPosition = (worldPosition - origin) / cellSize;
            var x = Mathf.FloorToInt(gridPosition.x);
            var y = Mathf.FloorToInt(gridPosition.y);
            return new Vector2Int(x, y);
        }
    }

    public class HorizontalConverter : CoordinateConverter
    {
        public override Vector3 Forward => -Vector3.up;

        public override Vector3 GridToWorld(int x, int y, float cellSize, Vector3 origin)
        {
            return new Vector3(x, 0, y) * cellSize + origin;
        }

        public override Vector3 GridToWorldCenter(int x, int y, float cellSize, Vector3 origin)
        {
            return new Vector3(x * cellSize + cellSize * 0.5f, 0, y * cellSize + cellSize * 0.5f) + origin;
        }

        public override Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 origin)
        {
            Vector3 gridPosition = (worldPosition - origin) / cellSize;
            var x = Mathf.FloorToInt(gridPosition.x);
            var y = Mathf.FloorToInt(gridPosition.z);
            return new Vector2Int(x, y);
        }
    }
}
