using UnityEngine;

namespace CFD
{
	[CreateAssetMenu(fileName = "New AnimationTriggerParameterData", menuName = BasePath + "Trigger")]
	public class AnimationTriggerParameterData : AAnimationParameterData
	{
		public override ParameterType Type => ParameterType.Trigger;

		public override void Apply(Animator animator)
		{
			animator.SetTrigger(ParameterHash);
		}

		public override void Apply(Animator animator, object value)
		{
			Apply(animator);
		}
	}
}
