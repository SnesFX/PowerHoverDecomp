using UnityEngine;
using UnityEngine.UI;

public class CopyText : MonoBehaviour
{
	public Text textToCopy;

	public TextMesh textMeshToCopy;

	private Text thisText;

	private TextMesh thisTextMesh;

	private bool meshOn;

	private void Awake()
	{
		meshOn = textToCopy == null;
		if (meshOn)
		{
			thisTextMesh = GetComponent<TextMesh>();
		}
		else
		{
			thisText = GetComponent<Text>();
		}
	}

	private void OnEnable()
	{
		UpdateText();
	}

	private void FixedUpdate()
	{
		UpdateText();
	}

	private void UpdateText()
	{
		if (meshOn && isChanged())
		{
			thisTextMesh.text = textMeshToCopy.text;
			thisTextMesh.font = textMeshToCopy.font;
			thisTextMesh.fontSize = textMeshToCopy.fontSize;
			thisTextMesh.GetComponent<MeshRenderer>().material.mainTexture = textMeshToCopy.GetComponent<MeshRenderer>().material.mainTexture;
		}
		else if (!meshOn && isChanged())
		{
			thisText.text = textToCopy.text;
			thisText.font = textToCopy.font;
		}
	}

	private bool isChanged()
	{
		if (meshOn)
		{
			return !thisTextMesh.text.Equals(textMeshToCopy.text) || thisTextMesh.font != textMeshToCopy.font;
		}
		return !thisText.text.Equals(textToCopy.text) || thisText.font != textToCopy.font;
	}
}
