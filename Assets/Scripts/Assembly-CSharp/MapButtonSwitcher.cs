using UnityEngine;

public class MapButtonSwitcher : MonoBehaviour
{
	public Animator anim;

	private bool onMap;

	public void SwitchButton()
	{
		onMap = !onMap;
		anim.Play((!onMap) ? "TownBackButtonTurnB" : "TownBackButtonTurn", -1, 0f);
	}
}
