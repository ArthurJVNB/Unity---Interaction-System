using System;
using UnityEngine;

namespace Project.InteractionSystem
{
	public class DropOnGround : MonoBehaviour
	{
		[SerializeField] private Transform _transform;
		[SerializeField] private float _distanceCheckGround = 1;
		[SerializeField] private LayerMask _groundLayer = 1 << 0; // Default
		[SerializeField] private Settings _position;
		[SerializeField] private Settings _rotation;

		private Transform Transform
		{
			get
			{
				if (!_transform)
					_transform = transform;
				return _transform;
			}
		}

		[Serializable]
		private struct Settings
		{
			public bool useX, useY, useZ;
			public Vector3 value;
		}

		private void Reset()
		{
			_transform = transform;
		}

		public void PutOnGround()
		{
			ApplyPosition(_position);
			ApplyRotation(_rotation);
		}

		private void ApplyPosition(Settings settings)
		{
			Ray ray = new(Transform.position + Vector3.up, Vector3.down);
			var hits = Physics.RaycastAll(ray, _distanceCheckGround, _groundLayer);
			foreach (var hit in hits)
			{
				if (hit.transform == Transform) continue;
				var offset = new Vector3(
					settings.useX ? settings.value.x : 0,
					settings.useY ? settings.value.y : 0,
					settings.useZ ? settings.value.z : 0);
				Transform.position = hit.point + offset;
				break;
			}
		}

		private void ApplyRotation(Settings settings)
		{
			var currentEuler = Transform.rotation.eulerAngles;
			Transform.rotation = Quaternion.Euler(
				settings.useX ? settings.value.x : currentEuler.x,
				settings.useY ? settings.value.y : currentEuler.y,
				settings.useZ ? settings.value.z : currentEuler.z);
		}
	}
}
