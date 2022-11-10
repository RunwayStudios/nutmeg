using System.Collections.Generic;

namespace Nutmeg.Runtime.Gameplay.Combat.CombatModules
{
    public abstract class DetectorModule : CombatModule
    {
        protected CombatEntity mostRecentTarget;

        
        public abstract bool TryGetTarget(out CombatEntity target);
        
        public abstract List<CombatEntity> GetTargets();
        
        public abstract int GetTargetsNonAlloc(CombatEntity[] targetBuffer);

        public CombatEntity MostRecentTarget => mostRecentTarget;
    }
}
