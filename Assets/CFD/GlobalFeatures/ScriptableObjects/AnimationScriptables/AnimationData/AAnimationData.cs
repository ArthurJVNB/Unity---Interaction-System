using UnityEngine;

namespace CFD
{
	public abstract class AAnimationData : ScriptableObject
	{
		public abstract void Apply(Animator animator);
	}
}
