using UnityEngine;

namespace CFD
{
	[CreateAssetMenu(fileName = "New AnimationFloatParameterData", menuName = BasePath + "Float/Float")]
	public class AnimationFloatParameterData : AAnimationFloatParameterData
	{
		[SerializeField] private float _value;

		public float Value => _value;

		public override void Apply(Animator animator)
		{
			Apply(animator, _value);
		}
	}
}
