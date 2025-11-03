using System;
using UnityEngine;
using UnityHFSM;

namespace Project.InteractionSystem
{
	[Obsolete("Please use InteractionStateMachine")]
	[Serializable]
	public class InteractionStateMachineObsolete : StateMachine
	{
		private const string StateNoneName = "None";
		private const string StateStartName = "Start";
		private const string StateLoopName = "Loop";
		private const string StateEndName = "End";

		public event Action<InteractionStateObsolete> OnStateEntered;

		public InteractionData data;
		
		private InteractionStateObsolete _emptyState;
		private InteractionStateObsolete _stateStart;
		private InteractionStateObsolete _stateLoop;
		private InteractionStateObsolete _stateEnd;
		private InteractionStateObsolete _currentState;

		public InteractionStateObsolete CurrentState => _currentState;
		public InteractionStateType CurrentStateType => _currentState?.type ?? InteractionStateType.None;

		public void Setup(InteractionData data)
		{
			this.data = data;
			_emptyState = new(StateNoneName, InteractionStateType.None);
			_stateStart = new(StateStartName, InteractionStateType.Start, data.Start);
			_stateLoop = new(StateLoopName, InteractionStateType.Loop, data.Loop);
			_stateEnd = new(StateEndName, InteractionStateType.Loop, data.End);

			AddState(_emptyState.name, _emptyState);
			AddState(_stateStart.name, _stateStart);
			AddState(_stateLoop.name, _stateLoop);
			AddState(_stateEnd.name, _stateEnd);

			SetStartState(_emptyState.name);
			SetTransitions();

			this.StateChanged -= OnStateChanged;
			this.StateChanged += OnStateChanged;
			Init();
		}

		public void GoNextState()
		{
			switch (ActiveStateName)
			{
				case StateNoneName:
					RequestStateChange(_stateStart.name);
					break;
				case StateStartName:
					RequestStateChange(_stateLoop.name);
					break;
				case StateLoopName:
					RequestStateChange(_stateEnd.name);
					break;
				case StateEndName:
					RequestStateChange(_stateStart.name);
					break;
				default:
					Debug.LogWarning($"State not recognized ({ActiveState.name})");
					break;
			}
		}

		public override void OnLogic()
		{
			base.OnLogic();
		}

		private void OnStateChanged(StateBase<string> @base)
		{
			var state = @base as InteractionStateObsolete;
			Debug.Log($"<color=white>changed state to {state.name}</color>");
			_currentState = state;
			OnStateEntered?.Invoke(state);
		}

		private void SetTransitions()
		{
			this.AddTransition(_stateStart.name, _stateLoop.name, condition: condition => _stateStart.wantsToExit);
			this.AddTransition(_stateLoop.name, _stateEnd.name, condition: condition => _stateLoop.wantsToExit);
			this.AddTransition(_stateEnd.name, _emptyState.name, condition: condition => _stateEnd.wantsToExit);
		}
	}
}
