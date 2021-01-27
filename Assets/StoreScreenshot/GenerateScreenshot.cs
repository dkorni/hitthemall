#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class GenerateScreenshot : MonoBehaviour
{

	public int screenNumber;
	public Vector2 dimension;
	private string currentTime;

	private List<StoreScreenshotUtils.ScreenshotResolution> resolutions;
	
	private bool[] defaultStates;

	[HideInInspector]
	public bool hideCanvases, saveAsJpg;
	[HideInInspector]
	public int jpgQuality;


	public void SetResolutionAndSave(List<StoreScreenshotUtils.ScreenshotResolution> savedResolutions) {
		resolutions = savedResolutions;

		TakeScreenshot();
	}

	public void TakeScreenshot() {
		StartCoroutine(TimedScreenshot());
	}

	IEnumerator TimedScreenshot() {
		float defaultTimeScale = Time.timeScale;
		Time.timeScale = 0;
		currentTime = System.DateTime.Now.ToString().Replace('/', '-').Replace(':', '.');

		if( hideCanvases ) {
			TurnOffCanvases();
		}
		for (int i = 0; i < resolutions.Count; i++)
		{
			if( resolutions[i].active) {
				StoreScreenshotUtils.AddCustomSize(resolutions[i].width, resolutions[i].height, "StoreScreenshot_" + resolutions[i].resName);
				StoreScreenshotUtils.ChangeGameViewSize(resolutions[i].width, resolutions[i].height, "StoreScreenshot_" + resolutions[i].resName);

				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();

				SaveScreenshot(resolutions[i].resName);
				StoreScreenshotUtils.RemoveCustomSize(resolutions[i].width, resolutions[i].height, "StoreScreenshot_" + resolutions[i].resName);
			}
		}

		yield return new WaitForEndOfFrame();

		if (hideCanvases) {
			TurnOnCanvases();
		}

		Debug.Log("All screenshot saved in \"Project Folder > Screenshot > Name\"!");
		Time.timeScale = defaultTimeScale;

		Destroy(gameObject);
	}

	public void SaveScreenshot(string name) {
		dimension = new Vector2(Screen.width, Screen.height);

		Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		screenshot.Apply();

		byte[] bytes;
		string extension = "";
		if( saveAsJpg )
        {
			bytes = screenshot.EncodeToJPG(jpgQuality);
			extension = ".jpg";
		}
		else
        {
			bytes = screenshot.EncodeToPNG();
			extension = ".png";
		}
		
		string filePath = Application.dataPath + "/../Screenshot/" + name + "/";

		if (!Directory.Exists(filePath)) {
			Directory.CreateDirectory(filePath);
		}
		string fileName = "Screen_" + dimension.x + "x" + dimension.y + "_" + currentTime + extension;
		File.WriteAllBytes(filePath + fileName, bytes);
		//Debug.Log("Screenshot Generated - " + filePath + fileName );
	}


	public void TurnOffCanvases() {
		Canvas[] allCanvases = GameObject.FindObjectsOfType<Canvas>();
		defaultStates = new bool[allCanvases.Length];

		for (int i = 0; i < allCanvases.Length; i++ ) {
			defaultStates[i] = allCanvases[i].enabled;

			allCanvases[i].enabled = false;
		}
	}

	public void TurnOnCanvases() {
		Canvas[] allCanvases = GameObject.FindObjectsOfType<Canvas>();

		for (int i = 0; i < allCanvases.Length; i++) {
			allCanvases[i].enabled = defaultStates[i];
		}
	}
}
#endif