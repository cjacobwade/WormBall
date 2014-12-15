using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WormText : MonoBehaviour 
{
	public string text = "";
	string prevText = "";

	public int fontSize = 150;
	int prevFontSize = 0;

	[SerializeField] GameObject letterPrefab;
	GameObject[] letterObjs = new GameObject[0];

	RectTransform rectTransform;

	[SerializeField] float kerning = 3.2f;
	[SerializeField] float wiggleSpeed = 0.7f;
	[SerializeField] float wiggleHeight = 0.9f;

	[SerializeField] Color leftColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
	Color prevLeftColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);

	[SerializeField] Color rightColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
	Color prevRightColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);

	void Awake () 
	{
		rectTransform = GetComponent<RectTransform>();
	}

	public void SetColor(Color a, Color b)
	{
		leftColor = a;
		rightColor = b;

		UpdateLetters();
	}

	void Update () 
	{
		UpdateText();
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

				if(prevFontSize != fontSize || 
				   prevLeftColor != leftColor ||
				   prevRightColor != rightColor)
				{
					Text letterText = letterObjs[i].GetComponent<Text>();
					letterText.fontSize = fontSize;
					letterText.color = Color.Lerp(leftColor, rightColor, i/(float)letterObjs.Length);

					if(i == letterObjs.Length)
					{
						prevFontSize = fontSize;
						prevLeftColor = leftColor;
						prevRightColor = rightColor;
					}
				}
			}
		}
	}

	void UpdateText()
	{
		if(text != prevText)
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
					letterText.rectTransform.SetParent(transform);
					letterText.rectTransform.localScale = Vector3.one;
					letterText.text = text[i].ToString();

					letterObjs[i] = letterObj;
				}
			}

			prevText = text;
		}
	}
}
