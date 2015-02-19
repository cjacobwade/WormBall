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
	[SerializeField] float multiBallSpawnOffset = 1f;

	[SerializeField] GameObject menuObj;
	[SerializeField] CharacterSelect characterSelect;
	[SerializeField] GameObject gameObj;
	[SerializeField] GameObject endGameObj;
	[SerializeField] WormText endWinnerText;

	public Color[] colorOptions;
	public Color[] complimentaryColors;
	Color team1Color;
	Color team2Color;

	float modeTime = 0.0f;

	void Awake () 
	{
		gameState = GameState.Menu;
		prevState = gameState;

		UpdateState();


	}

	void Update () 
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
			if(Input.anyKeyDown)
			{
				ChangeGameState(GameState.CharacterSelect);
			}
		}
		else if(Input.GetKeyDown(KeyCode.Escape))
		{
			Application.LoadLevel(Application.loadedLevel);
		}

		if(gameState == GameState.CharacterSelect)
		{
			if( modeTime > 3f )
			{
				for(int i = 0; i < 8; i++)
				{
					if(characterSelect.playerInfos[i].joined && characterSelect.IsReadyToPlay() &&
					   characterSelect.playerInfos[i].inputTimer > characterSelect.inputTime &&
					   Input.GetButtonDown("Start" + WadeUtils.platformName))
					{
						StartGame();
					}
				}
			}
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

	public void StartGame()
	{
		ChangeGameState(GameState.Game);
	}

	public void DestroyAllBalls()
	{
		Ball[] balls = GameObject.FindObjectsOfType<Ball>();
		foreach(Ball ball in balls)
		{
			if(ball)
			{
				Destroy(ball.gameObject);
			}
		}
	}

	public void ResetBall()
	{
		DestroyAllBalls();

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
		characterSelect.ForceAllPlayersLeave();
		characterSelect.gameObject.SetActive(true);
		DestroyAllBalls();
		// Fade in pre-game music
	}

	void CharacterSelectCleanup()
	{
		characterSelect.gameObject.SetActive(false);
		// Fade out pre-game music
	}

	void GameSetup()
	{
		gameObj.SetActive(true);

		team1Color = colorOptions[characterSelect.teamColorIndices[0]];
		team2Color = colorOptions[characterSelect.teamColorIndices[1]];

		ScoreManager sm = ScoreManager.instance;
		sm.team1TimeText.ToList().ForEach( text => text.color = team1Color);
		sm.team2TimeText.ToList().ForEach( text => text.color = team2Color);

		sm.ResetScore();
		sm.ResetTimer();

		int timerColor = characterSelect.teamColorIndices[0] + characterSelect.teamColorIndices[1];

		// This is a hack lol
		if( characterSelect.teamColorIndices[0] > 2 &&
		    characterSelect.teamColorIndices[1] > 2 )
		{
			timerColor += 6;
		}
		else if( characterSelect.teamColorIndices[0] > 1 &&
		         characterSelect.teamColorIndices[1] > 1 )
		{
			timerColor += 5;
		}
		else if( characterSelect.teamColorIndices[0] > 0 &&
		         characterSelect.teamColorIndices[1] > 0 )
		{
			timerColor += 3;
		}

		sm.SetTimerColor( complimentaryColors[timerColor - 1] );

		DestroyAllBalls();

		if(characterSelect.GetPlayerInfos().Length > 4)
		{
			WadeUtils.Instantiate(ballPrefab, Vector3.zero + Vector3.up * multiBallSpawnOffset, Quaternion.identity);
			WadeUtils.Instantiate(ballPrefab, Vector3.zero - Vector3.up * multiBallSpawnOffset, Quaternion.identity);
		}
		else
		{
			WadeUtils.Instantiate(ballPrefab);
		}

		SpawnPlayers();

		// Spawn players
		// Fade in game music
	}

	void SpawnPlayers()
	{
	 	PlayerInfo[] playerInfos = characterSelect.GetPlayerInfos();

		int team = 0;
		int playerNum = 0;

		bool lookUp = false;
		Color spawnColor = Color.white;

		Texture2D playerTex = null;

		Vector3 spawnPos = transform.position;
		Quaternion spawnRot = Quaternion.identity;

		int team1Count = playerInfos.Count( r => r.playerIndex < 5) + 1; // starts at the number of players on the first team
		int team2Count = 0;
		for(int i = 0; i < playerInfos.Length; i++)
		{
			playerNum = playerInfos[i].playerIndex;
			team = playerInfos[i].teamIndex;

			spawnPos = transform.position;
			playerTex = characterSelect.GetPlayerTex(playerInfos[i].texIndex);	

			if( team == 0 )
			{
				team1Count--;
				lookUp = true;
				spawnColor = team1Color;

				spawnPos += -Vector3.right * 4f * team1Count;
				spawnPos += Vector3.up * 3f;

				spawnRot = Quaternion.identity;
			}
			else
			{
				team2Count++;
				lookUp = false;
				spawnColor = team2Color;

				spawnPos += Vector3.right * 4f * team2Count;
				spawnPos += -Vector3.up * 3f;

				spawnRot = Quaternion.Euler( 0f, 0f, 180f );
			}
				
			GameObject wormObj = WormManager.instance.CreateWorm(spawnPos, 
			                                                     lookUp, 
			                                                     playerNum, 
			                                                     spawnColor, 
			                                                     playerTex).gameObject;

			wormObj.transform.rotation = spawnRot;
			wormObj.transform.parent.name = "P" + playerNum;
			wormObj.name = "Worm";
			wormObj.GetComponentInChildren<SpriteRenderer>().color = spawnColor - new Color( 0.2f, 0.2f, 0.2f, 0f);
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

		ScoreManager.instance.ResetTimerColor();

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
