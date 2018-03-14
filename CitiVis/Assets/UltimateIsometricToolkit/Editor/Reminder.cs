using UnityEngine;
using UnityEditor;
using System;

public class Reminder : EditorWindow {
	private bool showNotAgain;
	[MenuItem("Tools/UIT/Reminder")]
	public static void ShowWindow()
	{
		GetWindow<Reminder>("UIT news").ShowTab();
	}

	void OnGUI()
	{
		EditorGUILayout.TextArea("Thanks for purchasing my asset.\nThe best place to start is the online documentation \n http://code-beans.com/UltimateIsometricToolkit/documentation/ .\n  \n Please leave a comment and rating in the asset store. Thanks");
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button(new GUIContent("Comment & Rate", "Opens the asset store page")))
		{
			Help.BrowseURL("https://www.assetstore.unity3d.com/en/#!/content/33032");
			EditorPrefs.SetString("lastReminded", DateTime.Now.ToString("hh.mm.ss"));
		}
		if (GUILayout.Button(new GUIContent("Open documentation", "Opens online documentation in browser"))) {
			Help.BrowseURL("http://code-beans.com/UltimateIsometricToolkit/documentation/");
		}
		EditorGUILayout.EndHorizontal();
		showNotAgain = EditorPrefs.GetBool("showNotAgain23",false);
		showNotAgain = EditorGUILayout.Toggle("Do not show again", showNotAgain);
		EditorPrefs.SetBool("showNotAgain23", showNotAgain);

	}
}
