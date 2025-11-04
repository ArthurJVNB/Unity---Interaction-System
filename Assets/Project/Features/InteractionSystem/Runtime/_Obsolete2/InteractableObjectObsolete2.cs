using System;
using UnityEngine;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	[Obsolete]
	public class InteractableObjectObsolete2 : MonoBehaviour
	{
		public event Action<InteractionDataObsolete> OnInteract;
		public event Action<InteractionDataObsolete.Settings> OnInteractType;

		[SerializeField] private UnityEvent<InteractionDataObsolete> _onInteract;
		[SerializeField] private UnityEvent<InteractionDataObsolete.Settings> _onInteractType;
		[SerializeField] private InteractionDataObsolete _interactionData;
		[Min(0)]
		[SerializeField] private float _interactionDistance = .5f;

		public InteractionDataObsolete Data => _interactionData;
		public float InteractionDistance => _interactionDistance;

		public void Interact(InteractionStateType type)
		{
			_onInteract?.Invoke(_interactionData);
			OnInteract?.Invoke(_interactionData);

			switch (type)
			{
				case InteractionStateType.None:
					break;
				case InteractionStateType.Start:
					_onInteractType?.Invoke(_interactionData.Start);
					OnInteractType?.Invoke(_interactionData.Start);
					break;
				case InteractionStateType.Loop:
					_onInteractType?.Invoke(_interactionData.Loop);
					OnInteractType?.Invoke(_interactionData.Loop);
					break;
				case InteractionStateType.End:
					_onInteractType?.Invoke(_interactionData.End);
					OnInteractType?.Invoke(_interactionData.End);
					break;
				default:
					break;
			}
		}

		private void OnDrawGizmosSelected()
		{
			if (_interactionDistance <= 0) return;
			Gizmos.DrawWireSphere(transform.position, _interactionDistance);
		}
	}
}
