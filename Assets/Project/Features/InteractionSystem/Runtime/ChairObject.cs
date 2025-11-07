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
		public bool CanInteract(GameObject whoWantsToInteract, Vector3 position, Quaternion rotation)
		{
			return IsInteractionEnabled
				&& _interactionPoint.CanInteract(position, rotation);
				//&& Vector3.Distance(GetNearestInteractionPositionAndRotation(whoWantsToInteract.transform).position.Value, whoWantsToInteract.transform.position) < 1;
		}

		public Vector3? GetNearestInteractionPosition(Vector3 position)
		{
			return transform.position;
		}

		public (Vector3? position, Quaternion? rotation) GetNearestInteractionPositionAndRotation(Vector3 position, Quaternion rotation)
		{
			if (!IsInteractionEnabled) return (null, null);
			var posAndRot = _interactionPoint;
			return (posAndRot.Position, posAndRot.Rotation);
		}

		public bool Interact(GameObject whoIsInteracting, Vector3 position, Quaternion rotation)
		{
			if (!CanInteract(whoIsInteracting, position, rotation))
				return false;

			Debug.Log($"'{name}' interact");
			return true;
		}
		#endregion
	}
}
