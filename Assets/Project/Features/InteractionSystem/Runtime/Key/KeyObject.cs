using UnityEngine;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	public class KeyObject : MonoBehaviour, IKey
	{
		[SerializeField] private KeyData _key;
		[SerializeField] private GameObject _ownerEditor;

		[field: Space]
		[field: SerializeField] public UnityEvent<IKeyReceiver> OnPlaceKey { get; private set; }
		[field: SerializeField] public UnityEvent<IKeyReceiver> OnRemoveKey { get; private set; }

		public KeyData Key => _key;
		public GameObject KeyGameObject => gameObject;

		private IKeyReceiver _owner;

		public bool CanPlace(IKeyReceiver keyReceiver)
		{
			return keyReceiver.Key == Key;
		}

		public bool PlaceKey(IKeyReceiver keyReceiver)
		{
			if (!CanPlace(keyReceiver))
				return false;

			_ownerEditor = keyReceiver.KeyReceiverGameObject;
			_owner = keyReceiver;
			OnPlaceKey?.Invoke(keyReceiver);
			return true;
		}

		public bool RemoveKey(IKeyReceiver keyReceiver)
		{
			if (_owner == null)
				return false;

			_ownerEditor = null;
			OnRemoveKey?.Invoke(keyReceiver);
			return true;
		}

		public void RemoveOwner()
		{
			if (_owner == null) return;
			_owner.RemoveKey(this);
			_owner = null;
			_ownerEditor = null;
		}
	}
}
