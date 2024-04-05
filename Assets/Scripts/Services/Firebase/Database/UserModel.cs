using System;
using UnityEngine.Serialization;

namespace Services.Firebase.Database
{
    [Serializable]
    public class UserModel
    {
        public string UserName;
        public int Level;
        public int HighScore;
    }
}