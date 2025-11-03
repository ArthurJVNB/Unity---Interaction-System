using System;
using UnityEngine;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	public class PickableObject : MonoBehaviour, IInteractable, IPickable
	{
		[SerializeField] private SlotType _slotType;
		[SerializeField] private PickState _state;
		[field: Space]
		[field: SerializeField] public UnityEvent OnInteract { get; private set; }
		[field: SerializeField] public UnityEvent OnPick { get; private set; }
		[field: SerializeField] public UnityEvent OnDrop { get; private set; }

		private enum PickState { None, Picked }

		public SlotType PickType => _slotType;

		public void Interact(GameObject whoIsInteracting)
		{
			Debug.Log($"interact {name}");
			OnInteract?.Invoke();
			Pick(whoIsInteracting);
		}

		public void Pick(GameObject whoIsPicking)
		{
			Debug.Log($"pick {name} ({_slotType})");
			SwitchState();

			if (!whoIsPicking) return;

			if (whoIsPicking.TryGetComponent(out PickSlotManager pickSlotManager))
			{
				switch (_state)
				{
					case PickState.None:
						OnDrop?.Invoke();
						pickSlotManager.DropObject(gameObject);
						break;
					case PickState.Picked:
						OnPick?.Invoke();
						pickSlotManager.AssignObject(gameObject, _slotType);
						break;
					default:
						break;
				}
			}
			else
			{
				switch (_state)
				{
					case PickState.None:
						OnDrop?.Invoke();
						transform.SetParent(null, true);
						break;
					case PickState.Picked:
						OnPick?.Invoke();
						transform.SetParent(whoIsPicking.transform, true);
						break;
					default:
						break;
				}
			}
		}

		private void SwitchState()
		{
			switch (_state)
			{
				case PickState.None:
					_state = PickState.Picked;
					break;
				case PickState.Picked:
					_state = PickState.None;
					break;
				default:
					Debug.LogError("Current pick state is invalid!", this);
					break;
			}
		}

	}
}
