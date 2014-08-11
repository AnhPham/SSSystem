using UnityEngine;
using System.Collections;

public class P1 : SSController 
{
	private string m_Title;

	public override void Awake ()
	{
		BgmType = Bgm.SAME;
		BgmName = string.Empty;

		IsCache = false;
	}

	public override void OnSet (object data)
	{
		m_Title = (string)data;
	}

	public override void OnShow ()
	{
		Debug.Log("PopUp P1 Title: " + m_Title);
	}

	private void Update()
	{
		if (IsFocus && Input.GetMouseButtonDown(0))
		{
            Debug.Log("P1: Click " + Time.time);
		}
	}
}
