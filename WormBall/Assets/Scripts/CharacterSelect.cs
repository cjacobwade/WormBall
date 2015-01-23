using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlayerInfo
{
	public Image image;
	public int spriteIndex;
	public int teamIndex;
	public bool joined;
	public float inputTimer = 1000f;

	public void UpdateSprite(Sprite[] spriteArr)
	{
		image.sprite = spriteArr[spriteIndex];
	}
}

public class CharacterSelect : MonoBehaviour 
{
	[SerializeField] PlayerInfo[] playerInfos;

	[SerializeField] Sprite defaultPlayerSprite;
	Color defaultSpriteColor;

	[SerializeField] Sprite[] playerSprites;
	List<int>[] teamOpenSpriteIndices = new List<int>[2];
	
	List<int> openTeamColorIndices = new List<int>(); // available team colors

	int[] teamColorIndices = new int[2]; // colorIndex for each team

	[SerializeField] float inputTime;

	void Awake()
	{
		defaultSpriteColor = playerInfos[0].image.color;

		for(int i = 0; i < 2; i++)
		{
			teamOpenSpriteIndices[i] = new List<int>();
			teamColorIndices[i] = i;
		}

		for(int i = 0; i < 8; i++)
		{
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
					Debug.Log("Player Join");
					PlayerJoinGame(i);
					playerInfos[i].inputTimer = 0f;
				}

				// Leave Game
				if(playerInfos[i].joined && Input.GetButtonDown("Leave_P" + (i + 1) + WadeUtils.platformName))
				{
					Debug.Log("Player Leave");
					PlayerLeaveGame(i);
					playerInfos[i].inputTimer = 0f;
				}

				// Change Texure
				float scrollInput = Input.GetAxis("Horizontal_P" + (i + 1));
				if(playerInfos[i].joined && Mathf.Abs(scrollInput) > WadeUtils.SMALLNUMBER)
				{
					Debug.Log("Player Change Sprite");
					PlayerChangeSprite(i, scrollInput);
					playerInfos[i].inputTimer = 0f;
				}

				// Change Team Color
				float bumperInput = Input.GetAxis("Bumper_P" + (i + 1) + WadeUtils.platformName);
				if(playerInfos[i].joined & Mathf.Abs(bumperInput) > WadeUtils.SMALLNUMBER)
				{
					Debug.Log("Change Team Color");
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

		playerInfos[playerIndex].image.sprite = playerSprites[firstSpriteIndex];
		playerInfos[playerIndex].image.color = GameManager.instance.colorOptions[teamColorIndices[teamIndex]];
		teamOpenSpriteIndices[teamIndex].RemoveAt(0);
	}

	void PlayerLeaveGame(int playerIndex)
	{
		PlayerInfo playerInfo = playerInfos[playerIndex];

		playerInfo.joined = false;
		teamOpenSpriteIndices[playerInfo.teamIndex].Add(playerInfo.spriteIndex);
		playerInfo.image.sprite = defaultPlayerSprite;
		playerInfo.image.color = defaultSpriteColor;
	}

	void CheckAvailableSprites()
	{
		for(int i = 0; i < 2; i++)
		{
			teamOpenSpriteIndices[i].Clear();

			for(int j = 0; j < playerSprites.Length; j++)
			{
				bool spriteUnused = true;

				foreach(PlayerInfo playerInfo in playerInfos)
				{
					if(playerInfo.teamIndex == i && playerInfo.joined && 
					   playerInfo.image.sprite == playerSprites[j])
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

		int i = playerInfos[playerIndex].spriteIndex;
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
				Debug.Log(i);
				//Debug.Log("Looking for larger sprite index: " + teamOpenSpriteIndices[teamIndex][i] + " vs. " + playerInfos[playerIndex].spriteIndex);

				if(teamOpenSpriteIndices[teamIndex][i] != playerInfos[playerIndex].spriteIndex)
				{
					Debug.Log("In count: " + teamOpenSpriteIndices[teamIndex].Count);

					teamOpenSpriteIndices[teamIndex].Add(playerInfos[playerIndex].spriteIndex);
					playerInfos[playerIndex].spriteIndex = teamOpenSpriteIndices[teamIndex][i];
					teamOpenSpriteIndices[teamIndex].Remove(teamOpenSpriteIndices[teamIndex][i]);

					Debug.Log("Out count: " + teamOpenSpriteIndices[teamIndex].Count);

					playerInfos[playerIndex].UpdateSprite(playerSprites);
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
				if(teamOpenSpriteIndices[teamIndex][i] != playerInfos[playerIndex].spriteIndex)
				{
					teamOpenSpriteIndices[teamIndex].Add(playerInfos[playerIndex].spriteIndex);
					playerInfos[playerIndex].spriteIndex = teamOpenSpriteIndices[teamIndex][i];
					teamOpenSpriteIndices[teamIndex].Remove(teamOpenSpriteIndices[teamIndex][i]);

					playerInfos[playerIndex].UpdateSprite(playerSprites);
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
