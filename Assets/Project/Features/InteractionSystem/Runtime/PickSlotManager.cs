using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Project.InteractionSystem
{
	public class PickSlotManager : MonoBehaviour
	{
		[SerializeField] private MultiParentConstraint _primaryHand;
		[SerializeField] private MultiParentConstraint _secondaryHand;

		private Dictionary<GameObject, SlotType> _assignedGameObjects;

		public void AssignObject(GameObject gameObject, SlotType slot)
		{
			Debug.Log($"{gameObject.name} wants to assign to slot {slot}");
			
			_assignedGameObjects ??= new();
			_assignedGameObjects.Add(gameObject, slot);

			switch (slot)
			{
				case SlotType.World:
					AssignObjectWorld(gameObject);
					break;
				case SlotType.PrimaryHand:
					AssignObjectPrimaryHand(gameObject);
					break;
				case SlotType.SecondaryHand:
					AssignObjectSecondaryHand(gameObject);
					break;
				default:
					break;
			}
		}

		public void DropObject(GameObject gameObject)
		{
			Debug.Log($"Dropping {gameObject.name}");
			
			_assignedGameObjects ??= new();
			bool exists = _assignedGameObjects.Remove(gameObject, out SlotType slot);
			if (!exists)
			{
				Debug.LogWarning($"'{gameObject.name}' was not assigned to the manager", this);
				return;
			}

			switch (slot)
			{
				case SlotType.World:
					DropObjectWorld(gameObject);
					break;
				case SlotType.PrimaryHand:
					DropObjectPrimaryHand(gameObject);
					break;
				case SlotType.SecondaryHand:
					DropObjectSecondaryHand(gameObject);
					break;
				default:
					break;
			}
		}

		private void DropObjectWorld(GameObject gameObject)
		{
			gameObject.transform.SetParent(null, true);
		}

		private void DropObjectPrimaryHand(GameObject gameObject)
		{
			throw new NotImplementedException();
		}

		private void DropObjectSecondaryHand(GameObject gameObject)
		{
			throw new NotImplementedException();
		}

		private void AssignObjectWorld(GameObject gameObject)
		{
			gameObject.transform.SetParent(transform, true);
		}

		private void AssignObjectPrimaryHand(GameObject gameObject)
		{
			throw new NotImplementedException();
		}

		private void AssignObjectSecondaryHand(GameObject gameObject)
		{
			throw new NotImplementedException();
		}

	}
}
