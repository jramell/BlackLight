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
            currentLine = "Hey there. This is a demo that serves as a proof of concept for certain game mechanics. I, Blue P., will be your guide.";
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
            healthBar.SetActive(true);
            currentEvent++;
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
            currentLine = "Good! Now you know you can interact with certain objects by pressing F. Follow me outside.";
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
            currentLine = "Beautiful, huh? Well lit and flat, perfect for practice. Here, you can track your health using this.";
            currentEvent++;
        }

        else if (currentEvent == 5)
        {
            currentLine = "";
            StartCoroutine(ManageEvents());
        }

        else if (currentEvent == 6)
        {
            currentLine = "There. Now, time for the main event: Fighting mechanics. You can call me Blue, by the way.";
            currentEvent++;
        }

        else if (currentEvent == 7)
        {
            currentLine = "The basic enemy has a simple ranged attack that'll damage you on contact.";
            currentEvent++;
        }

        else if (currentEvent == 8)
        {
            currentLine = "To defeat it, you'll need to know how to <i>hit</i> first. I'll spawn a dummy for you to punch until you get the hang of it.";
            currentEvent++;
        }

        else if (currentEvent == 9)
        {
            currentLine = "Talk to me when you're done with it and feel ready to move on.";
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
            currentLine = "You're done? Okay. I'll take back the dummy and we'll move on to the next step: real combat.";
            dummy.SetActive(false);
            currentEvent++;
        }

        else if (currentEvent == 13)
        {
            lastCheckpoint = currentEvent;
            currentLine = "Done. Welp, now you know how to hit stuff. Don't you want an opportunity to do it against something a little more <i>challenging</i> this time?";
            currentEvent++;
        }

        else if (currentEvent == 14)
        {
            currentLine = "I'll create a basic enemy for you. Should be easy enough.";
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
                currentLine = "Easy, right? Did you notice how difficult it gets to dodge its attacks when as you approach him, though?";
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
            currentLine = "Yeah, you probably did. That's why the next thing to learn is to <color=orange>dash</color>";
            currentEvent++;
        }

        else if (currentEvent == 19)
        {
            currentLine = "You're immune to damage for a short amount of time after dashing, so it might be a good idea to do it when approaching your enemy";
            currentEvent++;
        }

        else if (currentEvent == 20)
        {
            lastCheckpoint = currentEvent;
            currentLine = "Also, you use one <color=red>dash stack</color> each time you dash. These stacks will regenerate after some time, but you can have a maximum of three in stock, so be careful. Here, try defeating this guy without taking damage";
            currentEvent++;
        }

        else if (currentEvent == 21)
        {
            currentLine = "Also, when I say it <i>might</i> be a good idea, I mean it <i>is</i>";
            GameObject.Find("Player").GetComponent<PlayerController>().DisplayWarning("Finishing the conversation will spawn an enemy");

            dashUI.SetActive(true);
            currentEvent++;
        }

        else if (currentEvent == 22)
        {
            GameObject.Find("Player").GetComponent<PlayerController>().SetTutorialText("");
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
                    currentLine = "Amazing...";
                    currentEvent = 25;
                }

                else
                {
                    currentLine = "Try again.";
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
