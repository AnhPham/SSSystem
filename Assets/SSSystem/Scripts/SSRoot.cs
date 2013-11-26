/**
 * Created by Anh Pham (anhpt.csit@gmail.com) on 2013/11/13
 */

using UnityEngine;
using System.Collections;

public class SSRoot : MonoBehaviour 
{
	protected virtual void Start()
	{
		CameraSetting ();
	}

	/// <summary>
	/// Cameras the setting. Set all cameras clearFlags to Depth. For avoid flicker when load to Base scene.
	/// </summary>
	protected void CameraSetting()
	{
		if (SSSceneManager.Instance == null) return;

		Camera[] cams = gameObject.GetComponentsInChildren<Camera> (true);
		foreach (Camera cam in cams) 
		{
			cam.clearFlags = CameraClearFlags.Depth;	
		}
	}
}
