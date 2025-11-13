namespace NPC
{
    public class TalkingState : INpcState
    {
        public NpcState State => NpcState.Talking;

        private readonly NpcStateMachine _stateMachine;
        private readonly NpcDialogue _dialogue;

        public TalkingState(NpcStateMachine stateMachine, NpcDialogue dialogue)
        {
            _stateMachine = stateMachine;
            _dialogue = dialogue;
        }
        
        public void OnEnter()
        {
            _dialogue.OnDialogueEnd += OnDialogueEnd;
        }

        public void OnUpdate() { }

        private void OnDialogueEnd()
        {
            _dialogue.OnDialogueEnd -= OnDialogueEnd;
            _stateMachine.ChangeState(NpcState.Idle);
        }
        
        public void OnExit() { }
    }
}