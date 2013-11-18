using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManagerTutorial : SSSceneManager 
{
	#region Singleton
	public static SceneManagerTutorial Inst
	{
		get { return (SceneManagerTutorial)m_Instance; }
	}
	#endregion

	private bool m_IsTutorialOn;

	#region Public
	public void TutorialOn()
	{
		if (m_IsTutorialOn)
			return;

		m_IsTutorialOn = true;

		AddTutCameras (m_Scenes);
	}

	public void TutorialOff()
	{
		if (!m_IsTutorialOn)
			return;

		m_IsTutorialOn = false;

		RemoveTutCameras (m_Scenes);
	}
	#endregion


	#region Protected override
	protected override void OnSceneLoad (GameObject scene)
	{
		if (m_IsTutorialOn) 
		{
			AddTutCameras (scene);
		}
	}

	protected override void OnSceneUnload (GameObject scene)
	{
	}
	#endregion


	#region Private
	private void AddTutCameras(GameObject obj)
	{
		Camera[] cams = obj.GetComponentsInChildren<Camera> (true);
		foreach (Camera cam in cams) 
		{
			if (cam.cullingMask != (1 << LayerMask.NameToLayer ("Tutorial"))) 
			{
				Camera tutCam = ((GameObject)Instantiate (cam.gameObject)).GetComponent<Camera> ();
				tutCam.transform.parent = cam.transform.parent;
				tutCam.cullingMask = (1 << LayerMask.NameToLayer ("Tutorial"));
				tutCam.depth += 1000;
			}
		}
	}

	private void RemoveTutCameras(GameObject obj)
	{
		Camera[] cams = obj.GetComponentsInChildren<Camera> (true);
		foreach (Camera cam in cams) 
		{
			if (cam.cullingMask == (1 << LayerMask.NameToLayer ("Tutorial")))
			{
				Destroy (cam.gameObject);
			}
		}
	}
	#endregion
}
