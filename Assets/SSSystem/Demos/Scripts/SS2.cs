using UnityEngine;
using System.Collections;

public class SS2 : SSController 
{
	public override void Awake ()
	{
		BgmType = Bgm.SAME;
		BgmName = string.Empty;

		IsCache = false;
	}

	public override void OnSet (object data)
	{
		Debug.Log("Set data : SS2");
	}
}
