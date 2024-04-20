using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardReferenceHolder : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _userNameText;
    [SerializeField] private TextMeshProUGUI _highScoreText;

    public TextMeshProUGUI GetUserNameTextReference()
    {
        return _userNameText;
    }

    public TextMeshProUGUI GetHighScoreTextReference()
    {
        return _highScoreText;
    }
}
