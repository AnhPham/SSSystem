using UnityEngine;
using System.Collections;

public class SSAutoAddUICamera : MonoBehaviour 
{
	private void Awake()
	{
		gameObject.AddComponent("UICamera");
	}
}
