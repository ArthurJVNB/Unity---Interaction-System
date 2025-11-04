using UnityEngine;

namespace Project.InteractionSystem
{
	[CreateAssetMenu(fileName = "New SocketData", menuName = "Project/Interaction System/SocketData")]
	public class SocketData : ScriptableObject
	{
		[SerializeField] private string _name;
		
		public string Name { get => _name; set => _name = value; }
	}
}
