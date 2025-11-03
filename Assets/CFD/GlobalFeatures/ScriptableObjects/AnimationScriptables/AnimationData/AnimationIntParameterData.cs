using UnityEngine;

namespace CFD
{
	[CreateAssetMenu(fileName = "New AnimationIntParameterData", menuName = BasePath + "Int/Int")]
	public class AnimationIntParameterData : AAnimationIntParameterData
	{
		[SerializeField] private int _value;

		public int Value => _value;

		public override void Apply(Animator animator)
		{
			animator.SetInteger(ParameterHash, Value);
		}
	}
}
