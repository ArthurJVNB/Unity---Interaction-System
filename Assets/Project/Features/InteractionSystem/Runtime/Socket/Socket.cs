using System.Collections.Generic;
using UnityEngine;

namespace Project.InteractionSystem
{
	public class Socket : MonoBehaviour
	{
		[SerializeField] private SocketData _socketData;
		[SerializeField] private Transform _root;
		[Min(0)]
		[SerializeField] private int _objectLimit = 1;

		private HashSet<GameObject> _assignedObjects;

		public SocketData Data => _socketData;

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
				gameObject.transform.localPosition = Vector3.zero;
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
