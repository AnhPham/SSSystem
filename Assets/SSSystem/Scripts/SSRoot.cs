using UnityEngine;
using System.Collections;

public class SSRoot : MonoBehaviour 
{
	private void Start()
	{
		CameraSetting ();
	}

	/// <summary>
	/// Cameras the setting. Set all cameras clearFlags to Depth. For avoid flicker when load to Base scene.
	/// </summary>
	private void CameraSetting()
	{
		Camera[] cams = gameObject.GetComponentsInChildren<Camera> (true);
		foreach (Camera cam in cams) 
		{
			cam.clearFlags = CameraClearFlags.Depth;	
		}
	}
}
