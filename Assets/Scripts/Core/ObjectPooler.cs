using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtils.BaseClasses;

namespace Core
{
    public class ObjectPooler : SingletonBehavior<ObjectPooler>
    {
        private Dictionary<int, Queue<Component>> _queuedPools;

        private void Awake()
        {
            _queuedPools = new Dictionary<int, Queue<Component>>();
        }

        public static Component GetObject(int poolId, bool active = true)
        {
            if (!Instance._queuedPools.TryGetValue(poolId, out var objects))
                return null;
            
            var result = Instance._queuedPools[poolId].Dequeue();
            
            //If the object is closed, it is not ready, we queue it again.
            while (result.gameObject.activeSelf)
            {
                Instance._queuedPools[poolId].Enqueue(result);    
                result = Instance._queuedPools[poolId].Dequeue();
            }
            
            if (active)
                result.gameObject.SetActive(true);
            
            Instance._queuedPools[poolId].Enqueue(result);
            return result;
        }

        public static int CreatePool(Component poolObject, int copy = 50)
        {
            var objects = new Queue<Component>();

            int key = Instance._queuedPools.Count;
            var parent = CreatePoolParent(poolObject.name, key);

            for (int i = 0; i < copy; i++)
            {
                
                var ins = Instantiate(poolObject, parent);
                ins.gameObject.SetActive(false);
                objects.Enqueue(ins);
            }

            Instance._queuedPools.Add(key, objects);
            return key;
        }

        private static Transform CreatePoolParent(string name, int key)
        {
            var result = new GameObject($"Name : {name}, key : {key}");
            result.transform.SetParent(Instance.transform);
            return result.transform;
        }
    }
}