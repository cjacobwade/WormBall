// This class turs writing singletons into 2 lines of code instead of a dozen
// I did not write it - Chris

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SingletonBehaviour<T> where T : MonoBehaviour
{
    private T _instance;
    public T instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<T>();
            }

            return _instance;
        }
    }

    public void DontDestroyElseKill(MonoBehaviour mb)
    {
        if (mb == instance)
        {
            MonoBehaviour.DontDestroyOnLoad(instance.gameObject);
        }
        else
        {
            MonoBehaviour.Destroy(mb); //GO may contain useful scripts 
        }
    }
}