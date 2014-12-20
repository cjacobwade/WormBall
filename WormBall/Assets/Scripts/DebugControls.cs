using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugControls : MonoBehaviour 
{
	Text uiText;

	void Awake()
	{
		uiText = GetComponent<Text>();
	}

	public void SetSchemeText(string schemeText)
	{
		uiText.text = "Current Control Scheme:\n" + schemeText;
	}
}
