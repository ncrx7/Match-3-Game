using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextManager
{
    public static void CreateWorldText(GameObject parent, string text, Vector3 position, Vector3 dir,
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
}
