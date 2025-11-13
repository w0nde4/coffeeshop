using UnityEditor.Animations;
using UnityEngine;

namespace NPC
{
    public class IdleState : INpcState
    {
        private readonly NpcStateMachine _stateMachine;
        private readonly NpcMovement _movement;
        private readonly Transform _player;
        public NpcState State => NpcState.Idle;

        public IdleState(NpcStateMachine stateMachine, NpcMovement movement, Transform player)
        {
            _movement = movement;
            _stateMachine = stateMachine;
            _player = player;
        }
    
        public void OnEnter()
        {
            _movement.Stop();
        }

        public void OnUpdate()
        {
            _movement.LookAtTarget(_player);
        }

        public void OnExit() { }
    }
}