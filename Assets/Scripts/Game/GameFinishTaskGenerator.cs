using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class GameFinishTaskGenerator : MonoBehaviour
{
    public static GameFinishTaskGenerator Instance { get; private set; }
    [SerializeField] private GameObject _targetGemPrefab;
    [SerializeField] private Transform _targetGemPrefabParent;
    public Dictionary<GemType, TargetGem> Target = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        Match3Events.CreateGameTask += CreateFinishTask;
        Match3Events.LaunchTheGame += RefreshTaskDictionary;
    }

    private void OnDisable()
    {
        Match3Events.CreateGameTask -= CreateFinishTask;
        Match3Events.LaunchTheGame -= RefreshTaskDictionary;
    }

    private void Start()
    {
        SetSwapAmount();
        //CreateFinishTask(GameManager.Instance.Level, GetSortedGemWeights(GameManager.Instance.GemTypes)); // we cant use this here because couldnt synchron with game level.
    }

    //TODO: Upgrade create task algorithm
    private void CreateFinishTask(int gameLevel, List<GemType> sortedGemTypes)
    {
        for (int i = 0; i < gameLevel; i++)
        {
            GameObject targetGemGameObject = Instantiate(_targetGemPrefab, _targetGemPrefabParent.transform.position, Quaternion.identity, _targetGemPrefabParent);
            
            Image targetGemImage = targetGemGameObject.GetComponent<Image>();
            targetGemImage.sprite = sortedGemTypes[i].Sprite;

            int targetAmount = (int)sortedGemTypes[i].Weight * UnityEngine.Random.Range(3, 7);
            TextMeshProUGUI targetAmountText = targetGemGameObject.GetComponentInChildren<TextMeshProUGUI>();
            targetAmountText.text = targetAmount.ToString();

            TargetGem targetGemObject = new TargetGem(targetGemGameObject, targetGemImage, sortedGemTypes[i].Sprite, targetAmount, targetAmountText);

            Target.Add(sortedGemTypes[i], targetGemObject);
        }

        //callback?.Invoke();
    }

    public List<GemType> GetSortedGemWeights(GemType[] gemTypes)
    {
        List<GemType> sortedWeight = new();

        foreach (var gemType in gemTypes)
        {
            sortedWeight.Add(gemType);
        }

        sortedWeight.Sort((x, y) => y.Weight.CompareTo(x.Weight));

        return sortedWeight;
    }

    public void RefreshTaskDictionary(Action callback)
    {
        Target.Clear();
        callback?.Invoke();
    }

    private void SetSwapAmount()
    {
        GameManager.Instance.SwapAmount = 15 / 1; // / GameManager.Instance.Level
        Match3Events.UpdateSwapAmountText.Invoke(GameManager.Instance.SwapAmount);
    }

}
