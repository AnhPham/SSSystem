using UnityEngine;
using System.Collections;

public class S3 : SSController 
{
    GameObject m_Cube1;

	public override void Awake ()
	{
		BgmType = Bgm.SAME;
		BgmName = string.Empty;

		IsCache = true;
	}

	public override void OnSet (object data)
	{
        if (m_Cube1 == null)
        {
            m_Cube1 = GameObject.Find("Cube1");
        }

        m_Cube1.SetActive(false);
	}

    public override void OnFocus(bool isFocus)
    {
        base.OnFocus(isFocus);

        m_Cube1.SetActive(isFocus);
    }

    public void OnButtonPopUp1Tap()
    {
        SceneManagerDemo.Instance.PopUp("P1");
    }

    public void OnButtonPopUp2Tap()
    {
        SceneManagerDemo.Instance.PopUp("P2");
    }

    public void OnButtonCloseTap()
    {
        SceneManagerDemo.Instance.Close();
    }
}
