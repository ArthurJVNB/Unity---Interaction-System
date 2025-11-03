using System;
using CFD;
using UnityEngine;

namespace Project.InteractionSystem
{
	[CreateAssetMenu(fileName = "New InteractionData", menuName = "Project/Interaction System/Interaction Data")]
	public class InteractionData : ScriptableObject
	{
		[SerializeField] private Settings _start;
		[SerializeField] private Settings _loop;
		[SerializeField] private Settings _end;

		[Serializable]
		public struct Settings
		{
			public AAnimationData animationData;
			public AudioClip audioClip;
			public bool audioLoop;
			public ParticleSystem vfx;
			[Min(0)] public float time;
		}

		public Settings Start => _start;
		public Settings Loop => _loop;
		public Settings End => _end;
	}
}
