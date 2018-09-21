using System;
using System.Collections.Generic;
using System.Linq;

namespace devoft.Core
{

    public class StateMachine<TState>
    {

        private readonly Dictionary<TState, StateMachine.StateDescriptor<TState>> states;
        private readonly StateMachine.StateDescriptor<TState> anyState;
        private readonly Action<TState, TState>[] anyTransition;
        private readonly Action<TState, TState> onInvalidTransition;
        private readonly IEqualityComparer<TState> stateComparer;

        internal StateMachine(
            string name,
            TState initialState,
            IEqualityComparer<TState> stateComparer,
            Action<TState, TState> onInvalidTransition,
            Dictionary<TState, StateMachine.StateDescriptor<TState>> states,
            StateMachine.StateDescriptor<TState> anyState,
            Action<TState, TState>[] anyTransition)
        {
            if (states == null) throw new ArgumentNullException("states");
            this.states = states;
            this.anyState = anyState;
            this.anyTransition = anyTransition;
            this.Name = name;
            this.Current = initialState;
            this.onInvalidTransition = onInvalidTransition;
            this.TriggerEnter();
        }

        public string Name { get; private set; }
        public TState Current { get; private set; }

        public bool TransitionTo(TState newState)
        {
            StateMachine.StateDescriptor<TState> desc;
            if (!states.TryGetValue(Current, out desc))
            {
                onInvalidTransition(Current, newState);
                return false;
            }

            Action<TState, TState>[] triggers;
            if (!desc.transitions.TryGetValue(newState, out triggers))
            {
                onInvalidTransition(Current, newState);
                return false;
            }

            TriggerExit();

            for (int i = 0; i < triggers.Length; i++)
                triggers[i](Current, newState);
            for (int i = 0; i < anyTransition.Length; i++)
                anyTransition[i](Current, newState);

            Current = newState;

            TriggerEnter();

            return true;
        }

        public void TriggerUpdate()
        {
            StateMachine.StateDescriptor<TState> desc;
            if (states.TryGetValue(Current, out desc))
                for (int i = 0; i < desc.onUpdateActions.Length; i++)
                    desc.onUpdateActions[i](Current);
            for (int i = 0; i < anyState.onUpdateActions.Length; i++)
                anyState.onUpdateActions[i](Current);
        }

        private void TriggerEnter()
        {
            StateMachine.StateDescriptor<TState> desc;
            if (states.TryGetValue(Current, out desc))
                for (int i = 0; i < desc.onEnterActions.Length; i++)
                    desc.onEnterActions[i](Current);
            for (int i = 0; i < anyState.onEnterActions.Length; i++)
                anyState.onEnterActions[i](Current);
        }

        private void TriggerExit()
        {
            StateMachine.StateDescriptor<TState> desc;
            if (states.TryGetValue(Current, out desc))
                for (int i = 0; i < desc.onExitActions.Length; i++)
                    desc.onExitActions[i](Current);
            for (int i = 0; i < anyState.onExitActions.Length; i++)
                anyState.onExitActions[i](Current);
        }
    }

    public static class StateMachine
    {
        public static Builder<TState> Create<TState>(string name, TState initialState)
        {
            return new Builder<TState>().WithInitialState(initialState).WithName(name);
        }

        public class Builder<TState>
        {
            private string name;
            private TState initialState;
            private Action<TState, TState> onInvalidTransition;

            private List<Activation<TState>> activations = new List<Activation<TState>>();
            private List<Activation<TState>> activationsAny = new List<Activation<TState>>();
            private List<Transition<TState>> transitions = new List<Transition<TState>>();
            private List<Transition<TState>> transitionsAny = new List<Transition<TState>>();
            private IEqualityComparer<TState> stateComparer;

            internal Builder()
            {
            }

            public Builder<TState> WithName(string name)
            {
                this.name = name;
                return this;
            }

            public Builder<TState> WithInitialState(TState initialState)
            {
                this.initialState = initialState;
                return this;
            }

            public Builder<TState> WithStateComparer(IEqualityComparer<TState> stateComparer)
            {
                this.stateComparer = stateComparer;
                return this;
            }

            public Builder<TState> WithInvalidTransition(Action<TState, TState> onInvalidTransition)
            {
                this.onInvalidTransition = onInvalidTransition;
                return this;
            }

            public Builder<TState> OnEnterAny(Action<TState> onEnter)
            {
                if (onEnter == null) throw new ArgumentNullException("onEnter");
                activationsAny.Add(new Activation<TState>(default(TState), ActivationMode.Enter, onEnter));
                return this;
            }

