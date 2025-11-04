using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.InteractionSystem
{
	[Obsolete("Please use Interactor.")]
	[RequireComponent(typeof(Collider))]
	public class InteractorObsolete : MonoBehaviour
	{
		public event Action<EventHandler> OnHasInteractablesChanged;

		[SerializeField] private List<InteractableObjectObsolete> _interactables;

		public class EventHandler : EventArgs
		{
			public InteractorObsolete interactor;
			public InteractableObjectObsolete newestInteractable;

			public EventHandler(InteractorObsolete interactor, InteractableObjectObsolete newestInteractable)
			{
				this.interactor = interactor;
				this.newestInteractable = newestInteractable;
			}
		}

		public bool HasInteractables => _interactables != null && _interactables.Count != 0;

		private void OnTriggerEnter(Collider other)
		{
			Init();
			if (other.TryGetComponent<InteractableObjectObsolete>(out var interactable))
			{
				_interactables.Add(interactable);
				Invoke_OnHasInteractablesChanged(interactable);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.TryGetComponent<InteractableObjectObsolete>(out var interactable))
			{
				_interactables.Remove(interactable);
				Invoke_OnHasInteractablesChanged(interactable);
			}
		}

		private void Init()
		{
			_interactables ??= new();
		}

		private void Invoke_OnHasInteractablesChanged(InteractableObjectObsolete interactable)
		{
			OnHasInteractablesChanged?.Invoke(new EventHandler(this, interactable));
		}

		public InteractableObjectObsolete GetInteractable()
		{
			return GetInteractable(null);
		}

		public InteractableObjectObsolete GetInteractable(Transform lookAt)
		{
			if (!HasInteractables) return null;
			
			if (!lookAt)
				lookAt = transform;

			float distance = float.MaxValue;
			InteractableObjectObsolete chosen = null;
			foreach (var interactable in _interactables)
			{
				if (!chosen)
				{
					chosen = interactable;
					distance = Vector3.Distance(lookAt.position, chosen.transform.position);
					continue;
				}

				float curDistance = Vector3.Distance(lookAt.position, interactable.transform.position);
				if (curDistance < distance)
				{
					distance = curDistance;
					chosen = interactable;
				}
			}
			return chosen;
		}
	}
}
