using UnityEngine;

namespace NPC
{
    public class EnteringState : INpcState
    {
        public NpcState State => NpcState.Entering;

        private readonly NpcStateMachine _stateMachine;
        private readonly NpcMovement _movement;

        public EnteringState(NpcStateMachine stateMachine, NpcMovement movement)
        {
            _stateMachine = stateMachine;
            _movement = movement;
        }
        
        public void OnEnter()
        {
            _movement.OnRouteComplete += OnRouteComplete;
            _movement.EnterCafe();
        }

        private void OnRouteComplete(RouteType route)
        {
            if (route != RouteType.Entering) return;
            
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