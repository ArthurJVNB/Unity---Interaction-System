using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Project.InteractionSystem
{
	public class Socket : MonoBehaviour
	{
		[SerializeField] private SocketData _socketData;
		[SerializeField] private MultiParentConstraint _constraint;
		[Min(0)]
		[SerializeField] private int _objectLimit = 1;

		private HashSet<GameObject> _assignedObjects;

		public SocketData Data => _socketData;

		private void Reset()
		{
			_constraint = GetComponentInChildren<MultiParentConstraint>();
		}

		public bool AssignObject(GameObject gameObject)
		{
			_assignedObjects ??= new();
			if (_assignedObjects.Count < _objectLimit)
				return _assignedObjects.Add(gameObject);
			return false;
		}

		public void DropObject(GameObject gameObject)
		{
			throw new NotImplementedException();
		}

	}
}
