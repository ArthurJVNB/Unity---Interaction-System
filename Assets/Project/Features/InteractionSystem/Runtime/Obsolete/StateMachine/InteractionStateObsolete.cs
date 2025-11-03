using System;
using UnityEngine;
using UnityHFSM;

namespace Project.InteractionSystem
{
	[Obsolete("Please use InteractionState")]
	[Serializable]
	public class InteractionStateObsolete : State
	{
		public InteractionData.Settings data;
		public bool wantsToExit;
		public InteractionStateType type;

		public InteractionStateObsolete(string name, InteractionStateType type) : this(name, type, default) { }
		public InteractionStateObsolete(string name, InteractionStateType type, InteractionData.Settings data) : base(needsExitTime: data.time > 0)
		{
			this.name = name;
			this.data = data;
			this.type = type;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			wantsToExit = false;
			Debug.Log($"entering {name}");
		}

		public override void OnLogic()
		{
			base.OnLogic();
			if (needsExitTime)
			{
				if (timer.Elapsed <= data.time) return;
				wantsToExit = true;
			}
		}

		public override void OnExit()
		{
			base.OnExit();
			Debug.Log($"exiting {name}");
		}

		public override void OnExitRequest()
		{
			base.OnExitRequest();
			if (wantsToExit)
				fsm.StateCanExit();
		}
	}
}
