using UnityEngine;

namespace UnityUtils.BaseClasses
{
    public class SingletonBehavior : MonoBehaviour
    {
    }

    /// <summary>
    /// Türetilecek tim sınıflara "Ins" tanımı ekler
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonBehavior<T> : SingletonBehavior where T : Object, new()
    {
        private static T instance;
        public static T Instance => instance ? instance : (instance = FindObjectOfType<T>());
    }
}