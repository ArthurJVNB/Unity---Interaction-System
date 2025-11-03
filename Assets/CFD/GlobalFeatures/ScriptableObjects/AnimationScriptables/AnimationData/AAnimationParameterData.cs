using UnityEngine;

namespace CFD
{
	public abstract class AAnimationParameterData : AAnimationData
	{
		protected const string BasePath = "Project/Animation Parameter/";

		[SerializeField] private string _parameterName;

		private int _parameterHash;

		public string ParameterName => _parameterName;
		public int ParameterHash
		{
			get
			{
#if UNITY_EDITOR
				ValidateHash();
#endif
				return _parameterHash;
			}
		}

		public abstract ParameterType Type { get; }

		public enum ParameterType
		{
			Float,
			Int,
			Bool,
			Trigger,
		}

		private void Awake()
		{
			ValidateHash();
		}

		private void ValidateHash()
		{
			if (_parameterHash == default)
				_parameterHash = Animator.StringToHash(_parameterName);
		}

		public abstract void Apply(Animator animator, object value);
	}
}
