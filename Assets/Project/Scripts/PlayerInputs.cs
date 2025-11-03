using UnityEngine;
using UnityEngine.InputSystem;

namespace Project
{
	public class PlayerInputs : MonoBehaviour
	{
		public bool interact;

		public void OnInteract(InputValue value)
		{
			InteractInput(value.isPressed);
		}

		public void InteractInput(bool newInteractState)
		{
			interact = newInteractState;
		}
	}
}
