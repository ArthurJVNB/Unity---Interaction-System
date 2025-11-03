using UnityEngine;

namespace CFD
{
	public abstract class AAnimationFloatParameterData : AAnimationParameterData
	{
		public override ParameterType Type => ParameterType.Float;

		public override void Apply(Animator animator, object value)
		{
			if (value is not float floatValue)
			{
				Debug.LogWarning($"<color=grey>{GetType().Name}:</color> Value is not float.");
				Apply(animator);
				return;
			}

			Apply(animator, floatValue);
		}

		public virtual void Apply(Animator animator, float value)
		{
			animator.SetFloat(ParameterHash, value);
		}
	}
}
