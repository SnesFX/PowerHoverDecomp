using UnityEngine;

public class SwitchTextureOnMaterial : MonoBehaviour
{
	public Texture[] textures;

	private Material usedMaterial;

	private int textureAmount;

	public float timeBetweenFrames;

	private float timer;

	public bool random;

	private int frameNumber;

	private void Start()
	{
		usedMaterial = GetComponent<MeshRenderer>().material;
		GetComponent<MeshRenderer>().material = usedMaterial;
		textureAmount = textures.Length;
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer >= timeBetweenFrames)
		{
			SwitchTextures();
		}
	}

	private void SwitchTextures()
	{
		timer = 0f;
		frameNumber++;
		if (frameNumber >= textureAmount)
		{
			frameNumber = 0;
		}
		if (random)
		{
			frameNumber = Random.Range(0, textureAmount);
		}
		usedMaterial.mainTexture = textures[frameNumber];
	}
}
