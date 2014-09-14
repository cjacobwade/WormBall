// This class turs writing singletons into 2 lines of code instead of a dozen
// I did not write it - Chris

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Make a subclass of this class with T as the subclass to make a singleton
public class SingletonBehaviour<T> : MonoBehaviour where T: MonoBehaviour
{
	private static T _instance;
	
	public static T instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = GameObject.FindObjectOfType<T>();
				
				if (_instance == null)
				{
					GameObject go = new GameObject();
					go.name = typeof(T).Name;
					_instance = go.AddComponent<T>();
				}
			}
			
			return _instance;
		}
	}
	
	// Call this to upgrade a singleton to a persistent singleton.
	// This will kill an instance that tries to be a persistent singleton but isn't the current instance.
	public void DontDestroyElseKill( MonoBehaviour mb )
	{
		if ( mb == instance )
		{
			MonoBehaviour.DontDestroyOnLoad( instance.gameObject );
		}
		else
		{
			MonoBehaviour.Destroy( mb );
		}
	}
}