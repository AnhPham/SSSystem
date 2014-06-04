using UnityEngine;
using System.Collections;

public class SS3 : SSController 
{
	public override void Awake ()
	{
		BgmType = Bgm.SAME;
		BgmName = string.Empty;

		IsCache = true;
	}

	public override void OnSet (object data)
	{
		Debug.Log("Set data : SS3");
	}
}
