using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour 
{
	public static SingletonBehaviour<ScoreManager> singleton = new SingletonBehaviour<ScoreManager>();

	public Text[] team1TimeText;
	public Text[] team2TimeText;

	float team1Time = 0.0f;
	float team2Time = 0.0f;
	
	public void AddTime(int playerNum)
	{
		GameManager gm = GameManager.singleton.instance;

		if(gm.twoPlayer)
		{
			if(playerNum == 1)
			{
				AddTeam1Time();
			}
			else
			{
				AddTeam2Time();
			}
		}
		else
		{
			if(playerNum < 3)
			{
				AddTeam1Time();
			}
			else
			{
				AddTeam2Time();
			}
		}
	}

	void AddTeam1Time()
	{
		team1Time += Time.deltaTime;
		team1TimeText[0].text = ((int)team1Time).ToString("0");
		team1TimeText[1].text = Mathf.Clamp(team1Time % 1.0f, 0.0f, 0.9f).ToString(".0");
	}

	void AddTeam2Time()
	{
		team2Time += Time.deltaTime;
		team2TimeText[0].text = ((int)team2Time).ToString("0");
		team2TimeText[1].text = Mathf.Clamp(team2Time % 1.0f, 0.0f, 0.9f).ToString(".0");
	}
}
