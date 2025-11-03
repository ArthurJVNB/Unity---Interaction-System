using System;
using System.Collections;
using Project.InteractionSystem;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
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
		[Space]
		[SerializeField] private UnityEvent _onInteractionSuccess;
		[SerializeField] private UnityEvent _onInteractionFailure;

		private RaycastHit _hit;
		private Coroutine _interactWhenPossibleRoutine;

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
			var value = context.ReadValue<Vector2>();
			Interact(value);
		}

		private void InputMove_performed(InputAction.CallbackContext context)
		{
			var value = context.ReadValue<Vector2>();
			Move(value);
		}

		private void Interact(Vector2 screenPoint)
		{
			Interact(_camera.ScreenPointToRay(screenPoint));
		}

		private void Interact(Ray ray)
		{
			if (_interactWhenPossibleRoutine != null)
				StopCoroutine(_interactWhenPossibleRoutine);

			if (!Physics.Raycast(ray, out _hit, float.MaxValue, _layerMask)) return;

			if (!_hit.transform.TryGetComponent(out IInteractable interactable)) return;

			_agent.ResetPath(); // Stop any previous movement

			if (!interactable.CanInteract(gameObject))
			{
				Vector3? position = interactable.GetInteractionPosition(gameObject);
				if (position.HasValue)
				{
					Debug.Log($"Moving to interaction position {position.Value}");
					_agent.SetDestination(position.Value);
					_interactWhenPossibleRoutine = StartCoroutine(InteractWhenPossibleRoutine(interactable));
					return;
				}

				Debug.Log("Cannot interact");
				_onInteractionFailure?.Invoke();
				return;
			}

			bool success = interactable.Interact(gameObject);
			if (success) _onInteractionSuccess?.Invoke();
			else _onInteractionFailure?.Invoke();
		}

		private void Move(Vector2 screenPoint)
		{
			Move(_camera.ScreenPointToRay(screenPoint));
		}

		private void Move(Ray ray)
		{
			if (!Physics.Raycast(ray, out _hit, float.MaxValue, _layerMask)) return;
			if (_hit.transform.TryGetComponent(out IInteractable _)) return;

			Debug.Log("Moving to position");
			_agent.SetDestination(_hit.point);
		}

		private IEnumerator InteractWhenPossibleRoutine(IInteractable interactable)
		{
			while (!interactable.CanInteract(gameObject))
				yield return null;

			interactable.Interact(gameObject);
			_interactWhenPossibleRoutine = null;
			_agent.ResetPath();
		}
	}
}
