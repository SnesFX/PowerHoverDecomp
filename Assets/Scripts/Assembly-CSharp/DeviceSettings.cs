using System;
using SA.Common.Pattern;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeviceSettings : MonoBehaviour
{
	private float controllerCheckTimer;

	private bool currentIn;

	public static DeviceSettings Instance { get; private set; }

	public bool EnableBloom { get; private set; }

	public bool EnableOptimizedFade { get; private set; }

	public bool EnableFloatingBatteryShadows { get; private set; }

	public bool EnableWaves { get; private set; }

	public bool OptimizedWorms { get; private set; }

	public bool EnableScreenParticles { get; private set; }

	public bool EnableBatteryRotation { get; private set; }

	public bool EnableMaterialColorFlicker { get; private set; }

	public bool EnableDrawDistanceLimitter { get; private set; }

	public bool EnableOptimizedMaterials { get; private set; }

	public bool EnableInputDevices { get; private set; }

	public bool RunningOnTV { get; private set; }

	private void Awake()
	{
		Instance = this;
		Application.targetFrameRate = 30;
		EnableBloom = false;
		EnableOptimizedFade = true;
		EnableFloatingBatteryShadows = true;
		EnableWaves = true;
		OptimizedWorms = false;
		EnableDrawDistanceLimitter = false;
		EnableScreenParticles = true;
		EnableBatteryRotation = true;
		EnableMaterialColorFlicker = true;
		EnableOptimizedMaterials = false;
		EnableInputDevices = false;
		RunningOnTV = false;
		if (Application.isMobilePlatform)
		{
			CheckTV();
			Singleton<ImmersiveMode>.Instance.EnableImmersiveMode();
			EnableOptimizedMaterials = true;
			EnableBatteryRotation = false;
			EnableMaterialColorFlicker = false;
			EnableDrawDistanceLimitter = true;
			QualitySettings.antiAliasing = 0;
			EnableBloom = false;
			EnableFloatingBatteryShadows = false;
			EnableOptimizedFade = true;
			EnableWaves = false;
			EnableScreenParticles = false;
			OptimizedWorms = true;
		}
	}

	private void SetupInputs(bool ingame)
	{
		GetComponentInChildren<StandaloneInputModule>().forceModuleActive = !ingame;
	}

	private void Update()
	{
		controllerCheckTimer += Time.deltaTime;
		if (controllerCheckTimer > 3f)
		{
			EnableInputDevices = Input.GetJoystickNames().Length > 0;
			controllerCheckTimer = 0f;
		}
		if (!Main.Instance.InMenu && GameController.Instance != null && (GameController.Instance.State == GameController.GameState.Running || GameController.Instance.State == GameController.GameState.Start))
		{
			if (!currentIn)
			{
				SetupInputs(true);
			}
			currentIn = true;
		}
		else
		{
			if (currentIn)
			{
				SetupInputs(false);
			}
			currentIn = false;
		}
	}

	private void CheckTV()
	{
		TVAppController.DeviceTypeChecked += OnDeviceTypeChecked;
		Singleton<TVAppController>.Instance.CheckForATVDevice();
	}

	private void OnDeviceTypeChecked()
	{
		EnableInputDevices = Singleton<TVAppController>.Instance.IsRuningOnTVDevice;
		RunningOnTV = Singleton<TVAppController>.Instance.IsRuningOnTVDevice;
		TVAppController.DeviceTypeChecked -= OnDeviceTypeChecked;
	}

	private int GetSDKLevel()
	{
		int num = 1;
		IntPtr clazz = AndroidJNI.FindClass("android.os.Build$VERSION");
		IntPtr staticFieldID = AndroidJNI.GetStaticFieldID(clazz, "SDK_INT", "I");
		return AndroidJNI.GetStaticIntField(clazz, staticFieldID);
	}
}
