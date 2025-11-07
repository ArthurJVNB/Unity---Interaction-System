using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	public class Socket : MonoBehaviour, ISocket
	{
		[SerializeField] private SocketData _socketData;
		[SerializeField] private Transform _root;
		[Min(0)]
		[SerializeField] private int _objectLimit = 1;

		[field: Space]
		[field: SerializeField] public UnityEvent<GameObject> OnAssign { get; private set; }
		[field: SerializeField] public UnityEvent<GameObject> OnDrop { get; private set; }

		private HashSet<GameObject> _assignedObjects;

		public SocketData SocketData => _socketData;

		private Transform Root
		{
			get
			{
				if (!_root)
					_root = transform;
				return _root;
			}
		}

		private void Reset()
		{
			_root = transform;
		}

		private void Awake()
		{
			if (!_root)
				_root = transform;
		}

		public bool AssignObject(GameObject gameObject)
		{
			bool couldAssign;
			_assignedObjects ??= new();
			if (_assignedObjects.Count < _objectLimit)
				couldAssign = _assignedObjects.Add(gameObject);
			else
				couldAssign = false;

			if (couldAssign)
			{
				gameObject.transform.SetParent(Root, true);
				gameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			}

			return couldAssign;
		}

		public void DropObject(GameObject gameObject)
		{
			_assignedObjects ??= new();
			if (_assignedObjects.Remove(gameObject))
				gameObject.transform.SetParent(null, true);
		}
	}
}
