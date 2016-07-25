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

    //--------------------------------------------------------------------------------------------------------------
    //Functions
    //--------------------------------------------------------------------------------------------------------------

    void Start()
    {
        Debug.Log("currentEvent: " + currentEvent);
        //currentEvent = 0;
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

        else if (currentEvent > 4)
        {
            gameObject.tag = "Interactive";
        }
    }

    //void initializeDialogList()
    //{
    //    dialog.Add("Hello, I'm Baroth");
    //    dialog.Add("You can't see, huh? Let's fix that");
    //    dialog.Add("");
    //}

    IEnumerator ManageEvents()
    {
        if (currentEvent == 0)
        {
            //So the player can't interact with it at first
            gameObject.tag = "Untagged";
            SetPlayerMovement(false);
            yield return new WaitForSeconds(1);

            //Starts talking, then leave it to the player to continue
            currentLine = "Wow, hello. This is a demo, and the dialog is not planned yet";
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

        else if (currentEvent == 1)
        {
           
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
        //Debug.Log("continued conversation with currentEvent: " + currentEvent);
        if (currentEvent == 0)
        {
            GameObject.Find("Player").GetComponent<PlayerController>().SetTutorialText("");
            currentLine = "(Something about the fact that you can't see)";
            currentEvent++;
        }
        //Debug.Log("currentLine after if: " + currentLine);

        else if (currentEvent == 1)
        {
            currentLine = "";
            StartCoroutine(ManageEvents());
        }

        else if (currentEvent == 2)
        {
            currentLine = "Let's get outta here";
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
            currentLine = "You're numb, huh? Don't worry. Here's something for you to track your own health";
            currentEvent++;
        }

        else if (currentEvent == 5)
        {
            currentLine = "";
            StartCoroutine(ManageEvents());
        }

        else if (currentEvent == 6)
        {
            currentLine = "There. Now, I'll teach you to fight so you're ready when the time comes";
            currentEvent++;
        }

        else if (currentEvent == 7)
        {
            currentLine = "In most fights in this world, your foe'll prolly have some form of ranged attack, and you prolly won't";
            currentEvent++;
        }

        else if (currentEvent == 8)
        {
            currentLine = "That's why you'll need to learn how to punch first. Go and punch that dummy 'till it disspears, then come back";
            currentEvent++;
        }

        else if (currentEvent == 9)
        {
            currentLine = "It can take a lot of hits, specially from a rookie like you, so don't feel bad if you take some time";
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
            currentLine = "It can take a lot of hits, specially from a rookie like you, so don't feel bad if it takes some time";
            currentEvent = 10;
        }

        else if (currentEvent == 12)
        {
            currentLine = "Wow. That was fast.";
            currentEvent++;
        }

        else if (currentEvent == 13)
        {
            lastCheckpoint = currentEvent;
            currentLine = "Okay, now some real combat experience. Defeat the guy that'll appear as soon as I'm done talking";
            currentEvent++;
        }

        else if (currentEvent == 14)
        {
            currentLine = "You'll know where he is 'cuz he'll start shooting energy thingies at ya";
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
            if(!firstEnemy)
            {
                currentLine = "Your speed is impressive. That'll not be enough against a lot of enemies though, so the next thing to learn is <color=red>dashing</color>";
                currentEvent = 18;
            }

            else
            {
            currentLine = "More punching, less talking!";
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
            currentLine = "When under heavy fire, <color=red>dash</color> because reasons";
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

    public void RegisterDummyDefeat()
    {
        GameObject.Find("Player").GetComponent<PlayerController>().SetTutorialText("");
        currentEvent = 12;
    }

    public void Retry()
    {
        currentEvent = lastCheckpoint;
    }
}
