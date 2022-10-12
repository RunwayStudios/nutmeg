using System.Collections.Generic;
using UnityEngine;

namespace Nutmeg.Runtime.Utility.GameObjectPooling
{
    public class GoPoolingManager : MonoBehaviour
    {
        public static GoPoolingManager Main;
        
        private Dictionary<GameObject, List<GameObject>> pools = new Dictionary<GameObject, List<GameObject>>();


        private void Awake()
        {
            Main = this;
        }


        private void CreatePool(GameObject prefab)
        {
            if (pools.ContainsKey(prefab))
                return;
            
            pools.Add(prefab, new List<GameObject>());
        }

        public GameObject Get(GameObject prefab)
        {
            if (!pools.ContainsKey(prefab))
                CreatePool(prefab);
            
            List<GameObject> gameObjects = pools[prefab];
            if (gameObjects.Count < 1)
                return Instantiate(prefab, transform);

            int endIndex = gameObjects.Count - 1;
            GameObject go = gameObjects[endIndex];
            gameObjects.RemoveAt(endIndex);
            return go;
        }
        
        public void Return(GameObject go, GameObject prefab)
        {
            if (!pools.TryGetValue(prefab, out List<GameObject> gameObjects))
                return;
            
            go.SetActive(false);
            gameObjects.Add(go);
        }
    }
}
