using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.InteractionSystem
{
	public class SocketManager : MonoBehaviour
	{
		[ContextMenuItem("Find Sockets", nameof(FindSockets))]
		[SerializeField] private Socket[] _sockets;
		[SerializeField] private Transform _worldSocket;
		[SerializeField, Min(0)] private int _worldObjectsLimit = 1;
		[SerializeField] private bool _worldObjectsSnaps;

		private Dictionary<GameObject, SocketType> _assignedGameObjectsObsolete;
		private Dictionary<GameObject, Socket> _assignedGameObjects;

		private Transform WorldSocket
		{
			get
			{
				if (!_worldSocket)
					_worldSocket = transform;
				return _worldSocket;
			}
		}

		private void Reset()
		{
			_worldSocket = transform;
		}

		private void Awake()
		{
			if (!_worldSocket)
				_worldSocket = transform;
		}

		/// <summary>
		/// Assign a GameObject to a Socket (optional).
		/// </summary>
		/// <param name="gameObject">GameObject to be assigned to a Socket.</param>
		/// <param name="socketData">Socket type to assign to. If null, it will be considered as "World",
		/// and will be assigned as a child of the Transform of this Socket Manager.</param>
		public void AssignObject(GameObject gameObject, SocketData socketData = null)
		{
			var socket = GetSocket(socketData);

			_assignedGameObjects ??= new();

			bool couldAssign;
			if (!socket)
				couldAssign = AssignObjectWorld(gameObject);
			else
				couldAssign = socket.AssignObject(gameObject);

			if (!couldAssign)
			{
				Debug.Log($"Socket '{(socket ? socket.Data.Name : "World")}' was already full");
				return;
			}

			_assignedGameObjects.Add(gameObject, socket);
		}

		public void DropObject(GameObject gameObject)
		{
			Socket socket = null;
			bool exists = _assignedGameObjects?.Remove(gameObject, out socket) ?? false;

			if (!exists)
			{
				Debug.LogWarning($"'{gameObject.name}' was not assigned to the manager", this);
				return;
			}

			if (!socket)
			{
				DropObjectWorld(gameObject);
				return;
			}

			socket.DropObject(gameObject);
		}

		private Socket GetSocket(SocketData socketData)
		{
			if (_sockets == null || _sockets.Length == 0)
				return null;

			return _sockets.FirstOrDefault(v => v.Data == socketData);
		}

		private bool AssignObjectWorld(GameObject gameObject)
		{
			if (_assignedGameObjects.Count(v => !v.Value) >= _worldObjectsLimit)
				return false;

			gameObject.transform.SetParent(WorldSocket, true);
			if (_worldObjectsSnaps) gameObject.transform.localPosition = Vector3.zero;
			return true;
		}

		private void DropObjectWorld(GameObject gameObject)
		{
			gameObject.transform.SetParent(null, true);
		}

		[ContextMenu("Find Sockets")]
		private void FindSockets()
		{
#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(this);
			UnityEditor.Undo.RecordObject(this, "Find Sockets");
#endif

			var sockets = GetComponentsInChildren<Socket>();
			_sockets = sockets;
		}

	}
}
