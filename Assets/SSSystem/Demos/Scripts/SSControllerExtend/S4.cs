using UnityEngine;
using System.Collections;

public class S4 : SSController 
{
    GameObject m_Sphere1;

	public override void Awake ()
	{
		BgmType = Bgm.SAME;
		BgmName = string.Empty;

		IsCache = true;
	}

    public override void OnSet (object data)
    {
        Debug.Log("Set data : S4");

        if (m_Sphere1 == null)
        {
            m_Sphere1 = GameObject.Find("Sphere1");
        }

        m_Sphere1.SetActive(false);
    }

    public override void OnShow()
    {
        m_Sphere1.SetActive(true);
    }

    public override void OnHide()
    {
        m_Sphere1.SetActive(false);
    }
}
