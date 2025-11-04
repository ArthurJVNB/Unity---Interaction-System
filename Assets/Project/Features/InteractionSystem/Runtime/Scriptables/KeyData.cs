using UnityEngine;

namespace Project.InteractionSystem
{
	[CreateAssetMenu(fileName = "New KeyData", menuName = "Project/Interaction System/Key")]
	public class KeyData : ScriptableObject
	{
		[SerializeField] private string _name;

		public string Name { get => _name; set => _name = value; }
	}
}
