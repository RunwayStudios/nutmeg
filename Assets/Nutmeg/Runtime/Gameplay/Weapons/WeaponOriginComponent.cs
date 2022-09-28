using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Weapons
{
    [DisallowMultipleComponent]
    public class WeaponOriginComponent : WeaponComponent
    {
        public OriginType originType;
        [HideInInspector] public Transform singleOrigin;
        [HideInInspector] public Transform[] multipleOrigins;
        [HideInInspector] public MultipleOriginType multipleOriginsType;

        private int originIndex;

        public override bool Get(out object data)
        {
            data = originType switch
            {
                OriginType.Single => singleOrigin,
                OriginType.Multiple => GetNextOrigin(),
                _ => transform.position
            };

            return true;
        }

        public void ResetOriginIndex() => originIndex = 0;
 
        private Transform GetNextOrigin()
        {
            var nextOrigin = multipleOriginsType switch
            {
                MultipleOriginType.Linear => GetNextLinearOrigin()
            };
            
            return nextOrigin;
        }

        private Transform GetNextLinearOrigin()
        {
            if(originIndex++ >= multipleOrigins.Length)
                ResetOriginIndex();
            
            return multipleOrigins[originIndex];
        }   
    }
    
    public enum OriginType {
        Single,
        Multiple
    }
    
    public enum MultipleOriginType {
        Linear,
        LinearReversed,
        InToOut,
        InToOutReversed,
        InToOutStacked,
        OutToIn,
        OutToInReversed,
        OutToInStacked,
        Random
    }
}