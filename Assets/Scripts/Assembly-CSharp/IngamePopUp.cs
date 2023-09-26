using UnityEngine;
using UnityEngine.UI;

public class IngamePopUp : MonoBehaviour
{
	public enum PopUpType
	{
		BreakableQuestion = 0
	}

	public Text popUpText;

	public Image popUpImage;

	public GameObject button1Obj;

	public GameObject button2Obj;

	public Text button1;

	public Text button2;

	public PopUpType Type;

	public Sprite[] TypeSprites;

	public void ShowGhostQuestion()
	{
		base.gameObject.SetActive(true);
	}
}
