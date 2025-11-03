using UnityEngine;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	public interface IPickable
	{
		public SlotType PickType { get; }
		public UnityEvent OnPick { get; }
		public UnityEvent OnDrop { get; }
		public void Pick(GameObject whoIsPicking);
	}
}
