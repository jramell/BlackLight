using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public PolygonPainter polygonPainter;

	/// <summary>
	/// if you kill the next enemy before this time elapsed you get a combo.
	/// </summary>
	public int comboTimerSeconds = 1;

	public Canvas HUD;
    public Text pointsText;
    public Text levelDoneText;

	public GameObject comboTextPrefab;
	public GameObject mainCameraPrefab;

    private int points = 0;
	private float lastTimeCollected = 0f;
	private int comboMultiplier = 1;

	/// <summary>
	/// Gets the points.
	/// </summary>
	/// <value>The points.</value>
    public int Points
    {
        get { return points; }
    }

	/// <summary>
	/// The enemies.
	/// </summary>
    private Collectable[] collectables;

	// Use this for initialization
	void Start () {		
		if(Camera.main == null)
		{			
			Debug.Log("Found no Main Camera in the scene and added it automatically.");			
			GameObject.Instantiate(mainCameraPrefab);
		}

        collectables = FindAllCollectablesInScene ();

        foreach(var e in collectables)
        {
            e.CollectableCollected += OnCollectableCollected;
        }
	}

	private Collectable[] FindAllCollectablesInScene ()
	{
		return (Collectable[])GameObject.FindObjectsOfType (typeof(Collectable));
	}

	/// <summary>
	/// Is raised by the enemy when it's killed.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
    void OnCollectableCollected(Collectable sender, CollectableEventArgs e)
	{	
		UpdateComboMultiplier(e);

        points += e.Points * comboMultiplier; //add the player points multiplied by the combo multiplier
        
        if(pointsText != null)
        {
            pointsText.text = string.Format("Points:{0}", points);
        }
		else
		{
			Debug.Log("No point text object is attached, can't set the text.");
		}

		lastTimeCollected = e.CollectedTime;
        sender.CollectableCollected -= OnCollectableCollected;
    }

	/// If the enemy is killed in combo time add a combo multiplier.
	/// The more enemies you kill in a combo the bigger the multiplier gets.
	void UpdateComboMultiplier(CollectableEventArgs e)
	{
		if (e.CollectedTime <= lastTimeCollected + comboTimerSeconds) {
			comboMultiplier++;
		}
		else {
			comboMultiplier = 1;
		}

		if(comboMultiplier > 1 && 
		   comboTextPrefab != null && 
		   HUD != null)
		{
			var comboText = GameObject.Instantiate(comboTextPrefab);
			comboText.GetComponent<ComboText>().SetMultiplier(comboMultiplier);
			comboText.transform.position = Camera.main.WorldToScreenPoint(e.Position);

			comboText.transform.SetParent(HUD.transform);
		}
	}

	// Update is called once per frame
	void Update () {
        
		bool levelFinished = LevelFinished();

		if(levelFinished)
        {
            levelDoneText.gameObject.SetActive(levelFinished);
        }
	
	}

	/// <summary>
	/// Returns true if the level is finished
	/// </summary>
	/// <returns><c>true</c>, if done was leveled, <c>false</c> otherwise.</returns>
	bool LevelFinished()
	{
		bool levelFinished = true;
		
		foreach(var e in collectables)
		{
			if(e != null)
			{
				levelFinished = false;
				break;
			}
		}

		return levelFinished;
	}
}
