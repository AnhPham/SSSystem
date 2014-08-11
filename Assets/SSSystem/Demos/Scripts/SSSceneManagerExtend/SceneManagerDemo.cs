using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManagerDemo : SSSceneManager
{
	[SerializeField]
	AudioClip[] m_Clips;

	Dictionary<string, AudioClip> m_Audios;

	protected override void Awake ()
	{
		base.Awake ();

		m_SolidCamera.camera.tag = "MainCamera";
		m_SolidCamera.AddComponent<AudioListener> ();
		m_SolidCamera.AddComponent<AudioSource> ();
		m_SolidCamera.audio.loop = true;

		m_Audios = new Dictionary<string, AudioClip> ();
		m_Audios.Add ("S1", m_Clips[0]);
		m_Audios.Add ("S2", m_Clips[1]);
	}

	protected override void PlayBGM (string bgmName)
	{
		AudioSource source = Camera.main.audio;
		AudioClip clip = source.clip;

		if (clip != null && clip.name == bgmName && source.isPlaying) return;

		source.clip = m_Audios [bgmName];
		source.clip.name = bgmName;
		source.Play();
	}

	protected override void StopBGM ()
	{
		Camera.main.audio.Stop();
	}

    protected override void OnAnimationFinish(string sceneName)
    {
        // If you have the problem with NGUI which display not correctly when animation finish,
        // You can SetDirty() to all UIPanel in UIPanel.list in this event for a refresh.
    }
}
