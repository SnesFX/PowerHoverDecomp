using UnityEngine;

public class ColorFader : MonoBehaviour
{
	public Color flashColor;

	public Color bgColor;

	private Material mat;

	private bool updating;

	private int distance;

	private float timer;

	private void Start()
	{
		mat = GetComponent<Renderer>().material;
	}

	private void Update()
	{
		if (!updating)
		{
			return;
		}
		timer += Time.deltaTime;
		if (timer < 0f)
		{
			mat.color = Color.Lerp(mat.color, flashColor, Time.deltaTime * (float)distance * 0.05f);
			return;
		}
		mat.color = Color.Lerp(mat.color, bgColor, Time.deltaTime * (float)distance * 0.1f);
		if (distance < 11 && timer > 1.5f)
		{
			timer = 0f;
			mat.color = Color.Lerp(mat.color, flashColor, Time.deltaTime * (float)distance);
		}
		else if (distance >= 11 && distance < 12 && timer > 1.5f)
		{
			timer = 0f;
			mat.color = Color.Lerp(mat.color, flashColor, Time.deltaTime * (float)distance * 0.5f);
		}
		if (distance > 11 && distance < 16 && timer > 0.1f)
		{
			timer = 0f;
			if (Random.Range(0, 10) < 3)
			{
				mat.color = Color.Lerp(mat.color, flashColor, Time.deltaTime * (float)distance * 0.09f);
			}
		}
	}

	public void MakeLerp(int dist)
	{
		updating = true;
		distance = dist;
		timer = -0.12f;
	}
}
