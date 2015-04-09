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
	
	public float totalGameTime = 45f;
	float gameTime = 0.0f;

	public DrawCircle drawCircle = null;
	[SerializeField] float MaxCircleSize = 11.5f;
	[SerializeField] float circlePulseSpeed = 0.7f;
	[SerializeField] float circleReturnSpeed = 2f;
	Color initCircleColor;

	[SerializeField] Image ittyBittyLogo;

	bool suddenDeath = false;

	void Awake()
	{
		gameTime = totalGameTime;
		initCircleColor = drawCircle.GetComponent<Renderer>().material.GetColor( "_Tint" );
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

	public void ResetTimerColor( float time)
	{
		StartCoroutine( ShiftTimerColor( initCircleColor, time ) );
	}

	IEnumerator ShiftTimerColor( Color targetColor, float time )
	{
		Color initColor = drawCircle.GetComponent<Renderer>().material.GetColor( "_Tint" );
		float timer = 0f;

		while( timer < time )
		{
			Color lerpColor = Color.Lerp( initColor, targetColor, timer/time );
			drawCircle.GetComponent<Renderer>().material.SetColor( "_Tint", lerpColor );
			ittyBittyLogo.color = lerpColor;

			timer += Time.deltaTime;
			yield return 0;
		}

		drawCircle.GetComponent<Renderer>().material.SetColor( "_Tint", targetColor );
		ittyBittyLogo.color = targetColor;
	}

	public void SetTimerColor( Color color )
	{
		drawCircle.GetComponent<Renderer>().material.SetColor( "_Tint", color );
		ittyBittyLogo.color = color;
	}

	public void AddTime(int playerNum)
	{
		if(GameManager.instance.gameState == GameState.Game)
		{
			if(playerNum <= 4)
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
