using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SSScreenModule : SSModule, IUnderPopUp
{
    protected Stack<string> m_CurrentStackScreen = new Stack<string>();
    protected Dictionary<string, Stack<string>> m_DictScreen = new Dictionary<string, Stack<string>>();

    public void OpenScreen(string sceneName, object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
    {
        string sn = sceneName;

        // Check valid
        if (!CanOpenScreen (sn)) 
        {
            SetBusy(false);
            return;
        }

        // Event change main screen
        OnScreenStartChange(sn);

        // Prev stack screen
        Stack<string> prevStack = m_CurrentStackScreen;

        // Set screen stack to current
        string top = CreateOrPeek (sn);
        if (sn == top) // First time load
        {
            OpenCommon (sn, 0, 0, data, true, string.Empty, () => 
            {
                // Hide All
                HideAll (prevStack);
            }, null, onActive, onDeactive);
        } 
        else // Not first time
        {
            // Hide All
            HideAll (prevStack);

            // Just active scene
            ActiveAScene (top);

            // Show and Bgm
            SSController ct = GetController (top);
            if (ct != null) 
            {
                FocusByOpen (top, ct.CurrentBgm);
            }

            SetBusy(false);
        }
    }

    public void OpenScreenAdd(string sceneName, object data = null, bool imme = false, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
    {
        string sn = sceneName;

        // Check valid
        if (!CanOpenScreenAdd (sn)) 
        {
            SetBusy(false);
            return;
        }

        // Bot Scene
        string bot = StackScreenBottom();
        if (string.IsNullOrEmpty (bot)) 
        {
            Debug.LogWarning ("Main screen is not exist, can't add this screen above!");
            return;
        }

        // Next index
        int ip = -2 - m_CurrentStackScreen.Count;
        float ic = 0.3f + (float)m_CurrentStackScreen.Count / SSConst.DEPTH_DISTANCE;

        // Prev Scene
        string preSn = m_CurrentStackScreen.Peek ();
        SSController ct = GetController (preSn);

        // Cur Bgm
        string curBgm = ct.CurrentBgm;

        // Thread 2
        OnFocusScene(preSn, ct, false);

        // Thread 1
        OpenCommon (sn, ip, ic, data, imme, curBgm, () => 
        {
            // IsCache for no destroy automatically, except some of special case.
            SSController newCt = GetController(sn);
            newCt.IsCache = true;

            // Push stack
            m_CurrentStackScreen.Push(sn);
        }, () => 
        {
            // Animation
            AnimType animType = (imme) ? AnimType.NO_ANIM : AnimType.HIDE_BACK;
            PlayAnimation (preSn, animType, () => 
            {
                DeactiveAScene(preSn);
            });
        }, onActive, onDeactive);
    }

    public void CloseScreen(bool imme, NoParamCallback callback = null)
    {
        string curSn = m_CurrentStackScreen.Pop ();
        string preSn = string.Empty;

        // Check if has prev scene
        if (m_CurrentStackScreen.Count > 0) 
        {
            preSn = m_CurrentStackScreen.Peek ();
        }

        // Thread 1 (current scene animation)
        CloseCommon (curSn, imme, () => 
        {
            if (callback != null)
            {
                callback();
            }
        });

        // Thread 2 (previous scene animation)
        if (!string.IsNullOrEmpty (preSn)) 
        {
            // Active
            ActiveAScene (preSn);

            // Animation
            AnimType animType = (imme) ? AnimType.NO_ANIM : AnimType.SHOW_BACK;
            PlayAnimation (preSn, animType, () => 
            {
                FocusByCloseOther (preSn);
            });
        }
        else
        {
            // Do nothing or application quit
        }
    }

    public bool IsScreenInAnyStack(string sceneName)
    {
        foreach (var screens in m_DictScreen)
        {
            if (screens.Value != null && screens.Value.Contains(sceneName))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsScreenInActiveStack(string sceneName)
    {
        return (m_CurrentStackScreen != null && m_CurrentStackScreen.Contains(sceneName));
    }

    public bool HasScreenInActiveStack()
    {
        return (m_CurrentStackScreen != null && m_CurrentStackScreen.Count > 0);
    }

    public bool CanOpenScreen(string sceneName)
    {
        foreach (var pair in m_DictScreen) 
        {
            string bot = SSTools.GetStackBottom(pair.Value);

            bool isCurStack = (m_CurrentStackScreen == pair.Value);
            bool isInStack = pair.Value.Contains (sceneName);

            if (sceneName == bot && isCurStack) 
            {
                if (m_CurrentStackScreen.Count > 1)
                {
                    BackToBottomScreen();
                }

                return false;
            }

            if (sceneName != bot && isInStack) 
            {
                Debug.LogWarning ("This screen was added to stack!");
                return false;
            }
        }

        return true;
    }

    public bool CanOpenScreenAdd(string sceneName)
    {
        foreach (var pair in m_DictScreen) 
        {
            bool isInStack = pair.Value.Contains (sceneName);

            if (isInStack)
            {
                Debug.LogWarning ("This screen was added to stack!");
                return false;
            }
        }

        return true;
    }
        
    private string StackScreenBottom()
    {
        return SSTools.GetStackBottom (m_CurrentStackScreen);
    }

    public void DestroyInactiveScreens(params string[] exceptList)
    {
        foreach (var screens in m_DictScreen)
        {
            string sn = screens.Key;
            if (SSTools.IsNotInExcept(sn, exceptList) && (screens.Value != null && screens.Value.Count != 0))
            {
                DestroyScreensFrom(sn);
            }
        }
    }

    public void DestroyScreensFrom(string sceneName)
    {
        string sn = sceneName;

        foreach (var screens in m_DictScreen)
        {
            Stack<string> stack = screens.Value;

            if (stack.Contains(sn))
            {
                // Not in current stack
                if (stack != m_CurrentStackScreen)
                {
                    if (stack.Count >= 1)
                    {
                        string s = stack.Pop();

                        while (string.Compare(s, sn) != 0)
                        {
                            DestroyScene(s);
                            s = stack.Pop();
                        }
                        DestroyScene(sn);
                    }
                }
                else
                {
                    Debug.LogWarning("Can't destroy this scene. It's in an active stack: " + sn);
                    return;
                }

                // Break because found the stack contain scene.
                break;
            }
        }
    }

    public string DestroyCurrentStack()
    {
        Stack<string> stack = m_CurrentStackScreen;
        string s = null;

        while (stack != null && stack.Count >= 1)
        {
            s = stack.Pop();
            DestroyScene(s);
        }

        m_CurrentStackScreen = new Stack<string>();

        return s;
    }

    private string CreateOrPeek(string sn)
    {
        // Check Exist
        if (!m_DictScreen.ContainsKey (sn)) 
        {
            m_DictScreen.Add (sn, new Stack<string> ());
        }
        m_CurrentStackScreen = m_DictScreen [sn];

        // If empty, push then return sn
        if (m_CurrentStackScreen.Count == 0) 
        {
            m_CurrentStackScreen.Push (sn);
            return sn;
        }

        // If not empty, return top of stack
        return Peek();
    }

    public string Peek()
    {
        return m_CurrentStackScreen.Peek ();
    }

    public int Count()
    {
        return m_CurrentStackScreen.Count;
    }

    public void RemoveMiddleOfStack()
    {
        string top = m_CurrentStackScreen.Pop();

        while (m_CurrentStackScreen.Count > 1)
        {
            string sn = m_CurrentStackScreen.Pop();

            SSController ct = GetController(sn);
            if (ct != null)
            {
                ct.OnHide();
                ct.Config();
            }

            DestroyOrCache(sn);
            if (ct != null)
            {
                ct.ResetMotion();
            }
        }

        m_CurrentStackScreen.Push(top);
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Print all stack"))
        {
            foreach (var item in m_DictScreen) 
            {
                Debug.Log ("Main: " + item.Key);
                foreach (var it in item.Value) 
                {
                    Debug.Log (it);
                }
            }
        }
    }

    public string GetCurrentSceneName()
    {
        if (HasScreenInActiveStack())
        {
            return Peek();
        }

        return null;
    }
        
    public bool IsBgmDecider()
    {
        return true;
    }
}
