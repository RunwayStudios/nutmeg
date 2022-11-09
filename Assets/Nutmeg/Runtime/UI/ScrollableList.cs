using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nutmeg.Runtime.UI
{
    public class ScrollableList : MonoBehaviour
    {
        [SerializeField] private GameObject listItemPrefab;
        [SerializeField] private float itemPadding;
        [SerializeField] private RectTransform listContent;

        private List<GameObject> items = new List<GameObject>();
        private float prefabRectHeight;


        private void Awake()
        {
            prefabRectHeight = listItemPrefab.GetComponent<RectTransform>().rect.height;
        }

        public GameObject AddElement()
        {
            listContent.sizeDelta = new Vector2(0, items.Count * (prefabRectHeight + itemPadding));
            GameObject go = Instantiate(listItemPrefab, listContent);
            items.Add(go);
            return go;
        }

        public void RemoveElement(GameObject go)
        {
            items.Remove(go);
            Destroy(go);
        }

        public void ClearList()
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                RemoveElement(items[i]);
            }
        }


        public List<GameObject> Items => items;
    }
}