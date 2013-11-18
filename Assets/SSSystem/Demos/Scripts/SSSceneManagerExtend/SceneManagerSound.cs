using UnityEngine;
using System.Collections;

public class SceneManagerSound : SSSceneManager
{
	protected override void Awake ()
	{
		base.Awake ();

		m_SolidCamera.camera.tag = "MainCamera";
		m_SolidCamera.AddComponent<AudioListener> ();
		m_SolidCamera.AddComponent<AudioSource> ();
		m_SolidCamera.audio.loop = true;
	}
	protected override void OnLock (GameObject scene)
	{
		base.OnLock (scene);
	}

	protected override void OnUnlock (GameObject scene)
	{
		base.OnUnlock (scene);
	}

	protected override void PlayBGM (string bgmName)
	{
		AudioSource source = Camera.main.audio;
		AudioClip clip = source.clip;

		if (clip != null && clip.name == bgmName && source.isPlaying) return;

		source.clip = Resources.Load("Sounds/" + bgmName) as AudioClip;
		source.clip.name = bgmName;
		source.Play();
	}

	protected override void StopBGM ()
	{
		Camera.main.audio.Stop();
	}
}
