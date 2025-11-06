using UnityEngine;

namespace Project.InteractionSystem
{
	public class PushRigidbody : MonoBehaviour
	{
		[SerializeField] private Rigidbody _rigidbody;
		[SerializeField] private Vector3 _force = Vector3.up;
		[SerializeField] private ForceMode _forceMode = ForceMode.Acceleration;
		[SerializeField] private bool _local;
		[SerializeField] private bool _autoDisableIsKinematic = true;

		private Vector3 RotatedForce => _local ? (transform.rotation * _force) : _force;

		private void Reset()
		{
			_rigidbody = GetComponentInChildren<Rigidbody>();
		}

		public void Push()
		{
			if (_autoDisableIsKinematic) _rigidbody.isKinematic = false;
			_rigidbody.AddForce(RotatedForce, _forceMode);
		}

		public void PushOther(GameObject other)
		{
			if (!other) return;
			if (!other.TryGetComponent(out PushRigidbody otherPush))
			{
				otherPush = other.AddComponent<PushRigidbody>();
				otherPush._rigidbody = other.GetComponentInChildren<Rigidbody>();
				otherPush._force = RotatedForce; // apply this object's rotated force
				otherPush._forceMode = _forceMode;
				otherPush._local = false; // to apply rotated force based on this object's rotation, and not on the other's rotation
				otherPush._autoDisableIsKinematic = _autoDisableIsKinematic;
				otherPush.Push();
				Destroy(otherPush);
				return;
			}
			PushOther(otherPush);
		}

		public void PushOther(PushRigidbody other)
		{
			other.Push();
		}
	}
}
