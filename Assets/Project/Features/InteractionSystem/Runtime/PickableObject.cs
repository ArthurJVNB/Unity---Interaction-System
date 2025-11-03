using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	public class PickableObject : MonoBehaviour, IInteractable, IPickable
	{
		[SerializeField] private SlotType _slotType;
		[SerializeField] private PickState _state;
		[SerializeField] private bool _canInteract = true;
		[Min(0)]
		[SerializeField] private float _interactionDistance = .5f;

		[field: Space]
		[field: SerializeField] public UnityEvent OnInteract { get; private set; }
		[field: SerializeField] public UnityEvent OnPick { get; private set; }
		[field: SerializeField] public UnityEvent OnDrop { get; private set; }

		private enum PickState { None, Picked }

		public SlotType PickType => _slotType;

		public bool IsInteractionEnabled => _canInteract;

		public bool CanInteract(GameObject whoWantsToInteract)
		{
			if (!IsInteractionEnabled) return false;
			return Vector3.Distance(whoWantsToInteract.transform.position, transform.position) <= _interactionDistance;
		}

		public Vector3? GetInteractionPosition(GameObject whoWantsToInteract)
		{
			//var agent = whoWantsToInteract.GetComponent<NavMeshAgent>();
			//bool hasPath = agent.SamplePathPosition(0, _interactionDistance, out NavMeshHit hit);
			//bool hasPath = NavMesh.FindClosestEdge(transform.position, out NavMeshHit hit, 0);
			//return hasPath ? hit.position : null;

			if (!IsInteractionEnabled) return null;
			return transform.position;
		}

		public bool Interact(GameObject whoIsInteracting)
		{
			if (!CanInteract(whoIsInteracting))
			{
				Debug.Log($"'{whoIsInteracting.name}' cannot interact '{name}'!", this);
				return false;
			}

			Debug.Log($"interact '{name}'");
			OnInteract?.Invoke();
			Pick(whoIsInteracting);
			return true;
		}

		public void Pick(GameObject whoIsPicking)
		{
			Debug.Log($"pick {name} ({_slotType})");
			SwitchState();

			if (!whoIsPicking) return;

			if (whoIsPicking.TryGetComponent(out PickSlotManager pickSlotManager))
			{
				switch (_state)
				{
					case PickState.None:
						OnDrop?.Invoke();
						pickSlotManager.DropObject(gameObject);
						break;
					case PickState.Picked:
						OnPick?.Invoke();
						pickSlotManager.AssignObject(gameObject, _slotType);
						break;
					default:
						break;
				}
			}
			else
			{
				switch (_state)
				{
					case PickState.None:
						OnDrop?.Invoke();
						transform.SetParent(null, true);
						break;
					case PickState.Picked:
						OnPick?.Invoke();
						transform.SetParent(whoIsPicking.transform, true);
						break;
					default:
						break;
				}
			}
		}

		private void SwitchState()
		{
			switch (_state)
			{
				case PickState.None:
					_state = PickState.Picked;
					break;
				case PickState.Picked:
					_state = PickState.None;
					break;
				default:
					Debug.LogError("Current pick state is invalid!", this);
					break;
			}
		}

	}
}
