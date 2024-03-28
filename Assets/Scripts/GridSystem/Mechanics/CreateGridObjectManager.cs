using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGridObjectManager : MonoBehaviour
{
    private void OnEnable()
    {
        Match3Events.CreateGemObject += CreateGem;
    }

    private void OnDisable()
    {
        Match3Events.CreateGemObject -= CreateGem;
    }

    private void CreateGem(int x, int y, GridSystem2D<GridObject<Gem>> grid, GemType[] gemTypes, Gem gemPrefab)
    {
        Gem gem = Instantiate(gemPrefab, grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform); //TODO: FARKLI PARENT
        //Debug.Log("gem position: " + grid.GetWorldPositionCenter(x, y));
        gem.SetGemType(gemTypes[UnityEngine.Random.Range(0, gemTypes.Length)]); //TODO: Generate algoritması geliştirilmeli
        var gridObject = new GridObject<Gem>(grid, x, y);
        gridObject.SetValue(gem); // Initiliaize gem to grid object
        grid.SetValue(x, y, gridObject); // Init grid object to grid matrice

    }

    //TODO: We can create other create method for creating any other gridobject
}
