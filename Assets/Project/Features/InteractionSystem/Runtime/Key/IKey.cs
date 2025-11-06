using UnityEngine;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	public interface IKey
	{
		public KeyData Key { get; }
		public GameObject KeyGameObject { get; }

		public UnityEvent<IKeyReceiver> OnPlaceKey { get; }
		public UnityEvent<IKeyReceiver> OnRemoveKey { get; }

		public bool PlaceKey(IKeyReceiver receiver);
		public bool RemoveKey(IKeyReceiver receiver);

		public bool CanPlace(IKeyReceiver receiver);
	}
}
