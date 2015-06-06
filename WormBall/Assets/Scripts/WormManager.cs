using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WormManager : SingletonBehaviour<WormManager> 
{
	public List<GameObject> worms = new List<GameObject>();

	[SerializeField] GameObject wormPrefab;

	public void EnableWormInput(bool enable)
	{
		GameObject[] wormObjs = worms.ToArray();

		for(int i = 0; i < wormObjs.Length; i++)
		{
			if(wormObjs[i] && wormObjs[i].GetComponent<Worm>())
			{
				wormObjs[i].GetComponent<Worm>().enabled = enable;
			}
		}
	}

	public void DestroyAllWorms()
	{
		GameObject[] wormObjs = worms.ToArray();

		for(int i = 0; i < wormObjs.Length; i++)
		{
			if(wormObjs[i])
			{
				Destroy(wormObjs[i]);
			}
		}
	}

	public void DestroyTeam(int teamNum)
	{
		GameObject[] wormObjs = worms.ToArray();

		for(int i = 0; i < wormObjs.Length; i++)
		{
			if(wormObjs[i])
			{
				int wormNum = wormObjs[i].GetComponentInChildren<Worm>().playerNum;

				if(teamNum == 1 && wormNum <= 4)
				{
					Destroy(wormObjs[i]);
				}
				else if(teamNum == 2 && wormNum > 4)
				{
					Destroy(wormObjs[i]);
				}
			}
		}
	}

	public Worm CreateWorm(Vector3 pos, bool lookUp, int playerNum, int teamNum, Color color, Texture2D tex)
	{
		Quaternion spawnRot = lookUp ? Quaternion.identity : Quaternion.Euler(0.0f, 0.0f, 180.0f);
		GameObject wormObj = WadeUtils.Instantiate(wormPrefab, pos, spawnRot);
		worms.Add(wormObj);

		Worm worm = wormObj.GetComponentInChildren<Worm>();

		worm.playerNum = playerNum;
		worm.teamNum = teamNum;

		worm.GetComponent<Renderer>().material.SetTexture("_MainTex", tex);
		worm.GetComponent<Renderer>().material.SetColor("_MainTint", color);

		return worm;
	}

	public void SetControlScheme(string schemeName)
	{
		foreach(Worm worm in GameObject.FindObjectsOfType<Worm>())
		{
			worm.SetControls(schemeName);
		}
	}
}
