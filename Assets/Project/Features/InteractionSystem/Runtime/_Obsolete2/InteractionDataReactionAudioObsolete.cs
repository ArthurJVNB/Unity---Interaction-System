using UnityEngine;
using UnityEngine.Events;

namespace Project.InteractionSystem
{
	[System.Obsolete]
	public class InteractionDataReactionAudioObsolete : MonoBehaviour
	{
		[SerializeField] private InteractableObjectObsolete2 _object;
		[Space]
		[Tooltip("Optional")]
		[SerializeField] private AudioSource _audioSource;
		[Tooltip("Only triggered if there is an audio clip to play")]
		[SerializeField] private UnityEvent<AudioClip> _onPlayAudioClip;

		private void Reset()
		{
			_object = GetComponentInChildren<InteractableObjectObsolete2>();
			_audioSource = GetComponentInChildren<AudioSource>();
		}

		private void OnEnable()
		{
			_object.OnInteractType += OnInteractType;
		}

		private void OnDisable()
		{
			_object.OnInteractType -= OnInteractType;
		}

		private void OnInteractType(InteractionDataObsolete.Settings settings)
		{
			if (!settings.audioClip) return;

			if (_audioSource)
			{
				_audioSource.clip = settings.audioClip;
				_audioSource.loop = settings.audioLoop;
				_audioSource.Play();
			}

			_onPlayAudioClip?.Invoke(settings.audioClip);
		}
	}
}
