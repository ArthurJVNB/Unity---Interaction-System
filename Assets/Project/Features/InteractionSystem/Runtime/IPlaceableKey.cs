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

		public bool PlaceKey(GameObject gameObject);
		public bool RemoveKey(GameObject gameObject);

		public bool CanPlaceKey(GameObject gameObject);
	}
}
