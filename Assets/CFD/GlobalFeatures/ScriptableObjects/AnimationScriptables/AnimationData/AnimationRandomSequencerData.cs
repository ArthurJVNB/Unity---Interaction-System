using UnityEngine;

namespace CFD
{
	[CreateAssetMenu(fileName = "New AnimationRandomSequencerData", menuName = "Project/Animation Parameter/Animation Sequencer/Animation Sequencer (Random)")]
	public class AnimationRandomSequencerData : AAnimationData
	{
		[SerializeField] private AnimationSequencerData[] _sequencers;

		public AnimationSequencerData[] Sequencers => _sequencers;
		private AnimationSequencerData RandomSequencer => _sequencers[Random.Range(0, _sequencers.Length)];

		public override void Apply(Animator animator)
		{
			RandomSequencer.Apply(animator);
		}
	}
}
