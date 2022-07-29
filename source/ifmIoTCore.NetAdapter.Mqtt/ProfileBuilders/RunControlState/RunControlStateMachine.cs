namespace ifmIoTCore.NetAdapter.Mqtt.ProfileBuilders.RunControlState
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using Logger;
    using Utilities;

    internal sealed class RunControlStateMachine : NotifyPropertyChangedBase
    {
        private readonly ILogger _log;
        private readonly List<StateTransition> _stateTransitions  = new List<StateTransition>();
        private readonly EqualityComparerState _stateComparer = new EqualityComparerState();
        private readonly object _syncRoot = new object();
        private readonly Guid _stateMachineGuid;
        private State _currentState;

        public RunControlStateMachine(ILogger logger) : this(Guid.NewGuid())
        {
            this._log = logger;
        }

        public RunControlStateMachine(Guid guid)
        {
            this._stateMachineGuid = guid;
        }

        internal State CurrentState
        {
            get => this._currentState;

            private set
            {
                if (this._stateComparer.Equals(this._currentState, value)) return;
                this._currentState = value;
                this.RaisePropertyChanged(nameof(this.CurrentState));
            }
        }

        internal bool IsCurrentState(State state)
        {
            return this._stateComparer.Equals(this._currentState, state);
        }

        internal void RegisterStateTransition(StateTransition stateTransition)
        {
            lock(this._syncRoot)
            {
                if (this._stateTransitions.Any(x => x.From == stateTransition.From && x.To == stateTransition.To))
                {
                    this._log.Error("This statetransition is already registered.");
                    throw new ArgumentException("This statetransition is already registered.");
                }

                this._stateTransitions.Add(stateTransition);
            }
        }

        internal bool TrySetState(State nextState, string action = null)
        {
            lock (this._syncRoot)
            {
                try
                {
                    this.SetState(nextState, action);
                }
                catch
                {
                    return false;
                }

                return true;
            }
        }

        internal void SetState(State nextState, string action = null)
        {
            lock(this._syncRoot) {

                if (this._currentState == null)
                {
                    this.CurrentState = nextState;
                }
                else
                {
                    var transition = this._stateTransitions.SingleOrDefault(x => x.From == this._currentState && x.To == nextState);
                    if (transition == null)
                    {
                        this._log.Error($"No transition from: {this._currentState} to: {nextState} in statemachine {this._stateMachineGuid} registered.");
                        throw new InvalidOperationException($"No transition from: {this._currentState} to: {nextState} in statemachine {this._stateMachineGuid} registered.");
                    }
                    else
                    {
                        var failedConditions = (from condition in transition.Conditions
                            where !condition.Condition()
                            select condition).ToArray();

                        if (failedConditions.Any())
                        {
                            foreach (var failedCondition in failedConditions)
                            {
                                this._log.Error($"Failed condition on action: '{action ?? "unnamed action"}' for statetransition from: {this._currentState} to: {nextState}. The condition was: {failedCondition.Description}.");
                            }
                            
                            var aggregatedExceptions = from exception in failedConditions
                                    select new Exception($"Failed condition {exception.Description}");
                            
                            throw new AggregateException(aggregatedExceptions);
                        }
                        else
                        {
                            this._log.Debug($"State transition in statemachine: {this._stateMachineGuid} from: {this.CurrentState}, to: {nextState}");
                            this.CurrentState = nextState;
                        }
                    }
                }
            }
        }
    }
}
