using System.Collections;
using System.Collections.Generic;
using Core;
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

    private void CreateGem(int x, int y, GridSystem2D<GridObject<Gem>> grid, GemType[] gemTypes, int gemPoolId)
    {
        Gem gem = ObjectPooler.GetObject(gemPoolId) as Gem;
        gem.transform.position = grid.GetWorldPositionCenter(x, y);
        gem.transform.SetParent(transform);
        
        //Gem gem = Instantiate(gemPrefab, grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform); //TODO: FARKLI PARENT
                                                                                                             //Debug.Log("gem position: " + grid.GetWorldPositionCenter(x, y));
        gem.SetGemType(GenerateGemType(gemTypes)); // Rastgele gem tipini seç
        var gridObject = new GridObject<Gem>(grid, x, y);
        gridObject.SetValue(gem); // Gem'i grid nesnesine initialize et
        grid.SetValue(x, y, gridObject); // Grid matrisine grid nesnesini initialize et
    }

    private GemType GenerateGemType(GemType[] gemTypes)
    {
        float[] weights = new float[gemTypes.Length];
        for (int i = 0; i < gemTypes.Length; i++)
        {
            weights[i] = gemTypes[i].Weight; 
        }

        int selectedIndex = WeightedRandom(weights);

        return gemTypes[selectedIndex];
    }

    private int WeightedRandom(float[] weights)
    {
        float totalWeight = 0f;
        foreach (float weight in weights)
        {
            totalWeight += weight;
        }

        float randomValue = Random.Range(0f, totalWeight);

        for (int i = 0; i < weights.Length; i++)
        {
            if (randomValue < weights[i])
            {
                return i;
            }
            randomValue -= weights[i];
        }

        return (int)Random.Range(0f, weights.Length);
    }

    //TODO: We can create other create method for creating any other gridobject
}
