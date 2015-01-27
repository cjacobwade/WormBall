using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class PlayerInfo
{
	public Image image;
	public int texIndex;
	public int playerIndex;
	public int teamIndex;
	public bool joined;
	public float inputTimer = 1000f;

	public void UpdateSprite(Texture2D[] texArr)
	{
		Texture2D spriteTex = texArr[texIndex];
		image.sprite = Sprite.Create(spriteTex, 
		                             new Rect(0f, 0f, spriteTex.width, spriteTex.height),
		                             new Vector2(0.5f, 0.5f));
	}
}

public class CharacterSelect : MonoBehaviour 
{
	public PlayerInfo[] playerInfos;

	[SerializeField] Texture2D defaultPlayerTex;
	Color defaultSpriteColor;

	[SerializeField] Texture2D[] playerTextures;
	List<int>[] teamOpenSpriteIndices = new List<int>[2];
	
	List<int> openTeamColorIndices = new List<int>(); // available team colors

	public int[] teamColorIndices = new int[2]; // colorIndex for each team

	public float inputTime;

	void Awake()
	{
		if(playerInfos.Length < 8)
		{
			Debug.LogError("Player Infos setup incorrectly. There should be 8 total.");
		}

		defaultSpriteColor = playerInfos[0].image.color;

		for(int i = 0; i < 2; i++)
		{
			teamOpenSpriteIndices[i] = new List<int>();
			teamColorIndices[i] = i;
		}

		for(int i = 0; i < 8; i++)
		{
			playerInfos[i].playerIndex = i + 1;
			playerInfos[i].joined = false;
			playerInfos[i].teamIndex = i < 4 ? 0 : 1;
		}
	}

	void Update()
	{
		for(int i = 0; i < 8; i++)
		{
			if(playerInfos[i].inputTimer > inputTime)
			{
				// Join Game
				if(!playerInfos[i].joined && Input.GetButtonDown("Propel_P" + (i + 1) + WadeUtils.platformName))
				{
					PlayerJoinGame(i);
					playerInfos[i].inputTimer = 0f;
				}

				// Leave Game
				if(playerInfos[i].joined && Input.GetButtonDown("Leave_P" + (i + 1) + WadeUtils.platformName))
				{
					PlayerLeaveGame(i);
					playerInfos[i].inputTimer = 0f;
				}

				// Change Texure
				float scrollInput = Input.GetAxis("Horizontal_P" + (i + 1));
				if(playerInfos[i].joined && Mathf.Abs(scrollInput) > WadeUtils.SMALLNUMBER)
				{
					PlayerChangeSprite(i, scrollInput);
					playerInfos[i].inputTimer = 0f;
				}

				// Change Team Color
				float bumperInput = Input.GetAxis("Bumper_P" + (i + 1) + WadeUtils.platformName);
				if(playerInfos[i].joined & Mathf.Abs(bumperInput) > WadeUtils.SMALLNUMBER)
				{
					ChangeTeamColor(i < 4 ? 0 : 1, bumperInput);
					playerInfos[i].inputTimer = 0f;
				}
			}

			playerInfos[i].inputTimer += Time.deltaTime;
		}
	}

	void PlayerJoinGame(int playerIndex)
	{
		CheckAvailableSprites();

		playerInfos[playerIndex].joined = true;

		int teamIndex = playerInfos[playerIndex].teamIndex;
		int firstSpriteIndex = teamOpenSpriteIndices[teamIndex][0];

		Texture2D spriteTex = playerTextures[firstSpriteIndex];
		playerInfos[playerIndex].texIndex = firstSpriteIndex;
		playerInfos[playerIndex].image.sprite = Sprite.Create(spriteTex, 
		                                                      new Rect(0f, 0f, spriteTex.width, spriteTex.height),
		                                                      new Vector2(0.5f, 0.5f));
		playerInfos[playerIndex].image.color = GameManager.instance.colorOptions[teamColorIndices[teamIndex]];


		teamOpenSpriteIndices[teamIndex].RemoveAt(0);
	}

	void PlayerLeaveGame(int playerIndex)
	{
		PlayerInfo playerInfo = playerInfos[playerIndex];

		playerInfo.joined = false;
		teamOpenSpriteIndices[playerInfo.teamIndex].Add(playerInfo.texIndex);

		Texture2D spriteTex = defaultPlayerTex;
		playerInfo.image.sprite = Sprite.Create(spriteTex, 
		                                        new Rect(0f, 0f, spriteTex.width, spriteTex.height),
		                                        new Vector2(0.5f, 0.5f));

		playerInfo.image.color = defaultSpriteColor;
	}

	public PlayerInfo[] GetPlayerInfos()
	{
		List<PlayerInfo> outPlayerInfos = new List<PlayerInfo>();
		
		foreach(PlayerInfo playerInfo in playerInfos)
		{
			if(playerInfo.joined)
			{
				outPlayerInfos.Add(playerInfo);
			}
		}
		
		return outPlayerInfos.ToArray();
	}

	public Texture2D GetPlayerTex(int texIndex)
	{
		return playerTextures[texIndex];
	}

	public PlayerInfo[] GetTeamPlayerInfos(int teamIndex)
	{
		List<PlayerInfo> outPlayerInfos = new List<PlayerInfo>();

		foreach(PlayerInfo playerInfo in playerInfos)
		{
			if(playerInfo.joined && playerInfo.teamIndex == teamIndex)
			{
				outPlayerInfos.Add(playerInfo);
			}
		}

		return outPlayerInfos.ToArray();
	}

	public int GetJoinedPlayerCount()
	{
		int playerCount = 0;

		foreach(PlayerInfo playerInfo in playerInfos)
		{
			if(playerInfo.joined)
			{
				playerCount++;
			}
		}

		return playerCount;
	}

