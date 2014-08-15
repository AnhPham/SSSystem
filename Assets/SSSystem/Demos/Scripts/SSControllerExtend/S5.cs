using UnityEngine;
using System.Collections;

public class S5 : SSController 
{
    GameObject m_Capsule1;

	public override void Config ()
	{
		BgmType = Bgm.SAME;
		BgmName = string.Empty;

		IsCache = true;
	}

	public override void OnSet (object data)
	{
        if (m_Capsule1 == null)
        {
            m_Capsule1 = GameObject.Find("Capsule1");
        }

        m_Capsule1.SetActive(false);
	}

    public override void OnShow()
    {
        m_Capsule1.SetActive(true);
    }

    public override void OnHide()
    {
        m_Capsule1.SetActive(false);
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
