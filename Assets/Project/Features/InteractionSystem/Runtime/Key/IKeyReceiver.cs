using UnityEngine;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	public interface IKeyReceiver
	{
		public KeyData Key { get; }
		public GameObject KeyReceiverGameObject { get; }

		public UnityEvent<IKey> OnPlaceKey { get; }
		public UnityEvent<IKey> OnRemoveKey { get; }

		public bool PlaceKey(IKey key);
		public bool RemoveKey(IKey key);

		public bool CanPlaceKey(IKey key);
	}
}
