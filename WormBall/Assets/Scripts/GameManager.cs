using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public enum GameState
{
	Menu				= 0,
	CharacterSelect		= 1,
	Game				= 2,
	EndGame				= 3
}

public enum GameType
{
	WrapBall		= 0,
	Soccer			= 1,
	KOTH			= 2
}

public class GameManager : SingletonBehaviour<GameManager> 
{
	public GameState gameState;
	GameState prevState;

	[HideInInspector] public bool twoPlayer = true;
	public GameObject ballPrefab;

	[SerializeField] GameObject menuObj;
	[SerializeField] GameObject characterSelectObj;
	[SerializeField] GameObject gameObj;
	[SerializeField] GameObject endGameObj;
	[SerializeField] WormText endWinnerText;

	[SerializeField] Color[] colorOptions;
	Color team1Color;
	Color team2Color;

	float modeTime = 0.0f;

	void Awake () 
	{
		gameState = GameState.Menu;
		prevState = gameState;

		UpdateState();
	}

	void FixedUpdate () 
	{
		if(Time.frameCount % 30 == 0)
		{
			WadeUtils.CheckForController();
		}

		if(gameState != prevState)
		{
			UpdateState();
		}
		else
		{
			modeTime += Time.deltaTime;
		}

		if(gameState == GameState.Menu)
		{
			for(int i = 0; i < 4; i++)
			{
//				if(Input.GetAxis("Horizontal-P" + (i + 1)) < -0.1f)
//				{
//					StartGame(true);
//				}
//				else if(Input.GetAxis("Horizontal-P" + (i + 1)) > 0.1f)
//				{
//					StartGame(false);
//				}
			}
		}
		else if(Input.GetKeyDown(KeyCode.Escape))
		{
			ChangeGameState(GameState.Menu);
		}

		if(gameState == GameState.EndGame)
		{
			if(Input.anyKeyDown)
			{
				ChangeGameState(GameState.Menu);
			}
		}
	}

	void ChangeGameState(GameState newState)
	{
		if(modeTime >= 1.5f)
		{
			gameState = newState;
		}
	}

	public void StartGame(bool inTwoPlayer)
	{
		twoPlayer = inTwoPlayer;
		ChangeGameState(GameState.Game);
	}

	public void ResetBall()
	{
		Ball[] balls = GameObject.FindObjectsOfType<Ball>();
		foreach(Ball ball in balls)
		{
			if(ball)
			{
				Destroy(ball.gameObject);
			}
		}

		WadeUtils.Instantiate(ballPrefab);
	}

	void UpdateState()
	{
		modeTime = 0.0f;

		// Cleanup old state
		switch(prevState)
		{
		case GameState.Menu:
			MenuCleanup();
			break;
		case GameState.CharacterSelect:
			CharacterSelectCleanup();
			break;
		case GameState.Game:
			GameCleanup();
			break;
		case GameState.EndGame:
			EndGameCleanup();
			break;
		}

		// Setup new state
		switch(gameState)
		{
		case GameState.Menu:
			MenuSetup();
			break;
		case GameState.CharacterSelect:
			CharacterSelectSetup();
			break;
		case GameState.Game:
			GameSetup();
			break;
		case GameState.EndGame:
			EndGameSetup();
			break;
		}

		prevState = gameState;
	}

	void MenuSetup()
	{
		menuObj.SetActive(true);
		ScoreManager.instance.drawCircle.enabled = true;
		// Fade in menu music

		ResetBall();
	}

	void MenuCleanup()
	{
		menuObj.SetActive(false);
		// Fade out menu music
	}

	void CharacterSelectSetup()
	{
		characterSelectObj.SetActive(true);
		// Fade in pre-game music
	}

	void CharacterSelectCleanup()
	{
		characterSelectObj.SetActive(false);
		// Fade out pre-game music
	}

	void GameSetup()
	{
		gameObj.SetActive(true);

		List<Color> colors = colorOptions.ToList();
		team1Color = colors[Random.Range(0, colors.Count - 1)];
		colors.Remove(team1Color);
		team2Color = colors[Random.Range(0, colors.Count - 1)];

		ScoreManager sm = ScoreManager.instance;
		sm.team1TimeText.ToList().ForEach( text => text.color = team1Color);
		sm.team2TimeText.ToList().ForEach( text => text.color = team2Color);

		sm.ResetScore();
		sm.ResetTimer();

		if(twoPlayer)
		{
			TwoPlayer();
		}
		else
		{
			FourPlayer();
		}

		// Enable score manager

		// Game start choreo

		// Spawn players
		// Fade in game music
	}

