using UnityEngine;

namespace CFD
{
	[CreateAssetMenu(fileName = "New AnimationSequencerData", menuName = "Project/Animation Parameter/Animation Sequencer/Animation Sequencer")]
	public class AnimationSequencerData : AAnimationData
	{
		[SerializeField] private AAnimationParameterData[] _sequence;

		public AAnimationParameterData[] Sequence => _sequence;

		public override void Apply(Animator animator)
		{
			foreach (var parameter in _sequence)
				parameter.Apply(animator);
		}
	}
}
