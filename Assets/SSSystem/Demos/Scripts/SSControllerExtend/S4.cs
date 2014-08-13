using UnityEngine;
using System.Collections;

public class S4 : SSController 
{
    GameObject m_Sphere1;

	public override void Config ()
	{
		BgmType = Bgm.SAME;
		BgmName = string.Empty;

		IsCache = true;
	}

    public override void OnEnable()
    {
        SceneManagerDemo.Instance.HideMenu();
    }

    public override void OnSet (object data)
    {
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
