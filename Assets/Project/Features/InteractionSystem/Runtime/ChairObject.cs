using UnityEngine;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	public class ChairObject : MonoBehaviour, IInteractable
	{
		[SerializeField] private InteractionPoint _interactionPoint;
		[SerializeField] private bool _canInteract = true;
		[field: SerializeField] public UnityEvent OnInteract { get; }

		public bool IsInteractionEnabled { get => _canInteract; set => _canInteract = value; }

		private void Reset()
		{
			_interactionPoint = GetComponentInChildren<InteractionPoint>();
		}

		#region IInteractable methods
		public bool CanInteract(GameObject whoWantsToInteract)
		{
			return IsInteractionEnabled
				&& _interactionPoint.CanInteract(whoWantsToInteract.transform.position, whoWantsToInteract.transform.rotation);
				//&& Vector3.Distance(GetNearestInteractionPositionAndRotation(whoWantsToInteract.transform).position.Value, whoWantsToInteract.transform.position) < 1;
		}

		public Vector3? GetInteractionPosition(GameObject whoWantsToInteract)
		{
			return transform.position;
		}

		public (Vector3? position, Quaternion? rotation) GetNearestInteractionPositionAndRotation(Transform reference)
		{
			if (!IsInteractionEnabled) return (null, null);
			var posAndRot = _interactionPoint;
			return (posAndRot.Position, posAndRot.Rotation);
		}

		public bool Interact(GameObject whoIsInteracting)
		{
			if (!CanInteract(whoIsInteracting))
				return false;

			Debug.Log($"'{name}' interact");
			return true;
		}
		#endregion
	}
}
