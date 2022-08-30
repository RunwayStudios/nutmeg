namespace Nutmeg.Runtime.Gameplay.Combat.CombatModules
{
    public abstract class DetectorModule : CombatModule
    {
        public abstract bool TryGetTarget(out CombatEntity target);
    }
}
