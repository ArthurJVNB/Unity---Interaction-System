using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	public class GrabableObject : MonoBehaviour, IInteractable, IGrabable
	{
		[SerializeField] private SocketData _socketType;
		[SerializeField] private GameObject _owner;
		[SerializeField] private PickStateObsolete _state; // DELETAR!
		[SerializeField] private bool _canInteract = true;
		[Min(0)]
		[SerializeField] private float _interactionDistance = .5f;
		[Tooltip("If true, it can be picked by another object. This will force the previous owner to \"drop\" this object.")]
		[SerializeField] private bool _canBeGrabbedEvenWithOwner;

		[field: Space]
		[field: SerializeField] public UnityEvent OnInteract { get; private set; }
		[field: SerializeField] public UnityEvent OnGrab { get; private set; }
		[field: SerializeField] public UnityEvent OnDrop { get; private set; }

		private enum PickStateObsolete { None, Picked }

		public SocketData SocketType => _socketType;

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
			Debug.Log($"pick {name} ({(_socketType ? _socketType.Name : "none")})");
			#region Backup
			//SwitchState();

			//if (!whoIsPicking) return;

			//if (whoIsPicking.TryGetComponent(out SocketManager socketManager))
			//{
			//	switch (_state)
			//	{
			//		case PickStateObsolete.None:
			//			OnDrop?.Invoke();
			//			//pickSlotManager.DropObjectObsolete(gameObject);
			//			socketManager.DropObject(gameObject);
			//			break;
			//		case PickStateObsolete.Picked:
			//			OnPick?.Invoke();
			//			//pickSlotManager.AssignObject(gameObject, _slotType);
			//			socketManager.AssignObject(gameObject, _socketType);
			//			break;
			//		default:
			//			break;
			//	}
			//}
			//else
			//{
			//	switch (_state)
			//	{
			//		case PickStateObsolete.None:
			//			OnDrop?.Invoke();
			//			transform.SetParent(null, true);
			//			break;
			//		case PickStateObsolete.Picked:
			//			OnPick?.Invoke();
			//			transform.SetParent(whoIsPicking.transform, true);
			//			break;
			//		default:
			//			break;
			//	}
			//}
			#endregion

			if (!whoIsPicking) return;

			if (_canBeGrabbedEvenWithOwner && _owner && _owner != whoIsPicking)
				DropFromOwner();

			bool isGrabbing = _owner != whoIsPicking;

			if (whoIsPicking.TryGetComponent(out SocketManager socketManager))
			{
				if (isGrabbing)
					socketManager.AssignObject(gameObject, _socketType);
				else
					socketManager.DropObject(gameObject);
			}
			else
			{
				if (isGrabbing)
					transform.SetParent(whoIsPicking.transform, true);
				else
					transform.SetParent(null, true);
			}

			if (isGrabbing)
			{
				_owner = whoIsPicking;
				OnGrab?.Invoke();
			}
			else
			{
				_owner = null;
				OnDrop?.Invoke();
			}
		}

		private void DropFromOwner()
		{
			if (!_owner) return;
			if (_owner.TryGetComponent(out SocketManager socketManager))
			{
				socketManager.DropObject(gameObject);
				return;
			}
			transform.SetParent(null, true);
		}

		private void SwitchState()
		{
			switch (_state)
			{
				case PickStateObsolete.None:
					_state = PickStateObsolete.Picked;
					break;
				case PickStateObsolete.Picked:
					_state = PickStateObsolete.None;
					break;
				default:
					Debug.LogError("Current pick state is invalid!", this);
					break;
			}
		}

	}
}
