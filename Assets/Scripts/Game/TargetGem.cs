using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetGem
{
    public GameObject TargetGemPrefab {get; set;}
    public Image TargetGemImage {get; set;}
    public Sprite TargetGemSprite{get; set;}
    public int TargetAmount {get; set;}
    public TextMeshProUGUI TextObject {get; set;}

    public TargetGem(GameObject targetGem, Image targetGemImage, Sprite targetGemSprite, int targetAmount, TextMeshProUGUI textObject)
    {
        this.TargetGemPrefab = targetGem;
        this.TargetGemImage = targetGemImage;
        this.TargetGemSprite = targetGemSprite;
        this.TargetAmount = targetAmount;
        this.TextObject = textObject;
    } 
}
