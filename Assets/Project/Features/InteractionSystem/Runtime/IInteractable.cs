using UnityEngine;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	public interface IInteractable
	{
		public UnityEvent OnInteract { get; }
		public bool IsInteractionEnabled { get; set; }
		public bool CanInteract(GameObject whoWantsToInteract);
		public Vector3? GetInteractionPosition(GameObject whoWantsToInteract);
		public (Vector3? position, Quaternion? rotation) GetNearestInteractionPositionAndRotation(Transform reference);
		public bool Interact(GameObject whoIsInteracting);
	}
}
