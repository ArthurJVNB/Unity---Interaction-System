using UnityEngine;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	public class InteractableObject : MonoBehaviour
	{
		[SerializeField] private UnityEvent<InteractionData> _onInteract;
		[SerializeField] private InteractionData _interactionData;

		public InteractionData Data => _interactionData;
	}
}
