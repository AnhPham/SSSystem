using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[ExecuteInEditMode]
public class SSRootScale : SSRoot
{
	#region Enum
	public enum SSAnchor
	{
		TOP,
		CENTER,
		BOTTOM
	}

	public enum ASPECT
	{
		_1x2,
		_9x16,
		_2x3,
		_3x4,
		_1x1,
		_4x3,
		_3x2,
		_16x9,
		_2x1,
	};
	#endregion

	#region Serialize Field
	[SerializeField]
	SSAnchor m_Anchor = SSAnchor.CENTER;

	[SerializeField]
	ASPECT defaultAspect = ASPECT._9x16;

	[SerializeField]
	ASPECT wideAspect = ASPECT._2x3;

	[SerializeField]
	ASPECT nallowAspect = ASPECT._9x16;

	[SerializeField]
	int defaultHeight = 1136;

    [SerializeField]
    Camera[] m_FullScreenCameras;

    [SerializeField]
    GameObject[] m_FullScreenPanels;

    [SerializeField]
    GameObject[] m_Roots;
	#endregion

	#region Private	
	Rect currentRect;

	bool isChanged;

	int currentHeight;
	int prevDefaultHeight;

	float currentAspectRatio;
	float screenAspectRatio;
	float camera2DOrthographicSize = 1;
	#endregion

	#region Public Function
	public float VisibleRealWidth()
	{
		RefreshCurrentAspectRatio ();

		return Screen.height * currentAspectRatio;
	}

	public float VisibleLogicWidth()
	{
		float defaultAspectRatio = CalcAspectRatio (defaultAspect);
		float w = defaultHeight * defaultAspectRatio;

		return w;
	}

	public float FullRealWidth()
	{
		return Screen.width;
	}

	public float FullLogicWidth()
	{
		float h = LogicHeight ();
		float w = h * Screen.width / Screen.height;

		return w;
	}
		
	public float LogicHeight()
	{
		float w = VisibleLogicWidth ();

		RefreshCurrentAspectRatio ();

		float h = w / currentAspectRatio;
		return h;
	}

	public float FullRealHeight()
	{
		return Screen.height;
	}
	#endregion
	
	#region Private Function
	protected override void Start()
	{
        /*
		// Find all cameras in Scene
		m_FullScreenCameras = gameObject.GetComponentsInChildren<Camera> (true);

		// Find all UIRoot in Scene
		m_Roots = GetRoots ();

		// Find all UIPanel in Scene
		m_FullScreenPanels = GetPanels ();
        */

		// Trick Root
		DisableUIRoot ();

		// RefreshCurrentAspectRatio
		RefreshCurrentAspectRatio ();

		// Refresh
		Refresh ();

		// Update Scale
		UpdateScale ();

		// Base Start
		base.Start ();
	}

	private void DisableUIRoot()
	{
		foreach (GameObject root in m_Roots)
		{
			MonoBehaviour uiroot = root.GetComponent ("UIRoot") as MonoBehaviour;
			if (uiroot != null)
			{
				uiroot.enabled = false;
				uiroot.enabled = true;
			}
		}
	}

	private void Update()
	{
		if (!Mathf.Approximately (screenAspectRatio, 1.0f * Screen.width / Screen.height))
		{
			UpdateScale ();
		}
	}

	private void UpdateScale()
	{
		if (GetRoot2DScale () == Mathf.Infinity)
			return;

		RefreshCurrentAspectRatio ();
		Refresh ();

		SetRootHeight ();
		SetPanelSize ();
		SetCameraRect (currentRect);
	}

	private void RefreshCurrentAspectRatio()
	{
		screenAspectRatio = 1.0f * Screen.width / Screen.height;

		float wideAspectRatio = CalcAspectRatio (wideAspect);
		float nallowAspectRatio = CalcAspectRatio (nallowAspect);

		if (screenAspectRatio > wideAspectRatio)
		{
			currentAspectRatio = wideAspectRatio;
		}
		else if (screenAspectRatio < nallowAspectRatio)
		{
			currentAspectRatio = nallowAspectRatio;
		}
		else
		{
			currentAspectRatio = screenAspectRatio;
		}

		float defaultAspectRatio = CalcAspectRatio (defaultAspect);

		currentHeight = (int)(defaultHeight * defaultAspectRatio / currentAspectRatio);
	}

