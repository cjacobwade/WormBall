using UnityEngine;
using System.Collections;

public enum GameState
{
	Menu				= 0,
	CharacterSelect		= 1,
	Game				= 2,
	EndGame				= 3
}

public class GameManager : MonoBehaviour 
{
	GameState gameState;
	GameState prevState;

	bool twoPlayer = true;

	[SerializeField] GameObject menuObj;
	[SerializeField] GameObject characterSelectObj;
	[SerializeField] GameObject gameObj;
	[SerializeField] GameObject endGameObj;

	void Awake () 
	{
		gameState = GameState.Menu;
		prevState = gameState;

		UpdateState();
	}

	void Update () 
	{
		if(gameState != prevState)
		{
			UpdateState();
		}

		if(gameState == GameState.Menu)
		{
			if(Input.GetKeyDown(KeyCode.LeftArrow))
			{
				twoPlayer = true;
				gameState = GameState.Game;
			}
			else if(Input.GetKeyDown(KeyCode.RightArrow))
			{
				twoPlayer = false;
				gameState = GameState.Game;
			}
		}
	}

	void UpdateState()
	{
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
		// Fade in menu music
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
		Debug.Log("Spawn");

		gameObj.SetActive(true);

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
			Color randColor = new Vector4(rand.x, rand.y, rand.z, 1.0f);
			
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
			
			GameObject wormObj = WormManager.singleton.instance.CreateWorm(spawnPos, lookUp, i + 1, randColor).gameObject;
			wormObj.transform.rotation = spawnRot;
			wormObj.transform.parent.name = "P" + (i + 1);
			wormObj.name = "Worm";
			wormObj.GetComponentInChildren<SpriteRenderer>().color = randColor;
		}
	}

	void FourPlayer()
	{
		for(int i = 0; i < 4; i++)
		{
			Vector3 rand = Random.insideUnitSphere * 0.4f;
			rand += Vector3.one * 0.6f;
			Color randColor = new Vector4(rand.x, rand.y, rand.z, 1.0f);
			
			Vector3 spawnPos;
			Quaternion spawnRot;
			bool lookUp = true;
			if(i < 2)
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
			
			GameObject wormObj = WormManager.singleton.instance.CreateWorm(spawnPos, lookUp, i + 1, randColor).gameObject;
			wormObj.transform.rotation = spawnRot;
			wormObj.transform.parent.name = "P" + (i + 1);
			wormObj.name = "Worm";
			wormObj.GetComponentInChildren<SpriteRenderer>().color = randColor;
		}
	}

	void GameCleanup()
	{
		if(gameState != GameState.EndGame)
		{
			gameObj.SetActive(false);
			// Disable score manager
			// Remove players
		}

		// Fade out game music
	}

	void EndGameSetup()
	{
		WormManager.singleton.instance.EnableWormInput(false);

		// End game choreo
			// Wiggling victory text: WINNERS w/ player names
			// Move score to middle
			// Fade losers to black/bg color

		endGameObj.SetActive(true);
		// Fade in end music
	}

	void EndGameCleanup()
	{
		endGameObj.SetActive(false);
		WormManager.singleton.instance.DestroyAllWorms();

		// Fade out end music
	}
}
