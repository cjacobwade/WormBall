using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WormText : MonoBehaviour 
{
	string lastText = "";
	public string text = "";
	[SerializeField] int fontSize = 100;

	[SerializeField] GameObject letterPrefab;
	GameObject[] letterObjs = new GameObject[0];

	RectTransform rectTransform;

	[SerializeField] float kerning = 1.0f;
	[SerializeField] float wiggleSpeed = 1.0f;
	[SerializeField] float wiggleHeight = 1.0f;

	[SerializeField] Color leftColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
	[SerializeField] Color rightColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);

	void Awake () 
	{
		rectTransform = GetComponent<RectTransform>();

		CreateText();
		lastText = text;
	}

	public void SetColor(Color a, Color b)
	{
		leftColor = a;
		rightColor = b;

		UpdateLetters();
	}

	void Update () 
	{
		if(text != lastText)
		{
			CreateText();
		}

		UpdateLetters();
	}

	void UpdateLetters()
	{
		for(int i = 0; i < letterObjs.Length; i++)
		{
			if(letterObjs[i])
			{
				Vector3 currentPos = letterObjs[i].transform.position;
				currentPos.y = rectTransform.position.y + Mathf.Sin(((Time.frameCount + i * 10)/10.0f) * wiggleSpeed) * wiggleHeight;
				currentPos.x = rectTransform.position.x + i * kerning - (letterObjs.Length - 1)/2.0f * kerning;
				letterObjs[i].transform.position = currentPos;
				
				Text letterText = letterObjs[i].GetComponent<Text>();
				letterText.fontSize = fontSize;
				letterText.color = Color.Lerp(leftColor, rightColor, i/(float)letterObjs.Length);
				
			}
		}
	}

	void CreateText()
	{
		for(int i = 0; i < letterObjs.Length; i++)
		{
			if(letterObjs[i]) 
			{
				Destroy(letterObjs[i]);
			}
		}

		letterObjs = new GameObject[text.Length];

		for(int i = 0; i < text.Length; i++)
		{
			if(text[i] != ' ')
			{
				GameObject letterObj = WadeUtils.Instantiate(letterPrefab);
				Text letterText = letterObj.GetComponent<Text>();
				letterText.rectTransform.parent = transform;
				letterText.rectTransform.localScale = Vector3.one;
				letterText.text = text[i].ToString();

				letterObjs[i] = letterObj;
			}
		}
	}

	void LateUpdate()
	{
		lastText = text;
	}
}
