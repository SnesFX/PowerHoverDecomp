using UnityEngine;
using UnityEngine.Rendering;

public class SceneRenderSettings : MonoBehaviour
{
	[HideInInspector]
	public bool fog;

	[HideInInspector]
	public FogMode fogMode;

	[HideInInspector]
	public Color fogColor;

	[HideInInspector]
	public float fogDensity;

	[HideInInspector]
	public float fogStartDistance;

	[HideInInspector]
	public float fogEndDistance;

	[HideInInspector]
	public AmbientMode ambientMode;

	[HideInInspector]
	public Color ambientSkyColor;

	[HideInInspector]
	public Color ambientEquatorColor;

	[HideInInspector]
	public Color ambientGroundColor;

	[HideInInspector]
	public Color ambientLight;

	[HideInInspector]
	public float ambientIntensity;

	[HideInInspector]
	public SphericalHarmonicsL2 ambientProbe;

	[HideInInspector]
	public float reflectionIntensity;

	[HideInInspector]
	public int reflectionBounces;

	[HideInInspector]
	public float haloStrength;

	[HideInInspector]
	public float flareStrength;

	[HideInInspector]
	public float flareFadeSpeed;

	[HideInInspector]
	public Material skybox;

	[HideInInspector]
	public DefaultReflectionMode defaultReflectionMode;

	[HideInInspector]
	public int defaultReflectionResolution;

	[HideInInspector]
	public Cubemap customReflection;

	public void SaveSettings()
	{
		fog = RenderSettings.fog;
		fogMode = RenderSettings.fogMode;
		fogColor = RenderSettings.fogColor;
		fogDensity = RenderSettings.fogDensity;
		fogStartDistance = RenderSettings.fogStartDistance;
		fogEndDistance = RenderSettings.fogEndDistance;
		ambientMode = RenderSettings.ambientMode;
		ambientSkyColor = RenderSettings.ambientSkyColor;
		ambientEquatorColor = RenderSettings.ambientEquatorColor;
		ambientGroundColor = RenderSettings.ambientGroundColor;
		ambientLight = RenderSettings.ambientLight;
		ambientIntensity = RenderSettings.ambientIntensity;
		ambientProbe = RenderSettings.ambientProbe;
		reflectionIntensity = RenderSettings.reflectionIntensity;
		reflectionBounces = RenderSettings.reflectionBounces;
		haloStrength = RenderSettings.haloStrength;
		flareStrength = RenderSettings.flareStrength;
		flareFadeSpeed = RenderSettings.flareFadeSpeed;
		skybox = RenderSettings.skybox;
		defaultReflectionMode = RenderSettings.defaultReflectionMode;
		defaultReflectionResolution = RenderSettings.defaultReflectionResolution;
		customReflection = RenderSettings.customReflection;
	}

	public void LoadSettings()
	{
		RenderSettings.fog = fog;
		RenderSettings.fogMode = fogMode;
		RenderSettings.fogColor = fogColor;
		RenderSettings.fogDensity = fogDensity;
		RenderSettings.fogStartDistance = fogStartDistance;
		RenderSettings.fogEndDistance = fogEndDistance;
		if (DeviceSettings.Instance != null && DeviceSettings.Instance.EnableDrawDistanceLimitter)
		{
			Debug.Log("fogStartDistance from " + RenderSettings.fogStartDistance + " to " + RenderSettings.fogStartDistance * 0.8f);
			Debug.Log("fogEndDistance from " + RenderSettings.fogEndDistance + " to " + RenderSettings.fogEndDistance * 0.8f);
			if (RenderSettings.fogStartDistance > 0f)
			{
				RenderSettings.fogStartDistance *= 0.8f;
			}
			RenderSettings.fogEndDistance *= 0.8f;
		}
		RenderSettings.ambientMode = ambientMode;
		RenderSettings.ambientSkyColor = ambientSkyColor;
		RenderSettings.ambientEquatorColor = ambientEquatorColor;
		RenderSettings.ambientGroundColor = ambientGroundColor;
		RenderSettings.ambientLight = ambientLight;
		RenderSettings.ambientIntensity = ambientIntensity;
		RenderSettings.ambientProbe = ambientProbe;
		RenderSettings.reflectionIntensity = reflectionIntensity;
		RenderSettings.reflectionBounces = reflectionBounces;
		RenderSettings.haloStrength = haloStrength;
		RenderSettings.flareStrength = flareStrength;
		RenderSettings.flareFadeSpeed = flareFadeSpeed;
		RenderSettings.skybox = skybox;
		RenderSettings.defaultReflectionMode = defaultReflectionMode;
		RenderSettings.defaultReflectionResolution = defaultReflectionResolution;
		RenderSettings.customReflection = customReflection;
	}
}
