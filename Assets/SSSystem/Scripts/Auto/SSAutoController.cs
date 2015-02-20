using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SSAutoController : MonoBehaviour
{
    SSRoot Root;

    void Awake()
    {
        CreateRoot();
    }

    void OnValidate()
    {
        CreateRoot();
    }

    void CreateRoot()
    {
        if (Root == null && !Application.isPlaying)
        {
            Transform root = gameObject.transform;
            while (root.parent != null)
            {
                root = root.parent;
            }

            SSRoot ssRoot = root.GetComponent<SSRoot>();
            if (ssRoot == null)
            {
                root.gameObject.AddComponent<SSRoot>();
            }

            Root = ssRoot;
        }
    }
}
