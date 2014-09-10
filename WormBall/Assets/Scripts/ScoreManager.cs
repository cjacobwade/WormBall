using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour 
{
	public static SingletonBehaviour<ScoreManager> singleton = new SingletonBehaviour<ScoreManager>();

	[SerializeField] Text[] p1TimeText;
	[SerializeField] Text[] p2TimeText;

	float p1Time = 0.0f;
	float p2Time = 0.0f;
	
	public void AddTime(int playerNum)
	{
		if(playerNum == 1)
		{
			p1Time += Time.deltaTime;
			p1TimeText[0].text = ((int)p1Time).ToString("0");
			p1TimeText[1].text = Mathf.Clamp(p1Time % 1.0f, 0.0f, 0.9f).ToString(".0");
		}
		else
		{
			p2Time += Time.deltaTime;
			p2TimeText[0].text = ((int)p2Time).ToString("0");
			p2TimeText[1].text = Mathf.Clamp(p2Time % 1.0f, 0.0f, 0.9f).ToString(".0");
		}
	}
}
