using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;


[Serializable]
public class SoundManager : IManager
{
    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }

            _audioSources[(int)Define.Sound.Bgm].loop = true;
            
            // 초기 볼륨 설정
            SetVolume(Define.Sound.Bgm, PlayerPrefs.GetFloat("BGMVolume", 0.8f));
            SetVolume(Define.Sound.Effect, PlayerPrefs.GetFloat("SFXVolume", 0.5f));
        }
    }

    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }

    public void Play3D(string path, Vector3 position, float volume = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, Define.Sound.Effect);
 
        if (audioClip == null)
            return;

        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }

    public void Play(string path, float volume = 1.0f, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, volume, type, pitch);
    }

	public void Play(AudioClip audioClip, float volume = 1.0f, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
	{
        if (audioClip == null)
            return;

		if (type == Define.Sound.Bgm)
		{
			AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];
			if (audioSource.isPlaying)
				audioSource.Stop();

			audioSource.pitch = pitch; 
			audioSource.clip = audioClip;
			audioSource.volume = volume; 
			audioSource.Play();
		}
		else
		{
			AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
			audioSource.pitch = pitch;
			audioSource.PlayOneShot(audioClip, volume);
		}
	}

	AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
		if (path.Contains("Sound/") == false)
			path = $"Sound/{path}"; 

		AudioClip audioClip = null;

		if (type == Define.Sound.Bgm)
		{
			audioClip = Managers.Resource.Load<AudioClip>(path);
		}
		else
		{
			if (_audioClips.TryGetValue(path, out audioClip) == false)
			{
				audioClip = Managers.Resource.Load<AudioClip>(path);
				_audioClips.Add(path, audioClip);
			}
		}

		if (audioClip == null)
			Debug.Log($"AudioClip Missing ! {path}");

		return audioClip;
    }
    
    // 볼륨 조절 메서드
    public void SetVolume(Define.Sound type, float volume)
    {
        if (type >= Define.Sound.MaxCount)
        {
            Debug.Log($"잘못된 사운드 유형: {type}");
            return;
        }

        AudioSource audioSource = _audioSources[(int)type];
        audioSource.volume = Mathf.Clamp01(volume); // 0~1 사이로 제한
    }

    // 현재 볼륨 가져오기
    public float GetVolume(Define.Sound type)
    {
        if (type >= Define.Sound.MaxCount)
        {
            Debug.Log($"잘못된 사운드 유형: {type}");
            return 0f;
        }

        return _audioSources[(int)type].volume;
    }
} 