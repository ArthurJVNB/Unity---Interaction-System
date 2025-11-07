using UnityEngine;

namespace Project.InteractionSystem
{
	[CreateAssetMenu(fileName = "New SocketData", menuName = "Project/Interaction System/Socket")]
	public class SocketData : ScriptableObject
	{
		[SerializeField] private string _name = "Socket";
		
		public string Name { get => _name; set => _name = value; }
	}
}