            public Builder<TState> OnEnterAny(Action onEnter)
            {
                if (onEnter == null) throw new ArgumentNullException("onEnter");
                return OnEnterAny(s => onEnter());
            }

            public Builder<TState> OnEnter(TState state, Action<TState> onEnter)
            {
                if (onEnter == null) throw new ArgumentNullException("onEnter");
                activations.Add(new Activation<TState>(state, ActivationMode.Enter, onEnter));
                return this;
            }

            public Builder<TState> OnEnter(TState state, Action onEnter)
            {
                if (onEnter == null) throw new ArgumentNullException("onEnter");
                return OnEnter(state, s => onEnter());
            }

            public Builder<TState> OnExitAny(Action<TState> onExit)
            {
                if (onExit == null) throw new ArgumentNullException("onExit");
                activationsAny.Add(new Activation<TState>(default(TState), ActivationMode.Exit, onExit));
                return this;
            }

            public Builder<TState> OnExitAny(Action onExit)
            {
                if (onExit == null) throw new ArgumentNullException("onExit");
                return OnExitAny(s => onExit());
            }

            public Builder<TState> OnExit(TState state, Action<TState> onExit)
            {
                if (onExit == null) throw new ArgumentNullException("onExit");
                activations.Add(new Activation<TState>(state, ActivationMode.Exit, onExit));
                return this;
            }

            public Builder<TState> OnExit(TState state, Action onExit)
            {
                if (onExit == null) throw new ArgumentNullException("onExit");
                return OnExit(state, s => onExit());
            }

            public Builder<TState> OnUpdateAny(Action<TState> onUpdate)
            {
                if (onUpdate == null) throw new ArgumentNullException("onUpdate");
                activationsAny.Add(new Activation<TState>(default(TState), ActivationMode.Update, onUpdate));
                return this;
            }

            public Builder<TState> OnUpdateAny(Action onUpdate)
            {
                if (onUpdate == null) throw new ArgumentNullException("onUpdate");
                return OnUpdateAny(s => onUpdate());
            }

            public Builder<TState> OnUpdate(TState state, Action<TState> onUpdate)
            {
                if (onUpdate == null) throw new ArgumentNullException("onUpdate");
                activations.Add(new Activation<TState>(state, ActivationMode.Update, onUpdate));
                return this;
            }

            public Builder<TState> OnUpdate(TState state, Action onUpdate)
            {
                if (onUpdate == null) throw new ArgumentNullException("onUpdate");
                return OnUpdate(state, s => onUpdate());
            }

            public Builder<TState> OnTransitionAny(Action<TState, TState> onTrigger)
            {
                if (onTrigger == null) throw new ArgumentNullException("onTrigger");
                transitionsAny.Add(new Transition<TState>(default(TState), default(TState), onTrigger));
                return this;
            }

            public Builder<TState> OnTransitionAny(Action onTrigger)
            {
                if (onTrigger == null) throw new ArgumentNullException("onTrigger");
                return OnTransitionAny((f, t) => onTrigger());
            }

            public Builder<TState> AddTransition(TState fromState, TState toState, Action<TState, TState> onTrigger)
            {
                if (onTrigger == null) throw new ArgumentNullException("onTrigger");
                transitions.Add(new Transition<TState>(fromState, toState, onTrigger));
                return this;
            }

            public Builder<TState> AddTransition(TState fromState, TState toState, Action onTrigger)
            {
                if (onTrigger == null) throw new ArgumentNullException("onTrigger");
                return AddTransition(fromState, toState, (f, t) => onTrigger());
            }

            public Builder<TState> AddTransition(TState fromState, TState toState)
            {
                return AddTransition(fromState, toState, (f, t) => { });
            }

