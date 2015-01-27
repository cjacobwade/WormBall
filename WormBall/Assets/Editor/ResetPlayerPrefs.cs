using UnityEngine;
using UnityEditor;
using System.Collections;

public class ResetPlayerPrefs : MonoBehaviour 
{

	[MenuItem("Edit/Reset Playerprefs")] 
	public static void DeletePlayerPrefs() 
	{ 
		PlayerPrefs.DeleteAll(); 
	}
}
