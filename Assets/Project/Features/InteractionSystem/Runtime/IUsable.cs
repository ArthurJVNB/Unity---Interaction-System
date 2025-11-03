using UnityEngine;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	public interface IUsable
	{
		public UnityEvent OnUse { get; }
		public void Use(GameObject whoIsUsing);
	}
}
