/**
 * Created by Anh Pham on 2013/12/20
 * Email: anhpt.csit@gmail.com
 */

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class SSRoot : MonoBehaviour 
{
    protected virtual void Start()
    {
        SetCameras ();
        SetEvent();
        SetController();
    }

	protected void SetCameras()
	{
		if (SSSceneManager.Instance == null) return;

		Camera[] cams = gameObject.GetComponentsInChildren<Camera> (true);
		foreach (Camera cam in cams) 
		{
			cam.clearFlags = CameraClearFlags.Depth;	
		}
	}
        
    protected void SetEvent()
    {
        if (SSSceneManager.Instance == null)
        {
            GameObject go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            go.AddComponent<TouchInputModule>();
        }
    }
        
    protected void SetController()
    {
        if (SSSceneManager.Instance != null)
        {
            SSApplication.OnLoaded(gameObject);
        }
        else
        {
            SSController ct = GetComponentInChildren<SSController>();
            if (ct != null)
            {
                ct.OnSetTest();
            }
        }
    }
}
