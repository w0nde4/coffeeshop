using Globals;
using UnityEngine;

namespace ChoiceEffect
{
    [CreateAssetMenu(fileName = "New Trigger Event Effect", menuName = "Dialogue/Effects/Trigger Story Event")]
    public class TriggerStoryEventEffect : ChoiceEffect
    {
        private enum StoryEvent
        {
            GuardEntered,
            GuardLeaved,
            HoodPassBy,
            HoodEntered,
            EndingArrest,
            EndingLeave,
            EndingMurder
        }

        [SerializeField] private StoryEvent storyEvent;
        
        public override void ApplyEffect()
        {
            switch (storyEvent)
            {
                case StoryEvent.GuardEntered: StoryLine.OnGuardEntered(); break;
                case StoryEvent.GuardLeaved: StoryLine.OnGuardLeaved(); break;
                case StoryEvent.HoodPassBy: StoryLine.OnHoodPassBy(); break;
                case StoryEvent.HoodEntered: StoryLine.OnHoodEntered(); break;
                case StoryEvent.EndingArrest: StoryLine.OnEnding(StoryLine.PossibleEndings.Arrest); break;
                case StoryEvent.EndingLeave: StoryLine.OnEnding(StoryLine.PossibleEndings.Leave); break;
                case StoryEvent.EndingMurder: StoryLine.OnEnding(StoryLine.PossibleEndings.Murder); break;
            }
        }
    }
}