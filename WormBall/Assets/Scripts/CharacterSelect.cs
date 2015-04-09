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
			teamColorIndices[i] = i;
		}

		for(int i = 0; i < 8; i++)
		{
			playerInfos[i].playerIndex = i + 1;
			playerInfos[i].joined = false;
			playerInfos[i].teamIndex = i < 4 ? 0 : 1;
		}

		ForceAllPlayersLeave();
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

		// 1 -> 1
		// 2 -> 5
		// 3 -> 2
		// 4 -> 6
		// 5 -> 3
		// 6 -> 7
		// 7 -> 4
		// 8 -> 8
		
		for(int i = 0; i < 8; i++)
		{
			int evenTeamNum = WadeUtils.GetOrderedPlayerNum( i );

			if(playerInfos[evenTeamNum].inputTimer > inputTime)
			{
				// Join Game
				if(!playerInfos[evenTeamNum].joined && Input.GetButtonDown("Propel_P" + (i + 1) + WadeUtils.platformName))
				{
					PlayerJoinGame(evenTeamNum, i);
					playerInfos[evenTeamNum].inputTimer = 0f;
				}

				// Leave Game
				if(playerInfos[evenTeamNum].joined && Input.GetButtonDown("Leave_P" + (i + 1) + WadeUtils.platformName))
				{
					PlayerLeaveGame(evenTeamNum);
					playerInfos[evenTeamNum].inputTimer = 0f;
				}

				// Change Texure
				float scrollInput = Input.GetAxis("Horizontal_P" + (i + 1));
				if(playerInfos[evenTeamNum].joined && Mathf.Abs(scrollInput) > WadeUtils.SMALLNUMBER)
				{
					PlayerChangeSprite(evenTeamNum, scrollInput);
					playerInfos[evenTeamNum].inputTimer = 0f;
				}

				// Change Team Color
				float bumperInput = Input.GetAxis("Bumper_P" + (i + 1) + WadeUtils.platformName);
				if(playerInfos[evenTeamNum].joined & Mathf.Abs(bumperInput) > WadeUtils.SMALLNUMBER)
				{
					ChangeTeamColor(evenTeamNum < 4 ? 0 : 1, bumperInput);
					playerInfos[evenTeamNum].inputTimer = 0f;
				}
			}

			playerInfos[evenTeamNum].inputTimer += Time.deltaTime;
		}
	}

	void PlayerJoinGame(int playerNum, int playerIndex)
	{
		playerInfos[playerNum].joined = true;
		playerInfos[playerNum].playerIndex = playerIndex + 1;

		int teamIndex = playerInfos[playerNum].teamIndex;

		int i = 0;
		int[] availableSpriteIndices = GetAvailableSpriteIndices(playerNum);
		while(true)
		{
			if( availableSpriteIndices.Contains(i) )
			{
				playerInfos[playerNum].texIndex = i;
				playerInfos[playerNum].UpdateSprite( playerTexInfos );
				break;
			}

			i++;
			WadeUtils.WrapAround(ref i, 0, playerTexInfos.Length - 1);
		}

		playerInfos[playerNum].image.color = GameManager.instance.colorOptions[teamColorIndices[teamIndex]];
		playerInfos[playerNum].image.transform.GetChild(0).gameObject.SetActive(false);
	}

	void PlayerLeaveGame(int playerIndex)
	{
		PlayerInfo playerInfo = playerInfos[playerIndex];

		playerInfo.joined = false;
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

	int[] GetAvailableSpriteIndices(int playerIndex)
	{
		List<int> texIndexList = new List<int>();

		for(int i = 0; i < playerTexInfos.Length; i++)
		{
			bool texUsed = false;
			foreach(PlayerInfo playerInfo in playerInfos)
			{
				if( playerInfo.teamIndex == playerInfos[playerIndex].teamIndex && 
				    playerInfo.joined && 
				    playerInfo.image.sprite == playerTexInfos[i].playerSprite )
				{
					texUsed = true;
				}
			}

			if( !texUsed )
			{
				texIndexList.Add( i );
			}
		}

		return texIndexList.ToArray();
	}

	void PlayerChangeSprite(int playerIndex, float scrollAmount)
	{
		int i = playerInfos[playerIndex].texIndex;
		int[] availableSprites = GetAvailableSpriteIndices(playerIndex);

		while(true)
		{
			i += scrollAmount > 0f ? 1 : -1;
			WadeUtils.WrapAround(ref i, 0, playerTexInfos.Length - 1);

			if( availableSprites.Contains(i) )
			{
				playerInfos[playerIndex].texIndex = i;
				playerInfos[playerIndex].UpdateSprite( playerTexInfos );
				return;
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
			WadeUtils.WrapAround(ref i, 0, GameManager.instance.colorOptions.Length - 1);

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
