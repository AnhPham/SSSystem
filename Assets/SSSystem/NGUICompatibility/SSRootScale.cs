using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	#endregion

	#region Private
	Camera[] m_Cameras;
	GameObject[] m_Roots;
	Rect currentRect;

	bool isChanged;

	int currentHeight;
	int prevDefaultHeight;

	float currentAspectRatio;
	float screenAspectRatio;
	float camera2DOrthographicSize = 1;
	#endregion
	
	#region Private Function
	protected override void Start()
	{
		// Find all cameras in Scene
		m_Cameras = gameObject.GetComponentsInChildren<Camera> (true);

		// Find all UIRoot in Scene
		m_Roots = GetRoots ();

		// Trick Root
		DisableUIRoot ();

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
			}
		}
	}

	private void Update()
	{
		if (!Mathf.Approximately (screenAspectRatio, 1.0f * Screen.width / Screen.height))
		{
			Refresh ();
		}
		UpdateScale ();
	}

	private void Refresh()
	{
		screenAspectRatio = 1.0f * Screen.width / Screen.height;

		float defaultAspectRatio = CalcAspectRatio (defaultAspect);
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

		currentHeight = (int)(defaultHeight * defaultAspectRatio / currentAspectRatio);

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

		SetRects (currentRect);
	}

	private void SetRects(Rect rect)
	{
		foreach (Camera camera2d in m_Cameras)
		{
			camera2d.rect = rect;
			camera2d.orthographicSize = this.camera2DOrthographicSize;
		}
	}

	private float CalcAspectRatio(ASPECT aspect)
	{
		switch (aspect)
		{
			case ASPECT._1x2:
				return 1.0f / 2;
			case ASPECT._9x16:
				return 9.0f / 16;
			case ASPECT._2x3:
				return 2.0f / 3;
			case ASPECT._3x4:
				return 3.0f / 4;
			case ASPECT._1x1:
				return 1;
			case ASPECT._4x3:
				return 4.0f / 3;
			case ASPECT._3x2:
				return 3.0f / 2;
			case ASPECT._16x9:
				return 16.0f / 9;
			case ASPECT._2x1:
				return 2.0f;
			default:
				return 1;
		}
	}

	private void UpdateScale()
	{
		if (m_Roots == null)
			return;

		float scale = this.GetRoot2DScale ();
		if (scale == Mathf.Infinity)
			return;

		foreach (GameObject r in m_Roots)
		{
			#if UNITY_EDITOR
			r.GetComponent ("UIRoot").GetType ().GetField ("scalingStyle").SetValue (r.GetComponent ("UIRoot"), 1);
			r.GetComponent ("UIRoot").GetType ().GetField ("manualHeight").SetValue (r.GetComponent ("UIRoot"), defaultHeight);
			#endif

			if (!Mathf.Approximately (r.transform.localScale.x, scale))
			{
				r.transform.localScale = new Vector3 (scale, scale, scale);
			}
		}
	}

	private float GetRoot2DScale()
	{
		return 2.0f * this.camera2DOrthographicSize / currentHeight;
	}

	private GameObject[] GetRoots()
	{
		List<GameObject> roots = new List<GameObject> ();

		Transform[] trans = GetComponentsInChildren<Transform> (true);
		foreach (Transform t in trans)
		{
			Component c = t.GetComponent ("UIRoot");
			if (c != null)
			{
				roots.Add (c.gameObject);
			}
		}

		return roots.ToArray ();
	}
	#endregion
}

