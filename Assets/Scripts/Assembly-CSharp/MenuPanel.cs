using UnityEngine;
using UnityEngine.EventSystems;

public class MenuPanel : MonoBehaviour, IDragHandler, IEndDragHandler, IEventSystemHandler
{
	public bool IsActive;

	public GameObject ActiveContent;

	public CustomInputController.InputButtons Inputs;

	public virtual void Awake()
	{
		Activate(IsActive);
	}

	public virtual void Activate(bool enable)
	{
		IsActive = enable;
		if (ActiveContent != null)
		{
			ActiveContent.SetActive(enable);
		}
		if (IsActive && Inputs != null && Inputs.InObjects != null && Inputs.InObjects.Length > 0)
		{
			Inputs.Panel = this;
			CustomInputController.Instance.SetCurrentButtons(Inputs);
		}
	}

	public virtual void OnEndDrag(PointerEventData data)
	{
	}

	public virtual void OnDrag(PointerEventData data)
	{
	}
}
