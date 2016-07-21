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

    public float textWriteDelay;

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

    //--------------------------------------------------------------------------------------------------------------
    //Functions
    //--------------------------------------------------------------------------------------------------------------

    void Start()
    {
        currentEvent = 0;
        shouldMove = false;
        StartCoroutine(ManageEvents());
        writingSoundEffect = GameObject.Find("Baroth").GetComponent<AudioSource>();
    }

    void Update()
    {
        if (shouldMove)
        {
            transform.position = NPCUtils.MoveTo(transform.position, frontOfDoor.position, speed);
            if (transform.position == frontOfDoor.position)
            {
                LeaveRoom();
            }
        }
    }

    IEnumerator ManageEvents()
    {
        if (currentEvent == 0)
        {
            SetPlayerMovement(false);
            yield return new WaitForSeconds(1);
            currentLine = "Hello, I'm Baroth";
            yield return StartCoroutine(Talk());
            yield return new WaitForSeconds(1);
            currentLine = "(Something about the fact that you can't see)";
            yield return StartCoroutine(Talk());
            roomLight.SetActive(true);
            GameObject.Find("Player").GetComponent<PlayerController>().SetCameraMovementEnabled(true);
            GameObject.Find("Player").GetComponent<PlayerController>().SetMovementEnabled(true);
            currentEvent++;

            yield break;
        }

        if (currentEvent == 1)
        {
            GameObject.Find("Player").GetComponent<PlayerController>().SetMovementEnabled(true);
            currentLine = "Now let's get out of here";
            yield return StartCoroutine(Talk());
            shouldMove = true;
        }

        if (currentEvent == 2)
        {

        }

        if (currentEvent == 3)
        {

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
        yield return StartCoroutine(GameObject.Find("Player").GetComponent<PlayerController>().IntroduceNewText(currentLine, writingSoundEffect, 1));
    }

    void LeaveRoom()
    {
        GameObject.Find("Door").GetComponent<AudioSource>().Play();
        Destroy(gameObject, GameObject.Find("Door").GetComponent<AudioSource>().clip.length);
       
        //Allows the player to interact with the door
        GameObject.Find("Player").GetComponent<PlayerController>().SetInteractivity(true);
    }

    //Should be moved to Player Controller later

    public override void DoAction()
    {
        StartCoroutine(ManageEvents());
    }
}
