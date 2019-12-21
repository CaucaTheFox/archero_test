using System;
using System.Collections.Generic;

namespace TheGame.Features.Caring.Model.States
{
    public interface ISimpleState
    {
        void Enter();
        void Exit();
    }

    public interface ISimpleStateMachine<StateIndex>
    {
        void ToState(StateIndex stateIndex);
    }

    public interface ISimpleStateFactory
    {
        ISimpleState Create();
    }
    
    public class SimpleStateMachine<StateIndex>: ISimpleStateMachine<StateIndex>
    {
        private Dictionary<StateIndex, ISimpleStateFactory> stateFactories;
        private ISimpleState currentState;


        public void ToState(StateIndex stateIndex)
        {
            if (!stateFactories.TryGetValue(stateIndex, out var stateFactory)) {
                throw new Exception($"[SimpleStateMachine] State '{stateIndex}' does not exist");
            }

            var state = stateFactory.Create();

            currentState?.Exit();
            currentState = state;
            state.Enter();
        }
    }
}