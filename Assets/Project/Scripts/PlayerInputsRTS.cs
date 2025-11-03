using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project
{
	public class PlayerInputsRTS : MonoBehaviour
	{
		public event Action<Vector2> OnMoveRTSPerformed;
		public event Action OnInteractPerformed;

		public Vector2 mousePosition;
		public bool interact;

		public void OnMoveRTS(InputValue value)
		{
			var position = value.Get<Vector2>();
			if (position == Vector2.zero) return;
			mousePosition = position;
			//Debug.Log($"MoveRTS {mousePosition}");
			OnMoveRTSPerformed?.Invoke(mousePosition);
		}

		public void OnInteract(InputValue value)
		{
			//Debug.Log($"interact {value.isPressed}");
			interact = value.isPressed;
			OnInteractPerformed?.Invoke();
		}
	}
}
