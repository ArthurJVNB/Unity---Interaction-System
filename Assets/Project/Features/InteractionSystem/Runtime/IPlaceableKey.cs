using UnityEngine;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	public interface IPlaceableKey
	{
		public KeyData Key { get; }
		public GameObject KeyGameObject { get; }

		public UnityEvent<GameObject> OnPlaceKey { get; }
		public UnityEvent<GameObject> OnRemoveKey { get; }

		public bool PlaceKey(GameObject keyOrReceiver);
		public bool RemoveKey(GameObject keyOrReceiver);

		public bool CanPlaceKey(GameObject keyOrReceiver);
	}
}
