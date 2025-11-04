using UnityEngine;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	public interface IGrabable
	{
		public SocketData SocketType { get; }
		public UnityEvent OnGrab { get; }
		public UnityEvent OnDrop { get; }
		public void Grab(GameObject whoIsGrabbing);
	}
}
