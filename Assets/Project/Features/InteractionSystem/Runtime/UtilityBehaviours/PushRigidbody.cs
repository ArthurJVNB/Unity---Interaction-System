using UnityEngine;

namespace Project.InteractionSystem
{
	public class PushRigidbody : MonoBehaviour
	{
		[SerializeField] private Rigidbody _rigidbody;
		[SerializeField] private Vector3 _force = Vector3.up;
		[SerializeField] private ForceMode _forceMode = ForceMode.Acceleration;
		[SerializeField] private bool _autoDisableIsKinematic = true;

		private void Reset()
		{
			_rigidbody = GetComponentInChildren<Rigidbody>();
		}

		public void Push()
		{
			if (_autoDisableIsKinematic) _rigidbody.isKinematic = false;
			_rigidbody.AddForce(_force, _forceMode);
		}

		public void PushOther(GameObject other)
		{
			if (!other) return;
			if (!other.TryGetComponent(out PushRigidbody otherPush))
			{
				otherPush = other.AddComponent<PushRigidbody>();
				otherPush._rigidbody = other.GetComponentInChildren<Rigidbody>();
				otherPush._force = _force;
				otherPush._forceMode = _forceMode;
				otherPush._autoDisableIsKinematic = _autoDisableIsKinematic;
			}
			PushOther(otherPush);
		}

		public void PushOther(PushRigidbody other)
		{
			other.Push();
		}
	}
}
