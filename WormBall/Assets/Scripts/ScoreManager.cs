using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreManager : SingletonBehaviour<ScoreManager>
{
	public Text[] team1TimeText;
	public Text[] team2TimeText;

	public GameObject suddenDeathText;

	float team1Time = 0.0f;
	float team2Time = 0.0f;
	
	[SerializeField] float totalGameTime = 30.0f;
	float gameTime = 0.0f;

	public DrawCircle drawCircle;
	[SerializeField] float MaxCircleSize = 11.5f;
	[SerializeField] float circlePulseSpeed = 0.7f;
	[SerializeField] float circleReturnSpeed = 2f;

	bool suddenDeath = false;

	void Awake()
	{
		gameTime = totalGameTime;
	}

	void Update()
	{
		if(GameManager.instance.gameState == GameState.Game)
		{
			if(gameTime <= 0.0f)
			{
				if(Mathf.Abs(team1Time - team2Time) > 0.1f)
				{
					GameManager.instance.EndGame(team1Time > team2Time ? 1 : 2);
					suddenDeathText.SetActive(false);
					drawCircle.ReturnToSize(circleReturnSpeed);
					suddenDeath = false;
				}
				else if(!suddenDeath)
				{
					team1TimeText[0].enabled = false;
					team1TimeText[1].enabled = false;

					team2TimeText[0].enabled = false;
					team2TimeText[1].enabled = false;

					suddenDeathText.SetActive(true);
					drawCircle.fillAmount = 1f;

					drawCircle.PingPongSize(MaxCircleSize, circlePulseSpeed);

					suddenDeath = true;
				}
			}
			else
			{
				gameTime -= Time.deltaTime;
				drawCircle.fillAmount = gameTime/totalGameTime;
			}
		}
	}

	public void ResetScore()
	{
		team1Time = 0.0f;

		team1TimeText[0].text = ((int)team1Time).ToString("0");
		team1TimeText[1].text = Mathf.Clamp(team1Time % 1.0f, 0.0f, 0.9f).ToString(".0");

		team2Time = 0.0f;

		team2TimeText[0].text = ((int)team2Time).ToString("0");
		team2TimeText[1].text = Mathf.Clamp(team2Time % 1.0f, 0.0f, 0.9f).ToString(".0");

		team1TimeText[0].enabled = true;
		team1TimeText[1].enabled = true;
		
		team2TimeText[0].enabled = true;
		team2TimeText[1].enabled = true;
	}

	public void ResetTimer()
	{
		gameTime = totalGameTime;
		drawCircle.fillAmount = gameTime/totalGameTime;
	}

	public void AddTime(int playerNum)
	{
		if(GameManager.instance.gameState == GameState.Game)
		{
			if(GameManager.instance.twoPlayer)
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
