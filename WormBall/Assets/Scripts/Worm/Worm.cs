using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Worm : MonoBehaviour 
{
	public int playerNum = 0;
	[HideInInspector] public bool carrying = false;

	[SerializeField] float defaultMoveSpeed = 12.0f;
	[SerializeField] float defaultRotSpeed = 5.0f;
	float defaultScale = 1.0f;

	[SerializeField] float carryMoveSpeed = 9.0f;
	[SerializeField] float carryRotSpeed = 3.7f;
	[SerializeField] Vector2 carryScale = new Vector2(0.5f, 2.0f);

	float moveSpeed = 5.0f;
	float rotSpeed = 3.0f;

	int initSegmentNum = 0;
	[SerializeField] int segmentNum = 13;
	int minSegments = 1;
	int maxSegments = 24;
	[SerializeField] GameObject segmentPrefab;
	[SerializeField] Transform segmentHolder;
	Transform[] segments = new Transform[0];

	[SerializeField] float swingDamp = 0.97f;
	[SerializeField] float circleDist;
	
	[SerializeField] float wiggleTime = 0.7f;
	float wiggleTimer = -1.0f;
	bool lastHitLeft = false;

	[SerializeField] float moveTime = 1.0f;
	float moveTimer = 0.0f;
	[SerializeField] AnimationCurve moveForce;
	
	Vector3 inputVec = Vector3.zero;
	[SerializeField] bool debug;

	[SerializeField] float throbSpeed = 1.0f;
	[SerializeField] float throbScaleAmount = 1.0f;
	[SerializeField] float throbColorAmount = 1.2f;
	float carryTimer = 0.0f;

	bool isPuking = false;
	float pukeTimer = 0.0f;
	[SerializeField] float pukeTime = 1.4f;
	[SerializeField] float pukeForce = 50.0f;
	[SerializeField] float scaleTime = 0.7f;

	Transform mouth;
	[SerializeField] Sprite openSprite;
	[SerializeField] Sprite closedSprite;

	[SerializeField] GameObject catchCheckPrefab;
	[SerializeField] GameObject pukeEffectPrefab;
	[SerializeField] GameObject ballPrefab;

	[SerializeField] Vector2 texOffsetMinMax;
	[SerializeField] Vector2 texScaleMinMax;

	Vector3[] vertices;
	Vector2[] uvs;
	int[] triangles;

	int uvIndex = 0;
	float currentUVY = 0.0f;

	// Use this for initialization
	void Awake () 
	{
		initSegmentNum = segmentNum;

		mouth = transform.GetChild(0);

		moveSpeed = defaultMoveSpeed;
		rotSpeed = defaultRotSpeed;
		defaultScale = transform.localScale.x;

		SetupBody(segmentNum);
		renderer.material.SetFloat("_OverlayAlpha", 0.0f);
	}

	public void SetupBody(int length)
	{
		if(!Application.isPlaying)
		{
			return;
		}

		for(int i = 0; i < segments.Length; i++)
		{
			Destroy(segments[i].gameObject);
		}

		segmentNum = length;
		segments = new Transform[length];

		transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

		for(int i = 0; i < segmentNum; i++)
		{
			GameObject segment = CreateSegment(transform.position - transform.up * (i + 1) * circleDist);

			segment.name = "Segment" + i;
			segment.transform.localScale = Vector3.one * transform.localScale.x;
			segment.GetComponent<SpringJoint2D>().connectedBody = (i == 0) ? rigidbody2D : segments[i - 1].rigidbody2D;
			segments[i] = segment.transform;

			if(i == segmentNum - 1)
			{
				Tail tail = segment.AddComponent<Tail>();
				tail.Setup (this);
			}
		}

		SetupMeshData();
		CreateMesh();
	}

	GameObject CreateSegment(Vector3 spawnPos)
	{
		GameObject segment = WadeUtils.Instantiate( segmentPrefab, spawnPos, segmentPrefab.transform.rotation);
		segment.transform.parent = segmentHolder;
		segment.layer = segmentHolder.gameObject.layer;
		
		SpringJoint2D segJoint = segment.GetComponent<SpringJoint2D>();
		segJoint.collideConnected = false;
		segJoint.frequency = 0.0f;
		segJoint.distance *= circleDist;

		return segment;
	}

	void AddSegment()
	{
		if( segments.Length <= 0)
		{
			Debug.LogError("Length of worm is ZERO!");
		}

		Transform lastSeg = segments[segments.Length - 1];
		Destroy(lastSeg.GetComponent<Tail>());

		foreach(CircleCollider2D col in lastSeg.GetComponents<CircleCollider2D>())
		{
			if(col.isTrigger)
			{
				Destroy(col);
			}
		}

		Transform[] newSegments = new Transform[segments.Length + 1];
		for(int i = 0; i < segments.Length; i++)
		{
			newSegments[i] = segments[i];
		}

		GameObject segment = CreateSegment(lastSeg.position - lastSeg.up * circleDist);
		segment.name = "Segment" + segments.Length;
		segment.transform.localScale = Vector3.one * lastSeg.localScale.x;

		segment.GetComponent<SpringJoint2D>().connectedBody = lastSeg.rigidbody2D;

		Tail tail = segment.AddComponent<Tail>();
		tail.Setup(this);

		newSegments[newSegments.Length - 1] = segment.transform;

		segments = newSegments;
		segmentNum = segments.Length;
	}

	void RemoveSegment()
	{
		if( segments.Length == 1)
		{
			Debug.LogError("Can't remove this segment or there will be no tail");
		}

		Transform prevSeg = segments[segments.Length - 2];
		Transform[] newSegments = new Transform[segments.Length - 1];
		for(int i = 0; i < newSegments.Length; i++)
		{
			newSegments[i] = segments[i];
		}
	
		Tail tail = prevSeg.gameObject.AddComponent<Tail>();
		tail.Setup(this);

		Destroy(segments[segments.Length - 1].gameObject);

		segments = newSegments;
		segmentNum = segments.Length;
	}

	void SetupMeshData()
	{
		vertices = new Vector3[segments.Length * 4 + 12];
		uvs = new Vector2[vertices.Length];
		List<int> triangleList = new List<int>();
	
		float scale = transform.localScale.x;

		uvIndex = -1;
		currentUVY = 0.0f;

		// Draw mesh head
		vertices[0] = transform.InverseTransformPoint (transform.position + segments[0].up * 0.5f * scale);

		vertices[1] = transform.InverseTransformPoint (transform.position + (segments[0].up * 0.924f - segments[0].right * 0.383f) * 0.5f * scale);
		vertices[2] = transform.InverseTransformPoint (transform.position + (segments[0].up * 0.924f + segments[0].right * 0.383f) * 0.5f * scale); 

		vertices[3] = transform.InverseTransformPoint (transform.position + (segments[0].up - segments[0].right) * 0.707f * 0.5f * scale);
		vertices[4] = transform.InverseTransformPoint (transform.position + (segments[0].up + segments[0].right) * 0.707f * 0.5f * scale);

		vertices[5] = transform.InverseTransformPoint (transform.position - segments[0].right * 0.5f * scale);
		vertices[6] = transform.InverseTransformPoint (transform.position + segments[0].right * 0.5f * scale);

		triangleList.Add(0);
		triangleList.Add(1);
		triangleList.Add(2);

		SetupTriangleUV(0, 1, 2);

		triangleList.Add(2);
		triangleList.Add(1);
		triangleList.Add(3);

		triangleList.Add(2);
		triangleList.Add(3);
		triangleList.Add(4);

		triangleList.Add(4);
		triangleList.Add(3);
		triangleList.Add(5);

		triangleList.Add(4);
		triangleList.Add(5);
		triangleList.Add(6);

		SetupSquareUV(3, 4, 5, 6);

		for(int i = 0; i < segments.Length; i++)
		{
			Transform seg = segments[i];
			scale = seg.localScale.x;

			Vector3 vert9 = transform.InverseTransformPoint (seg.position + seg.right * scale/2.0f);
			Vector3 vert10 = transform.InverseTransformPoint (seg.position - seg.right * scale/2.0f);

			if(i == 0)
			{
				Vector3 vert7 = transform.InverseTransformPoint (seg.position + (seg.up + seg.right) * 0.5f * scale);
				Vector3 vert8 = transform.InverseTransformPoint (seg.position + (seg.up - seg.right) * 0.5f * scale);

				vertices[i * 4 + 7] = (vert7 + vert9) * 0.5f;
				vertices[i * 4 + 8] = (vert8 + vert10) * 0.5f;
			}
			else
			{
				Transform lastSeg = segments[i - 1];

				Vector3 averageRight = ((seg.position + lastSeg.position) * 0.5f) + ((seg.right + lastSeg.right) * 0.25f * (scale + lastSeg.localScale.x) * 0.5f);
				Vector3 averageLeft = ((seg.position + lastSeg.position) * 0.5f) - ((seg.right + lastSeg.right) * 0.25f * (scale + lastSeg.localScale.x) * 0.5f);

				vertices[i * 4 + 7] = transform.InverseTransformPoint (averageRight);
				vertices[i * 4 + 8] = transform.InverseTransformPoint (averageLeft);
			}

			vertices[i * 4 + 9] = vert9;
			vertices[i * 4 + 10] = vert10;

			if(i == 0)
			{
				triangleList.Add(6);
				triangleList.Add(5);
				triangleList.Add(7);

				triangleList.Add(8);
				triangleList.Add(7);
				triangleList.Add(5);
			}
			else
			{
				triangleList.Add(i * 4 + 8);
				triangleList.Add(i * 4 + 7);
				triangleList.Add((i - 1) * 4 + 10);

				triangleList.Add(i * 4 + 7);
				triangleList.Add((i - 1) * 4 + 9);
				triangleList.Add((i - 1) * 4 + 10);
			}

			triangleList.Add(i * 4 + 7);
			triangleList.Add(i * 4 + 8);
			triangleList.Add(i * 4 + 9);

			triangleList.Add(i * 4 + 10);
			triangleList.Add(i * 4 + 9);
			triangleList.Add(i * 4 + 8);

			if(i < segments.Length - 1)
			{
				SetupSquareUV(i * 4 + 7, i * 4 + 8, i * 4 + 9, i * 4 + 10);
			}
			else
			{
				// COS(PI/4) = 0.707
				vertices[i * 4 + 11] = transform.InverseTransformPoint (seg.position - (seg.up - seg.right) * 0.707f * scale/2.0f);
				vertices[i * 4 + 12] = transform.InverseTransformPoint (seg.position - (seg.up + seg.right) * 0.707f * scale/2.0f);

				// COS(PI * 3/8) = 0.924
				vertices[i * 4 + 13] = transform.InverseTransformPoint (seg.position - (seg.up * 0.924f - seg.right * 0.383f) * scale/2.0f);
				vertices[i * 4 + 14] = transform.InverseTransformPoint (seg.position - (seg.up * 0.924f + seg.right * 0.383f) * scale/2.0f);

				vertices[i * 4 + 15] = transform.InverseTransformPoint (seg.position - seg.up * scale/2.0f);

				triangleList.Add(i * 4 + 12);
				triangleList.Add(i * 4 + 9);
				triangleList.Add(i * 4 + 10);

				triangleList.Add(i * 4 + 12);
				triangleList.Add(i * 4 + 11);
				triangleList.Add(i * 4 + 9);

				triangleList.Add(i * 4 + 13);
				triangleList.Add(i * 4 + 11);
				triangleList.Add(i * 4 + 12);

				triangleList.Add(i * 4 + 14);
				triangleList.Add(i * 4 + 13);
				triangleList.Add(i * 4 + 12);

				triangleList.Add(i * 4 + 15);
				triangleList.Add(i * 4 + 13);
				triangleList.Add(i * 4 + 14);

				SetupSquareUV(i * 4 + 8, i * 4 + 7, i * 4 + 9, i * 4 + 10);
				SetupSquareUV(i * 4 + 11, i * 4 + 12, i * 4 + 13, i * 4 + 14);

				// Finish last UVs manually because there's an odd number of head/tail quads
				float segW = Vector3.Distance(vertices[i * 4 + 13], vertices[i * 4 + 14]);
				float segH = Vector3.Distance(vertices[i * 4 + 13], (vertices[i * 4 + 15] + vertices[i * 4 + 14]) * 0.5f);

				uvs[++uvIndex] = new Vector2(.5f, currentUVY - segH/segW);
			}
		}

		triangles = triangleList.ToArray();
	}

	void SetupSquareUV(int v1, int v2, int v3, int v4)
	{
		float segW = Vector3.Distance(vertices[v1], vertices[v2]);
		float segH = Vector3.Distance(vertices[v1], vertices[v3]);

		float topY = currentUVY - segH/segW;

		uvs[++uvIndex] = new Vector2(.0f, currentUVY);
		uvs[++uvIndex] = new Vector2(1.0f, currentUVY);
		uvs[++uvIndex] = new Vector2(.0f, topY);
		uvs[++uvIndex] = new Vector2(1.0f, topY);

		currentUVY = topY;
	}

	void SetupTriangleUV(int v1, int v2, int v3)
	{
		float segW = Vector3.Distance(vertices[v2], vertices[v3]);
		float segH = Vector3.Distance(vertices[v1], (vertices[v2] + vertices[v3]) * 0.5f);
		
		float topY = currentUVY - segH/segW;

		uvs[++uvIndex] = new Vector2(0.5f, currentUVY);
		uvs[++uvIndex] = new Vector2(.0f, topY);
		uvs[++uvIndex] = new Vector2(1.0f, topY);

		currentUVY = topY;
	}

	void CreateMesh()
	{
		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		mesh.vertices = vertices;

		mesh.uv = uvs;
		mesh.triangles = triangles;

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

		SetMaterialRelative();
	}

	void UpdateMesh()
	{
		SetMaterialRelative();

		Mesh mesh = GetComponent<MeshFilter>().mesh;
		if(mesh.vertices.Length != vertices.Length)
		{
			mesh = new Mesh();
			GetComponent<MeshFilter>().mesh = mesh;
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.uv = uvs;
		}
		else
		{
			mesh.vertices = vertices;
			mesh.uv = uvs;
		}

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}

	public void Catch()
	{
		carrying = true;
		carryTimer = 0.0f;

		moveSpeed = carryMoveSpeed;
		rotSpeed = carryRotSpeed;

		while(segments.Length > 6)
		{
			RemoveSegment();
		}

		ChangeHeadSize(defaultScale);
		ChangeSegmentSize(0, transform.localScale.x);
		ChangeSegmentSize(1, carryScale.y);
		ChangeSegmentSize(2, carryScale.y);
		ChangeSegmentSize(3, (carryScale.x + carryScale.y)/2.0f);
		ChangeSegmentSize(4, carryScale.x);		
		ChangeSegmentSize(5, carryScale.x);	
	}

	public void Puke()
	{
		carrying = false;

		renderer.material.SetFloat("_OverlayAlpha", 0.0f);

		moveSpeed = defaultMoveSpeed;
		rotSpeed = defaultRotSpeed;

		while(segments.Length < initSegmentNum)
		{
			AddSegment();
		}

		ChangeHeadSize(defaultScale);
		ChangeAllSegmentSizes(defaultScale);

		StartCoroutine(PukeEffects());
	}

	IEnumerator PukeEffects()
	{
		isPuking = true;
		pukeTimer = 0.0f;
		bool ballSpawned = false;

		GameObject pukeEffectObj = WadeUtils.Instantiate(pukeEffectPrefab, transform.position, transform.rotation);
		mouth.GetComponent<SpriteRenderer>().sprite = openSprite;

		rigidbody2D.velocity = Vector3.zero;
		for(int i = 0; i < segments.Length; i++)
		{
			segments[i].rigidbody2D.velocity = Vector3.zero;
		}

		while(pukeTimer < pukeTime)
		{
			if(pukeEffectObj)
			{
				pukeEffectObj.transform.position = mouth.position;
				pukeEffectObj.transform.rotation = transform.rotation;
				pukeEffectObj.transform.localRotation *= Quaternion.Euler(-90.0f, 0.0f, 0.0f);
			}

			if(pukeTimer > pukeTimer/10.0f)
			{
				mouth.localScale = new Vector3(1.0f + Mathf.Cos(pukeTimer * 20.0f)/2.0f, mouth.localScale.y, mouth.localScale.z);
			}

			if(pukeTimer > pukeTimer/3.0f)
			{
				if(!ballSpawned)
				{
					GameObject ballObj = WadeUtils.Instantiate(ballPrefab, transform.position + transform.up, transform.rotation);
					ballObj.layer = LayerMask.NameToLayer("IgnorePlayer");
					ballObj.rigidbody2D.AddForce(transform.up * pukeForce * 100);
					ballObj.rigidbody2D.AddTorque(pukeForce * 0.01f);

					Ball ball = ballObj.GetComponent<Ball>();
					ball.StartCoroutine(ball.ScaleUp(0.0f));
					ballSpawned = true;
				}
				else
				{
					rigidbody2D.AddForce(-transform.up * pukeForce * (1.0f - pukeTimer/pukeTime));
					for(int i = 0; i < segments.Length; i++)
					{
						segments[i].rigidbody2D.AddForce(-transform.up * pukeForce * (1.0f - pukeTimer/pukeTime));
					}
				}
			}

			if(pukeTimer > pukeTime * 0.5f)
			{
				if(pukeEffectObj)
				{
					pukeEffectObj.particleSystem.enableEmission = false;
				}
			}
			else
			{
				rigidbody2D.AddForce(-transform.up * pukeForce);
			}

			pukeTimer += Time.deltaTime;
			yield return 0;
		}

		pukeTimer = 0.0f;
		mouth.GetComponent<SpriteRenderer>().sprite = closedSprite;

		while(Vector3.Distance(mouth.localScale, Vector3.one) > 0.05f)
		{
			mouth.localScale = Vector3.Lerp(mouth.localScale, Vector3.one, Time.deltaTime);
			yield return 0;
		}

		Destroy(pukeEffectObj);
		mouth.localScale = Vector3.one;
	
		isPuking = false;
	}

	void SetMaterialRelative()
	{
		Material mat = renderer.material;
		float alpha = segments.Length/(float)(maxSegments - minSegments);

//		Debug.Log(alpha);

		mat.SetTextureOffset("_MainTex", new Vector2(0.0f, Mathf.Lerp(texOffsetMinMax.x, texOffsetMinMax.y, alpha)));
		mat.SetTextureOffset("_OverlayTex", new Vector2(0.0f, Mathf.Lerp(texOffsetMinMax.x, texOffsetMinMax.y, alpha)));

		mat.SetTextureScale("_MainTex",  new Vector2(1.0f, Mathf.Lerp(texScaleMinMax.x, texScaleMinMax.y, alpha)));
		mat.SetTextureScale("_OverlayTex",  new Vector2(1.0f, Mathf.Lerp(texScaleMinMax.x, texScaleMinMax.y, alpha)));
	}

	// Update is called once per frame
	void Update () 
	{
		if(Input.GetButtonDown("Restart"))
		{
			Application.LoadLevel(Application.loadedLevel);
		}

		if(Input.GetKeyDown(KeyCode.C))
		{
			Catch();
		}
		if(Input.GetKeyDown(KeyCode.P))
		{
			Puke ();
		}

		inputVec = new Vector3(Input.GetAxis("Horizontal-P" + playerNum), Input.GetAxis("Vertical-P" + playerNum), 0.0f);

		if(segmentNum < maxSegments && Input.GetKeyDown(KeyCode.Equals))
		{
			AddSegment();
		}

		if(segmentNum > minSegments && Input.GetKeyDown(KeyCode.Minus))
		{
			RemoveSegment();
		}

		if(wiggleTimer > wiggleTime)
		{
			wiggleTimer = -1.0f;
		}

		SetupMeshData();

		for(int i = 0; i < segments.Length; i++)
		{
			// What is this for?
			segments[i].GetComponent<SpringJoint2D>().distance = segments[i].localScale.x * circleDist;
		}

		UpdateMesh();

		if(carrying)
		{
			renderer.material.SetFloat("_OverlayAlpha", throbColorAmount + Mathf.Sin(carryTimer * throbSpeed) * throbColorAmount);
			ChangeSegmentSize(segments.Length - 1, carryScale.x + throbScaleAmount + Mathf.Sin(carryTimer * throbSpeed) * throbScaleAmount);
			carryTimer += Time.deltaTime;
		}
	}

	void FixedUpdate()
	{
		Movement();
		Rotation();

		if(carrying)
		{
			ScoreManager.singleton.instance.AddTime(playerNum);
		}
	}

	public void SetMoveSpeed(float newSpeed)
	{
		moveSpeed = newSpeed;
	}

	public void SetRotSpeed(float newSpeed)
	{
		rotSpeed = newSpeed;
	}

	void Movement()
	{
		WiggleLogic();

		float clampedMoveTimer = Mathf.Clamp(moveTimer, 0.0f, moveTime);
		float appliedSpeed = moveSpeed * moveForce.Evaluate(clampedMoveTimer/moveTime); //* wiggleBoost;

		if(moveTimer > 0.0f)
		{
			if(pukeTimer <= 0.0f)
			{
				rigidbody2D.velocity = transform.up * appliedSpeed;
			}

			for(int i = 0; i < segments.Length; i++)
			{
				segments[i].rigidbody2D.velocity *= swingDamp;
			}
		}
		else if(!isPuking)
		{
			rigidbody2D.velocity = Vector2.Lerp(rigidbody2D.velocity, Vector2.zero, Time.deltaTime);

			for(int i = 0; i < segments.Length; i++)
			{
				segments[i].rigidbody2D.velocity = Vector3.Lerp(segments[i].rigidbody2D.velocity, Vector2.zero, Time.deltaTime/2.0f);
			}
		}

		if(moveTimer >= 0.0f)
		{
			moveTimer -= Time.deltaTime;
		}
		else
		{
			moveTimer = 0.0f;
		}
	}

	// Check for alternating between left and right
	void WiggleLogic()
	{
		if(wiggleTimer < 0.0f)
		{
			if(inputVec.x > 0.1f)
			{
				wiggleTimer = 0.0f;
				lastHitLeft = false;
			}
			else if(inputVec.x < -0.1f)
			{
				wiggleTimer = 0.0f;
				lastHitLeft = true;
			}
		}
		else
		{
			wiggleTimer += Time.deltaTime;
			
			if(Mathf.Abs(inputVec.x) > 0.1f)
			{
				bool hitLeft = inputVec.x < 0.0f;
				if(lastHitLeft != hitLeft)
				{
					lastHitLeft = hitLeft;
					moveTimer = moveTime;
				}
			}
			else
			{
				wiggleTimer = 0.0f;
			}
		}
	}

	void Rotation()
	{
		transform.rotation *= Quaternion.Euler(0.0f, 0.0f, -rotSpeed * inputVec.x);

		segments[0].LookAt(transform.position, Vector3.forward);
		segments[0].rotation *= Quaternion.Euler(90.0f, 0.0f, 0.0f);
		
		for(int i = 1; i < segments.Length; i++)
		{
			segments[i].LookAt(segments[i - 1].position, Vector3.forward);
			segments[i].rotation *= Quaternion.Euler(90.0f, 0.0f, 0.0f);
		}
	}

	public void ChangeHeadSize(float scale)
	{
		if(segments.Length > 0)
		{
			transform.localScale = Vector3.one * scale;
			float distance = (transform.localScale.x + segments[0].localScale.x)/2.0f;
			segments[0].GetComponent<SpringJoint2D>().distance = distance;
		}
	}

	void ChangeSegmentSize(int index, float scale)
	{
		if(segments.Length > 0)
		{
			if(index >= segments.Length || index < 0)
			{
				Debug.Break();
				Debug.LogError("Invalid segment index");
			}

			segments[index].localScale = Vector3.one * scale;
			float distance = 0.0f;

			if(index > 0)
			{
				distance = (segments[index - 1].localScale.x + segments[index].localScale.x)/2.0f;
				segments[index].GetComponent<SpringJoint2D>().distance = distance;
			}

			if(index < segments.Length - 2)
			{
				distance = (segments[index].localScale.x + segments[index + 1].localScale.x)/2.0f;
				segments[index + 1].GetComponent<SpringJoint2D>().distance = distance;
			}
		}
	}

	public void ChangeAllSegmentSizes(float scale)
	{
		if(segments.Length > 0)
		{
			for(int i = 0; i < segments.Length; i++)
			{
				ChangeSegmentSize(i, scale);
			}
		}
	}

	void OnGUI()
	{
		for(int i = 0; debug && i < vertices.Length; i++)
		{
			Vector3 screenVec = Camera.main.WorldToScreenPoint(transform.position + transform.rotation * vertices[i] * transform.localScale.x);
			GUI.Label(new Rect(screenVec.x - 10.0f,Screen.height - (screenVec.y + 10.0f), 20.0f, 20.0f), i.ToString());
		}
	}

	void OnDrawGizmos()
	{
		for(int i = 0; segments.Length > 0 && i < segments.Length; i++)
		{
			Gizmos.DrawWireSphere(segments[i].position, segments[i].localScale.x/2.0f);
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		Tail tail = col.GetComponent<Tail>();
		if(tail)
		{
			if(tail.worm == this)
			{
				if(!carrying)
				{
					Vector3 checkPos = transform.position;
					for(int i = 0; i < segments.Length; i++)
					{
						checkPos += segments[i].position;
					}
					checkPos *= 1.0f/(segments.Length + 1.0f);

					GameObject catchCheckObj = WadeUtils.Instantiate(catchCheckPrefab, checkPos, transform.rotation);
					catchCheckObj.GetComponent<CatchCheck>().worm = this;
				}
			}
			else
			{
				if(tail.worm.carrying)
				{
					tail.worm.Puke();
				}
			}
		}
	}
}
