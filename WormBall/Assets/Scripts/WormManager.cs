using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WormManager : MonoBehaviour 
{
	public static SingletonBehaviour<WormManager> singleton = new SingletonBehaviour<WormManager>();
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

	public Worm CreateWorm(Vector3 pos, bool lookUp, int playerNum, Color color)
	{
		Quaternion spawnRot = lookUp ? Quaternion.identity : Quaternion.Euler(0.0f, 0.0f, 180.0f);
		GameObject wormObj = WadeUtils.Instantiate(wormPrefab, pos, spawnRot);
		worms.Add(wormObj);

		Worm worm = wormObj.GetComponentInChildren<Worm>();
		worm.playerNum = playerNum;
		worm.renderer.material.SetColor("_MainTint", color);

		return worm;
	}
}
