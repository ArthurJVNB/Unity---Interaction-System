using UnityEngine;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	public class GrabableObject : MonoBehaviour, IInteractable, IGrabable, ISocket
	{
		[SerializeField] private SocketData[] _socketDatas;
		[SerializeField] private GameObject _owner;
		[SerializeField] private bool _canInteract = true;
		[Min(0)]
		[SerializeField] private float _interactionDistance = .5f;
		[Tooltip("If true, it can be picked by another object. This will force the previous owner to \"drop\" this object.")]
		[SerializeField] private bool _canBeGrabbedEvenWithOwner;

		[field: Space]
		[field: SerializeField] public UnityEvent OnInteract { get; private set; }
		[field: SerializeField] public UnityEvent OnBeforeGrab { get; private set; }
		[field: SerializeField] public UnityEvent OnGrab { get; private set; }
		[field: SerializeField] public UnityEvent OnBeforeDrop { get; private set; }
		[field: SerializeField] public UnityEvent OnDrop { get; private set; }

		public SocketData SocketData => _socketDatas?[0];
		public bool IsInteractionEnabled { get => _canInteract; set => _canInteract = value; }

		public bool CanInteract(GameObject whoWantsToInteract)
		{
			if (!IsInteractionEnabled) return false;
			return Vector3.Distance(whoWantsToInteract.transform.position, transform.position) <= _interactionDistance;
		}

		public Vector3? GetInteractionPosition(GameObject whoWantsToInteract)
		{
			if (!IsInteractionEnabled) return null;
			return transform.position;
		}

		public bool Interact(GameObject whoIsInteracting)
		{
			if (!CanInteract(whoIsInteracting))
			{
				Debug.Log($"'{whoIsInteracting.name}' cannot interact '{name}'!", this);
				return false;
			}

			Debug.Log($"'{name}' interact");
			OnInteract?.Invoke();
			return GrabOrDrop(whoIsInteracting);
		}

		private bool GrabOrDrop(GameObject whoIsInteracting)
		{
			if (!whoIsInteracting) return false;

			if (_canBeGrabbedEvenWithOwner && _owner && _owner != whoIsInteracting)
				DropFromOwner();

			bool shouldGrab = _owner != whoIsInteracting;
			if (shouldGrab)
				return Grab(whoIsInteracting);
			else
			{
				Drop();
				return true;
			}
		}

		public void GrabEventCompatible(GameObject whoIsGrabbing)
		{
			Grab(whoIsGrabbing);
		}

		public bool Grab(GameObject whoIsGrabbing)
		{
			Debug.Log($"'{name}' grab");

			bool couldGrab = false;
			if (whoIsGrabbing.TryGetComponent(out SocketManager socketManager))
			{
				couldGrab = socketManager.AssignObject(gameObject, _socketDatas);
				//foreach (var socketData in _socketDatas)
				//{
				//	Debug.Log($"-- trying socket {(socketData ? $"'{socketData.Name}'" : "none")}");
				//	couldGrab = socketManager.AssignObject(gameObject, socketData);
				//	Debug.Log($"-- Could put '{name}' in socket {(socketData ? $"'{socketData.Name}'" : "none")}? {couldGrab}");
				//	if (couldGrab) break;
				//}
			}
			else
			{
				transform.SetParent(whoIsGrabbing.transform, true);
				couldGrab = true;
			}

			if (!couldGrab)
			{
				Debug.Log($"Could not grab '{name}'");
				return false;
			}

			OnBeforeGrab?.Invoke();
			_owner = whoIsGrabbing;
			OnGrab?.Invoke();
			return true;
		}

		public void Drop()
		{
			if (!_owner) return;

			OnBeforeDrop?.Invoke();

			if (_owner.TryGetComponent(out SocketManager socketManager))
				socketManager.DropObject(gameObject);
			else
				transform.SetParent(null, true);

			_owner = null;
			OnDrop?.Invoke();
		}

		private void DropFromOwner()
		{
			if (!_owner) return;
			if (_owner.TryGetComponent(out SocketManager socketManager))
			{
				socketManager.DropObject(gameObject);
				return;
			}
			transform.SetParent(null, true);
		}

	}

	#region Custom Editor (Backup)
	//#if UNITY_EDITOR
	//	[UnityEditor.CustomEditor(typeof(GrabableObject))]
	//	internal class GrabableObjectEditor : UnityEditor.Editor
	//	{
	//		private UnityEditorInternal.ReorderableList _socketList;
	//		private int _selectedSocketIndex = 0;

	//		private void OnEnable()
	//		{
	//			UnityEditor.SerializedProperty socketProp = serializedObject.FindProperty("_socketDatas");

	//			_socketList = new UnityEditorInternal.ReorderableList(serializedObject, socketProp, true, true, true, true)
	//			{
	//				drawHeaderCallback = rect =>
	//				{
	//					UnityEditor.EditorGUI.LabelField(rect, "Socket Type Priority (Top = Highest)");
	//				},

	//				drawElementCallback = (rect, index, isActive, isFocused) =>
	//				{
	//					var element = _socketList.serializedProperty.GetArrayElementAtIndex(index);
	//					var socketData = element.objectReferenceValue as SocketData;

	//					if (socketData != null)
	//					{
	//						// Gerar cor baseada no nome
	//						Color bgColor = Color.HSVToRGB(Mathf.Abs(socketData.Name.GetHashCode() % 100) / 100f, 0.3f, 1f);
	//						UnityEditor.EditorGUI.DrawRect(rect, bgColor);

	//						// Mostrar nome como label
	//						UnityEditor.EditorGUI.LabelField(rect, $"• {socketData.Name}");
	//					}
	//					else
	//					{
	//						UnityEditor.EditorGUI.LabelField(rect, "Missing SocketData reference");
	//					}
	//				}
	//			};

	//		}

	//		public override void OnInspectorGUI()
	//		{
	//			serializedObject.Update();
	//			_socketList?.DoLayoutList();

	//			AddSockets();
	//			CheckDuplicates();

	//			serializedObject.ApplyModifiedProperties();

	//		}

	//		private void CheckDuplicates()
	//		{
	//			// Verificar duplicatas
	//			var socketProp = serializedObject.FindProperty("_socketDatas");
	//			HashSet<SocketData> seen = new HashSet<SocketData>();
	//			List<SocketData> duplicates = new List<SocketData>();

	//			if (socketProp != null)
	//			{
	//				for (int i = 0; i < socketProp.arraySize; i++)
	//				{
	//					var element = socketProp.GetArrayElementAtIndex(i);
	//					var socketData = element.objectReferenceValue as SocketData;

	//					if (socketData != null)
	//					{
	//						if (!seen.Add(socketData))
	//						{
	//							duplicates.Add(socketData);
	//						}
	//					}
	//				}
	//			}

	//			if (duplicates.Count > 0)
	//			{
	//				UnityEditor.EditorGUILayout.HelpBox("Duplicate socket types detected! Each socket type should appear only once in the priority list.", UnityEditor.MessageType.Warning);
	//			}
	//		}

	//		private void AddSockets()
	//		{
	//			UnityEditor.EditorGUILayout.Space();
	//			UnityEditor.EditorGUILayout.LabelField("Add Socket Type", UnityEditor.EditorStyles.boldLabel);

	//			SocketData selectedSocket = null;
	//			string[] guids = UnityEditor.AssetDatabase.FindAssets("t:SocketData");
	//			List<SocketData> allSockets = guids
	//				.Select(g => AssetDatabase.LoadAssetAtPath<SocketData>(AssetDatabase.GUIDToAssetPath(g)))
	//				.Where(s => s != null)
	//				.ToList();

	//			string[] socketNames = allSockets.Select(s => s.Name).ToArray();

	//			if (socketNames.Length > 0)
	//			{
	//				_selectedSocketIndex = UnityEditor.EditorGUILayout.Popup("Socket", _selectedSocketIndex, socketNames);
	//				selectedSocket = allSockets[_selectedSocketIndex];

	//				if (GUILayout.Button("Add to Priority List"))
	//				{
	//					var socketProp = serializedObject.FindProperty("_socketDatas");

	//					bool alreadyExists = Enumerable.Range(0, socketProp.arraySize)
	//						.Select(i => socketProp.GetArrayElementAtIndex(i).objectReferenceValue)
	//						.Contains(selectedSocket);

	//					if (!alreadyExists)
	//					{
	//						socketProp.InsertArrayElementAtIndex(socketProp.arraySize);
	//						socketProp.GetArrayElementAtIndex(socketProp.arraySize - 1).objectReferenceValue = selectedSocket;
	//					}
	//					else
	//					{
	//						UnityEditor.EditorUtility.DisplayDialog("Duplicate Socket", "This socket is already in the list.", "OK");
	//					}
	//				}
	//			}
	//			else
	//			{
	//				EditorGUILayout.HelpBox("No SocketData assets found in project.", MessageType.Info);
	//			}
	//		}
	//	}
	//#endif
	#endregion
}
