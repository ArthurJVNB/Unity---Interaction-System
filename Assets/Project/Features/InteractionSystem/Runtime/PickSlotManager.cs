using System;
using UnityEngine;

namespace Project.InteractionSystem
{
	public class PickSlotManager : MonoBehaviour
	{
		public void AssignObject(GameObject gameObject, SlotType slot)
		{
			Debug.Log($"{gameObject.name} wants to assign to slot {slot}");
		}

		public void DropObject(GameObject gameObject)
		{
			Debug.Log($"Dropping {gameObject.name}");
		}
	}
}
