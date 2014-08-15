using UnityEngine;
using System.Collections;

public class P2 : SSController 
{
    GameObject m_ButtonPopUp1;

	public override void Config ()
	{
        BgmType = Bgm.SAME;
		BgmName = string.Empty;

        IsCache = false;
	}

    public override void OnSet(object data)
    {
        if (m_ButtonPopUp1 == null)
        {
            m_ButtonPopUp1 = transform.Find("Root UI/Canvas/Panel/Button PopUp1").gameObject;
        }

        bool openFromP2 = (data != null && string.Compare((string)data, "From P1") == 0);
        m_ButtonPopUp1.SetActive(!openFromP2);
    }

    public override void OnSetTest()
    {
        OnSet("From P1");
    }

    public void OnButtonCloseTap()
    {
        SceneManagerDemo.Instance.Close();
    }

    public void OnButtonPopUp1Tap()
    {
        SceneManagerDemo.Instance.PopUp("P1", "From P2");
    }
}
