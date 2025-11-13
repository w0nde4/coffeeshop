using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    public enum NpcState
    {
        Idle,
        Entering,
        Exiting,
        Talking,
        FollowPlayer
    }

    public class NpcStateMachine : MonoBehaviour
    {
        private readonly Dictionary<NpcState, INpcState> _states = new();
        private INpcState _currentState;
    
        public NpcState CurrentState => _currentState?.State ?? NpcState.Idle;

        public void RegisterState(INpcState state)
        {
            _states.TryAdd(state.State, state);
        }

        public void ChangeState(NpcState newState)
        {
            if(_currentState != null && _currentState.State == newState)
                return;
        
            _currentState?.OnExit();
            _currentState = _states[newState];
            _currentState?.OnEnter();
        }

        public void Update()
        {
            _currentState?.OnUpdate();
        }
    }
}