	private void Refresh()
	{
		float marginX = 0;
		float marginY = 0;

		if (currentAspectRatio != screenAspectRatio)
		{
			if (screenAspectRatio > currentAspectRatio)
			{
				marginX = (1.0f - currentAspectRatio / screenAspectRatio) / 2;
				marginY = 0;
			}
			else if (screenAspectRatio < currentAspectRatio)
			{
				marginX = 0;
				marginY = (1.0f - screenAspectRatio / currentAspectRatio) / 2;
			}
		}

		switch (m_Anchor)
		{
			case SSAnchor.BOTTOM:
				currentRect = new Rect (marginX, 0, 1 - marginX * 2, 1 - marginY * 2);
				break;
			case SSAnchor.TOP:
				currentRect = new Rect (marginX, marginY * 2, 1 - marginX * 2, 1 - marginY * 2);
				break;
			case SSAnchor.CENTER:
				currentRect = new Rect (marginX, marginY, 1 - marginX * 2, 1 - marginY * 2);
				break;
		}
	}

	private void SetCameraRect(Rect rect)
	{
		if (m_FullScreenCameras == null)
			return;

		foreach (Camera cam in m_FullScreenCameras)
		{
            cam.rect = rect;
            cam.orthographicSize = this.camera2DOrthographicSize;
		}
	}

	private void SetRootHeight()
	{
		if (m_Roots == null)
			return;

		foreach (GameObject r in m_Roots)
		{
            MonoBehaviour uiroot = r.GetComponent ("UIRoot") as MonoBehaviour;

            if (uiroot != null)
            {
                uiroot.GetType().GetField("scalingStyle").SetValue(uiroot, 1);
                uiroot.GetType().GetField("manualHeight").SetValue(uiroot, currentHeight);
            }
		}
	}

	private void SetPanelSize()
	{
		if (m_FullScreenPanels == null)
			return;

		foreach (GameObject p in m_FullScreenPanels)
		{
            MonoBehaviour uipanel = p.GetComponent ("UIPanel") as MonoBehaviour;

            if (uipanel != null) 
            {
                float w = defaultHeight * CalcAspectRatio(defaultAspect);
                float width = w + 2;
                float height = w / currentAspectRatio;

                uipanel.GetType ().InvokeMember ("clipping", BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, System.Type.DefaultBinder, uipanel, new object[]{ 3 });
                uipanel.GetType ().InvokeMember ("clipSoftness", BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, System.Type.DefaultBinder, uipanel, new object[]{ new Vector2(0, 0) });
                uipanel.GetType ().InvokeMember ("baseClipRegion", BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, System.Type.DefaultBinder, uipanel, new object[]{ new Vector4 (0, 0, width, height) });
            }
		}
	}

	private float GetRoot2DScale()
	{
		return 2.0f * this.camera2DOrthographicSize / currentHeight;
	}

	private GameObject[] GetRoots()
	{
		return GetObjectsByString ("UIRoot");
	}

	private GameObject[] GetPanels()
	{
		return GetObjectsByString ("UIPanel");
	}

	private GameObject[] GetObjectsByString(string className)
	{
		List<GameObject> objs = new List<GameObject> ();

		Transform[] trans = GetComponentsInChildren<Transform> (true);
		foreach (Transform t in trans)
		{
			Component c = t.GetComponent (className);
			if (c != null)
			{
				objs.Add (c.gameObject);
			}
		}

		return objs.ToArray ();
	}

	private float CalcAspectRatio(ASPECT aspect)
	{
		switch (aspect)
		{
		case ASPECT._1x2:
			return 1.0f / 2;
		case ASPECT._9x16:
			return 640f / 1136;
		case ASPECT._2x3:
			return 2f / 3;
		case ASPECT._3x4:
			return 3.0f / 4;
		case ASPECT._1x1:
			return 1;
		case ASPECT._4x3:
			return 4.0f / 3;
		case ASPECT._3x2:
			return 3.0f / 2;
		case ASPECT._16x9:
			return 1136.0f / 640;
		case ASPECT._2x1:
			return 2.0f;
		default:
			return 1;
		}
	}
	#endregion
}