	void TwoPlayer()
	{
		for(int i = 0; i < 2; i++)
		{
			Vector3 rand = Random.insideUnitSphere * 0.4f;
			rand += Vector3.one * 0.6f;
			
			Vector3 spawnPos;
			Quaternion spawnRot;
			bool lookUp = true;
			if(i < 1)
			{
				spawnRot = Quaternion.identity;
				spawnPos = transform.position - Vector3.right * 5.0f;
				spawnPos += Vector3.up * 3.0f;
			}
			else
			{
				lookUp = false;
				spawnRot = Quaternion.Euler(0.0f, 0.0f, 180.0f);
				spawnPos = transform.position + Vector3.right * 5.0f;
				spawnPos -= Vector3.up * 3.0f;
			}

			Color spawnColor = i < 1 ? team1Color : team2Color;

			GameObject wormObj = WormManager.instance.CreateWorm(spawnPos, lookUp, i + 1, spawnColor).gameObject;
			wormObj.transform.rotation = spawnRot;
			wormObj.transform.parent.name = "P" + (i + 1);
			wormObj.name = "Worm";
			wormObj.GetComponentInChildren<SpriteRenderer>().color = spawnColor - new Color(0.15f, 0.15f, 0.15f, 0.0f);
		}
	}

	void FourPlayer()
	{
		for(int i = 0; i < 6; i++)
		{
			Vector3 rand = Random.insideUnitSphere * 0.4f;
			rand += Vector3.one * 0.6f;
			
			Vector3 spawnPos;
			Quaternion spawnRot;
			bool lookUp = true;
			if(i < 3)
			{
				spawnRot = Quaternion.identity;
				spawnPos = transform.position - Vector3.right * 5.0f * (i + 1);
				spawnPos += Vector3.up * 3.0f;
			}
			else
			{
				lookUp = false;
				spawnRot = Quaternion.Euler(0.0f, 0.0f, 180.0f);
				spawnPos = transform.position + Vector3.right * 5.0f * (i - 1);
				spawnPos -= Vector3.up * 3.0f;
			}

			Color spawnColor = i < 3 ? team1Color : team2Color;

			if(i % 3 != 0)
			{
				spawnColor += new Color(0.25f, 0.25f, 0.25f, 0.0f);
			}

			GameObject wormObj = WormManager.instance.CreateWorm(spawnPos, lookUp, i + 1, spawnColor).gameObject;
			wormObj.transform.rotation = spawnRot;
			wormObj.transform.parent.name = "P" + (i + 1);
			wormObj.name = "Worm";
			wormObj.GetComponentInChildren<SpriteRenderer>().color = spawnColor - new Color(0.15f, 0.15f, 0.15f, 0.0f);
		}
	}

	void GameCleanup()
	{
		if(gameState != GameState.EndGame)
		{
			gameObj.SetActive(false);
			// Disable score manager
			// Remove players
			WormManager.instance.DestroyAllWorms();
		}

		// Fade out game music
	}

	public void EndGame(int winNum)
	{
		Color winnerColor = (winNum == 1) ? team1Color : team2Color;
		endWinnerText.SetColor(winnerColor, winnerColor);

		if(winNum == 1)
		{
			WormManager.instance.DestroyTeam(2);
		}
		else
		{
			WormManager.instance.DestroyTeam(1);
		}

		endWinnerText.text = "Team " + winNum + " Wins";

		GameManager.instance.gameState = GameState.EndGame;
		ScoreManager.instance.drawCircle.enabled = false;
	}

	void EndGameSetup()
	{
		WormManager.instance.EnableWormInput(false);

		// End game choreo
			// Wiggling victory text: WINNERS w/ player names
			// Move score to middle
			// Fade losers to black/bg color

		endGameObj.SetActive(true);
		// Fade in end music
	}

	void EndGameCleanup()
	{
		gameObj.SetActive(false);
		endGameObj.SetActive(false);
		WormManager.instance.DestroyAllWorms();
		ScoreManager.instance.ResetTimer();

		// Fade out end music
	}
}
