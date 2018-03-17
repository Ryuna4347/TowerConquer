using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Button handler.
/// </summary>
public class ButtonHandler : MonoBehaviour
{
	public AudioClip audioClip;

	/// <summary>
	/// Buttons pressed.
	/// </summary>
	/// <param name="buttonName">Button name.</param>
	public void ButtonPressed(string buttonName)
	{
		StartCoroutine(PressedCoroutine(buttonName));
	}

    /// <summary>
    /// Presseds the coroutine.
    /// </summary>
    /// <returns>The coroutine.</returns>
    /// <param name="buttonName">Button name.</param>
    private IEnumerator PressedCoroutine(string buttonName)
	{
		// Play sound effect
		if (audioClip != null && AudioManager.instance != null)
		{
			Button button = GetComponent<Button>();
			button.interactable = false; //오디오 트는 동안 버튼이 먹히지 않도록 조정하기 위함인듯
			AudioManager.instance.PlaySound(audioClip);
			// Wayt for sound effect end
			yield return new WaitForSecondsRealtime(audioClip.length); //오디오 트는 동안 작동안함
			button.interactable = true; //오디오 끝나면 버튼 복귀
		}
		// Send global event about button preesing
		EventManager.TriggerEvent("ButtonPressed", gameObject, buttonName);
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	void OnDestroy()
	{
		StopAllCoroutines();
	}
}
