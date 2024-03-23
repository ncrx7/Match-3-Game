using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        ///     An object extension method that converts the @this to a float.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>@this as a float.</returns>
        public static float ToFloat(this object @this) => Convert.ToSingle(@this);
        
        public static void SetValue(this object target, string fieldName, object value)
        {
            var type = target.GetType();
            FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(target, value);
                Debug.Log("Field value is : " + value + ". Result" + 
                          type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public).GetValue(target));
                return;
            }
            
            var propertyInfo = type.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(target, value);
                Debug.Log("Property value is : " + value + ". Result" + 
                          type.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public).GetValue(target));
                return;
            }

            Debug.Log($"Field with name {fieldName} not found in class {type.Name}.");
        }
        
        public static object GetValue(this object target, string fieldName)
        {
            var type = target.GetType();
            FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
            if (fieldInfo != null)
                return fieldInfo.GetValue(target);
            
            var propertyInfo = type.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public);
            if (propertyInfo != null)
                return propertyInfo.GetValue(target);

            Debug.Log($"Field with name {fieldName} not found in class {type.Name}.");
            return null;
        }
        
        public static bool ValueIsDefined(this object target, string fieldName)
        {
            var type = target.GetType();
            FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
            if (fieldInfo != null)
                return true;
            
            var propertyInfo = type.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public);
            return propertyInfo != null;
        }

        public static bool TryGetComponentInParent<T>(this Object thisGameObject, out T component)
            where T : Component
        {
            component = thisGameObject.GameObject().GetComponentInParent<T>();
            return component is not null;
        }

        public static bool TryGetComponentsInChildren<T>(this Object thisGameObject, out List<T> components)
            where T : Component
        {
            components = thisGameObject.GameObject().GetComponentsInChildren<T>().ToList();
            if (components == null) return false;
            return components.Count > 0;
        }

        public static bool TryGetComponentInChildren<T>(this Object thisGameObject, out T component,
            bool includeInactive = false) where T : Component
        {
            component = thisGameObject.GameObject().GetComponentInChildren<T>(includeInactive);
            return component is not null;
        }
        
        public static bool TryGetComponent<T>(this Object thisGameObject, out T component) where T : Component
        {
            component = thisGameObject.GameObject().GetComponent<T>();
            return component;
        }
        
        public static bool TryAddComponent<T>(this Object thisGameObject, out T component) where T : Component
        {
            if (thisGameObject.TryGetComponent(out component))
                return true;
            
            component = thisGameObject.AddComponent<T>();
            return component;
        }

        #region Unity Editor

#if UNITY_EDITOR
        public static void OpenObjectInEditor(this Object target)
        {
            string path = AssetDatabase.GetAssetPath(target);
            Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            Selection.activeObject = obj;
        }
#endif

        #endregion
    }
}