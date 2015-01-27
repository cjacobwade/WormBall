using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class AdviceController : SingletonBehaviour<AdviceController> 
{
	[SerializeField] GameObject adviceTextPrefab;
	List<Text> activeAdviceTexts = new List<Text>();

	string[] adviceTexts = new string[] {	"Change your team's color with the bumpers.",
											"Change your pattern with the left stick.",
											"Move faster by alternating left and right on the joystick.",
											"You can pin players against walls with your beak.",
											"Be careful. You can pop your teammates when they have the ball."};
	List<string> noRepeatAdviceText = new List<string>();
	
	[SerializeField] float spawnDistance = 700f;

	void Awake()
	{
		noRepeatAdviceText = adviceTexts.ToList();
	}

	void Update()
	{
		if(Time.frameCount % 60 == 0)
		{
			ClearDistantTexts();
		}
	}

	void OnEnable()
	{
		if(activeAdviceTexts.Count > 0)
		{
			ClearAdviceTexts();
		}

		CreateAdviceText();
	}

	void OnDisable()
	{
		ClearAdviceTexts();
	}

	void CreateAdviceText()
	{
		GameObject newAdviceText = WadeUtils.Instantiate(adviceTextPrefab);

		newAdviceText.transform.SetParent(transform);
		newAdviceText.transform.localScale = Vector3.one;
		newAdviceText.GetComponent<RectTransform>().localPosition = Vector3.right * spawnDistance;

		int randNum = Random.Range(0, noRepeatAdviceText.Count - 1);

		newAdviceText.GetComponent<Text>().text = noRepeatAdviceText[randNum];
		noRepeatAdviceText.Remove(noRepeatAdviceText[randNum]);
		if(noRepeatAdviceText.Count < 1)
		{
			noRepeatAdviceText = adviceTexts.ToList();
		}

		activeAdviceTexts.Add(newAdviceText.GetComponent<Text>());
	}

	void ClearDistantTexts()
	{
		Text[] curAdviceTexts = activeAdviceTexts.ToArray();
		for(int i = 0; i < curAdviceTexts.Length; i++)
		{
			if(curAdviceTexts[i].GetComponent<RectTransform>().localPosition.x < -spawnDistance)
			{
				activeAdviceTexts.Remove(curAdviceTexts[i]);
				Destroy(curAdviceTexts[i].gameObject);

				CreateAdviceText();
			}
		}
	}

	public void ClearAdviceTexts()
	{
		Text[] curAdviceTexts = activeAdviceTexts.ToArray();
		activeAdviceTexts.Clear();

		for(int i = 0; i < curAdviceTexts.Length; i++)
		{
			Destroy(curAdviceTexts[i]);
		}
	}
}
