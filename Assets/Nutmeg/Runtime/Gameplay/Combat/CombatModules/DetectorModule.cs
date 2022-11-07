namespace Nutmeg.Runtime.Gameplay.Combat.CombatModules
{
    public abstract class DetectorModule : CombatModule
    {
        protected CombatEntity mostRecentTarget;

        
        public abstract bool TryGetTarget(out CombatEntity target);

        public CombatEntity MostRecentTarget => mostRecentTarget;
    }
}
