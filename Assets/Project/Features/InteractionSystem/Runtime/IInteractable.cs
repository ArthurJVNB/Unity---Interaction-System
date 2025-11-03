using UnityEngine;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	public interface IInteractable
	{
		public UnityEvent OnInteract { get; }
		public void Interact(GameObject whoIsInteracting);
	}
}
