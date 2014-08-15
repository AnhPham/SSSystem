using UnityEngine;
using System.Collections;

public class P1 : SSController 
{
    GameObject m_ButtonPopUp2;

	public override void Config ()
	{
		BgmType = Bgm.SAME;
		BgmName = string.Empty;

        IsCache = true;
	}

    public override void OnSet(object data)
    {
        if (m_ButtonPopUp2 == null)
        {
            m_ButtonPopUp2 = transform.Find("Root UI/Canvas/Panel/Button PopUp2").gameObject;
        }

        bool openFromP2 = (data != null && string.Compare((string)data, "From P2") == 0);
        m_ButtonPopUp2.SetActive(!openFromP2);
    }

    public override void OnSetTest()
    {
        OnSet("From P2");
    }

    public void OnButtonCloseTap()
    {
        SceneManagerDemo.Instance.Close();
    }

    public void OnButtonPopUp2Tap()
    {
        SceneManagerDemo.Instance.PopUp("P2", "From P1");
    }
}
