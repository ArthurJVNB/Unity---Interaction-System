using UnityEngine;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	public interface IGrabable : ISocket
	{
		public UnityEvent OnGrab { get; }
		public UnityEvent OnDrop { get; }
		public void Grab(GameObject whoIsGrabbing);
		public void Drop();
	}
}
