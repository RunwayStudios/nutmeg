using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nutmeg.Runtime.Utility.WorldSpaceUI
{
    public class WorldSpaceUI : MonoBehaviour
    {
        public static int maxConcurrentWorldSpaceUIs = 400;

        private static Dictionary<int, WSUIElement> elements;
        private static List<int> activeElements;

        public static int LastID { get; private set; }

        public static int NextID => LastID++;

        private void Start()
        {
            elements = new Dictionary<int, WSUIElement>();
            activeElements = new List<int>();
        }

        private void Update()
        {
            UpdateElements();
        }

        private void UpdateElements()
        {
            foreach (int i in new List<int>(activeElements))
            {
                if (elements.TryGetValue(i, out var e))
                    e.UpdateInternal(Time.deltaTime);
            }
        }

        private static void OnCompleteElement(WSUIElement element)
        {
            if (activeElements.Count != 1)
            {
                activeElements.Remove(element.Id);
            }
            else
            {
                activeElements = new List<int>();
            }

            if (element.destroyOnComplete)
                Remove(element);
        }

        public static bool GetElementFromId(int id, out WSUIElement element) => elements.TryGetValue(id, out element);

        public static void Remove(WSUIElement element)
        {
            if(elements.Count != 1)
            {
                elements.Remove(element.Id);
            }
            else
            {
                elements = new Dictionary<int, WSUIElement>();
            }

            element.DestroySelf();
        }

        public static WSUIElement Create(GameObject gameObject, Vector3 position, Quaternion rotation, float time = -1f)
        {
            var element = new WSUIElement();
            var obj = Instantiate(gameObject, position, rotation);

            element.Id = NextID;
            element.GameObject = obj;
            element.LifeTime = time;

            element.onComplete = () => OnCompleteElement(element);

            elements.Add(LastID, element);
            if (time >= 0)
                activeElements.Add(LastID);

            return element;
        }

        public static WSUIElement Create(GameObject gameObject, Transform transform, float time = -1f)
        {
            return Create(gameObject, transform.position, transform.rotation, time);
        }
    }
}