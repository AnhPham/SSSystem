/**
 * Created by Anh Pham on 2013/12/20
 * Email: anhpt.csit@gmail.com
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

[ExecuteInEditMode]
public class SSRoot : MonoBehaviour 
{
	public bool PreventLoadCallBack { get; set; }

    [SerializeField]
    protected Camera[] m_Cameras;

    [SerializeField]
    protected EventSystem m_EventSystem;

    [HideInInspector]
    public Dictionary<Camera, CameraClearFlags> AllCameraClearFlags = new Dictionary<Camera, CameraClearFlags>();

    public Camera[] cameras
    {
        get { return m_Cameras; }
    }

    public EventSystem eventSystem
    {
        get { return m_EventSystem; }
    }

    /*
    #if UNITY_EDITOR
    void OnValidate()
    {
        Rename();
        FindCameras();
        FindEventSystems();
    }
    #endif
    */

	protected virtual void Awake()
	{
        /*
        #if UNITY_EDITOR
        Rename();
        FindCameras();
        FindEventSystems();
        #endif
        */
        if (Application.isPlaying && !PreventLoadCallBack && SSSceneManager.Instance != null)
        {
            foreach (Camera cam in m_Cameras)
            {
                AllCameraClearFlags.Add(cam, cam.clearFlags);
                cam.clearFlags = CameraClearFlags.Depth;
            }

            SSApplication.OnLoaded(gameObject);
        }
	}

	protected virtual void Start()
	{
	}

    protected virtual void Update()
    {
        #if UNITY_EDITOR
        Rename();
        FindCameras();
        FindEventSystems();
        #endif
    }

    #if UNITY_EDITOR
    protected void Rename()
    {
        if (!Application.isPlaying)
        {
			gameObject.name = Path.GetFileNameWithoutExtension(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name);
        }
    }

    protected void FindCameras()
    {
        if (!Application.isPlaying)
        {
            m_Cameras = FindObjectsOfType<Camera>();
        }
    }

    protected void FindEventSystems()
    {
        if (!Application.isPlaying && m_EventSystem == null)
        {
            m_EventSystem = FindObjectOfType<EventSystem>();

            if (m_EventSystem != null && !(m_EventSystem is SSEventSystemAutoDestroy))
            {
                GameObject go = m_EventSystem.gameObject;

                SSEventSystemAutoDestroy esad = go.AddComponent<SSEventSystemAutoDestroy>();

                DestroyImmediate(m_EventSystem);
                m_EventSystem = esad;
            }
        }
    }
    #endif
}
