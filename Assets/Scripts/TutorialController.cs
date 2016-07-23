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

    //--------------------------------------------------------------------------------------------------------------
    //Variables
    //--------------------------------------------------------------------------------------------------------------

    //Which event is the tutorial currently in?
    //Event 0: Baroth introduces himself and tells the player he is heading towards a dangerous place.
    //Event 1: secondDialog plays. Baroth leaves the room
    //Event 2: The player is now outside. Baroth explains health bar.
    private static int currentEvent;

    //Should Baroth be moving?
    private bool shouldMove;

    //Should Baroth leave the room?
    private bool leave;

    //What Baroth should be saying
    private string currentLine;

    //Sounds when 'talking'
    private AudioSource writingSoundEffect;

    private bool talkingToPlayer;

    private ArrayList dialog;

    private int currentDialogIndex;

    //--------------------------------------------------------------------------------------------------------------
    //Functions
    //--------------------------------------------------------------------------------------------------------------

    void Start()
    {
        Debug.Log("currentEvent: " + currentEvent);
        dialog = new ArrayList();
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
            StartCoroutine(ManageEvents());
        }

        else if (currentEvent == 4)
        {
            currentLine = "You're numb, huh? Don't worry. Here's something for you to track your own health";
            currentEvent++;
        }

        else if ( currentEvent == 5)
        {
            currentLine = "";
            StartCoroutine(ManageEvents());
        }

        //Debug.Log("currentLine: " + currentLine);
        if (currentLine == "")
        {
            FinishConversation();
        }

        else if (currentLine != "")
        {
            StartCoroutine(Talk());
        }
    }
}
