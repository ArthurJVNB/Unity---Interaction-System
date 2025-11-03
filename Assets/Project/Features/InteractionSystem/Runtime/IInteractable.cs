using UnityEngine;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	public interface IInteractable
	{
		public UnityEvent OnInteract { get; }
		public bool IsInteractionEnabled { get; }
		public bool CanInteract(GameObject whoWantsToInteract);
		public Vector3? GetInteractionPosition(GameObject whoWantsToInteract);
		public bool Interact(GameObject whoIsInteracting);
	}
}
