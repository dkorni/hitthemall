using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class StoreScreenshot : EditorWindow {
	private Vector2 scrollPosition;
	private int rowNum;

	private static string separator = "\\-/";
	private static int numberOfResolutions;
	private static bool hideUGUICanvases, savedHideUGUICanvases;
	private static bool saveAsJpg, savedSaveAsJpg;
	private static int jpgQuality, savedJpgQuality;

	private static List<StoreScreenshotUtils.ScreenshotResolution> resolutions;
	private static List<StoreScreenshotUtils.ScreenshotResolution> resolutionsToSave;
	private Color lightRed = new Color(1, 0.5f, 0.5f);
	private Color lightGreen = new Color(0.5f, 1f, 0.5f);
	private static bool changesToSave;

	[MenuItem ("Store Screenshot/Settings")]
	static void Init () {
		ReadValues ();

		StoreScreenshot window = (StoreScreenshot)EditorWindow.GetWindow (typeof (StoreScreenshot));
		window.Show();
	}

	[MenuItem("Store Screenshot/Save %&s")]
	static void SaveScreenshot() {
		ReadValues();

		Debug.Log("Saving screenshot...");
		if( EditorApplication.isPlaying ) {
			GameObject generateScreenshot = GameObject.Find("GenerateScreenshot");

			if( generateScreenshot == null ) {
				generateScreenshot = new GameObject("GenerateScreenshot");
				generateScreenshot.AddComponent<GenerateScreenshot>();
			}

			GenerateScreenshot generateScript = generateScreenshot.GetComponent<GenerateScreenshot>();
			generateScript.saveAsJpg = savedSaveAsJpg;
			generateScript.jpgQuality = savedJpgQuality;
			generateScript.hideCanvases = savedHideUGUICanvases;

			generateScript.SetResolutionAndSave(resolutions);

		} else {
			Debug.Log("You need to be in playmode to save your screenshots!");
		}
	}

	static void ReadValues() {
		numberOfResolutions = EditorPrefs.GetInt("NumberOfResolutions");

		hideUGUICanvases = EditorPrefs.GetBool("HideUGUICanvases");
		savedHideUGUICanvases = hideUGUICanvases;

		saveAsJpg = EditorPrefs.GetBool("SaveAsJpg");
		savedSaveAsJpg = saveAsJpg;

		jpgQuality = EditorPrefs.GetInt("JpgQuality");
		savedJpgQuality = jpgQuality;


		resolutions = new List<StoreScreenshotUtils.ScreenshotResolution>();
		resolutionsToSave = new List<StoreScreenshotUtils.ScreenshotResolution>();

		for(int i = 0; i < numberOfResolutions; i++ ) {
			string savedResToSplit = EditorPrefs.GetString("StoreScreenshotResolution_"+i);
			string[] savedResArray = savedResToSplit.Split( new string[] {separator}, System.StringSplitOptions.None );
			StoreScreenshotUtils.ScreenshotResolution savedRes = new StoreScreenshotUtils.ScreenshotResolution();
			savedRes.resName = savedResArray[0];
			savedRes.width = int.Parse( savedResArray[1]);
			savedRes.height = int.Parse( savedResArray[2] );
			if( savedResArray.Length > 3 ) {
				savedRes.active = bool.Parse(savedResArray[3]);
			}


			resolutions.Add( savedRes );
		}

		changesToSave = false;
	}

	void SaveResolutions() {
		for(int i = 0; i < numberOfResolutions; i++ ) {
			EditorPrefs.DeleteKey("StoreScreenshotResolution_"+i);
		}

		for(int i = 0; i < resolutions.Count; i++ ) {
			if( resolutions[i].toDelete ) {
				resolutions.Remove( resolutions[i] );
			}
		}

		for(int i = 0; i < resolutionsToSave.Count; i++ ) {
			if( resolutionsToSave[i].width != 0 && resolutionsToSave[i].height != 0 && resolutionsToSave[i].resName != "" ) {
				resolutions.Add( resolutionsToSave[i] );
			}
		}

		resolutionsToSave.Clear();

		numberOfResolutions = resolutions.Count;

		EditorPrefs.SetInt("NumberOfResolutions", numberOfResolutions );
		for(int i = 0; i < resolutions.Count; i++ ) {
			EditorPrefs.SetString("StoreScreenshotResolution_"+i, resolutions[i].resName + separator + resolutions[i].width + separator + resolutions[i].height + separator + resolutions[i].active );
		}

		changesToSave = false;

		Debug.Log("StoreScreenshot settings saved!");
	}

	void SaveHideCanvases() {
		EditorPrefs.SetBool("HideUGUICanvases", hideUGUICanvases);
		savedHideUGUICanvases = hideUGUICanvases;
	}
	void SaveAsJpg()
	{
		EditorPrefs.SetBool("SaveAsJpg", saveAsJpg);
		savedSaveAsJpg = saveAsJpg;

		EditorPrefs.SetInt("JpgQuality", jpgQuality);
		savedJpgQuality = jpgQuality;
	}

	void OnGUI () {
		if( resolutions == null ) {
			ReadValues();
		}

		int screenWidth = Mathf.CeilToInt(Screen.width / EditorGUIUtility.pixelsPerPoint);

		Color defaultColor = GUI.color;
		EditorStyles.label.wordWrap = true;

		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("By pressing \"Store Screenshot > Save\" (Ctrl/Cmd+Alt+S) while in Play Mode, the screenshot will be saved for every \"Active\" resolution you have here.");

		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();

		EditorGUILayout.LabelField("Options:", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
		hideUGUICanvases = EditorGUILayout.Toggle( hideUGUICanvases, GUILayout.Width(15));
		EditorGUILayout.LabelField("Hide uGUI Canvases", GUILayout.Width(screenWidth * 0.4f));

		if (savedHideUGUICanvases != hideUGUICanvases) {
			if (GUILayout.Button("Save", GUILayout.Width(screenWidth * 0.5f))) {
				SaveHideCanvases();
			}
		}
		EditorGUILayout.EndHorizontal();

		GUI.changed = false;

		EditorGUILayout.BeginHorizontal();
		saveAsJpg = EditorGUILayout.Toggle(saveAsJpg, GUILayout.Width(15));
		EditorGUILayout.LabelField("Save as JPG");
		if( saveAsJpg)
        {
			jpgQuality = EditorGUILayout.IntField(jpgQuality, GUILayout.Width(30));
			EditorGUILayout.LabelField("Jpg Quality(0-100)");
		}
		
		if (savedSaveAsJpg != saveAsJpg || savedJpgQuality != jpgQuality)
		{
			if (GUILayout.Button("Save", GUILayout.Width(screenWidth * 0.5f)))
			{
				SaveAsJpg();
			}
		} else
        {
			GUILayout.Space(screenWidth * 0.5f);
        }
		EditorGUILayout.EndHorizontal();
		GUI.changed = false;


		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();


		EditorGUILayout.LabelField("Add new resolutions or remove the ones you don't need anymore.");

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField( "Resolution Name", EditorStyles.miniLabel, GUILayout.Width(screenWidth * 0.2f) );
		EditorGUILayout.LabelField( "Width", EditorStyles.miniLabel, GUILayout.Width(screenWidth * 0.2f));
		EditorGUILayout.LabelField( "Height", EditorStyles.miniLabel, GUILayout.Width(screenWidth * 0.2f));
		EditorGUILayout.LabelField( "Delete", EditorStyles.miniLabel, GUILayout.Width(screenWidth * 0.2f));
		EditorGUILayout.LabelField( "Active", EditorStyles.miniLabel, GUILayout.Width(screenWidth * 0.2f));
		EditorGUILayout.EndHorizontal();

		bool canSave = false;

		GUIStyle centeredStyle = GUI.skin.toggle;
		centeredStyle.alignment = TextAnchor.MiddleCenter;

		StoreScreenshotUtils.ScreenshotResolution[] resArray = resolutions.ToArray();
		for (int i = 0; i < resArray.Length; i++) {
			EditorGUILayout.BeginHorizontal();
			if (resArray[i].toDelete) {
				GUI.color = lightRed;
			}
			resArray[i].resName = EditorGUILayout.TextField(resArray[i].resName, GUILayout.Width(screenWidth * 0.2f));
			resArray[i].width = EditorGUILayout.IntField(resArray[i].width, GUILayout.Width(screenWidth * 0.2f));
			resArray[i].height = EditorGUILayout.IntField(resArray[i].height, GUILayout.Width(screenWidth * 0.2f));

			resArray[i].toDelete = EditorGUILayout.Toggle(resArray[i].toDelete, centeredStyle, GUILayout.Width(screenWidth * 0.2f));
			if( !resArray[i].active ) {
				GUI.color = lightRed;
			} else {
				GUI.color = lightGreen;	
			}

			resArray[i].active = EditorGUILayout.Toggle(resArray[i].active, GUILayout.Width(screenWidth * 0.2f));
			GUI.color = defaultColor;

			if (resArray[i].toDelete) {
				GUI.color = defaultColor;
				canSave = true;
			}

			EditorGUILayout.EndHorizontal();

		}

		if (GUI.changed) {
			changesToSave = true;
		}

		resolutions = new List<StoreScreenshotUtils.ScreenshotResolution>(resArray);

		GUI.color = lightRed;
		StoreScreenshotUtils.ScreenshotResolution[] resSaveArray = resolutionsToSave.ToArray();
		for (int i = 0; i < resSaveArray.Length; i++) {
			EditorGUILayout.BeginHorizontal();

			resSaveArray[i].resName = EditorGUILayout.TextField( resSaveArray[i].resName, GUILayout.Width(screenWidth * 0.2f));
			resSaveArray[i].width = EditorGUILayout.IntField( resSaveArray[i].width, GUILayout.Width(screenWidth * 0.2f));
			resSaveArray[i].height = EditorGUILayout.IntField( resSaveArray[i].height, GUILayout.Width(screenWidth * 0.2f));
			resSaveArray[i].active = true;
			EditorGUILayout.EndHorizontal();
		}
		resolutionsToSave = new List<StoreScreenshotUtils.ScreenshotResolution>(resSaveArray);
		GUI.color = defaultColor;

		if (GUILayout.Button( "Add Resolution", GUILayout.Width(screenWidth*0.75f))) {
			resolutionsToSave.Add(new StoreScreenshotUtils.ScreenshotResolution());
		}

		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();

		if (resolutionsToSave.Count != 0 || canSave || changesToSave) {
			EditorGUILayout.LabelField( "Press \"Save Resolutions\" to confirm your changes." );

			GUI.color = lightRed;
			if (GUILayout.Button("Save Resolutions", GUILayout.Height(30)) ) {
				SaveResolutions();
			}
			GUI.color = defaultColor;

			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
		}


		EditorGUILayout.LabelField( "Screenshots are saved in \"Project Folder > Screenshot > Name\"");
		if (GUILayout.Button("Show Screenshot Folder")){
			EditorUtility.RevealInFinder(Application.dataPath + "/../Screenshot/");
		}
	}
}