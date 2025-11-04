using UnityEngine;

namespace Project.InteractionSystem
{
	public class PlaceableSocket : MonoBehaviour
	{
		[Tooltip("Identifier to check if an item can be placed here")]
		[SerializeField] private SocketData _socketKey;
		[SerializeField] private PlaceableType _type;
		
		public enum PlaceableType
		{
			Key,
			ReceiveKey,
		}
	}
}
