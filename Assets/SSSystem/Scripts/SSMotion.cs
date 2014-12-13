/**
 * Created by Anh Pham on 2013/11/13
 * Email: anhpt.csit@gmail.com
 */

using UnityEngine;
using System.Collections;

public class SSMotion : MonoBehaviour 
{
    /// <summary>
    /// The state of the motion
    /// </summary>
    protected AnimType m_State;

	/// <summary>
	/// Awake function.
	/// </summary>
	protected virtual void Awake()
	{
	}

	/// <summary>
	/// Start function.
	/// </summary>
	protected virtual void Start()
	{
	}

	/// <summary>
	/// Update function.
	/// </summary>
	protected virtual void Update()
	{
	}

    /// <summary>
    /// Reset() called after activating a game object and before playing an animation.
    /// </summary>
    public virtual void Reset(AnimType animType)
    {
    }

	/// <summary>
	/// Time of show - animation by second.
	/// </summary>
	/// <returns>Time show.</returns>
	public virtual float TimeShow()
	{
		return 0;
	}

	/// <summary>
	/// Time of hide - animation by second.
	/// </summary>
	/// <returns>Time hide.</returns>
	public virtual float TimeHide()
	{
		return 0;
	}

	/// <summary>
	/// Play the the show - animation.
	/// </summary>
	public virtual void PlayShow()
	{
	}

	/// <summary>
	/// Play the the hide - animation.
	/// </summary>
	public virtual void PlayHide()
	{
	}

	/// <summary>
	/// Time of show back - animation by second.
	/// </summary>
	/// <returns>Time show.</returns>
	public virtual float TimeShowBack()
	{
		return 0;
	}

	/// <summary>
	/// Time of hide back - animation by second.
	/// </summary>
	/// <returns>Time hide.</returns>
	public virtual float TimeHideBack()
	{
		return 0;
	}

	/// <summary>
	/// Play the the show back - animation.
	/// </summary>
	public virtual void PlayShowBack()
	{
	}

	/// <summary>
	/// Play the the hide back - animation.
	/// </summary>
	public virtual void PlayHideBack()
	{
	}

    /// <summary>
    /// Raises the enable event.
    /// </summary>
    protected virtual void OnEnable()
    {
    }

    /// <summary>
    /// Raises the disable event.
    /// </summary>
    protected virtual void OnDisable()
    {
    }
}
