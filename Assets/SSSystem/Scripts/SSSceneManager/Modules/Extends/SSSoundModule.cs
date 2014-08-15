using UnityEngine;
using System.Collections;

public enum Bgm
{
    /// <summary>
    /// When the scene changed, turn off BGM.
    /// </summary>
    NONE,

    /// <summary>
    /// When the scene changed, BGM will not be changed.
    /// </summary>
    SAME,

    /// <summary>
    /// When the scene changed, play a new BGM.
    /// </summary>
    PLAY,

    /// <summary>
    /// When the scene changed, turn off BGM.
    /// You will play BGM by your own code.
    /// You must to set the BgmName for SSController.
    /// </summary>
    CUSTOM
}

public class SSSoundModule : SSModule
{
    protected string m_GlobalBgm = string.Empty;

    public void BgmWhenOpenScene(string curBgm, SSController ctrl)
    {
        if (!string.IsNullOrEmpty (m_GlobalBgm))
        {
            PlayBGM (m_GlobalBgm);
        }
        else
        {
            switch (ctrl.BgmType)
            {
                case Bgm.NONE:
                    StopBGM ();
                    break;

                case Bgm.PLAY:
                    ctrl.CurrentBgm = ctrl.BgmName;
                    if (!string.IsNullOrEmpty (ctrl.BgmName))
                    {
                        PlayBGM (ctrl.BgmName);
                    }
                    break;

                case Bgm.SAME:
                    ctrl.CurrentBgm = curBgm;
                    if (!string.IsNullOrEmpty(curBgm))
                    {
                        PlayBGM(curBgm);
                    }
                    break;
                case Bgm.CUSTOM:
                    StopBGM ();
                    ctrl.CurrentBgm = ctrl.BgmName;
                    break;
            }
        }
    }

    public void BgmWhenCloseOtherScene(SSController ctrl)
    {
        if (!string.IsNullOrEmpty (m_GlobalBgm))
        {
            // Do nothing
        }
        else
        {
            switch (ctrl.BgmType)
            {
                case Bgm.NONE:
                    StopBGM ();
                    break;

                case Bgm.PLAY:
                case Bgm.SAME:
                case Bgm.CUSTOM:
                    if (!string.IsNullOrEmpty (ctrl.CurrentBgm))
                    {
                        PlayBGM (ctrl.CurrentBgm);
                    }
                    break;
            }
        }
    }

    public void SetGlobalBgm(string bgmName)
    {
        m_GlobalBgm = bgmName;
    }
        
    public void ClearGlobalBgm()
    {
        m_GlobalBgm = string.Empty;
    }
}
