using System;
using System.Collections;
using Project.InteractionSystem;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityHFSM;

namespace Project
{
	[RequireComponent(typeof(PlayerInputsRTS))]
	public class PlayerControllerRTS : MonoBehaviour
	{
		[SerializeField] private Camera _camera;
		[SerializeField] private NavMeshAgent _agent;
		[SerializeField] private LayerMask _layerMask = 1 << 0; // "Default"
		[SerializeField] private Animator _animator;

		private PlayerInputsRTS _input;
		private StateMachine _fsm;
		private InteractableObject _currentInteractable;
		private Vector3 _movePosition;
		private bool _shouldMove;
		private bool _lastMoveWasInteractable;
		private float _stateElapsed = 0;
		private bool _stateCancelGoToInteractionPoint;

		private void Reset()
		{
			_agent = GetComponentInChildren<NavMeshAgent>();
			_animator = GetComponentInChildren<Animator>();
		}

		private void Awake()
		{
			_input = GetComponent<PlayerInputsRTS>();
			if (!_camera) _camera = Camera.main;
			SetupFSM();
		}

		private void OnEnable()
		{
			_input.OnMoveRTSPerformed += Input_OnMoveRTSPerformed;
		}

		private void OnDisable()
		{
			_input.OnMoveRTSPerformed -= Input_OnMoveRTSPerformed;
		}

		private void Update()
		{
			_fsm.OnLogic();
			_input.interact = false; // consume value;
		}

		private void SetupFSM()
		{
			_fsm = new();

			// Default State
			_fsm.AddState("Default",
				onEnter =>
				{
					Debug.Log("<color=white>Default</color>");
					_agent.stoppingDistance = 0;
				},
				onLogic =>
				{
					if (_shouldMove)
					{
						_agent.SetDestination(_movePosition);
						_shouldMove = false;
					}
				},
				onExit =>
				{

				});

			// GoToInteractionPoint State
			_fsm.AddState("GoToInteractionPoint",
				onEnter =>
				{
					Debug.Log("<color=white>GoToInteractionPoint</color>");
					_stateElapsed = 0;
					_agent.stoppingDistance = 1;
					_stateCancelGoToInteractionPoint = false;
					if (_currentInteractable.TryGetComponent(out NavMeshObstacle obstacle))
						obstacle.enabled = false;
				},
				onLogic =>
				{
					_stateElapsed += Time.deltaTime;
					if (!_lastMoveWasInteractable)
					{
						_stateCancelGoToInteractionPoint = true;
						_agent.SetDestination(_movePosition);
						_shouldMove = false;
						_currentInteractable = null;
						_fsm.Trigger("OnCancelInteraction");
					}
					if (_shouldMove)
					{
						_movePosition = _currentInteractable.transform.position;
						_agent.SetDestination(_movePosition);
						_shouldMove = false;
					}
				},
				onExit =>
				{
				},
				canExit =>
				{
					return _stateCancelGoToInteractionPoint || (_agent.hasPath && _agent.remainingDistance <= _agent.stoppingDistance);
				},
				needsExitTime: true);

			_fsm.AddState("StartInteraction",
				onEnter =>
				{
					Debug.Log("<color=white>StartInteraction</color>");
					_agent.enabled = false;
					_stateElapsed = _currentInteractable.Data.Start.time;
					var animationData = _currentInteractable.Data.Start.animationData;
					if (animationData) animationData.Apply(_animator);
				},
				onLogic =>
				{
					_stateElapsed -= Time.deltaTime;
				},
				onExit =>
				{

				},
				canExit =>
				{
					return _stateElapsed <= 0;
				},
				needsExitTime: true);

			_fsm.AddState("Interacting",
				onEnter =>
				{
					Debug.Log("<color=white>Interacting</color>");
					var animationData = _currentInteractable.Data.Loop.animationData;
					if (animationData) animationData.Apply(_animator);
					_agent.enabled = true;
					_agent.ResetPath();
				},
				onLogic =>
				{
					if (_shouldMove)
					{
						_agent.SetDestination(_movePosition);
						_shouldMove = false;
					}
					if (_input.interact)
					{
						Debug.Log("end interaction");
						_fsm.Trigger("OnEndInteraction");
					}
				},
				onExit =>
				{

				});

			_fsm.AddState("EndInteraction",
				onEnter =>
				{
					Debug.Log("<color=white>EndInteraction</color>");
					_stateElapsed = 0;
					_agent.ResetPath();
					_agent.enabled = false;
					var animationData = _currentInteractable.Data.End.animationData;
					if (animationData) animationData.Apply(_animator);
				},
				onLogic =>
				{
					_stateElapsed += Time.deltaTime;
				},
				onExit =>
				{
					if (_currentInteractable.TryGetComponent(out NavMeshObstacle obstacle))
						obstacle.enabled = true;
					_currentInteractable = null;
					_agent.enabled = true;
				},
				canExit =>
				{
					return _stateElapsed > _currentInteractable.Data.End.time;
				},
				needsExitTime: true);

			_fsm.AddTransition("GoToInteractionPoint", "StartInteraction");
			_fsm.AddTransition("StartInteraction", "Interacting");
			_fsm.AddTransition("EndInteraction", "Default");
			_fsm.AddTriggerTransition("OnStartInteraction", "Default", "GoToInteractionPoint");
			_fsm.AddTriggerTransition("OnEndInteraction", "Interacting", "EndInteraction");
			_fsm.AddTriggerTransition("OnCancelInteraction", "GoToInteractionPoint", "Default");

			_fsm.SetStartState("Default");
			_fsm.Init();
		}

		private void Input_OnMoveRTSPerformed(Vector2 pointerPosition)
		{
			StartCoroutine(DelayedMoveRTS(pointerPosition));
		}

		private IEnumerator DelayedMoveRTS(Vector2 pointerPosition)
		{
			yield return null;
			if (EventSystem.current.IsPointerOverGameObject())
			{
				Debug.Log("pointer is over ui");
				yield break;
			}

			Ray ray = _camera.ScreenPointToRay(pointerPosition);
			if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000, _layerMask))
			{
				_movePosition = hitInfo.point;
				_shouldMove = true;
				if (hitInfo.transform.TryGetComponent(out InteractableObject interactable))
				{
					Debug.Log($"touched interactable object '{interactable.name}'");
					_currentInteractable = interactable;
					_lastMoveWasInteractable = true;
					_fsm.Trigger("OnStartInteraction");
				}
				else
				{
					Debug.Log("touched ground");
					_lastMoveWasInteractable = false;
				}
				//_agent.SetDestination(hitInfo.point);
			}
		}
	}
}
