using UnityEngine;

namespace CFD
{
	public abstract class AAnimationIntParameterData : AAnimationParameterData
	{
		public override ParameterType Type => ParameterType.Int;

		public override void Apply(Animator animator, object value)
		{
			if (value is not int intValue)
			{
				Debug.LogWarning($"<color=grey>{GetType().Name}</color>: Value is not an int");
				Apply(animator);
				return;
			}

			Apply(animator, intValue);
		}

		protected void Apply(Animator animator, int value)
		{
			animator.SetInteger(ParameterHash, value);
		}
	}
}
