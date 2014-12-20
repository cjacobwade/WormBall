using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugWormSpeed : MonoBehaviour 
{
	Text uiText;

	// Use this for initialization
	void Awake () 
	{
		uiText = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Time.frameCount % 5 == 0)
		{
			uiText.text = "";
		
			foreach(Worm worm in GameObject.FindObjectsOfType<Worm>())
			{
				uiText.text += "Worm " + worm.playerNum + " speed boost: " + worm.speedBoost.ToString("F2") + "\n";
			}
		}
	}
}
