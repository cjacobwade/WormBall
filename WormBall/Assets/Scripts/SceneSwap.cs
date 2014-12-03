using UnityEngine;
using System.Collections;

public class SceneSwap : MonoBehaviour 
{
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Equals))
		{
			if(Application.loadedLevel >= Application.levelCount - 1)
			{
				Application.LoadLevel(0);
			}
			else
			{
				Application.LoadLevel(Application.loadedLevel + 1);
			}
		}
	}
}
