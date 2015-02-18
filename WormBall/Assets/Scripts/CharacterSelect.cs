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

	public void UpdateSprite(PlayerTextureInfo[] texArr)
	{
		image.sprite = texArr[texIndex].playerSprite;
	}
}

[System.Serializable]
public struct PlayerTextureInfo
{
	public Texture2D playerTex;
	public Sprite playerSprite;
}

public class CharacterSelect : MonoBehaviour 
{
	public PlayerInfo[] playerInfos;

	[SerializeField] PlayerTextureInfo defaultPlayerTexInfo;

	[SerializeField] PlayerTextureInfo[] playerTexInfos;
	List<int>[] teamOpenSpriteIndices = new List<int>[2];

	public int[] teamColorIndices = new int[2]; // colorIndex for each team

	public float inputTime;

	void Awake()
	{
		if(playerInfos.Length < 8)
		{
			Debug.LogError("Player Infos setup incorrectly. There should be 8 total.");
		}

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
		if( Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.LeftArrow) )
		{
			ChangeTeamColor( 0, -1f );
		}

		if( Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.RightArrow) )
		{
			ChangeTeamColor( 0, 1f );
		}

		if( !Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.LeftArrow) )
		{
			ChangeTeamColor( 1, -1f );
		}
		
		if( !Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.RightArrow) )
		{
			ChangeTeamColor( 1, 1f );
		}

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
		
		playerInfos[playerIndex].texIndex = firstSpriteIndex;
		playerInfos[playerIndex].image.sprite = playerTexInfos[firstSpriteIndex].playerSprite;
		playerInfos[playerIndex].image.color = GameManager.instance.colorOptions[teamColorIndices[teamIndex]];
		playerInfos[playerIndex].image.transform.GetChild(0).gameObject.SetActive(false);


		teamOpenSpriteIndices[teamIndex].RemoveAt(0);
	}

	void PlayerLeaveGame(int playerIndex)
	{
		PlayerInfo playerInfo = playerInfos[playerIndex];

		playerInfo.joined = false;
		teamOpenSpriteIndices[playerInfo.teamIndex].Add(playerInfo.texIndex);
		
		playerInfo.image.sprite = defaultPlayerTexInfo.playerSprite;
		playerInfo.image.color = Color.white;
		playerInfos[playerIndex].image.transform.GetChild(0).gameObject.SetActive(true);
	}

	public void ForceAllPlayersLeave()
	{
		for(int i = 0; i < 8; i++)
		{
			if(playerInfos[i].joined)
			{
				PlayerLeaveGame(i);
			}
		}
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
		return playerTexInfos[texIndex].playerTex;
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

			for(int j = 0; j < playerTexInfos.Length; j++)
			{
				bool spriteUnused = true;

				foreach(PlayerInfo playerInfo in playerInfos)
				{
					if(playerInfo.teamIndex == i && playerInfo.joined && 
					   playerInfo.image.sprite == playerTexInfos[j].playerSprite)
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

					playerInfos[playerIndex].UpdateSprite(playerTexInfos);
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

					playerInfos[playerIndex].UpdateSprite(playerTexInfos);
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

	void ChangeTeamColor(int teamIndex, float bumperInput)
	{
		int i = teamColorIndices[teamIndex];
		int otherTeamColorIndex = teamColorIndices[teamIndex == 0 ? 1 : 0];
		while(true)
		{
			i += bumperInput > 0f ? 1 : -1;

			if( i >= GameManager.instance.colorOptions.Length)
			{
				i = 0;
			}

			if( i < 0 )
			{
				i = GameManager.instance.colorOptions.Length - 1;
			}

			if( i != otherTeamColorIndex )
			{
				teamColorIndices[teamIndex] = i;

				for(int j = 0; j < playerInfos.Length; j++)
				{
					if(playerInfos[j].teamIndex == teamIndex && playerInfos[j].joined)
					{
						playerInfos[j].image.color = GameManager.instance.colorOptions[teamColorIndices[teamIndex]];
					}
				}

				return;
			}
		}
	}
}
