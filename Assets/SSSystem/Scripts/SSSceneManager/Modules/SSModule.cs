using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using DictObject = System.Collections.Generic.Dictionary<string, UnityEngine.GameObject>;

public class SSModule
{
    public static List<SSModule> list = new List<SSModule>();

    #region Delegate Define
    public delegate void OpenDelegate(string sceneName, int ip, float ic, object data, bool imme, string curBgm, NoParamCallback onAnimEnded = null, NoParamCallback onLoaded = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null);
    public delegate void CloseDelegate(string sceneName, bool imme, NoParamCallback onAnimEnded = null);

    public delegate void PlayBGMDelegate(string bgmName);
    public delegate void StopBGMDelegate();

    public delegate void ShieldOnDelegate(int i);
    public delegate void ShieldOffDelegate();

    public delegate void OnFocusSceneDelegate(string sceneName, SSController controller, bool isFocus);
    public delegate void OnFocusDelegate(bool isFocus);

    public delegate void FocusByOpenDelegate(string sceneName, string curBgm);
    public delegate void FocusByCloseOtherDelegate(string sceneName);

    public delegate void SetBusyDelegate(bool isBusy);
    public delegate void BackToBottomScreenDelegate();
    public delegate void HideAllDelegate(Stack<string> stackScreen);
    public delegate void PlayAnimationDelegate(string sceneName, AnimType animType, NoParamCallback callback = null);
    public delegate void OnScreenStartChangeDelegate(string sceneName);

    public delegate void ActiveASceneDelegate(string sceneName);
    public delegate void DeactiveASceneDelegate(string sceneName);
    public delegate void DestroySceneDelegate(string sceneName);
    public delegate void DestroyOrCacheDelegate(string sceneName);

    public delegate string GetHighestSceneDelegate();
    public delegate DictObject GetSceneDictionaryDelegate();
    public delegate SSController GetControllerDelegate(string sceneName);
    #endregion

    #region Delegate
    public OpenDelegate open;
    public CloseDelegate close;

    public PlayBGMDelegate playBGM;
    public StopBGMDelegate stopBGM;

    public ShieldOnDelegate shieldOn;
    public ShieldOffDelegate shieldOff;

    public GetSceneDictionaryDelegate getSceneDictionary;
    public GetControllerDelegate getController;

    public OnFocusSceneDelegate onFocusScene;
    public OnFocusDelegate onFocus;

    public FocusByOpenDelegate focusByOpen;
    public FocusByCloseOtherDelegate focusByCloseOther;

    public SetBusyDelegate setBusy;
    public BackToBottomScreenDelegate backToBottomScreen;
    public HideAllDelegate hideAll;
    public PlayAnimationDelegate playAnimation;
    public OnScreenStartChangeDelegate onScreenStartChange;

    public ActiveASceneDelegate activeAScene;
    public DeactiveASceneDelegate deactiveAScene;
    public DestroySceneDelegate destroyScene;
    public DestroyOrCacheDelegate destroyOrCache;

    public GetHighestSceneDelegate getHighestScene;
    #endregion

    public SSModule()
    {
        list.Add(this);
    }

    protected void OpenCommon(string sceneName, int ip, float ic, object data, bool imme, string curBgm, NoParamCallback onAnimEnded = null, NoParamCallback onLoaded = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
    {
        if (open != null)
        {
            open(sceneName, ip, ic, data, imme, curBgm, onAnimEnded, onLoaded, onActive, onDeactive);
        }
    }

    protected void CloseCommon(string sceneName, bool imme, NoParamCallback onAnimEnded = null)
    {
        if (close != null)
        {
            close(sceneName, imme, onAnimEnded);
        }
    }

    protected void PlayBGM(string bgmName)
    {
        if (playBGM != null)
        {
            playBGM(bgmName);
        }
    }

    protected void StopBGM()
    {
        if (stopBGM != null)
        {
            stopBGM();
        }
    }

    protected void ShieldOn(int i)
    {
        if (shieldOn != null)
        {
            shieldOn(i);
        }
    }

    protected void ShieldOff()
    {
        if (shieldOff != null)
        {
            shieldOff();
        }
    }

    protected DictObject GetSceneDictionary()
    {
        if (getSceneDictionary != null)
        {
            return getSceneDictionary();
        }
        return null;
    }

    protected SSController GetController(string sceneName)
    {
        if (getController != null)
        {
            return getController(sceneName);
        }

        return null;
    }

    protected void OnFocusScene(string sceneName, SSController controller, bool isFocus)
    {
        if (onFocusScene != null)
        {
            onFocusScene(sceneName, controller, isFocus);
        }
    }

    protected void OnFocus(bool isFocus)
    {
        if (onFocus != null)
        {
            onFocus(isFocus);
        }
    }

    protected void FocusByOpen(string sceneName, string curBgm)
    {
        if (focusByOpen != null)
        {
            focusByOpen(sceneName, curBgm);
        }
    }

    protected void FocusByCloseOther(string sceneName)
    {
        if (focusByCloseOther != null)
        {
            focusByCloseOther(sceneName);
        }
    }

    protected void SetBusy(bool isBusy)
    {
        if (setBusy != null)
        {
            setBusy(isBusy);
        }
    }

    protected void BackToBottomScreen()
    {
        if (backToBottomScreen != null)
        {
            backToBottomScreen();
        }
    }
        
    protected void HideAll(Stack<string> stackScreen)
    {
        if (hideAll != null)
        {
            hideAll(stackScreen);
        }
    }

    protected void PlayAnimation(string sceneName, AnimType animType, NoParamCallback callback = null)
    {
        if (playAnimation != null)
        {
            playAnimation(sceneName, animType, callback);
        }
    }

    protected void OnScreenStartChange(string sceneName)
    {
        if (onScreenStartChange != null)
        {
            onScreenStartChange(sceneName);
        }
    }

    protected void ActiveAScene(string sceneName)
    {
        if (activeAScene != null)
        {
            activeAScene(sceneName);
        }
    }

    protected void DeactiveAScene(string sceneName)
    {
        if (deactiveAScene != null)
        {
            deactiveAScene(sceneName);
        }
    }

    protected void DestroyScene(string sceneName)
    {
        if (destroyScene != null)
        {
            destroyScene(sceneName);
        }
    }

    protected void DestroyOrCache(string sceneName)
    {
        if (destroyOrCache != null)
        {
            destroyOrCache(sceneName);
        }
    }

    protected string GetHighestScene()
    {
        if (getHighestScene != null)
        {
            return getHighestScene();
        }

        return null;
    }
}
