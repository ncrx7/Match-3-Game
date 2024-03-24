using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Gem : MonoBehaviour 
{
    public GemType type;

    public void SetGemType(GemType type)
    {
        this.type = type;
        GetComponent<SpriteRenderer>().sprite = type.sprite;
    }

    public GemType GetGemType()
    {
        return type;
    }

    public void DestroyGem()
    {
        Destroy(gameObject);
    }
}
