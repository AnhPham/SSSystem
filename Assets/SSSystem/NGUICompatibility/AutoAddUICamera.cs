using UnityEngine;
using System.Collections;

public class AutoAddUICamera : MonoBehaviour 
{
	private void Awake()
	{
		gameObject.AddComponent("UICamera");
	}
}
