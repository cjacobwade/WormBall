#define USINGMOUSECONTROLS

using UnityEngine;
using System.Collections;

public static class WadeUtils
{

	///////////////////////
	/// INPUT		//////
	////////////////////

	#if UNITY_WEBPLAYER
	public static string platformName = "_MOUSE";
	#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
	public static string platformName = "_WIN";
	#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX 
	public static string platformName = "_OSX";
	#endif

	public static void CheckForController()
	{
		if(Input.GetJoystickNames().Length > 0)
		{
			#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
			platformName = "_WIN";
			#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX 
			platformName = "_OSX";
			#endif
		}
		else
		{
			platformName = "_MOUSE";
		}
	}

	public static float SMALLNUMBER = 0.00000001f;
	public static float LARGENUMBER = 100000000f;

	///////////////////////
	////	FLOATS	 /////
	//////////////////////

	public static void Clamp(ref float value, float min, float max)
	{
		value = Mathf.Clamp (value, min, max);
	}
	
	public static void Lerp(ref float from, float to, float t)
	{
		from = Mathf.Lerp (from, to, t);
	}
	
	///////////////////////
	////	VECTORS	 /////
	//////////////////////

	public static void Lerp(ref Vector2 from, Vector2 to, float t)
	{
		from = Vector2.Lerp(from, to, t);
	}
	
	public static void Lerp(ref Vector3 from, Vector3 to, float t)
	{
		from = Vector3.Lerp(from, to, t);
	}

	public static void Lerp(ref Vector4 from, Vector4 to, float t)
	{
		from = Vector4.Lerp(from, to, t);
	}

	public static float DistanceTo(this Vector2 pointA, Vector4 pointB)
	{
		return ((Vector4)pointA).DistanceTo(pointB);
	}

	public static float DistanceTo(this Vector3 pointA, Vector4 pointB)
	{
		return ((Vector4)pointA).DistanceTo(pointB);
	}
	
	public static float DistanceTo(this Vector4 pointA, Vector4 pointB)
	{
		return Vector4.Distance (pointA, pointB);
	}

	public static void Slerp(ref Vector3 from, Vector3 to, float t)
	{
		from = Vector3.Slerp(from, to, t);
	}

	////////////////////////////
	////	QUATERNIONS	////
	///////////////////////////

	public static void SetRotationX(ref Quaternion rotation, float angle)
	{
		Vector3 eulers = rotation.eulerAngles;
		eulers.x = angle;
		
		rotation.eulerAngles = eulers;
	}
	
	public static void SetRotationY(ref Quaternion rotation, float angle)
	{
		Vector3 eulers = rotation.eulerAngles;
		eulers.y = angle;
		
		rotation.eulerAngles = eulers;
	}

	public static void SetRotationZ(ref Quaternion rotation, float angle)
	{
		Vector3 eulers = rotation.eulerAngles;
		eulers.z = angle;
		
		rotation.eulerAngles = eulers;
	}

	public static void Lerp(ref Quaternion from, Quaternion to, float t)
	{
		from = Quaternion.Lerp (from, to, t);
	}

	public static void Slerp(ref Quaternion from, Quaternion to, float t)
	{
		from = Quaternion.Slerp (from, to, t);
	}
	
	////////////////////////////
	////	TRANSFORM	////
	///////////////////////////

	public static void SetPositionX(this Transform transform, float x)
	{
		transform.position = new Vector3 (x, transform.position.y, transform.position.z);
	}

	public static void SetPositionY(this Transform transform, float y)
	{
		transform.position = new Vector3 (transform.position.x, y, transform.position.z);
	}

	public static void SetPositionZ(this Transform transform, float z)
	{
		transform.position = new Vector3 (transform.position.x, transform.position.y, z);
	}

	public static void SetPosition(this Transform transform, float x, float y, float z)
	{
		transform.position = new Vector3 (x, y, z);
	}

	public static void SetPosition(this Transform transform, Vector3 vec)
	{
		transform.position = vec;
	}

	public static void AddPosition(this Transform transform, float x, float y, float z)
	{
		transform.position += new Vector3(x, y, z);
	}

	public static void AddPosition(this Transform transform, Vector3 offset)
	{
		transform.position += offset;
	}

	public static void SetRotationX(this Transform transform, float x)
	{
		transform.rotation *= Quaternion.Euler(x, transform.position.y, transform.position.z);
	}

	public static void SetRotationY(this Transform transform, float y)
	{
		transform.rotation *= Quaternion.Euler(transform.position.x, y, transform.position.z);
	}

	public static void SetRotationZ(this Transform transform, float z)
	{
		transform.rotation *= Quaternion.Euler(transform.position.x, transform.position.y, z);
	}

	public static void AddRotation(this Transform transform, float x, float y, float z)
	{
		transform.rotation *= Quaternion.Euler(new Vector3(x, y, z));
	}

	public static void AddRotation(this Transform transform, Vector3 offset)
	{
		transform.rotation *= Quaternion.Euler(offset);
	}

	public static void SetScaleX(this Transform transform, float x)
	{
		transform.localScale = new Vector3(x, transform.position.y, transform.position.z);
	}

