using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Analytics;
using System.Collections.Generic;

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

    public GameObject speedUI;

    public GameObject firstPowerUpEnemies;

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

    public GameObject sfxContainer;

    private PlayerController playerController;

    //--------------------------------------------------------------------------------------------------------------
    //Functions
    //--------------------------------------------------------------------------------------------------------------

    void Awake()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }
    void Start()
    {
       // currentEvent = 4;
        Cursor.visible = false;
        shouldMove = false;
        StartCoroutine(ManageEvents());
        writingSoundEffect = GameObject.Find("BlueP").GetComponent<AudioSource>();
        //Debug.Log("enemies childs: " + firstPowerUpEnemies.transform.childCount);
        //Debug.Log("exists: " + firstPowerUpEnemies.transform.Find("Basic_Enemy (1)"));
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

        //if (currentEvent >= 26 && currentEvent < 28)
        //{
        //    Collider[] colliders = Physics.OverlapSphere(transform.position, 10);
        //    for (int i = 0; i < colliders.Length; i++)
        //    {
        //        if (colliders[i].gameObject.tag == "PowerUpPlate")
        //        {
        //            //playerController.DisplayTip("");
        //            playerController.DisplayTip("Touch the spot to get a power up");
        //            currentEvent = 28;
        //        }
        //    }
        //}

        //if (currentEvent >= 28 && currentEvent < 30)
        //{
        //    bool touchedPowerUp = true;
        //    Collider[] colliders = Physics.OverlapSphere(transform.position, 10);
        //    for (int i = 0; i < colliders.Length; i++)
        //    {
        //        if (colliders[i].gameObject.tag == "PowerUpPlate")
        //        {
        //            touchedPowerUp = false;
        //        }
        //    }

        //    if(touchedPowerUp)
        //    {
        //        currentEvent = 30;
        //    }
        //}
    }

    public static void SetCurrentEvent(int current)
    {
        currentEvent = current;
    }

    IEnumerator ManageEvents()
    {
        if (currentEvent == 0)
        {
           // Debug.Log("starts 0 event");
            //So the player can't interact with it at first
            gameObject.tag = "Untagged";
            SetPlayerMovement(false);
            yield return new WaitForSeconds(1);

            //Starts talking, then leave it to the player to continue
            currentLine = "Hey there!|1| This is a demo that serves as a proof of concept for certain game mechanics.|0.7| I,|0.2| Blue P.,|0.2| will be your guide.";
            //playerController.SetInteractivity(false);
            yield return StartCoroutine(Talk());
            yield break;
        }

        if (currentEvent == 1)
        {
            playerController.DisplayTip("");
            currentEvent++;
            yield return new WaitForSeconds(0.3f);
            AudioSource audio = sfxContainer.GetComponents<AudioSource>()[1];
            audio.Play();
            yield return new WaitForSeconds(audio.clip.length + 0.05f);
            roomLight.SetActive(true);
            gameObject.tag = "Interactive";
            playerController.SetCameraMovementEnabled(true);
            //So the player can interact with it from now on
            yield break;
        }

        if (currentEvent == 3)
        {
            playerController.SetMovementEnabled(true);
            shouldMove = true;
            currentEvent++;
            yield break;
        }

        if (currentEvent == 5)
        {
            playerController.DisableInteraction();
            Vector3 initialScale = healthBar.transform.localScale;
            Vector3 tempScale = new Vector3(1.6f, 4, 1);
            RectTransform healthBarRect = healthBar.GetComponent<RectTransform>();
            Vector3 initialPosition = healthBarRect.localPosition;
             //Debug.Log("healthBar initialPosition: " + initialPosition);
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

            sfxContainer.GetComponents<AudioSource>()[0].Play();
            currentEvent++;
            playerController.EnableInteraction();
            yield break;
        }

        if(currentEvent == 1001)
        {
            playerController.SetDashStacks(3);
            playerController.DisableInteraction();
            playerController.DisplayTip("");
            currentEvent = 21;
            Vector3 initialScale = dashUI.transform.localScale;
            Vector3 tempScale = new Vector3(3f, 4, 1);
            RectTransform dashUIRect = dashUI.GetComponent<RectTransform>();
            Vector3 initialPosition = dashUIRect.localPosition;
            Vector3 tempPosition = new Vector3(600, -450, 0);
            dashUIRect.localScale = tempScale;
            dashUIRect.localPosition = tempPosition;
            dashUI.SetActive(true);

            //Because with 0.01f per delta, there's 1000 deltas per second
            float timeOfChange = 1 / (1.5f * 100);
            float rateOfChangeX = (tempScale.x - initialScale.x) * timeOfChange;
            float rateOfChangeY = (tempScale.y - initialScale.y) * timeOfChange;
            float rateOfPositionChangeX = (tempPosition.x - initialPosition.x) * timeOfChange;

            //Because both y localPositions are negative
            float rateOfPositionChangeY = (initialPosition.y - tempPosition.y) * timeOfChange;
            while (dashUIRect.localScale != initialScale)
            {
                tempScale.x -= rateOfChangeX;
                tempScale.y -= rateOfChangeY;
                dashUIRect.localScale = tempScale;
                tempPosition.x -= rateOfPositionChangeX;
                tempPosition.y += rateOfPositionChangeY;
                dashUIRect.localPosition = tempPosition;
                //Gets slower as the time gets shorter... bad code
                yield return new WaitForSeconds(0.01f);
            }

            sfxContainer.GetComponents<AudioSource>()[0].Play();
            //currentEvent = 21;
            playerController.EnableInteraction();

        }


        if (currentEvent == 1003)
        {
            playerController.DisableInteraction();
            playerController.DisplayTip("");
            currentEvent = 1004;
            Vector3 initialScale = speedUI.transform.localScale;
            Vector3 tempScale = new Vector3(3f, 4, 0.2f);
            RectTransform speedUIRect = speedUI.GetComponent<RectTransform>();
            Vector3 initialPosition = speedUIRect.localPosition;
            Vector3 tempPosition = new Vector3(600, -450, 0);
            speedUIRect.localScale = tempScale;
            speedUIRect.localPosition = tempPosition;
            speedUI.SetActive(true);

            //Because with 0.01f per delta, there's 1000 deltas per second
            float timeOfChange = 1 / (1.5f * 100);
            float rateOfChangeX = (tempScale.x - initialScale.x) * timeOfChange;
            float rateOfChangeY = (tempScale.y - initialScale.y) * timeOfChange;
            float rateOfPositionChangeX = (tempPosition.x - initialPosition.x) * timeOfChange;

            //Because both y localPositions are negative
            float rateOfPositionChangeY = (initialPosition.y - tempPosition.y) * timeOfChange;
            while (speedUIRect.localScale.x > initialScale.x && speedUIRect.localScale.y > initialScale.y)
            {
                //Debug.Log("speedUI local scale: " + speedUIRect.localScale + " -- VS -- " + initialScale);
                tempScale.x -= rateOfChangeX;
                tempScale.y -= rateOfChangeY;
                speedUIRect.localScale = tempScale;
                tempPosition.x -= rateOfPositionChangeX;
                tempPosition.y += rateOfPositionChangeY;
                speedUIRect.localPosition = tempPosition;
                //Gets slower as the time gets shorter... bad code
                yield return new WaitForSeconds(0.01f);
            }

            sfxContainer.GetComponents<AudioSource>()[0].Play();
            playerController.EnableInteraction();
        }
    }

    //Enables or disables player movement according to the parameter
    void SetPlayerMovement(bool enabled)
    {
        playerController.SetMovementEnabled(enabled);
        playerController.SetCameraMovementEnabled(enabled);
    }

    IEnumerator Talk()
    {
        talkingToPlayer = true;
        yield return StartCoroutine(playerController.IntroduceNewText(currentLine, writingSoundEffect, characterWriteDelay, 1000, gameObject));

        if (currentEvent == 0)
        {
            playerController.DisplayTip("Press F to continue the conversation");
        }

    }

    void FinishConversation()
    {
        talkingToPlayer = false;
        playerController.FinishConversation();
    }

    void LeaveRoom()
    {
        GameObject.Find("Door").GetComponent<AudioSource>().Play();
        playerController.SetInteractivity(true);
        shouldMove = false;
        Destroy(gameObject, GameObject.Find("Door").GetComponent<AudioSource>().clip.length);
        //Allows the player to interact with the door
    }

    //Should be moved to Player Controller later
    public override void DoAction()
    {
        ContinueConversation();
    }

    IEnumerator CheckIfThePlayerTakesTooLong()
    {
        //Checks after waiting 
        if(currentEvent == 0)
        {
            yield return new WaitForSeconds(4);
            if(currentEvent <= 1)
            {
                //If the player hasn't understood that he presses F to forward conversations and takes too long to forward
                //the next one, this will appear.
                playerController.DisplayTip("Press F to continue the conversation");
            }
        }

        if (currentEvent == 2)
        {
            yield return new WaitForSeconds(6);
            if (currentEvent <= 2)
            {
                //If the player hasn't understood that he presses F to forward conversations and takes too long to forward
                //the next one, this will appear.
                playerController.DisplayTip("Press F to continue the conversation");
            }
        }

        if (currentEvent == 4)
        {
            yield return new WaitForSeconds(11);
            if (currentEvent <= 5)
            {
                //If the player hasn't understood that he presses F to forward conversations and takes too long to forward
                //the next one, this will appear.
                playerController.DisplayTip("Press F to continue the conversation");
            }
        }

        if (currentEvent == 20)
        {
            yield return new WaitForSeconds(8);
            if (currentEvent == 1001)
            {
                //If the player hasn't understood that he presses F to forward conversations and takes too long to forward
                //the next one, this will appear.
                playerController.DisplayTip("Press F to continue the conversation");
            }
        }
    }

    void ContinueConversation()
    {
        if (currentEvent == 0)
        {
            playerController.DisplayTip("");
            currentLine = "Now,|0.1| allow me to turn on the lights.";
            StartCoroutine(CheckIfThePlayerTakesTooLong());
            currentEvent++;
        }

        else if (currentEvent == 1)
        {
            playerController.DisplayTip("");
            currentLine = "";
            StartCoroutine(ManageEvents());
        }

        else if (currentEvent == 2)
        {
            currentLine = "Good!|0.5| You just learned you can interact with certain objects by pressing F.|0.4| Follow me outside.";
            StartCoroutine(CheckIfThePlayerTakesTooLong());
            currentEvent++;
        }

        else if (currentEvent == 3)
        {
            playerController.DisplayTip("");
            currentLine = "";
            playerController.DisplayTip("Use W, A, S and D to move");
            StartCoroutine(ManageEvents());
        }

        else if (currentEvent == 4)
        {
            currentLine = "Beautiful, huh?|0.2| Well lit and flat,|0.2| perfect for practice.|0.4| Here,|0.2| you can track your health using this.";
            StartCoroutine(CheckIfThePlayerTakesTooLong());
            currentEvent++;
        }

        else if (currentEvent == 5)
        {
            playerController.DisplayTip("");
            currentLine = "";
            StartCoroutine(ManageEvents());
        }

        else if (currentEvent == 6)
        {
            currentLine = "There.|0.4| Now, time for the main event:|0.3| Fighting mechanics.";
            currentEvent++;
        }

        else if (currentEvent == 7)
        {
            currentLine = "The basic enemy has a simple ranged attack that'll damage you on contact.";
            currentEvent++;
        }

        else if (currentEvent == 8)
        {
            currentLine = "To defeat it,|0.1| you'll need to learn how to punch first.";
            currentEvent++;
        }

        else if (currentEvent == 9)
        {
            currentLine = "I'll spawn an immortal dummy for you to practice.";
            currentEvent++;
        }

        else if (currentEvent == 10)
        {
            currentLine = "";
            playerController.DisplayTip("Press Left Click to attack");
            if (!dummySpawned)
            {
                dummy.SetActive(true);
            }
            currentEvent++;
        }

        else if (currentEvent == 11)
        {
            currentLine = "Go ahead.|0.4| Punch it.";
            currentEvent = 10;
        }

        else if (currentEvent == 12)
        {
            // currentLine = "You're done?|0.4| Okay.|0.2| I'll take back the dummy and we'll move on to the next step.";
            //  Use this call for wherever a player triggers a custom event

            AnalyticsUtils.RegisterDummyEvent();
            playerController.DisplayTip("");
            currentLine = "You're done?|0.4| Okay.|0.2| Next step is real combat.";
            dummy.SetActive(false);
            currentEvent = 14;
        }

        else if (currentEvent == 13)
        {
            lastCheckpoint = currentEvent;
            currentLine = "Done.|0.3| Welp,|0.2| the next step is real combat.";
            currentEvent++;
        }

        else if (currentEvent == 14)
        {
            currentLine = "I'll create an enemy for you.";
            playerController.DisplayWarning("Finishing this conversation will spawn an enemy");
            currentEvent++;
        }

        else if (currentEvent == 15)
        {
            currentLine = "";
            playerController.DisplayTip("");
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
                playerController.ReplenishHealth();
                currentLine = "Good punch!|0.4| Now:|0.2| dashing.";
                currentEvent = 18;
            }

            else
            {
                currentLine = "Go ahead.|0.4| Fight it.";
                currentEvent++;
            }
        }

        else if (currentEvent == 17)
        {
            currentLine = "";
            currentEvent = 16;
        }

        //else if (currentEvent == 18)
        //{
        //    currentLine = "You become invulnerable for a short time after dashing, so you can use it to avoid damage.";
        //    currentEvent++;
        //}

        //else if (currentEvent == 19)
        //{
        //    currentLine = "It's simple:|0.2| you press Left Shift to dash in the direction you're moving.";
        //    currentEvent++;
        //}

        else if (currentEvent == 18)
        {
            currentLine = "It's simple:|0.2| press LeftShift to dash in the direction you're moving.";
            currentEvent++;
        }

        else if (currentEvent == 19)
        {

            currentLine = "";
            playerController.DisplayTip("Press LeftShift to dash");
            currentEvent = 1007;
        }

        else if (currentEvent == 1007)
        {
            playerController.DisplayTip("");
            //currentLine = "You become @<b>i</b>@@<b>n</b>@@<b>v</b>@@<b>u</b>@@<b>l</b>@@<b>n</b>@@<b>e</b>@@<b>r</b>@@<b>a</b>@@<b>b</b>@@<b>l</b>@@<b>e</b>@ for a short time after dashing, so you can use it to avoid damage.";
            currentLine = "You become invulnerable for a short time after dashing.";
            currentEvent = 20;
        }

        else if (currentEvent == 20)
        {
            lastCheckpoint = currentEvent;
            currentLine = "However,|0.1| you only have so much stamina.|0.2| Use this to track the dashes you have left.";
            StartCoroutine(CheckIfThePlayerTakesTooLong());
            currentEvent = 1001;
        }

        else if (currentEvent == 1001)
        {
            playerController.DisplayTip("");
            currentLine = "";
            StartCoroutine(ManageEvents());
        }

        else if (currentEvent == 21)
        {
            currentLine = "I'll spawn another basic enemy for you to practice.";
            playerController.DisplayWarning("Finishing the conversation will spawn an enemy");
            currentEvent = 1002 ;
        }

        else if (currentEvent == 1002)
        {
            currentLine = "Use your invulnerability to go through one of his attacks!";
            currentEvent = 22;
        }

        else if (currentEvent == 22)
        {
            currentLine = "";
            playerController.DisplayTip("Press LeftShift to dash");
            secondEnemy = (GameObject)Instantiate(secondEnemyPrefab, secondEnemySpawner.transform.position, Quaternion.identity);
            //Debug.Log(secondEnemy.transform.position);
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
                if (playerController.GetBlockedAttacks() > 0)
                {
                    playerController.DisplayTip("");
                    playerController.ReplenishHealth();
                    currentLine = "Well done!|0.2| Final lesson then:|0.3| power ups.";
                    currentEvent = 25;
                }

                else
                {
                    currentLine = "Not so easy to dash through attacks, huh?|0.4| Here, try again.";
                    playerController.ReplenishHealth();
                    playerController.DisplayWarning("Finishing the conversation will spawn an enemy");
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
            currentLine = "You pick them up, they make you faster. ";
            currentEvent++;
        }

        else if (currentEvent == 26)
        {
            currentLine = "Use this to track them.";
            currentEvent = 1003;
        }

        else if (currentEvent == 1003)
        {
            currentLine = "";
            StartCoroutine(ManageEvents());
        }

        else if (currentEvent == 1004)
        {
            currentLine = "Next step:|0.3| seeing power ups.";
            currentEvent = 1005;
        }

        else if (currentEvent == 1005)
        {
            currentLine = "Hold LeftCtrl to see the power ups around you.";
            currentEvent = 27;
        }

        else if (currentEvent == 27)
        {
            visorUI.SetActive(true);
            currentLine = "";
            playerController.DisplayTip("Hold LeftCtrl to see power ups");
            currentEvent = 1005;
        }

        else if (currentEvent == 28)
        {
            currentLine = "Pick them up and they'll make you faster.";
            currentEvent++;
        }

        else if (currentEvent == 29)
        {
            currentLine = "";
            currentEvent--;
        }

        else if (currentEvent == 30)
        {
            currentLine = "Yeah!|0.2| Now, how would it be to fight|0.1| WITH|0.1| power ups on?";
            currentEvent++;
        }

        else if (currentEvent == 31)
        {
            //currentLine = "Thank you for playing!|0.2| I'll be adding the following stuff soon:";
            lastCheckpoint = 31;
            currentLine = "Find out for yourself!";
            playerController.DisplayWarning("Finishing this conversation will spawn an enemy");
            currentEvent++;
        }

        else if (currentEvent == 32)
        {
            currentLine = "";
            playerController.DisplayTip("");
            SpawnFirstPowerUpEnemies();
            currentEvent++;
        }

        else if (currentEvent == 33)
        {
         //   currentLine = "Enemies grunt when you kill them. Enemies do not dissappear instantly, but are pushed by your punches.";
            if(FirstPowerUpEnemiesWereDefeated())
            {
                currentLine = "That was awesome!";
                currentEvent++;
            }

            else
            {
                currentLine = "Fight!!";
                currentEvent--;
            }

            //currentEvent++;
        }

        else if (currentEvent == 34)
        {
            currentLine = "This is as far as the demo goes right now. Thank you so much for playing!";
           // currentLine = "Footsteps sound effects. Small practice with enemies using the power up mechanic. After that, final challenge with lots of enemies and a cool soundtrack.";
            currentEvent++;
        }

        else if (currentEvent == 35)
        {
            currentLine = "For feedback, you can comment the project at itch.io or answer the form at https://goo.gl/vriZqH";
            currentEvent++;
        }

        else if (currentEvent == 36)
        {
            currentLine = "Or, you can send an email to the developer at ramell.jack@gmail.com!|0.3| He'd be really happy!";
            currentEvent++;
        }

        else if (currentEvent == 37)
        {
            currentLine = "I'll now talk about coming features, so I'll give you a chance to quit in case you're not interested";
            currentEvent++;
        }

        else if (currentEvent == 38)
        {
            currentLine = "";
            currentEvent++;
        }

        else if (currentEvent == 39)
        {
            currentLine = "Soon a big final challenge will be available so you can test everything learned in the demo at a greater scale.";
            currentEvent++;
        }

        else if (currentEvent == 40)
        {
            currentLine = "Also, game feel will be improved through the addition footsteps sound effects and betterment of game feedback enemies are hit";
            currentEvent++;
        }

        else if (currentEvent == 41)
        {
            currentLine = "Better credits, animations and optimization are a must,|0.1| but they'll take a little longer.";
            currentEvent++;
        }

        else if (currentEvent == 42)
        {
            currentLine = "Remember, for feedback you can comment the project at itch.io or answer the form at https://goo.gl/vriZqH";
            currentEvent++;
        }

        else if (currentEvent == 43)
        {
            //currentLine = "I know this is not a pretty way to do it, but releasing any kind of build without them would be unethical.";
            currentLine = "";
            currentEvent--;
        }

        else if (currentEvent == 44)
        {
            //currentLine = "I'll give you a chance to quit again, in case you're not interested.";
            currentLine = "";
            currentEvent = 42;
        }

        else if (currentEvent == 45)
        {
            currentLine = "";
            currentEvent++;
        }

        else if (currentEvent == 46)
        {
            currentLine = "Here is a link to the credits: https://goo.gl/X6Xap1";
            currentEvent++;
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
        playerController.DisplayTip("");
        currentEvent = 12;
    }

    public void Retry()
    {
        currentEvent = lastCheckpoint;
    }

    public static int GetCurrentEvent()
    {
        return currentEvent;
    }

    public void PressedLeftCtrl()
    {
        if(currentEvent == 1005)
        {
        currentEvent = 28;
            playerController.DisplayTip("Touch a power up to get faster");
        }
    }

    public void PlayerTouchedPowerUp()
    {
        if (currentEvent == 28)
        {
            currentEvent = 30;
        }
    }

    private void SpawnFirstPowerUpEnemies()
    {
        if(!FirstPowerUpEnemiesWereDefeated())
        {
            firstPowerUpEnemies.SetActive(true);
            float childCount = firstPowerUpEnemies.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                firstPowerUpEnemies.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    private bool FirstPowerUpEnemiesWereDefeated()
    {
        return firstPowerUpEnemies.transform.childCount == 0;
    }
}
