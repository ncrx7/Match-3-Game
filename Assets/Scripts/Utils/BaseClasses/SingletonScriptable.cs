using System.IO;
using UnityEngine;

namespace UnityUtils.BaseClasses
{
    public class SingletonScriptable : ScriptableObject
    {
        
    }
    
    /// <summary>
    /// Türetilecek tim sınıflara "Ins" tanımı ekler
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonScriptable<T> : SingletonScriptable where T : Object, new()
    {
        private static T instance;
        public static T Instance => instance ? instance : Resources.Load(Path.Combine("Scriptables", typeof(T).Name), typeof(T)) as T;
    }
}