            public StateMachine<TState> Build()
            {
                var machineName = name;

                onInvalidTransition = onInvalidTransition ?? ((f, t) =>
                {
                    throw new NotSupportedException(String.Format("{0} transition not supported: {1} -> {2}", machineName, f, t));
                });

                stateComparer = stateComparer ?? EqualityComparer<TState>.Default;

                var enter = new Dictionary<TState, List<Action<TState>>>(stateComparer);
                var exit = new Dictionary<TState, List<Action<TState>>>(stateComparer);
                var update = new Dictionary<TState, List<Action<TState>>>(stateComparer);
                var trans = new Dictionary<TState, Dictionary<TState, List<Action<TState, TState>>>>(stateComparer);

                foreach (var item in activations)
                {
                    Dictionary<TState, List<Action<TState>>> dict = null;
                    switch (item.mode)
                    {
                        case ActivationMode.Enter:
                            dict = enter;
                            break;
                        case ActivationMode.Update:
                            dict = update;
                            break;
                        case ActivationMode.Exit:
                            dict = exit;
                            break;

                    }
                    List<Action<TState>> list;
                    if (!dict.TryGetValue(item.state, out list))
                    {
                        list = new List<Action<TState>>();
                        dict.Add(item.state, list);
                    }

                    list.Add(item.onTrigger);
                }

                foreach (var item in transitions)
                {
                    Dictionary<TState, List<Action<TState, TState>>> listDict;
                    if (!trans.TryGetValue(item.fromState, out listDict))
                    {
                        listDict = new Dictionary<TState, List<Action<TState, TState>>>(stateComparer);
                        trans.Add(item.fromState, listDict);
                    }

                    List<Action<TState, TState>> list;
                    if (!listDict.TryGetValue(item.toState, out list))
                    {
                        list = new List<Action<TState, TState>>();
                        listDict.Add(item.toState, list);
                    }

                    list.Add(item.onTrigger);
                }

                var states = new Dictionary<TState, StateDescriptor<TState>>(stateComparer);
                var keys = enter.Keys
                    .Concat(exit.Keys).Concat(update.Keys).Concat(trans.Keys)
                    .Distinct(stateComparer);

                var EmptyActions = new Action<TState>[0];

                states = keys.ToDictionary(
                    s => s,
                    s =>
                    {
                        var onEnterActions = enter.ContainsKey(s) ? enter[s].ToArray() : EmptyActions;
                        var onUpdateActions = update.ContainsKey(s) ? update[s].ToArray() : EmptyActions;
                        var onExitActions = exit.ContainsKey(s) ? exit[s].ToArray() : EmptyActions;
                        var transitions = trans.ContainsKey(s) ? trans[s].ToDictionary(p => p.Key, p => p.Value.ToArray(), stateComparer) : new Dictionary<TState, Action<TState, TState>[]>();

                        return new StateDescriptor<TState>
                        {
                            current = s,
                            onEnterActions = onEnterActions,
                            onUpdateActions = onUpdateActions,
                            onExitActions = onExitActions,
                            transitions = transitions,
                        };
                    },
                    stateComparer);

                var anyState = new StateDescriptor<TState>
                {
                    current = default(TState),
                    onEnterActions = activationsAny.Where(a => a.mode == ActivationMode.Enter).Select(a => a.onTrigger).ToArray(),
                    onUpdateActions = activationsAny.Where(a => a.mode == ActivationMode.Update).Select(a => a.onTrigger).ToArray(),
                    onExitActions = activationsAny.Where(a => a.mode == ActivationMode.Exit).Select(a => a.onTrigger).ToArray(),
                    transitions = new Dictionary<TState, Action<TState, TState>[]>(),
                };

                var anyTransition = transitionsAny.Select(t => t.onTrigger).ToArray();

                return new StateMachine<TState>(
                    machineName,
                    initialState,
                    stateComparer ?? EqualityComparer<TState>.Default,
                    onInvalidTransition,
                    states,
                    anyState,
                    anyTransition
                );
            }
        }

        internal class Transition<TState>
        {
            internal Transition(TState fromState, TState toState, Action<TState, TState> onTrigger)
            {
                this.fromState = fromState;
                this.toState = toState;
                this.onTrigger = onTrigger;
            }
            public readonly TState fromState;
            public readonly TState toState;
            public readonly Action<TState, TState> onTrigger;
        }

        internal enum ActivationMode
        {
            Enter,
            Update,
            Exit,
        }

        internal class Activation<TState>
        {
            internal Activation(TState state, ActivationMode mode, Action<TState> onTrigger)
            {
                this.state = state;
                this.mode = mode;
                this.onTrigger = onTrigger;
            }
            public readonly TState state;
            public readonly ActivationMode mode;
            public readonly Action<TState> onTrigger;
        }

        internal class StateDescriptor<TState>
        {
            public TState current;
            public Action<TState>[] onEnterActions;
            public Action<TState>[] onUpdateActions;
            public Action<TState>[] onExitActions;
            public Dictionary<TState, Action<TState, TState>[]> transitions;
        }
    }
}