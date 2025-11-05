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

		public bool PlaceKey(GameObject owner)
		{
			if (!owner.TryGetComponent(out IPlaceableKey placeable))
			{
				Debug.LogWarning($"'{owner}' is not an {typeof(IPlaceableKey)}", owner);
				return false;
			}

			_owner = owner;
			OnPlaceKey?.Invoke(owner);
			return true;
		}

		public bool RemoveKey(GameObject owner)
		{
			if (!owner.TryGetComponent(out IPlaceableKey placeable))
			{
				Debug.LogWarning($"'{owner}' is not an {typeof(IPlaceableKey)}", owner);
				return false;
			}

			_owner = null;
			OnRemoveKey?.Invoke(owner);
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
