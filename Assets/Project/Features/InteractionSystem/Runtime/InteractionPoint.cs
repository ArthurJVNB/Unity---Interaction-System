#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Project.InteractionSystem
{
	public class InteractionPoint : MonoBehaviour
	{
		[Min(0)]
		[SerializeField] private float _validDistance = .2f;
		[Range(.1f, 180)]
		[SerializeField] private float _validRotationAngleDif = 5;
		[SerializeField] private bool _useRotation;

		public Vector3 Position => transform.position;
		public Quaternion Rotation => transform.rotation;

		public bool CanInteract(Vector3 position, Quaternion rotation)
		{
			return IsCloseEnough(position) && IsCloseEnough(rotation);
		}

		private bool IsCloseEnough(Vector3 position)
		{
			float distance = Vector3.Distance(Position, position);
			//Debug.Log($"distance {distance} <= {_validDistance} ? {distance <= _validDistance}");
			return distance <= _validDistance;
		}

		private bool IsCloseEnough(Quaternion rotation)
		{
			if (!_useRotation) return true;
			float angle = Quaternion.Angle(Rotation, rotation);
			//Debug.Log($"rotation {angle} <= {_validRotationAngleDif} ? {angle <= _validRotationAngleDif}");
			return angle <= _validRotationAngleDif;
		}

		#region Debug Handles
#if UNITY_EDITOR
		[DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
		static void DrawHandles(InteractionPoint scr, GizmoType gizmoType)
		{
			if (scr._useRotation) Handles.ArrowHandleCap(1, scr.Position, scr.Rotation, 1, EventType.Repaint);
			Handles.CircleHandleCap(1, scr.Position, Quaternion.LookRotation(Vector3.up), scr._validDistance, EventType.Repaint);
		}
#endif
		#endregion
	}
}
