using UnityEngine;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	public interface IInteractable
	{
		public UnityEvent OnInteract { get; }
		public bool IsInteractionEnabled { get; set; }
		public bool CanInteract(GameObject whoWantsToInteract, Vector3 position, Quaternion rotation);
		public Vector3? GetInteractionPosition(Vector3 position);
		public (Vector3? position, Quaternion? rotation) GetNearestInteractionPositionAndRotation(Vector3 position, Quaternion rotation);
		public bool Interact(GameObject whoIsInteracting, Vector3 position, Quaternion rotation);
	}
}
