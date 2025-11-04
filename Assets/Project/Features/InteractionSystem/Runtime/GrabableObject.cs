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
		[SerializeField] private bool _canInteract = true;
		[Min(0)]
		[SerializeField] private float _interactionDistance = .5f;
		[Tooltip("If true, it can be picked by another object. This will force the previous owner to \"drop\" this object.")]
		[SerializeField] private bool _canBeGrabbedEvenWithOwner;

		[field: Space]
		[field: SerializeField] public UnityEvent OnInteract { get; private set; }
		[field: SerializeField] public UnityEvent OnGrab { get; private set; }
		[field: SerializeField] public UnityEvent OnDrop { get; private set; }

		public SocketData SocketType => _socketType;

		public bool IsInteractionEnabled => _canInteract;

		public bool CanInteract(GameObject whoWantsToInteract)
		{
			if (!IsInteractionEnabled) return false;
			return Vector3.Distance(whoWantsToInteract.transform.position, transform.position) <= _interactionDistance;
		}

		public Vector3? GetInteractionPosition(GameObject whoWantsToInteract)
		{
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
			Grab(whoIsInteracting);
			return true;
		}

		public void Grab(GameObject whoIsGrabbing)
		{
			Debug.Log($"Grab {name} ({(_socketType ? _socketType.Name : "none")})");

			if (!whoIsGrabbing) return;

			if (_canBeGrabbedEvenWithOwner && _owner && _owner != whoIsGrabbing)
				DropFromOwner();

			bool shouldGrab = _owner != whoIsGrabbing;

			if (whoIsGrabbing.TryGetComponent(out SocketManager socketManager))
			{
				if (shouldGrab)
					socketManager.AssignObject(gameObject, _socketType);
				else
					socketManager.DropObject(gameObject);
			}
			else
			{
				if (shouldGrab)
					transform.SetParent(whoIsGrabbing.transform, true);
				else
					transform.SetParent(null, true);
			}

			if (shouldGrab)
			{
				_owner = whoIsGrabbing;
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

	}
}
