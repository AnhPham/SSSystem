using UnityEngine;
using System.Collections;

public class P2 : SSController 
{
	public delegate void LeftMouseClickDelegate();

	public LeftMouseClickDelegate LeftMouseClick;
	
	private string m_Title;

	public override void Awake ()
	{
		BgmType = Bgm.NONE;
		BgmName = string.Empty;

		IsCache = false;
	}

	public override void OnSet (object data)
	{
		m_Title = (string)data;
	}

	public override void OnShow ()
	{
		Debug.Log("PopUp P2 Title: " + m_Title);
	}

	private void Update()
	{
        if (IsFocus && Input.GetMouseButtonDown(0))
		{
			if (LeftMouseClick != null)
			{
				LeftMouseClick();
			}
		}
	}
}
