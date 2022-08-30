using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Combat
{
    public class CombatEntityManager : MonoBehaviour
    {
        public static CombatEntityManager Main;

        public Dictionary<CombatGroup, List<TransformCombatEntityStruct>> activeGroups = new Dictionary<CombatGroup, List<TransformCombatEntityStruct>>();


        private void Awake()
        {
            Main = this;
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }


        public void AddCombatEntity(CombatEntity entity, CombatGroup group)
        {
            if (activeGroups.TryGetValue(group, out List<TransformCombatEntityStruct> entities))
            {
                entities.Add(new TransformCombatEntityStruct(entity.transform, entity));
            }
            else
            {
                activeGroups.Add(group, new List<TransformCombatEntityStruct> { new TransformCombatEntityStruct(entity.transform, entity) });
            }
        }
        
        public void RemoveCombatEntity(CombatEntity entity, CombatGroup group)
        {
            if (activeGroups.TryGetValue(group, out List<TransformCombatEntityStruct> entities))
            {
                for (int i = entities.Count - 1; i >= 0; i--)
                {
                    if (entities[i].Entity == entity)
                    {
                        entities.RemoveAt(i);
                        return;
                    }
                }
            }
        }
        
        
        
        public struct TransformCombatEntityStruct
        {
            Transform transform;
            CombatEntity entity;

            public TransformCombatEntityStruct(Transform transform, CombatEntity entity)
            {
                this.transform = transform;
                this.entity = entity;
            }

            public Transform Transform => transform;

            public CombatEntity Entity => entity;
        }
    }
}
