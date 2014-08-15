using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SSShieldModule : SSModule
{
    protected GameObject m_Shields;         // Shield container
    protected GameObject m_ShieldTop;       // Shield top object
    protected GameObject m_ShieldEmpty;     // Shield empty object (higher all scene except top shield, alpha = 0)

    protected int m_ShieldEmptyCount;       // Shield empty counter

    protected List<GameObject>  m_ListShield = new List<GameObject>();

    public SSShieldModule() : base ()
    {
        m_Shields = new GameObject("Shields");
    }

    public GameObject CreateShield(int i)
    {
        // Instantiate from resources
        GameObject sh = GameObject.Instantiate(Resources.Load("Shield")) as GameObject;
        sh.name = "Shield" + i;
        sh.transform.localPosition = new Vector3(i * SSConst.SCENE_DISTANCE, 0, 0);
        sh.transform.parent = m_Shields.transform;

        // Set camera depth
        Camera c = sh.GetComponentInChildren<Camera>();
        c.depth = (i+1) * SSConst.DEPTH_DISTANCE;

        return sh;
    }

    public void ShieldTopOn(float alpha)
    {
        if (m_ShieldTop == null) 
        {
            m_ShieldTop = CreateShield (SSConst.SHIELD_TOP_INDEX-1);
        } else 
        {
            m_ShieldTop.SetActive (true);
        }

        Image image = m_ShieldTop.GetComponentInChildren<Image> ();
        image.color = new Color (0, 0, 0, alpha);
    }

    public void ShieldTopOff()
    {
        if (m_ShieldTop != null) 
        {
            m_ShieldTop.SetActive (false);
        }
    }

    public void ShieldOn(int i, Color color)
    {
        if (i < 0) return;

        if (m_ListShield.Count <= i)
        {
            // Create shield
            GameObject sh = CreateShield (i);

            // Add to List
            m_ListShield.Add(sh);
        }
        else
        {
            m_ListShield[i].SetActive(true);
        }

        Image image = m_ListShield[i].GetComponentInChildren<Image> ();
        image.color = color;
    }

    public void ShieldOff(int i)
    {
        if (i < 0) return;

        if (m_ListShield.Count >= i)
            m_ListShield[i].SetActive(false);
    }

    public void ShowEmptyShield()
    {
        if (m_ShieldEmpty == null) 
        {
            m_ShieldEmpty = CreateShield (SSConst.SHIELD_TOP_INDEX-2);
        } else 
        {
            m_ShieldEmpty.SetActive (true);
        }

        Image image = m_ShieldEmpty.GetComponentInChildren<Image> ();
        image.color = new Color (0, 0, 0, 0);

        m_ShieldEmptyCount++;
    }

    public void HideEmptyShield()
    {
        if (m_ShieldEmpty != null) 
        {
            m_ShieldEmptyCount--;

            if (m_ShieldEmptyCount == 0) 
            {
                m_ShieldEmpty.SetActive (false);
            }
        }
    }

    public bool IsShieldActive(int i)
    {
        if (i >= m_ListShield.Count)
            return false;

        return (m_ListShield [i].activeInHierarchy);
    }
}
