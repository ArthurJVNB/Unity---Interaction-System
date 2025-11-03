using System;
using Project.InteractionSystem;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Project
{
	public class PlayerControllerRTS : MonoBehaviour
	{
		[SerializeField] private Camera _camera;
		[SerializeField] private NavMeshAgent _agent;
		[SerializeField] private LayerMask _layerMask = 1 << 0; // Default
		[SerializeField] private InputActionReference _inputMove;
		[SerializeField] private InputActionReference _inputInteract;

		private RaycastHit _hit;

		private void Awake()
		{
			if (!_camera)
				_camera = Camera.main;
		}

		private void OnEnable()
		{
			_inputMove.action.performed += InputMove_performed;
			_inputInteract.action.performed += InputInteract_performed;
		}

		private void OnDisable()
		{
			_inputMove.action.performed -= InputMove_performed;
			_inputInteract.action.performed -= InputInteract_performed;
		}

		private void InputInteract_performed(InputAction.CallbackContext context)
		{
			Debug.Log("interaction performed");
		}

		private void InputMove_performed(InputAction.CallbackContext context)
		{
			var value = context.ReadValue<Vector2>();
			Debug.Log($"move {value}");
			Move(value);
		}

		private void Move(Vector2 screenPoint)
		{
			Move(_camera.ScreenPointToRay(screenPoint));
		}

		private void Move(Ray ray)
		{
			if (!Physics.Raycast(ray, out _hit, float.MaxValue, _layerMask)) return;

			if (_hit.transform.TryGetComponent(out IInteractable interactable))
			{
				//Debug.Log("Going to interact");
				interactable.Interact(gameObject);
				return;
			}
			
			_agent.SetDestination(_hit.point);
		}

	}
}
