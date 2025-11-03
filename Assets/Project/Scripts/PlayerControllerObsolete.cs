using System;
using System.Collections;
using Project.InteractionSystem;
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;

namespace Project
{
	[Obsolete("Please use Player Controller.")]
	[RequireComponent(typeof(ThirdPersonController))]
	[RequireComponent(typeof(PlayerInputs))]
	public class PlayerControllerObsolete : MonoBehaviour
	{
		[SerializeField] private NavMeshAgent _agent;
		[SerializeField] private Animator _animator;

		[Header("Interaction System (optional)")]
		[SerializeField] private InteractorObsolete _interactor;
		[SerializeField] private GameObject _interactionUI;

		private ThirdPersonController _thirdPersonController;
		private PlayerInputs _inputs;

		private void Awake()
		{
			_thirdPersonController = GetComponent<ThirdPersonController>();
			_inputs = GetComponent<PlayerInputs>();
		}

		private void OnEnable()
		{
			if (_interactor)
			{
				_interactor.OnHasInteractablesChanged += Interactor_OnHasInteractablesChanged;
			}
		}

		private void OnDisable()
		{
			if (_interactor)
			{
				_interactor.OnHasInteractablesChanged -= Interactor_OnHasInteractablesChanged;
			}
		}

		private void Interactor_OnHasInteractablesChanged(InteractorObsolete.EventHandler args)
		{
			if (args.interactor.HasInteractables)
				ShowInteractionUI();
			else
				HideInteractionUI();
		}

		private void ShowInteractionUI()
		{
			_interactionUI.SetActive(true);
		}

		private void HideInteractionUI()
		{
			_interactionUI.SetActive(false);
		}

		private void Update()
		{
			HandleInteraction();
		}

		private void HandleInteraction()
		{
			//if (!_interactor) return;
			Interact();
		}

		private void Interact()
		{
			if (!_inputs.interact) return;
			_inputs.interact = false;

			var interactable = _interactor.GetInteractable();
			if (!interactable) return;

			StartCoroutine(InteractionRoutine(interactable));
		}

		private IEnumerator InteractionRoutine(InteractableObjectObsolete interactable)
		{
			if (interactable.CurrentStateType == InteractionStateType.None && interactable.InteractionDistance < Vector3.Distance(transform.position, interactable.transform.position))
			{
				var position = interactable.GetInteractionPosition(transform.position);
				_thirdPersonController.enabled = false;
				//_agent.stoppingDistance = interactable.InteractionDistance;

				_agent.SetDestination(position);

				Debug.Log("Path pending");
				yield return new WaitWhile(() => _agent.pathPending);
				Debug.Log("Moving");
				yield return new WaitWhile(() => _agent.remainingDistance > _agent.stoppingDistance);

				_agent.ResetPath();
				_thirdPersonController.enabled = true;
			}

			interactable.OnStateEntered -= Interactable_OnStateEntered;
			interactable.OnStateEntered += Interactable_OnStateEntered;
			interactable.Interact();
		}

		private void Interactable_OnStateEntered(InteractionStateObsolete state)
		{
			Debug.Log("Player controller: " + state.name);
			if (state.data.animationData) state.data.animationData.Apply(_animator);

		}
	}
}
