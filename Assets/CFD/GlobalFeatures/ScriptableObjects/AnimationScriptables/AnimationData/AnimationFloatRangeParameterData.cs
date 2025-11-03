using UnityEngine;

namespace CFD
{
	[CreateAssetMenu(fileName = "New AnimationFloatRangeParameterData", menuName = BasePath + "Float/Float (Range)")]
	public class AnimationFloatRangeParameterData : AAnimationFloatParameterData
	{
		[SerializeField] private float _minValue;
		[SerializeField] private float _maxValue;

		public float MinValue => _minValue;
		public float MaxValue => _maxValue;
		public float RandomValue => Random.Range(_minValue, _maxValue);

		public override void Apply(Animator animator)
		{
			Apply(animator, RandomValue);
		}
	}
}
