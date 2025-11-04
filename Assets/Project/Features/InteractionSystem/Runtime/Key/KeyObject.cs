using UnityEngine;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	public class KeyObject : MonoBehaviour, IPlaceableKey
	{
		[SerializeField] private KeyData _key;
		[SerializeField] private GameObject _owner;

		[field: Space]
		[field: SerializeField] public UnityEvent<GameObject> OnPlaceKey { get; private set; }
		[field: SerializeField] public UnityEvent<GameObject> OnRemoveKey { get; private set; }

		public KeyData Key => _key;
		public GameObject KeyGameObject => gameObject;

		public bool CanPlaceKey(GameObject gameObject)
		{
			return true;
		}

		public bool PlaceKey(GameObject gameObject)
		{
			if (!gameObject.TryGetComponent(out IPlaceableKey placeable))
			{
				Debug.LogWarning($"'{gameObject}' is not an {typeof(IPlaceableKey)}", gameObject);
				return false;
			}

			_owner = gameObject;
			OnPlaceKey?.Invoke(gameObject);
			return true;
		}

		public bool RemoveKey(GameObject gameObject)
		{
			if (!gameObject.TryGetComponent(out IPlaceableKey placeable))
			{
				Debug.LogWarning($"'{gameObject}' is not an {typeof(IPlaceableKey)}", gameObject);
				return false;
			}

			_owner = null;
			return true;
		}

		public void RemoveOwner()
		{
			if (!_owner) return;
			_owner.GetComponent<IPlaceableKey>().RemoveKey(gameObject);
			OnRemoveKey?.Invoke(gameObject);
			_owner = null;
		}
	}
}
