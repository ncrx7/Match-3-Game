using System.Collections;
using System.Collections.Generic;
using TMPro;

//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class GameFinishTaskGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _targetGemPrefab;
    [SerializeField] private Transform _targetGemPrefabParent;
    private Dictionary<GemType, int> _target = new();
    
    private void Start()
    {
        SetSwapAmount();
        CreateFinishTask(GameManager.Instance.Level, GetSortedGemWeights(GameManager.Instance.GemTypes));
    }

    private void CreateFinishTask(int gameLevel, List<GemType> sortedGemTypes)
    {
        for (int i = 0; i < gameLevel; i++)
        {
            GameObject targetGem = Instantiate(_targetGemPrefab, _targetGemPrefabParent.transform.position, Quaternion.identity, _targetGemPrefabParent);
            Image targetGemImage = targetGem.GetComponent<Image>();
            targetGemImage.sprite = sortedGemTypes[i].Sprite;

            int targetAmount = (int)sortedGemTypes[i].Weight * Random.Range(1, 3);
            TextMeshProUGUI targetAmountText = targetGem.GetComponentInChildren<TextMeshProUGUI>();
            targetAmountText.text = targetAmount.ToString();
            _target.Add(sortedGemTypes[i], targetAmount);
        }
    }

    private List<GemType> GetSortedGemWeights(GemType[] gemTypes)
    {
        List<GemType> sortedWeight = new();

        foreach (var gemType in gemTypes)
        {
            sortedWeight.Add(gemType);
        }

        sortedWeight.Sort((x, y) => x.Weight.CompareTo(y.Weight));

        return sortedWeight;
    }

    private void SetSwapAmount()
    {
        GameManager.Instance.SwapAmount = GameManager.Instance.Level * 3;
    }
    
}
