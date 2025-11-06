using System;
using UnityEngine;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	public class KeyReceiver : MonoBehaviour, IInteractable, IKeyReceiver
	{
		[Tooltip("Identifier to check if an item can be placed here.")]
		[SerializeField] private KeyData _key;
		[Tooltip("Where to put the key when received.")]
		[SerializeField] private Transform _keyPosition;
		[SerializeField] private bool _canInteract = true;
		[SerializeField] private bool _canChangeKeyInteraction = true;
		[SerializeField] private bool _disableInteractionWhenPlaced = true;
		[SerializeField, Min(0)] private float _interactionDistance = 1;
		[SerializeField, Space] private GameObject _currentKeyEditor;

		[field: Space]
		[field: SerializeField] public UnityEvent OnInteract { get; private set; }
		[field: SerializeField] public UnityEvent<IKey> OnPlaceKey { get; private set; }
		[field: SerializeField] public UnityEvent<GameObject> OnPlaceKeyGameObject { get; private set; }
		[field: SerializeField] public UnityEvent<IKey> OnRemoveKey { get; private set; }
		[field: SerializeField] public UnityEvent<GameObject> OnRemoveKeyGameObject { get; private set; }

		private IKey _currentKey;

		public bool IsInteractionEnabled
		{
			get => _canInteract;
			set
			{
				_canInteract = value;
				if (_canChangeKeyInteraction && _currentKeyEditor)
				{
					if (_currentKeyEditor.TryGetComponent(out IInteractable interactableKey))
						interactableKey.IsInteractionEnabled = value;
				}
			}
		}

		public KeyData Key => _key;
		public GameObject KeyReceiverGameObject => gameObject;

		private Transform KeyPosition
		{
			get
			{
				if (!_keyPosition)
					_keyPosition = transform;
				return _keyPosition;
			}
		}

		private void Reset()
		{
			_keyPosition = transform;
		}

		private void Awake()
		{
			if (!_keyPosition)
				_keyPosition = transform;
		}

		public bool CanInteract(GameObject whoWantsToInteract)
		{
			return IsInteractionEnabled
				&& Vector3.Distance(whoWantsToInteract.transform.position, transform.position) <= _interactionDistance;
		}

		public Vector3? GetInteractionPosition(GameObject whoWantsToInteract)
		{
			if (!IsInteractionEnabled) return null;
			return transform.position;
		}

		public bool Interact(GameObject whoIsInteracting)
		{
			if (_currentKey == null)
				return PlaceOrRemoveKey(whoIsInteracting);

			return RemoveKey(_currentKey);
		}

		private bool PlaceOrRemoveKey(GameObject whoIsInteracting)
		{
			#region Backup
			//IPlaceableKey placeable = null;

			//if (whoIsInteracting.TryGetComponent(out SocketManager socketManager))
			//{
			//	var objs = socketManager.AssignedGameObjects;
			//	if (objs != null)
			//	{
			//		foreach (var obj in objs)
			//		{
			//			if (!obj.TryGetComponent(out placeable)) continue;
			//			if (placeable.Key != _key) continue;
			//			break;
			//		}
			//	}
			//}
			//else
			//{
			//	placeable = whoIsInteracting.GetComponent<IPlaceableKey>();
			//}

			//if (placeable != null)
			//{
			//	if (_currentKey == placeable.KeyGameObject)
			//		return RemoveKey(placeable.KeyGameObject);
			//	else
			//		return PlaceKey(placeable.KeyGameObject);
			//}

			//return false;
			#endregion

			if (whoIsInteracting.TryGetComponent(out IKey key))
				return PlaceOrRemoveKeyFromKey(key);

			if (whoIsInteracting.TryGetComponent(out SocketManager socketManager))
				return PlaceOrRemoveKeyFromSocketManager(socketManager);

			return false;
		}

		private bool PlaceOrRemoveKeyFromKey(IKey key)
		{
			throw new NotImplementedException();
		}

		private bool PlaceOrRemoveKeyFromSocketManager(SocketManager socketManager)
		{
			var objs = socketManager.AssignedGameObjects;
			if (objs == null || objs.Length == 0)
				return false;

			IKey key = null;
			foreach (var obj in objs)
			{
				if (!obj.TryGetComponent(out key)) continue;
				if (key.Key != _key) continue;
				break;
			}

			if (key == null)
				return false;

			bool success = false;
			if (_currentKey == key)
			{
				success = RemoveKey(key);
				if (success)
				{
					var socket = key.KeyGameObject.GetComponent<ISocket>();
					socketManager.AssignObject(key.KeyGameObject, socket.SocketData);
				}
			}
			else
			{
				if (CanPlaceKey(key))
				{
					socketManager.DropObject(key.KeyGameObject);
					success = PlaceKey(key);
				}
			}

			return success;
		}

		public bool PlaceKey(IKey key)
		{
			if (!CanPlaceKey(key)) return false;

			//if (!key.TryGetComponent(out IPlaceableKeyObsolete placeable))
			//{
			//	Debug.LogWarning($"'{key}' is not an {typeof(IPlaceableKeyObsolete)}", key);
			//	return false;
			//}

			Debug.Log($"Placing key '{key.Key.Name}' ('{key.KeyGameObject.name}')");
			key.PlaceKey(this);

			_currentKey = key;
			_currentKeyEditor = key.KeyGameObject;
			
			key.KeyGameObject.transform.SetParent(KeyPosition, true);
			key.KeyGameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			
			OnPlaceKey?.Invoke(key);
			OnPlaceKeyGameObject?.Invoke(key.KeyGameObject);

			if (_disableInteractionWhenPlaced)
				IsInteractionEnabled = false;

			return true;
		}

		public bool RemoveKey(IKey key)
		{
			//if (!key.TryGetComponent(out IPlaceableKeyObsolete placeable))
			//{
			//	Debug.LogWarning($"'{key}' is not an {typeof(IPlaceableKeyObsolete)}", key);
			//	return false;
			//}

			if (_currentKey == null || _currentKey != key)
				return false;

			Debug.Log($"Removing key '{key.Key.Name}' ('{key.KeyGameObject.name}')");
			if (key.KeyGameObject.transform.parent == KeyPosition)
				key.KeyGameObject.transform.SetParent(null, true);
			key.RemoveKey(this);

			_currentKey = null;
			_currentKeyEditor = null;

			OnRemoveKey?.Invoke(key);
			OnRemoveKeyGameObject?.Invoke(key.KeyGameObject);

			return true;
		}

		public bool CanPlaceKey(IKey key)
		{
			return IsInteractionEnabled && key.Key == Key;
		}

	}
}
