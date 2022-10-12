using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nutmeg.Runtime.Utility.GameObjectPooling
{
    public class GoPoolingManager : MonoBehaviour
    {
        public static GoPoolingManager Main;

        [SerializeField] private float gameObjectCooldown = 0.1f;

        private Dictionary<GameObject, List<GameObject>> pools = new();
        private List<GameObjectToReturn> gameObjectsToReturn = new();


        private void Awake()
        {
            Main = this;
        }

        private void Update()
        {
            for (int i = gameObjectsToReturn.Count - 1; i >= 0; i--)
            {
                if (gameObjectsToReturn[i].t <= 0f)
                {
                    ReturnInternal(gameObjectsToReturn[i].go, gameObjectsToReturn[i].prefab);
                    gameObjectsToReturn.RemoveAt(i);
                    continue;
                }

                GameObjectToReturn gotr = gameObjectsToReturn[i];
                gotr.t -= Time.deltaTime;
                gameObjectsToReturn[i] = gotr;
            }
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
            {
                GameObject temp = Instantiate(prefab, transform);
                temp.SetActive(false);
                return temp;
            }

            int endIndex = gameObjects.Count - 1;
            GameObject go = gameObjects[endIndex];
            gameObjects.RemoveAt(endIndex);
            return go;
        }

        public void Return(GameObject go, GameObject prefab)
        {
            go.SetActive(false);
            gameObjectsToReturn.Add(new GameObjectToReturn(go, prefab, gameObjectCooldown));
        }

        private void ReturnInternal(GameObject go, GameObject prefab)
        {
            if (!pools.TryGetValue(prefab, out List<GameObject> gameObjects))
                return;

            gameObjects.Add(go);
        }

        private struct GameObjectToReturn
        {
            public GameObjectToReturn(GameObject go, GameObject prefab, float t)
            {
                this.go = go;
                this.prefab = prefab;
                this.t = t;
            }

            public GameObject go;
            public GameObject prefab;
            public float t;
        }
    }
}