	public static void SetScaleY(this Transform transform, float y)
	{
		transform.localScale = new Vector3(transform.position.x, y, transform.position.z);
	}

	public static void SetScaleZ(this Transform transform, float z)
	{
		transform.localScale = new Vector3(transform.position.x, transform.position.y, z);
	}

	public static void SetScale(this Transform transform, float x, float y, float z)
	{
		transform.localScale = new Vector3(x, y, z);
	}

	public static void SetScale(this Transform transform, Vector3 vec)
	{
		transform.localScale = vec;
	}

	public static void AddScale(this Transform transform, float x, float y, float z)
	{
		transform.localScale += new Vector3(x, y, z);
	}

	public static void AddScale(this Transform transform, Vector3 offset)
	{
		transform.localScale += offset;
	}
	
	public static void LerpLookAt(this Transform transform, Transform target, float t)
	{
		Quaternion currentRot = transform.rotation;
		transform.LookAt (target);
		Quaternion lookRot = transform.rotation;
		
		transform.rotation = Quaternion.Lerp (currentRot, lookRot, t);
	}

	public static void LerpLookAt(this Transform transform, Vector3 target, float t)
	{
		Quaternion currentRot = transform.rotation;
		transform.LookAt (target);
		Quaternion lookRot = transform.rotation;
		
		transform.rotation = Quaternion.Lerp (currentRot, lookRot, t);
	}

	public static void LookAtWithAxisControl(this Transform transform, Transform target, bool xRotate, bool yRotate)
	{
		transform.LookAtWithAxisControl (target.position, xRotate, yRotate);
	}

	public static void LookAtWithAxisControl(this Transform transform, Vector3 targetPos, bool xRotate, bool yRotate)
	{
		if(!xRotate && !yRotate)
		{
			Debug.LogError("Error: No LookAt because both axes are disabled");
			Debug.DebugBreak();
		}
		
		Vector3 lookPos = targetPos;
		lookPos.y = xRotate ? targetPos.y : transform.position.y;
		lookPos.x = yRotate ? targetPos.x : transform.position.x;
		
		transform.LookAt(lookPos);
	}

	public static void LerpLookAtWithAxisControl(this Transform transform, Transform target, bool xRotate, bool yRotate, float t)
	{
		transform.LerpLookAtWithAxisControl(target.position, xRotate, yRotate, t);
	}

	public static void LerpLookAtWithAxisControl(this Transform transform, Vector3 targetPos, bool xRotate, bool yRotate, float t)
	{
		if(!xRotate && !yRotate)
		{
			Debug.LogError("Error: No LookAt because both axes are disabled");
			Debug.DebugBreak();
		}
		
		Vector3 lookPos = targetPos;
		lookPos.y = xRotate ? targetPos.y : transform.position.y;
		lookPos.x = yRotate ? targetPos.x : transform.position.x;
		
		transform.LerpLookAt(lookPos, t);
	}

	public static Vector3 SetVectorRelative(this Transform transform, Vector3 vec)
	{
		return transform.TransformDirection (vec);
	}

	public static Vector3 SetVectorRelative(this Transform transform, float x, float y, float z)
	{
		return transform.TransformDirection (x, y, z);
	}
	
	///////////////////////////
	/////	PHYSICS	///////////
	//////////////////////////

	public static RaycastHit RaycastAndGetInfo(Ray ray, LayerMask layer, float dist = Mathf.Infinity)
	{
		return RaycastAndGetInfo (ray.origin, ray.direction, layer, dist);
	}
	
	public static RaycastHit RaycastAndGetInfo(Vector3 origin, Vector3 dir, LayerMask layer, float dist = Mathf.Infinity)
	{
		RaycastHit hit;
		Physics.Raycast(origin, dir, out hit, dist, layer);
		return hit;
	}

	public static RaycastHit RaycastAndGetInfo( Ray ray, float dist = Mathf.Infinity)
	{
		return RaycastAndGetInfo (ray.origin, ray.direction, dist);
	}

	public static RaycastHit RaycastAndGetInfo(Vector3 origin, Vector3 dir, float dist = Mathf.Infinity)
	{
		RaycastHit hit;
		Physics.Raycast (origin, dir, out hit, dist);
		return hit;
	}
	
	///////////////////////////////
	/////	COROUTINES	///////
	///////////////////////////////
	
	public static YieldInstruction Wait(float time)
	{
		return new WaitForSeconds (time);
	}
	
	///////////////////////////////
	/////	BIT OPERATIONS  ///////
	///////////////////////////////

	public static bool CheckBit(int bit, int compare)
	{
		return (bit & compare) == bit;
	}

	///////////////////////////////
	/////	GAMEOBJECTS  	///////
	///////////////////////////////

	public static GameObject Instantiate(GameObject obj)
	{
		return (GameObject)MonoBehaviour.Instantiate (obj);
	}
	
	public static GameObject Instantiate(GameObject obj, Vector3 pos, Quaternion rot)
	{
		return (GameObject)MonoBehaviour.Instantiate (obj, pos, rot);
	}
}

[System.Serializable]
public struct MinMax
{
	public MinMax(float _min, float _max)
	{
		min = _min;
		max = _max;
	}

	public float min;
	public float max;
}