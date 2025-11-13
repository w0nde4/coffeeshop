using Globals;
using UnityEngine;

namespace ChoiceEffect
{
    [CreateAssetMenu(fileName = "New SetVariableEffect", menuName = "Dialogue/Effects/Set Variable Effect")]
    public class SetVariableEffect : ChoiceEffect
    {
        public enum Variable
        {
            IsHoodSus,
            IsCalledPolice
        }

        [SerializeField] private Variable variable;
        [SerializeField] private bool value;
        
        public override void ApplyEffect()
        {
            switch (variable)
            {
                case Variable.IsHoodSus: StoryLine.IsHoodSus = value; break;
                case Variable.IsCalledPolice: StoryLine.IsCalledPolice = value; break;
            }
        }
    }
}