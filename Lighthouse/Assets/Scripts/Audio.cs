using UnityEngine;
using System.Collections;

public class Audio : MonoBehaviour
{
	#region Variables

	public enum BuildInSound
	{
		Crash,
		Bell,

		Count,
		None
	}

	[System.Serializable]
	public struct BuildInSoundInfo
	{
		public BuildInSound buildInSound;
		public AudioClip[] sounds;
	}

	[SerializeField]
	private BuildInSoundInfo[] _buildInSoundInfos = null;
	[HideInInspector]
	[SerializeField]
	private int _buildInSoundCount = 0;

	private static Audio _instance = null;
	public static Audio Instance { get { return _instance; } }

	private AudioSource[] _sources = null;
	private const int sourceCount = 16;

	private int _nextSourceToUse = 0;

	#endregion Variables

	#region Monobehaviour Methods

#if UNITY_EDITOR

	void OnValidate()
	{
		ValidateAudio();
	}
	void OnDrawGizmos()
	{
		ValidateAudio();
	}

#endif


	void Awake()
	{
		InitializeAudio();
    }

#endregion Monobehaviour Methods

#region Methods

	void ValidateAudio()
	{
		BuildInSoundInfo[] oldBuildInSoundInfos = _buildInSoundInfos;
		int oldBuildInSoundCount = oldBuildInSoundInfos != null ? oldBuildInSoundInfos.Length : 0;
		_buildInSoundCount = (int)BuildInSound.Count;
		if(_buildInSoundCount != oldBuildInSoundCount)
		{
			_buildInSoundInfos = new BuildInSoundInfo[_buildInSoundCount];
		}

		for(int i = 0;i < _buildInSoundCount;++i)
		{
			if (i < oldBuildInSoundCount)
			{
				_buildInSoundInfos[i] = oldBuildInSoundInfos[i];
			}
			_buildInSoundInfos[i].buildInSound = (BuildInSound)i;
		}
	}

	private void InitializeAudio()
	{
		_instance = this;

		_sources = new AudioSource[sourceCount];
		for(int i = 0;i < sourceCount;++i)
		{
			AudioSource createdSource = this.gameObject.AddComponent<AudioSource>();
			_sources[i] = createdSource;
		}
	}

	private void StopAllSound()
	{
		if (_sources != null)
		{
			for (int i = 0; i < sourceCount; ++i)
			{
				if (_sources[i] != null)
				{
					_sources[i].Stop();
				}
			}
		}
	}

	public void PlaySound(AudioClip[] clips,float volume = 1.0f)
	{
		if (GameLord.Instance.CurrentPlayerData.audio)
		{
			if (clips != null)
			{
				int clipCount = clips.Length;

				int randomClipIndex = UnityEngine.Random.Range(0, clipCount);

				AudioClip clipToUse = clips[randomClipIndex];
				if (clipToUse != null)
				{
					AudioSource sourceToUse = _sources[_nextSourceToUse];
					_nextSourceToUse = (_nextSourceToUse + 1) % sourceCount;

					if (clipToUse != null && sourceToUse != null)
					{
						sourceToUse.clip = clipToUse;
						sourceToUse.volume = volume;

						sourceToUse.Play();
					}
				}
			}
		}
	}

	public void PlayBuildInSound(BuildInSound buildInSound, float volume = 1.0f)
	{
		int index = (int)buildInSound;
		if (index >= 0 && index < _buildInSoundCount)
		{
			PlaySound(_buildInSoundInfos[index].sounds, volume);
		}
	}

#endregion Methods
}
