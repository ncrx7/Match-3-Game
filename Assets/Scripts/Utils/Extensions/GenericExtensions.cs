using UnityEditor;

namespace Utils.Extensions
{
    public static class GenericExtensions 
    {
        #region Unity Editor
#if UNITY_EDITOR
        public static T GetObjectFromPath<T>(this string filter, string searchInFolder) where T : class
        {
            var paths = AssetDatabase.FindAssets(filter, new [] { searchInFolder });
            var path = AssetDatabase.GUIDToAssetPath(paths[0]);
            return AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
        }
#endif
        #endregion
    }
}