using UnityEngine;

namespace NPC
{
    public class FollowState : INpcState
    {
        public NpcState State => NpcState.FollowPlayer;

        private readonly NpcStateMachine _stateMachine;
        private readonly NpcMovement _movement;
        private readonly Transform _player;

        public FollowState(NpcStateMachine stateMachine, NpcMovement movement, Transform player)
        {
            _stateMachine = stateMachine;
            _movement = movement;
            _player = player;
        }
        
        public void OnEnter()
        {
            _movement.Resume();
        }

        public void OnUpdate()
        {
            _movement.FollowTarget(_player);
        }

        public void OnExit()
        {
            _movement.Stop();
        }
    }
}