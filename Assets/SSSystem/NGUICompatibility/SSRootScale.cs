using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SSRootScale : SSRoot 
{
	#region Enum
	//アスペクト比
	public enum ASPECT{
		_1x2,		// 縦持ち 1:2
		_9x16,		// 縦持ち iPhone4inch (iPhone5)
		_2x3,		// 縦持ち iPhone3.5inch (iPhone4s以前)
		_3x4,		// 縦持ち iPad
		_1x1,		// 正方形
		_4x3,		// 横持ち iPad
		_3x2,		// 横持ち iPhone3.5inch (iPhone4s以前)
		_16x9,		// 横持ち iPhone4inch (iPhone5)
		_2x1,		// 横持ち 2:1
	};
	#endregion


	#region Serialize Field
	float camera2DOrthographicSize = 1;

	//ゲーム画面を画面中央ではなく、下にくっつける形にする。広告表示などのレイアウトに合わせるために
	[SerializeField]
	bool isAnchorBottom = false;

	[SerializeField]
	ASPECT defaultAspect = ASPECT._2x3;	//基本のアスペクト比

	[SerializeField]
	ASPECT wideAspect = ASPECT._2x3;	//最も横長になった場合のアスペクト比

	[SerializeField]
	ASPECT nallowAspect = ASPECT._9x16;	//最も縦長になった場合のアスペクト比

	[SerializeField]
	int defaultHeight = 960;
	#endregion


	#region Private
	Camera[] m_Cameras;
	GameObject[] m_Roots;

	bool isChanged;

	int currentHeight;
	int currentWidth;

	float currentAspectRatio;
	float screenAspectRatio;
	int prevDefaultHeight;

	Rect currentRect;
	#endregion
	

	#region Public
	public float GetRoot2DScale() { return 2.0f*this.camera2DOrthographicSize / currentHeight; }

	public int DefaultHeight{ get { return defaultHeight; } }

	public int DefaultWidth{ get { return (int)(defaultHeight*CalcAspectRatio(defaultAspect)); } }

	public int CurrentHeight{ get { return currentHeight; } }

	public int CurrentWidth{ get { return currentWidth; } }
	#endregion


	#region Private Function
	protected override void Start()
	{
		// Find all cameras in Scene
		m_Cameras = gameObject.GetComponentsInChildren<Camera> (true);

		// Find all UIRoot in Scene
		m_Roots = GetRoots ();

		// Refresh
		Refresh();

		// Update Scale
		UpdateScale();

		// Base Start
		base.Start ();
	}

	void Update()
	{
		if( !Mathf.Approximately( screenAspectRatio, 1.0f*Screen.width/Screen.height) )
		{
			Refresh();
		}
	}

	void LateUpdate()
	{
		UpdateScale ();
	}


	void Refresh()
	{
		screenAspectRatio = 1.0f*Screen.width/Screen.height;

		float defaultAspectRatio = CalcAspectRatio(defaultAspect);
		float wideAspectRatio = CalcAspectRatio(wideAspect);
		float nallowAspectRatio = CalcAspectRatio(nallowAspect);

		//スクリーンのアスペクト比から、ゲームのアスペクト比を決める
		if( screenAspectRatio > wideAspectRatio ){
			//アスペクト比が設定よりも横長
			currentAspectRatio = wideAspectRatio;
		}else if( screenAspectRatio < nallowAspectRatio ){
			//アスペクト比が設定よりも縦長
			currentAspectRatio = nallowAspectRatio;
		}else{
			//アスペクト比が設定の範囲内ならそのままスクリーンのアスペクト比を使う
			currentAspectRatio = screenAspectRatio;
		}

		//ゲームの画面サイズを決める
		if( currentAspectRatio < defaultAspectRatio ){
			//ゲームのアスペクト比が、デフォルトのアスペクト比よりも縦長
			currentHeight = (int)(defaultHeight*defaultAspectRatio/currentAspectRatio);
		}else{
			currentHeight = defaultHeight;
		}
		currentWidth = (int)(currentHeight*currentAspectRatio);

		float marginX = 0;
		float marginY = 0;
		//描画領域をクリップする
		if( currentAspectRatio != screenAspectRatio ){
			if( screenAspectRatio > currentAspectRatio ){
				//スクリーンのほうが横長なので、左右をクリップ.
				marginX = ( 1.0f - currentAspectRatio/screenAspectRatio )/2;
				marginY = 0;
			}
			else if( screenAspectRatio < currentAspectRatio ){
				//スクリーンのほうが縦長なので、上下をクリップ.
				marginX = 0;
				marginY = ( 1.0f - screenAspectRatio/currentAspectRatio )/2;
			}
		}

		if( isAnchorBottom ){
			currentRect = new Rect ( marginX, 0, 1- marginX*2, 1 - marginY*2 );
		}else{
			currentRect = new Rect ( marginX, marginY, 1- marginX*2, 1 - marginY*2 );
		}

		SetRects(currentRect);
	}

	void SetRects( Rect rect )
	{
		foreach( Camera camera2d in m_Cameras ){
			camera2d.rect = rect;
			camera2d.orthographicSize = this.camera2DOrthographicSize;
		}
	}

	float CalcAspectRatio( ASPECT aspect )
	{
		switch(aspect){
		case ASPECT._1x2:
			return 1.0f/2;
		case ASPECT._9x16:
			return 9.0f/16;
		case ASPECT._2x3:
			return 2.0f/3;
		case ASPECT._3x4:
			return 3.0f/4;
		case ASPECT._1x1:
			return 1;
		case ASPECT._4x3:
			return 4.0f/3;
		case ASPECT._3x2:
			return 3.0f/2;
		case ASPECT._16x9:
			return 16.0f/9;
		case ASPECT._2x1:
			return 2.0f;
		default:
			return 1;
		}
	}

	//カメラの更新
	void UpdateScale()
	{
		if (m_Roots == null) return;

		float scale = this.GetRoot2DScale();
		if (scale == Mathf.Infinity) return;

		foreach (GameObject r in m_Roots)
		{
			#if UNITY_EDITOR
			r.GetComponent ("UIRoot").GetType ().GetField ("manualHeight").SetValue (r.GetComponent ("UIRoot"), defaultHeight);
			#endif

			if (!Mathf.Approximately (r.transform.localScale.x, scale)) 
			{
				r.transform.localScale = new Vector3 (scale, scale, scale);
			}
		}
	}

	GameObject[] GetRoots()
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

