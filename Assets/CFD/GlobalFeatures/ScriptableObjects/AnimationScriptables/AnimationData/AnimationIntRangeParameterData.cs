using UnityEngine;

namespace CFD
{
	[CreateAssetMenu(fileName = "New AnimationIntRangeParameterData", menuName = BasePath + "Int/Int (Range)")]
	public class AnimationIntRangeParameterData : AAnimationIntParameterData
	{
		[SerializeField] private int _minValue;
		[SerializeField] private int _maxValue;

		public int MinValue => _minValue;
		public int MaxValue => _maxValue;
		public int RandomValue => Random.Range(_minValue, _maxValue + 1);

		public override void Apply(Animator animator)
		{
			Apply(animator, RandomValue);
		}
	}
}
