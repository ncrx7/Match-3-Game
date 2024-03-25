using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject<T>
{
    //We made this class generic so that it would be more dynamic and convenient for us to put another object instead of a gem.
    GridSystem2D<GridObject<T>> grid;
    private int _x;
    private int _y;
    private T _gem;

    public GridObject(GridSystem2D<GridObject<T>> grid, int x, int y)
    {
        this.grid = grid;
        this._x = x;
        this._y = y;
    }

    public void SetValue(T gem)
    {
        this._gem = gem;
    }

    public T GetValue()
    {
        return _gem;
    }
}
