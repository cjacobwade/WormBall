using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum EffectType
{
	Clash,
	MoveTrail,
	FatTrail
};

public class EffectsManager : SingletonBehaviour<EffectsManager> 
{
	[SerializeField] GameObject[] effectPrefabs;
	Dictionary<EffectType, GameObject> effectDict = new Dictionary<EffectType, GameObject>();

	void Awake()
	{
		EffectType[] effectTypes = (EffectType[])Enum.GetValues(typeof(EffectType));
		for( int i = 0; i < effectTypes.Length; ++i )
		{
			if( effectPrefabs[i] )
			{
				effectPrefabs[i].CreatePool(10);
				effectDict.Add( effectTypes[i], effectPrefabs[i] );
			}
			else
			{
				Debug.LogWarning( "Missing VFX prefab for enum " + effectTypes[i] );
			}
		}
	}

	public GameObject PlayEffect( EffectType effectType, Vector3 position, Quaternion rotation, float lifeTime = -1f)
	{
		GameObject effectObj = effectDict[effectType].Spawn();

		effectObj.transform.position = position;
		effectObj.transform.rotation = rotation;

		if( lifeTime > 0f )
		{
			StartCoroutine( WaitAndRecycle( effectObj, lifeTime ) );
		}

		return effectObj;
	}

	public void DestroyAllEffects()
	{
		StopAllCoroutines();

		foreach( ParticleSystem ps in GameObject.FindObjectsOfType<ParticleSystem>() )
		{
			Destroy( ps );
		}
	}

	IEnumerator WaitAndRecycle( GameObject go, float waitTime )
	{
		yield return new WaitForSeconds( waitTime );
		go.Recycle();
	}
}
