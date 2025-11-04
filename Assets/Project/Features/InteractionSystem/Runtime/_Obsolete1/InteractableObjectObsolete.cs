using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityHFSM;

namespace Project.InteractionSystem
{
	[Obsolete("Please use InteractableObject")]
	public class InteractableObjectObsolete : MonoBehaviour
	{
		public event Action<InteractionStateObsolete> OnStateEntered;

		[SerializeField] private UnityEvent<InteractionDataObsolete> _onInteract;
		[SerializeField] private InteractionDataObsolete _interactionData;
		[SerializeField] private float _interactionDistance;

		private Coroutine _routine;
		private InteractionStateMachineObsolete _fsm;

		public float InteractionDistance => _interactionDistance;
		public InteractionStateType CurrentStateType => _fsm?.CurrentStateType ?? InteractionStateType.None;

		public void Interact()
		{
			Debug.Log($"Interacted {name}", this);
			InteractionLogic();
			_onInteract?.Invoke(_interactionData);
		}

		public Vector3 GetInteractionPosition(Vector3 otherPosition)
		{
			var delta = transform.position - otherPosition;
			var targetMagnitude = (float)delta.magnitude - _interactionDistance;
			var target = Vector3.ClampMagnitude(delta, targetMagnitude);
			Debug.DrawLine(otherPosition, otherPosition + delta, Color.green, 2);
			return otherPosition + target;
		}

		private void InteractionLogic()
		{
			if (!_interactionData) return;

			InitFSM();
			_fsm.GoNextState();

			if (_routine != null) return;
			_routine = StartCoroutine(InteractionLogicRoutine());
		}

		private IEnumerator InteractionLogicRoutine()
		{
			bool shouldContinue = true;
			while (shouldContinue)
			{
				_fsm.OnLogic();
				yield return null;
			}
			_routine = null;
		}

		private void InitFSM()
		{
			if (_fsm != null && _fsm.data) return;
			_fsm = new();
			_fsm.Setup(_interactionData);
			_fsm.OnStateEntered -= Fsm_OnStateEntered;
			_fsm.OnStateEntered += Fsm_OnStateEntered;
		}

		private void Fsm_OnStateEntered(InteractionStateObsolete state)
		{
			OnStateEntered?.Invoke(state);
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawWireSphere(transform.position, _interactionDistance);
		}
	}
}
