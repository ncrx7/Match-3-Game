using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Gem : MonoBehaviour 
{
    public GemType Type;

    public void SetGemType(GemType type)
    {
        this.Type = type;
        GetComponent<SpriteRenderer>().sprite = type.Sprite;
    }

    public GemType GetGemType()
    {
        return Type;
    }

    public void DestroyGem()
    {
        Destroy(gameObject);
    }
}
