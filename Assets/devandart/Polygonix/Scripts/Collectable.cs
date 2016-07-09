using UnityEngine;
using System.Collections;

public class Collectable : MonoBehaviour {

    public delegate void CollectableDelegate(Collectable sender, CollectableEventArgs e);

	/// <summary>
	/// Occurs when enemy killed.
	/// </summary>
    public event CollectableDelegate CollectableCollected;

	/// <summary>
	/// The points the player gets when the enemy dies.
	/// </summary>
    public int points = 10;

	/// <summary>
	/// If minSpeed and maxSpeed are different, the speed will variate
	/// </summary>
	public float minSpeed = 10f;
	
	/// <summary>
	/// If minSpeed and maxSpeed are different, the speed will variate
	/// </summary>
	public float maxSpeed = 15f;

	/// <summary>
	/// If minScale and maxScale are different, the scale will variate
	/// </summary>
	public float minScale = 0.5f;
	
	/// <summary>
	/// If minScale and maxScale are different, the scale will variate
	/// </summary>
	public float maxScale = 1.5f;

	/// <summary>
	/// If bigger then 0, the collectable will change it's rotation after x seconds
	/// </summary>
	public float changeDirectionAfterSeconds = 0f;

	/// <summary>
	/// the prefab instantiates when the enemy dies at the enemy position.
	/// </summary>
    public GameObject explosionPrefab;

	/// <summary>
	/// the clip plays when the enemy dies.
	/// </summary>
    public AudioClip explosionClip;
    
    private PolygonPainter polygonPainter;
    private Vector3 viewportTopLeft, viewportBottomRight;
    private SpriteRenderer spriteRenderer;
	private float speed;
	private float lastTimeRotationChanged;

	void Start () {
        //viewportTopLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0));
        //viewportBottomRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));

        spriteRenderer = GetComponent<SpriteRenderer>();

		if(maxScale < minScale) maxScale = minScale;
		var scale = Random.Range(minScale, maxScale);
		spriteRenderer.transform.localScale = new Vector3(scale, scale, 1);

		if(maxSpeed < minSpeed) maxSpeed = minSpeed;
		speed = Random.Range(minSpeed, maxSpeed);

		if(changeDirectionAfterSeconds > 0)
		{
			lastTimeRotationChanged = changeDirectionAfterSeconds + Time.timeSinceLevelLoad;
		}
	}

    void Awake()
    {
        transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);
        polygonPainter = (PolygonPainter)GameObject.FindObjectOfType(typeof(PolygonPainter));
    }

    void OnEnable()
    {
        polygonPainter.PolygonPainted += polygonPainter_PolygonPainted;
    }

    void OnDisable()
    {
        polygonPainter.PolygonPainted -= polygonPainter_PolygonPainted;
    }

    void OnDestroy()
    {
        polygonPainter.PolygonPainted -= polygonPainter_PolygonPainted;
    }

	/// <summary>
	/// When the player painted a polygon, check if this collectable is inside, 
	/// collect and destroy it and tell it to the polygonpainter to handle it.
	/// </summary>
	/// <param name="e">E.</param>
    void polygonPainter_PolygonPainted(PolygonPainterEventArgs e)
    {
        if(PolygonMath.Intersect(e.Points, e.PointCount, transform.position))
        {
			DestroyMe();
        }
    }

	/// <summary>
	/// Destroy me and tell it to the polygonpainter
	/// </summary>
	void DestroyMe()
	{
		if(explosionClip != null)
		{
			AudioSource.PlayClipAtPoint(explosionClip, transform.position);
		}
		
		if(explosionPrefab != null)
		{
			GameObject.Instantiate(explosionPrefab, transform.position, Quaternion.identity);
		}
		
		GameObject.Destroy(gameObject);
		
		if(CollectableCollected != null)
		{
			CollectableCollected(this, new CollectableEventArgs() { 
				Position = transform.position,
				Points = points,
				CollectedTime = Time.time 
			});
		}
	}

	void Update () {
        UpdatePosition ();	
		UpdateRandomRotation ();
	}

	/// <summary>
	/// Updates the position and mirrors it when the player leaves the camera bounds.
	/// </summary>
	void UpdatePosition ()
	{		
		viewportTopLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0));
		viewportBottomRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));
		transform.position += transform.right * speed * Time.deltaTime;

		if (transform.position.x - spriteRenderer.bounds.extents.x > viewportBottomRight.x) {
			transform.position = new Vector3 (-transform.position.x, -transform.position.y, transform.position.z);
		}
		else if (transform.position.x + spriteRenderer.bounds.extents.x < viewportTopLeft.x) {
				transform.position = new Vector3 (-transform.position.x, -transform.position.y, transform.position.z);
			}
		if (transform.position.y - spriteRenderer.bounds.extents.y > viewportTopLeft.y) {
			transform.position = new Vector3 (-transform.position.x, -transform.position.y, transform.position.z);
		}
		else
		if (transform.position.y + spriteRenderer.bounds.extents.y < viewportBottomRight.y) {
			transform.position = new Vector3 (-transform.position.x, -transform.position.y, transform.position.z);
		}
	}

	void UpdateRandomRotation ()
	{
		if (changeDirectionAfterSeconds > 0) {
			if (lastTimeRotationChanged <= Time.timeSinceLevelLoad) {
				lastTimeRotationChanged = changeDirectionAfterSeconds + Time.timeSinceLevelLoad;
				transform.rotation = Quaternion.AngleAxis (Random.Range(0, 360), Vector3.forward);
			}
		}
	}
}
