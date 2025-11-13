using UnityEngine;

namespace NPC
{
    public class ExitingState : INpcState
    {
        public NpcState State => NpcState.Exiting;
        
        private readonly NpcStateMachine _stateMachine;
        private readonly NpcMovement _movement;
        
        public ExitingState(NpcStateMachine stateMachine, NpcMovement movement)
        {
            _stateMachine = stateMachine;
            _movement = movement;
        }
        
        public void OnEnter()
        {
            _movement.OnRouteComplete += OnRouteComplete;
            _movement.ExitCafe();
        }

        private void OnRouteComplete(RouteType route)
        {
            if (route != RouteType.Exiting) return;
            
            _movement.OnRouteComplete -= OnRouteComplete;
            _stateMachine.ChangeState(NpcState.Idle);
        }

        public void OnUpdate() { }

        public void OnExit()
        {
            _movement.OnRouteComplete -= OnRouteComplete;
        }
    }
}