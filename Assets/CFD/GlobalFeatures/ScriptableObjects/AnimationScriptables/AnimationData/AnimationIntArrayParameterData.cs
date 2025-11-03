using UnityEngine;

namespace CFD
{
	[CreateAssetMenu(fileName = "New AnimationIntArrayParameterData", menuName = BasePath + "Int/Int (Array)")]
	public class AnimationIntArrayParameterData : AAnimationIntParameterData
	{
		[SerializeField] private int[] _values;

		public override ParameterType Type => ParameterType.Int;
		public int[] Values => _values;
		public int RandomValue => _values[Random.Range(0, _values.Length)];

		public override void Apply(Animator animator)
		{
			Apply(animator, RandomValue);
		}
	}
}
