using UnityUtils.BaseClasses;
using Utils.Extensions;
using Object = UnityEngine.Object;

namespace Utils.BaseClasses
{
    public class DontDestroyOnLoadObjects<T> : SingletonBehavior<T> where T : Object, new()
    {
        protected virtual void Awake() => gameObject.DontDestroyOnLoadIfSingle<T>();
    }
}