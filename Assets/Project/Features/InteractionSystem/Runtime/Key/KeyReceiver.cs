using System;
using UnityEngine;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	public class KeyReceiver : MonoBehaviour, IInteractable, IPlaceableKey
	{
		[Tooltip("Identifier to check if an item can be placed here.")]
		[SerializeField] private KeyData _key;
		[Tooltip("Where to put the key when received.")]
		[SerializeField] private Transform _keyPosition;
		[SerializeField] private bool _canInteract = true;
		[SerializeField] private bool _canChangeKeyInteraction = true;
		[SerializeField, Min(0)] private float _interactionDistance = 1;
		[SerializeField, Space] private GameObject _currentKey;

		[field: Space]
		[field: SerializeField] public UnityEvent OnInteract { get; private set; }
		[field: SerializeField] public UnityEvent<GameObject> OnPlaceKey { get; private set; }
		[field: SerializeField] public UnityEvent<GameObject> OnRemoveKey { get; private set; }


		public bool IsInteractionEnabled
		{
			get => _canInteract;
			set
			{
				_canInteract = value;
				if (_canChangeKeyInteraction && _currentKey)
				{
					if (_currentKey.TryGetComponent(out IInteractable interactableKey))
						interactableKey.IsInteractionEnabled = value;
				}
			}
		}

		public KeyData Key => _key;
		public GameObject KeyGameObject => gameObject;

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

			if (whoIsInteracting.TryGetComponent(out IPlaceableKey placeable))
				return PlaceOrRemoveKeyFromPlaceable(placeable);

			if (whoIsInteracting.TryGetComponent(out SocketManager socketManager))
				return PlaceOrRemoveKeyFromSocketManager(socketManager);

			return false;
		}

		private bool PlaceOrRemoveKeyFromPlaceable(IPlaceableKey placeable)
		{
			throw new NotImplementedException();
		}

		private bool PlaceOrRemoveKeyFromSocketManager(SocketManager socketManager)
		{
			var objs = socketManager.AssignedGameObjects;
			if (objs == null || objs.Length == 0)
				return false;

			IPlaceableKey placeableKey = null;
			foreach (var obj in objs)
			{
				if (!obj.TryGetComponent(out placeableKey)) continue;
				if (placeableKey.Key != _key) continue;
				break;
			}

			if (placeableKey == null)
				return false;

			bool success = false;
			if (_currentKey == placeableKey.KeyGameObject)
			{
				success = RemoveKey(placeableKey.KeyGameObject);
				if (success)
				{
					var socket = placeableKey.KeyGameObject.GetComponent<ISocket>();
					socketManager.AssignObject(placeableKey.KeyGameObject, socket.SocketData);
				}
			}
			else
			{
				if (CanPlaceKey(placeableKey.KeyGameObject))
				{
					socketManager.DropObject(placeableKey.KeyGameObject);
					success = PlaceKey(placeableKey.KeyGameObject);
				}
			}

			return success;
		}

		public bool CanPlaceKey(GameObject gameObject)
		{
			return IsInteractionEnabled
				&& gameObject.TryGetComponent(out IPlaceableKey placeable)
				&& placeable.Key == Key;
		}

		public bool PlaceKey(GameObject key)
		{
			if (!CanPlaceKey(key)) return false;

			if (!key.TryGetComponent(out IPlaceableKey placeable))
			{
				Debug.LogWarning($"'{key}' is not an {typeof(IPlaceableKey)}", key);
				return false;
			}

			Debug.Log($"Placing key '{placeable.Key}'");
			placeable.PlaceKey(this.gameObject);
			_currentKey = placeable.KeyGameObject;
			placeable.KeyGameObject.transform.SetParent(KeyPosition, true);
			placeable.KeyGameObject.transform.localPosition = Vector3.zero;
			placeable.KeyGameObject.transform.localRotation = Quaternion.identity;

			OnPlaceKey?.Invoke(placeable.KeyGameObject);

			return true;
		}

		public bool RemoveKey(GameObject key)
		{
			if (!key.TryGetComponent(out IPlaceableKey placeable))
			{
				Debug.LogWarning($"'{key}' is not an {typeof(IPlaceableKey)}", key);
				return false;
			}

			Debug.Log($"Removing key '{placeable.Key}'");
			placeable.RemoveKey(this.gameObject);
			_currentKey = null;

			OnRemoveKey?.Invoke(placeable.KeyGameObject);

			return true;
		}


	}
}
