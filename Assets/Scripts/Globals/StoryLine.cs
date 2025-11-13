using System;

namespace Globals
{
    public static class StoryLine
    {
        public enum PossibleEndings
        {
            Arrest,
            Leave,
            Murder
        }

        public static bool IsHoodSus;
        public static bool IsCalledPolice;
        
        public static event Action GuardEntered;
        public static event Action GuardLeaved;
        public static event Action HoodPassBy;
        public static event Action HoodEntered;
        
        public static event Action<PossibleEndings> Ending;

        public static void OnGuardEntered() => GuardEntered?.Invoke();
        public static void OnGuardLeaved() => GuardLeaved?.Invoke();
        public static void OnHoodPassBy() => HoodPassBy?.Invoke();
        public static void OnHoodEntered() => HoodEntered?.Invoke();
        
        public static void OnEnding(PossibleEndings ending) => Ending?.Invoke(ending);
    }
}