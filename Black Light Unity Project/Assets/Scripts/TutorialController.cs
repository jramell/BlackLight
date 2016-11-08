using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class TutorialController : Interactive
{

    public Transform frontOfDoor;

    //Speed per frame of Baroth
    public float speed;

    public GameObject roomLight;

    public float characterWriteDelay;

    public GameObject healthBar;

    //Dummy to defeat
    public GameObject dummy;

    //First enemy to defeat
    public GameObject firstEnemy;

    public GameObject secondEnemyPrefab;

    public GameObject secondEnemySpawner;

    public GameObject dashUI;

    public GameObject visorUI;

    public float timeOfHealthChange;

    //--------------------------------------------------------------------------------------------------------------
    //Variables
    //--------------------------------------------------------------------------------------------------------------

    //Which event is the tutorial currently in?
    //Event 0: Baroth introduces himself and tells the player he is heading towards a dangerous place.
    //Event 1: secondDialog plays. Baroth leaves the room
    //Event 2: The player is now outside. Baroth explains health bar.
    private static int currentEvent;

    private static int lastCheckpoint;

    //Has the dummy been spawned?
    private bool dummySpawned;

    //Should Baroth be moving?
    private bool shouldMove;

    //Should Baroth leave the room?
    private bool leave;

    //What Baroth should be saying
    private string currentLine;

    //Sounds when 'talking'
    private AudioSource writingSoundEffect;

    private bool talkingToPlayer;

    private int currentDialogIndex;

    //Second enemy instance
    private GameObject secondEnemy;

    //--------------------------------------------------------------------------------------------------------------
    //Functions
    //--------------------------------------------------------------------------------------------------------------

    void Start()
    {
        shouldMove = false;
        StartCoroutine(ManageEvents());
        writingSoundEffect = GameObject.Find("Baroth").GetComponent<AudioSource>();
    }

    void Update()
    {
        if (shouldMove)
        {
            gameObject.tag = "Untagged";
            transform.position = NPCUtils.MoveTo(transform.position, frontOfDoor.position, speed);
            if (transform.position == frontOfDoor.position)
            {
                LeaveRoom();
            }
        }

        if (currentEvent >= 26 && currentEvent < 28)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 10);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject.tag == "PowerUpPlate")
                {
                    //GameObject.Find("Player").GetComponent<PlayerController>().SetTutorialText("");
                    GameObject.Find("Player").GetComponent<PlayerController>().DisplayTip("Touch the spot to get a power up");
                    currentEvent = 28;
                }
            }
        }

        if (currentEvent >= 28 && currentEvent < 30)
        {
            bool touchedPowerUp = true;
            Collider[] colliders = Physics.OverlapSphere(transform.position, 10);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject.tag == "PowerUpPlate")
                {
                    touchedPowerUp = false;
                }
            }

            if(touchedPowerUp)
            {
                currentEvent = 30;
            }
        }
    }

    IEnumerator ManageEvents()
    {
        if (currentEvent == 0)
        {
            //So the player can't interact with it at first
            gameObject.tag = "Untagged";
            SetPlayerMovement(false);
            yield return new WaitForSeconds(1);

            //Starts talking, then leave it to the player to continue
            currentLine = "Hey there!|1| This is a demo that serves as a proof of concept for certain game mechanics.|0.7| I,|0.2| Blue P.,|0.2| will be your guide.";
            //GameObject.Find("Player").GetComponent<PlayerController>().SetInteractivity(false);
            yield return StartCoroutine(Talk());
            yield break;
        }

        if (currentEvent == 1)
        {
            roomLight.SetActive(true);
            gameObject.tag = "Interactive";
            GameObject.Find("Player").GetComponent<PlayerController>().SetCameraMovementEnabled(true);
            //So the player can interact with it from now on
            currentEvent++;
            yield break;
        }

        if (currentEvent == 2)
        {

        }

        if (currentEvent == 3)
        {
            GameObject.Find("Player").GetComponent<PlayerController>().SetMovementEnabled(true);
            shouldMove = true;
            currentEvent++;
            yield break;
        }

        if (currentEvent == 5)
        {
            GameObject.Find("Player").GetComponent<PlayerController>().DisableInteraction();
            Vector3 initialScale = healthBar.transform.localScale;
            Vector3 tempScale = new Vector3(1.6f, 4, 1);
            RectTransform healthBarRect = healthBar.GetComponent<RectTransform>();
            Vector3 initialPosition = healthBarRect.localPosition;
            Debug.Log("healthBar initialPosition: " + initialPosition);
            Vector3 tempPosition = new Vector3(70, -85, 0);
            healthBarRect.localScale = tempScale;
            healthBarRect.localPosition = tempPosition;
            healthBar.SetActive(true);

            //Health Bar size change
            //Because with 0.01f per delta, there's 1000 deltas per second
            float timeOfChange = 1 / (timeOfHealthChange * 100);
            float rateOfChangeX = (tempScale.x - initialScale.x) * timeOfChange;
            float rateOfChangeY = (tempScale.y - initialScale.y) * timeOfChange;
            float rateOfPositionChangeX = (tempPosition.x - initialPosition.x) * timeOfChange;
            //Because both y localPositions are negative
            float rateOfPositionChangeY = (initialPosition.y - tempPosition.y) * timeOfChange;
            while(healthBarRect.localScale != initialScale)
            {
                tempScale.x -= rateOfChangeX;
                tempScale.y -= rateOfChangeY;
                healthBarRect.localScale = tempScale;
                tempPosition.x -= rateOfPositionChangeX;
                tempPosition.y += rateOfPositionChangeY;
                healthBarRect.localPosition = tempPosition;
                //Gets slower as the time gets shorter... bad code
                yield return new WaitForSeconds(0.01f);
            }

            currentEvent++;
            GameObject.Find("Player").GetComponent<PlayerController>().EnableInteraction();
            yield break;
        }
    }

    //Enables or disables player movement according to the parameter
    void SetPlayerMovement(bool enabled)
    {
        GameObject.Find("Player").GetComponent<PlayerController>().SetMovementEnabled(enabled);
        GameObject.Find("Player").GetComponent<PlayerController>().SetCameraMovementEnabled(enabled);
    }

    IEnumerator Talk()
    {
        talkingToPlayer = true;
        yield return StartCoroutine(GameObject.Find("Player").GetComponent<PlayerController>().IntroduceNewText(currentLine, writingSoundEffect, characterWriteDelay, 1000, gameObject));

        if (currentEvent == 0)
        {
            GameObject.Find("Player").GetComponent<PlayerController>().SetTutorialText("Press F to continue");
        }

    }

    void FinishConversation()
    {
        talkingToPlayer = false;
        GameObject.Find("Player").GetComponent<PlayerController>().SendMessage("FinishConversation");
    }

    void LeaveRoom()
    {
        GameObject.Find("Door").GetComponent<AudioSource>().Play();
        GameObject.Find("Player").GetComponent<PlayerController>().SetInteractivity(true);
        shouldMove = false;
        Destroy(gameObject, GameObject.Find("Door").GetComponent<AudioSource>().clip.length);
        //Allows the player to interact with the door
    }

    //Should be moved to Player Controller later

    public override void DoAction()
    {
        ContinueConversation();
    }

    void ContinueConversation()
    {
        if (currentEvent == 0)
        {
            GameObject.Find("Player").GetComponent<PlayerController>().SetTutorialText("");
            currentLine = "Now, allow me to turn on the lights.";
            currentEvent++;
        }

        else if (currentEvent == 1)
        {
            currentLine = "";
            StartCoroutine(ManageEvents());
        }

        else if (currentEvent == 2)
        {
            currentLine = "Good!|0.5| Now you know you can interact with certain objects by pressing F.|0.5| Follow me outside.";
            currentEvent++;
        }

        else if (currentEvent == 3)
        {
            currentLine = "";
            GameObject.Find("Player").GetComponent<PlayerController>().SetTutorialText("Use 'WASD' to move");
            StartCoroutine(ManageEvents());
        }

        else if (currentEvent == 4)
        {
            currentLine = "Beautiful, huh?|0.3| Well lit and flat,|0.2| perfect for practice.|0.7| Here,|0.2| you can track your health using this.";
            currentEvent++;
        }

        else if (currentEvent == 5)
        {
            currentLine = "";
            StartCoroutine(ManageEvents());
        }

        else if (currentEvent == 6)
        {
            currentLine = "There.|0.5| Now, time for the main event:|0.3| Fighting mechanics.";
            currentEvent++;
        }

        else if (currentEvent == 7)
        {
            currentLine = "The basic enemy has a simple ranged attack that'll damage you on contact.";
            currentEvent++;
        }

        else if (currentEvent == 8)
        {
            currentLine = "To defeat it, you'll need to learn how to punch first. I'll spawn a dummy for you to hit until you get the hang of it.";
            currentEvent++;
        }

        else if (currentEvent == 9)
        {
            currentLine = "Talk to me when you're done with it.";
            currentEvent++;
        }

        else if (currentEvent == 10)
        {
            currentLine = "";
            GameObject.Find("Player").GetComponent<PlayerController>().SetTutorialText("Press Left Click to attack");
            if (!dummySpawned)
            {
                dummy.SetActive(true);
            }
            currentEvent++;
        }

        else if (currentEvent == 11)
        {
            currentLine = "Talk to me when you're done with it.";
            currentEvent = 10;
        }

        else if (currentEvent == 12)
        {
            currentLine = "You're done?|0.5| Okay.|0.3| I'll take back the dummy and we'll move on to the next step.";
            dummy.SetActive(false);
            currentEvent++;
        }

        else if (currentEvent == 13)
        {
            lastCheckpoint = currentEvent;
            currentLine = "Done.|0.3| Welp,|0.2| the next step is real combat.";
            currentEvent++;
        }

        else if (currentEvent == 14)
        {
            currentLine = "I'll create a basic enemy for you.|0.3| Should be easy enough.";
            GameObject.Find("Player").GetComponent<PlayerController>().DisplayWarning("Finishing this conversation will spawn an enemy");
            currentEvent++;
        }

        else if (currentEvent == 15)
        {
            currentLine = "";
            GameObject.Find("Player").GetComponent<PlayerController>().SetTutorialText("");
            if (!firstEnemy.activeInHierarchy)
            {
                firstEnemy.SetActive(true);
            }
            firstEnemy.SetActive(true);
            currentEvent++;
        }

        else if (currentEvent == 16)
        {
            if (!firstEnemy)
            {
                GameObject.Find("Player").GetComponent<PlayerController>().ReplenishHealth();
                currentLine = "Easy, right?|0.5| Did you notice how difficult it gets to dodge its attacks when as you approach him, though?";
                currentEvent = 18;
            }

            else
            {
                currentLine = "Reading this doesn't seem like the best use of your time.";
                currentEvent++;
            }
        }

        else if (currentEvent == 17)
        {
            currentLine = "";
            currentEvent = 16;
        }

        else if (currentEvent == 18)
        {
            currentLine = "Yeah, you probably did.|0.3| That's why the next thing to learn is to dash.";
            currentEvent++;
        }

        else if (currentEvent == 19)
        {
            currentLine = "It's simple:|0.2| you press Q to dash left, E to dash right, and R to dash forward.";
            currentEvent++;
        }

        else if (currentEvent == 20)
        {
            lastCheckpoint = currentEvent;
            currentLine = "However,|0.1| you only have so much stamina.|0.1| Your dash attacks are just below your health.";
            currentEvent++;
        }

        else if (currentEvent == 21)
        {
            currentLine = "Also, you become invulnerable for a short time after dashing.|0.5| Test it by defeating this enemy.|0.1|.|0.1|.|0.3| without taking damage!";
            GameObject.Find("Player").GetComponent<PlayerController>().DisplayWarning("Finishing the conversation will spawn an enemy");

            dashUI.SetActive(true);
            currentEvent++;
        }

        else if (currentEvent == 22)
        {
            GameObject.Find("Player").GetComponent<PlayerController>().SetTutorialText("Press Q, E or R to dash");
            secondEnemy = (GameObject)Instantiate(secondEnemyPrefab, secondEnemySpawner.transform.position, Quaternion.identity);
            currentLine = "";
            currentEvent++;
        }

        else if (currentEvent == 23)
        {
            //If the second enemy hasn't been defeated
            if (secondEnemy)
            {
                currentLine = "More dashing, less talking!";
                currentEvent++;
            }

            //If the second enemy was already defeated
            else
            {
                if (GameObject.Find("Player").GetComponent<PlayerController>().HasMaxHealth())
                {
                    GameObject.Find("Player").GetComponent<PlayerController>().SetTutorialText("");
                    currentLine = "Well done!";
                    currentEvent = 25;
                }

                else
                {
                    currentLine = "Not that straightforward this time, huh?|0.4| Here, try again.";
                    GameObject.Find("Player").GetComponent<PlayerController>().ReplenishHealth();
                    GameObject.Find("Player").GetComponent<PlayerController>().DisplayWarning("Finishing the conversation will spawn an enemy");
                    currentEvent = 22;
                }
            }
        }

        else if (currentEvent == 24)
        {
            currentLine = "";
            currentEvent--;
        }

        else if (currentEvent == 25)
        {
            currentLine = "Final lesson then. This one is about manipulating the environment's energy; it's different for everyone, but I'd guess yours is related to speed";
            currentEvent++;
        }

        else if (currentEvent == 26)
        {
            currentLine = "First step is visualization. There're certain spots in which energy is more dense and can be used easily. Concentrate and you'll be able to see them";
            currentEvent++;
        }

        else if (currentEvent == 27)
        {
            visorUI.SetActive(true);
            currentLine = "";
            GameObject.Find("Player").GetComponent<PlayerController>().DisplayTip("Hold LeftShift to concentrate");
            currentEvent--;
        }

        else if (currentEvent == 28)
        {
            currentLine = "That's it. Now go to the spot and something should happen";
            currentEvent++;
        }

        else if (currentEvent == 29)
        {
            currentLine = "";
            currentEvent--;
        }

        else if (currentEvent == 30)
        {
            currentLine = "Nice, I knew it had something to do with speed. Apparently, each stack makes you faster and your " +
               "dashes more effective. You lose them after some time, though, and surely there's a limit to how many you can have at a time";
            currentEvent++;
        }

        else if (currentEvent == 31)
        {
            currentLine = "Also, you can prolly only see them spots a few meters around you each time you concentrate, so it's prolly "+ 
                "a good idea to try to see them constantly while fighting. ";
            currentEvent++;
        }

        else if (currentEvent == 32)
        {
            currentLine = "Er... Hmm... Uh...";
            currentEvent++;
        }

        else if (currentEvent == 33)
        {
            currentLine = "I don't really have a way to explain that better right now. Just spam LeftShift if you want to see more power ups. " + 
                "If I come up with an idea to teach you, I'll put it here";
            currentEvent++;
        }

        else if (currentEvent == 34)
        {
            currentLine = "";
            currentEvent++;
        }

        else if (currentEvent == 35)
        {

        }

        if (currentLine == "")
        {
            FinishConversation();
        }

        else if (currentLine != "")
        {
            StartCoroutine(Talk());
        }
    }

    public void RegisterDummyPunch()
    {
        GameObject.Find("Player").GetComponent<PlayerController>().SetTutorialText("");
        currentEvent = 12;
    }

    public void Retry()
    {
        currentEvent = lastCheckpoint;
    }
}
