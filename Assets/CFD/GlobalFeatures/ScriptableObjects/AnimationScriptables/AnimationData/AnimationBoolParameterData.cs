using UnityEngine;

namespace CFD
{
	[CreateAssetMenu(fileName = "New AnimationBoolParameterData", menuName = BasePath + "Bool", order = 0)]
	public class AnimationBoolParameterData : AAnimationParameterData
	{
		[SerializeField] private bool _value;

		public override ParameterType Type => ParameterType.Bool;
		public bool Value => _value;

		public override void Apply(Animator animator)
		{
			Apply(animator, Value);
		}

		public override void Apply(Animator animator, object value)
		{
			if (value is not bool boolValue)
			{
				Debug.LogWarning($"<color=grey>{GetType().Name}:</color> Value is not bool.");
				Apply(animator);
				return;
			}

			Apply(animator, boolValue);
		}

		protected void Apply(Animator animator, bool value)
		{
			animator.SetBool(ParameterHash, value);
		}
	}
}
