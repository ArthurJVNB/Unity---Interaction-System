using UnityEngine;

namespace Project
{
	public class AnimatorMovement : MonoBehaviour
	{
		[SerializeField] private Animator _animator;
		[SerializeField] private string _forwardSpeedParameter = "ForwardSpeed";
		[SerializeField] private string _rightSpeedParameter = "RightSpeed";
		[Range(0, 1)]
		[SerializeField] private float _smoothTime = .1f;

		private Vector3 _lastPosition;
		private Vector3 _lastSpeed;
		private Vector3 _currentVelocity;
		private float _currentVelocityForward;
		private float _currentVelocityRight;

		private int _forwardSpeedParameterHash;
		private int _rightSpeedParameterHash;

		private void OnValidate() => SetupHashes();

		private void Reset() => _animator = GetComponentInChildren<Animator>();

		private void Awake() => SetupHashes();

		private void SetupHashes()
		{
			_forwardSpeedParameterHash = Animator.StringToHash(_forwardSpeedParameter);
			_rightSpeedParameterHash = Animator.StringToHash(_rightSpeedParameter);
		}

		private void OnEnable()
		{
			_lastPosition = transform.position;
			_lastSpeed = Vector3.zero;
			_currentVelocity = Vector3.zero;
		}

		private void Update()
		{
			var delta = transform.position - _lastPosition;
			_lastPosition = transform.position;

			var deltaForward = Vector3.Project(delta, transform.forward);
			var deltaRight = Vector3.Project(delta, transform.right);

			var forwardSpeed = (Vector3.Dot(transform.forward, deltaForward) > 0 ? 1 : -1) * deltaForward.magnitude / Time.deltaTime;
			var rightSpeed = (Vector3.Dot(transform.right, deltaRight) > 0 ? 1 : -1) * deltaRight.magnitude / Time.deltaTime;

			_lastSpeed.z = Mathf.SmoothDamp(_lastSpeed.z, forwardSpeed, ref _currentVelocityForward, _smoothTime);
			_lastSpeed.x = Mathf.SmoothDamp(_lastSpeed.x, rightSpeed, ref _currentVelocityRight, _smoothTime);

			_animator.SetFloat(_forwardSpeedParameterHash, _lastSpeed.z);
			_animator.SetFloat(_rightSpeedParameterHash, _lastSpeed.x);
		}
	}
}