	public int GetJoinedPlayerCountPerTeam(int teamIndex)
	{
		int playerCount = 0;

		for(int i = teamIndex * 4; i < teamIndex * 4 + 4; i++)
		{
			if(playerInfos[i].joined)
			{
				playerCount++;
			}
		}

		return playerCount;
	}

	public bool IsReadyToPlay()
	{
		return IsTeamReady(0) && IsTeamReady(1);
	}

	bool IsTeamReady(int teamIndex)
	{
		for(int i = teamIndex * 4; i < teamIndex * 4 + 4; i++)
		{
			if(playerInfos[i].joined)
			{
				return true;
			}
		}

		return false;
	}

	void CheckAvailableSprites()
	{
		for(int i = 0; i < 2; i++)
		{
			teamOpenSpriteIndices[i].Clear();

			for(int j = 0; j < playerTextures.Length; j++)
			{
				bool spriteUnused = true;

				foreach(PlayerInfo playerInfo in playerInfos)
				{
					if(playerInfo.teamIndex == i && playerInfo.joined && 
					   playerInfo.image.sprite.texture == playerTextures[j])
					{
						spriteUnused = false;
					}
				}

				if(spriteUnused)
				{
					teamOpenSpriteIndices[i].Add(j);
				}
			}
		}
	}

	void PlayerChangeSprite(int playerIndex, float scrollAmount)
	{
		CheckAvailableSprites();

		int teamIndex = playerInfos[playerIndex].teamIndex;
		teamOpenSpriteIndices[teamIndex].Sort();

		int i = playerInfos[playerIndex].texIndex;
		while(true)
		{
			if(i > teamOpenSpriteIndices[teamIndex].Count - 1)
			{
				i = 0;
			}
			else if(i < 0)
			{
				i = teamOpenSpriteIndices[teamIndex].Count - 1;
			}
			
			if(scrollAmount > 0f)
			{
				if(teamOpenSpriteIndices[teamIndex][i] != playerInfos[playerIndex].texIndex)
				{
					teamOpenSpriteIndices[teamIndex].Add(playerInfos[playerIndex].texIndex);
					playerInfos[playerIndex].texIndex = teamOpenSpriteIndices[teamIndex][i];
					teamOpenSpriteIndices[teamIndex].Remove(teamOpenSpriteIndices[teamIndex][i]);

					playerInfos[playerIndex].UpdateSprite(playerTextures);
					break;
				}

				i++;
				if(i > teamOpenSpriteIndices[teamIndex].Count - 1)
				{
					i = 0;
				}
			}
			else
			{
				if(teamOpenSpriteIndices[teamIndex][i] != playerInfos[playerIndex].texIndex)
				{
					teamOpenSpriteIndices[teamIndex].Add(playerInfos[playerIndex].texIndex);
					playerInfos[playerIndex].texIndex = teamOpenSpriteIndices[teamIndex][i];
					teamOpenSpriteIndices[teamIndex].Remove(teamOpenSpriteIndices[teamIndex][i]);

					playerInfos[playerIndex].UpdateSprite(playerTextures);
					break;
				}
				
				i--;
				if(i < 0)
				{
					i = teamOpenSpriteIndices[teamIndex].Count - 1;
				}
			}
		}
	}

	void CheckAvailableColors()
	{
		openTeamColorIndices.Clear();
		
		for(int i = 0; i < GameManager.instance.colorOptions.Length; i++)
		{ 
			bool colorUsed = false;
			for(int j = 0; j < teamColorIndices.Length; j++)
			{
				if(teamColorIndices[j] == i)
				{
					colorUsed = true;
				}
			}

			if(!colorUsed)
			{
				openTeamColorIndices.Add(i);
			}
		}
	}

	void ChangeTeamColor(int teamIndex, float bumperInput)
	{
		CheckAvailableColors();

		openTeamColorIndices.Sort();

		int i = teamColorIndices[teamIndex];
		while(true)
		{
			if(i >= openTeamColorIndices.Count)
			{
				i = 0;
			}
			else if(i < 0)
			{
				i = openTeamColorIndices.Count - 1;
			}

			if(bumperInput > 0f)
			{
				if(openTeamColorIndices[i] != teamColorIndices[teamIndex])
				{
					openTeamColorIndices.Add(teamColorIndices[teamIndex]);
					teamColorIndices[teamIndex] = openTeamColorIndices[i];
					openTeamColorIndices.Remove(openTeamColorIndices[i]);
				
					for(int j = 4 * teamIndex; j < 4 * teamIndex + 4; j++)
					{
						if(playerInfos[j].joined)
						{
							playerInfos[j].image.color = GameManager.instance.colorOptions[teamColorIndices[teamIndex]];
						}
					}

					break;
				}
				
				i++;
				if(i >= openTeamColorIndices.Count)
				{
					i = 0;
				}
			}
			else
			{
				if(openTeamColorIndices[i] != teamColorIndices[teamIndex])
				{
					openTeamColorIndices.Add(teamColorIndices[teamIndex]);
					teamColorIndices[teamIndex] = openTeamColorIndices[i];
					openTeamColorIndices.Remove(openTeamColorIndices[i]);
					
					for(int j = 4 * teamIndex; j < 4 * teamIndex + 4; j++)
					{
						if(playerInfos[j].joined)
						{
							playerInfos[j].image.color = GameManager.instance.colorOptions[teamColorIndices[teamIndex]];
						}
					}

					break;
				}
				
				i--;
				if(i < 0)
				{
					i = openTeamColorIndices.Count - 1;
				}
			}
		}
	}
}
