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
		[SerializeField] private Transform _interactionPosition;
		[Space]
		[SerializeField] private UnityEvent _onInteractionSuccess;
		[SerializeField] private UnityEvent _onInteractionFailure;

		private RaycastHit _hit;
		private Coroutine _interactWhenPossibleRoutine;

		private Transform InteractionPosition
		{
			get
			{
				if (!_interactionPosition)
					_interactionPosition = transform;
				return _interactionPosition;
			}
		}

		private void Awake()
		{
			if (!_camera)
				_camera = Camera.main;

			if (!_interactionPosition)
				_interactionPosition = transform;
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
			{
				StopCoroutine(_interactWhenPossibleRoutine);
				_interactWhenPossibleRoutine = null;
			}

			if (!Physics.Raycast(ray, out _hit, float.MaxValue, _layerMask)) return;

			if (!_hit.transform.TryGetComponent(out IInteractable interactable)) return;

			_agent.ResetPath(); // Stop any previous movement

			bool canInteract = CanInteract(interactable, out Transform whoCanInteract);
			if (!canInteract)
			{
				Vector3? position = interactable.GetInteractionPosition(InteractionPosition.position);
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

			CallbacksInteraction(interactable.Interact(gameObject, whoCanInteract.position, whoCanInteract.rotation));
		}

		private void CallbacksInteraction(bool couldInteract)
		{
			if (couldInteract) _onInteractionSuccess?.Invoke();
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

			_agent.SetDestination(_hit.point);
		}

		private IEnumerator InteractWhenPossibleRoutine(IInteractable interactable)
		{
			Transform whoCanInteract;
			while (!CanInteract(interactable, out whoCanInteract))
			{
				// DEBUG
				(Vector3? position, Quaternion? rotation) = interactable.GetNearestInteractionPositionAndRotation(transform.position, transform.rotation);

				Transform reference = transform;
				Vector3 start = reference.position;
				Vector3 end = position ?? reference.position;

				Debug.DrawLine(start, end, Color.yellow, .1f);
				if (rotation.HasValue)
				{
					Debug.DrawLine(
						start,
						rotation.Value * Vector3.forward + start,
						Color.blue, .1f);
					rot = rotation.Value.eulerAngles;
				}
				// -----

				float distance = Vector3.Distance(reference.position, position.Value);
				if (distance <= 1 && rotation.HasValue)
				{
					_agent.updateRotation = false;
					transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation.Value, _agent.angularSpeed * Time.deltaTime);
				}

				yield return null;
			}

			_agent.ResetPath();
			_agent.updateRotation = true;
			_interactWhenPossibleRoutine = null;
			CallbacksInteraction(interactable.Interact(gameObject, whoCanInteract.position, whoCanInteract.rotation));
		}

		private bool CanInteract(IInteractable interactable, out Transform whoCanInteract)
		{
			if (interactable.CanInteract(gameObject, transform.position, transform.rotation))
			{
				whoCanInteract = transform;
				return true;
			}

			if (interactable.CanInteract(gameObject, InteractionPosition.position, InteractionPosition.rotation))
			{
				whoCanInteract = InteractionPosition;
				return true;
			}

			whoCanInteract = null;
			return false;
		}

		public Vector3 dir;
		public Vector3 rot;
		private void OnDrawGizmos()
		{
			Quaternion rot = Quaternion.Euler(this.rot);
			Gizmos.DrawLine(transform.position, transform.position + transform.forward);
			Gizmos.DrawLine(transform.position, transform.position + rot * dir);
		}
	}
}
