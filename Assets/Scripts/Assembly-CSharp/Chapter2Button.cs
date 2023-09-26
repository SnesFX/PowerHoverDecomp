using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Chapter2Button : MonoBehaviour
{
	public GameObject Lock;

	public Text buttonText;

	public Animator lockAnim;

	private void OnEnable()
	{
		UpdateLock();
	}

	public void SetChapterText(bool chapter2)
	{
		LocalizationLoader.Instance.SetText(buttonText, "MainMenu.ChapterGoto");
		buttonText.text = string.Format("{0} {1}", buttonText.text, chapter2 ? 1 : 2);
		UpdateLock(true);
	}

	private void UpdateLock(bool animate = false)
	{
		if (SceneLoader.Instance != null && SceneLoader.Instance.IsChapter2Unlocked())
		{
			if (animate && Lock.activeSelf)
			{
				lockAnim.enabled = true;
				StartCoroutine(WaitAndDisable());
			}
			else
			{
				Lock.SetActive(false);
			}
		}
	}

	private IEnumerator WaitAndDisable()
	{
		yield return new WaitForSeconds(1f);
		Lock.SetActive(false);
	}
}
