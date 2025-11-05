using System;
using UnityEngine;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	public class KeyReceiverManager : MonoBehaviour
	{
		[SerializeField] private KeyReceiver[] _keyReceivers;
		[SerializeField, Min(0)] private int _numberOfKeys = 2;
		[SerializeField] private bool _disableInteractionIfReachedNumberOfKeys = true;
		[field: Space]
		[field: SerializeField] public UnityEvent OnReachedNumberOfKeys { get; private set; }
		[field: SerializeField] public UnityEvent OnLostNumberOfKeys { get; private set; }

		private int _keyCount = 0;
		private bool _hasReached;

		private void Reset()
		{
			_keyReceivers = GetComponentsInChildren<KeyReceiver>();
			if (_keyReceivers != null && _keyReceivers.Length > 0)
				_numberOfKeys = _keyReceivers.Length;
		}

		private void OnEnable()
		{
			foreach (var keyReceiver in _keyReceivers)
			{
				keyReceiver.OnPlaceKey.AddListener(KeyReceiver_OnPlaceKey);
				keyReceiver.OnRemoveKey.AddListener(KeyReceiver_OnRemoveKey);
			}

			HandleKeys();
		}

		private void OnDisable()
		{
			foreach (var keyReceiver in _keyReceivers)
			{
				keyReceiver.OnPlaceKey.RemoveListener(KeyReceiver_OnPlaceKey);
				keyReceiver.OnRemoveKey.RemoveListener(KeyReceiver_OnRemoveKey);
			}
		}

		private void KeyReceiver_OnPlaceKey(GameObject key)
		{
			_keyCount++;
			HandleKeys();
		}

		private void KeyReceiver_OnRemoveKey(GameObject key)
		{
			_keyCount--;
			HandleKeys();
		}

		private void HandleKeys()
		{
			bool hasReached = _keyCount >= _numberOfKeys;
			if (hasReached != _hasReached)
			{
				if (hasReached)
				{
					if (_disableInteractionIfReachedNumberOfKeys)
						SetKeyInteractions(false);
					OnReachedNumberOfKeys?.Invoke();
				}
				else
				{
					OnLostNumberOfKeys?.Invoke();
				}
				_hasReached = hasReached;
			}
		}

		private void SetKeyInteractions(bool interactable)
		{
			foreach (var keyReceiver in _keyReceivers)
				keyReceiver.IsInteractionEnabled = interactable;
		}
	}
}
