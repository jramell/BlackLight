﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;

public class PlayerController : MonoBehaviour
{

    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    //--------------------------------------------------------------------------------------------------------------
    //Public variables
    //--------------------------------------------------------------------------------------------------------------

    ///Player's current speed
    public float speed;

    ///Attack cooldown
    public float attackCooldown;

    ///Player's health points
    public int healthPoints;

    ///(Main) camera's sensitivity
    public float sensitivity;

    ///Force applied when the player jumps
    public float jumpForce;

    ///Player's attack range
    public float attackRange;

    ///How much attack does the player have
    public int attack;

    ///Dash cooldown
    public float dashCooldown;

    //public float lateralDashForce;

    public float dashForce;

    //public float forwardDashForce;

    //Maximum dash stacks 
    public int dashMaxStacks;

    //Time a dash stack takes to charge
    public float dashStackChargingTime;

    //Percentage of speed each speed stack adds
    public float speedStackEffect;

    //Time shift has to be pressed to activate Vission
    public float timeToActivateVission;

    //Time a speed stack takes to dissapear
    public float speedStackLoseTime;

    //Vission radius
    public float vissionRadius;

    public int maxSpeedStacks;

    public float interactionRange;

    //How much time is the player invulnerable after dashing?
    public float dashInvulnerabilityTime;

    //The time it takes for health to smooth into its new fill amount
    public float healthUpdateTime;


    public float contrastInBattleMode;

    public float maxFieldOfView;

    public float fieldOfViewAugment;

    //Health bar foreground to be depleted when damage is taken
    public Image healthBar;

    //Screen that appears when damage is taken
    public GameObject damageEffect;

    public Image dashStackLoadingCounter;

    public Text dashNumberText;

    public GameObject visorEffect;

    public Image speedStackForeground;

    public GameObject speedStackBackground;

    public Text speedStackText;

    public Text dialogText;

    public Text tutorialText;

    public GameObject deathUI;

    public GameObject crosshair;

    public GameObject dashInvulnerabilityEffect;

    public GameObject sfxContainer;


    //--------------------------------------------------------------------------------------------------------------
    //Private variables
    //--------------------------------------------------------------------------------------------------------------

    private bool isVisorOn;
    //Player's speed without modifiers
    private float baseSpeed;

    ///Ray cast from the player's mouse position. Stored as a variable so it can be updated without allocating a new variable
    ///in each frame
    private Ray ray;

    ///Main camera of the game. Stored as a variable so it is not necessary to get it from the instantiated game objects in scene 
    ///every frame
    private Camera camera;

    ///Time when the last attack happened
    private float lastAttack;

    ///Stores horizontal (relative to the y axis) camera rotation
    private float xRot;

    ///Stores vertical (relative to the x axis) camera rotation
    private float yRot;

    ///True only if the player is on the ground
    private bool isGrounded;

    ///Crosshair that appears when attack succeeds
    private Image crosshairSuccess;

    ///Crosshair that appears when attack fails
    private Image crosshairFail;

    ///Sound that plays when attack fails 
    private AudioSource attackFail;

    ///Sound when attack succeeds
    private AudioSource attackSuccess;

    ///True only if the player is not dead
    private bool isDead;

    ///The moment the last dash occurred
    private float lastDash;

    //Number of stacks at the moment
    private int dashStackAmount;

    ///Player's rigidbody
    private Rigidbody rigidbody;

    private int maxHealthPoints;

    ///How much time has the last stack been charging
    private float accumulatedStackChargingTime;

    private bool isChargingStacks;

    //The effect the speed stacks are having
    private float effect;

    //Is the game paused?
    private bool isPaused;

    //Number of stacks of speed power up
    private int speedPowerUpStacks;

    //If true, the time a speed stack takes to be lost will reset 
    private bool resetSpeedStackLoss;

    //Is the player losing speed stacks right now?
    private bool losingSpeedStacks;

    //private float baseForwardDashForce;

    private float baseDashForce;

    //private float baseLateralDashForce;

    //Sound effect played at death
    private AudioSource deathSound;

    //Should interactions be possible?
    private bool canInteract;

    private RaycastHit hit;

    //Pause menu
    public GameObject pauseMenu;

    //Text component that will display interaction information
    public GameObject interactText;

    //Text element in which the dialog will be introduced
    //private Text dialogText;

    //Current instance of the text coroutine. Null if it is not in progress.
    private IEnumerator introduceTextCoroutineInstance;

    private AudioSource dialogSoundBeingReproduced;

    //If set to true, the player will be able to look around. 
    private bool canLookAround;

    //Is the movement enabled for the player?
    private bool movementEnabled;

    private GameObject objectTalkingTo;

    //Is the camera movement enabled for the player?
    private bool cameraMovementEnabled;

    private bool introducingText;

    private bool shouldSkipText;

    //Used to store the force applied to the enemies when they die
    private Vector3 force;

    //Traslation vector, updated each frame
    private Vector3 translate;
    // private bool canSkipText;
    private ContrastEnhance contrastEnhance;

    private float baseDashChargingTime;

    private float baseFieldOfView;

    private TutorialController tutorialController;

    private const string MOUSE_Y = "Mouse Y";
    private const string  MOUSE_X = "Mouse X";
    private const string ENEMY_TAG = "Enemy";
    private const string ENEMY_SPAWNER_TAG = "EnemySpawner";

    //--------------------------------------------------------------------------------------------------------------
    //Unity Analytics / Statistics
    //--------------------------------------------------------------------------------------------------------------

    //Should Unity Analytics be active?
    public const bool ANALYTICS_ACTIVE = false;

    //Name of the event that is registered when the player talks to Blue after he spawns the dummy.
    public const string PUNCH_DUMMY_EVENT_NAME = "Talk after dummy punch";

    //Name of the event that is registed when the player kills the first basic enemy
    public const string FIRST_ENEMY_KILL_EVENT_NAME = "First enemy kill";

    //Name of the event that is registered when the game is quitted by the player
    public const string QUIT_EVENT_NAME = "Quit game";

    //Number of attacks the player has received while invulnerable
    private static int blockedAttacks;

    //Number of times the player punched the dummy
    private static int timesDummyWasPunched;

    //Amount of enemies the player has killed
    private static int amountOfEnemiesKilled;

    //Amount of times the player died
    private static int amountOfTimesDied;

    //--------------------------------------------------------------------------------------------------------------
    //Functions
    //--------------------------------------------------------------------------------------------------------------

    void Awake()
    {
        contrastEnhance = transform.Find("Main Camera").gameObject.GetComponent<ContrastEnhance>();
        tutorialController = GameObject.Find("BlueP").GetComponent<TutorialController>();
        movementEnabled = true;
        cameraMovementEnabled = true;
    }

    void Start()
    {
        xRot = transform.rotation.eulerAngles.x;
        yRot = transform.rotation.eulerAngles.y;
        canInteract = true;
        objectTalkingTo = null;
        introducingText = false;
        maxHealthPoints = healthPoints;
        //    baseForwardDashForce = forwardDashForce;
        //  baseLateralDashForce = lateralDashForce;
        baseDashChargingTime = dashStackChargingTime;
      
        baseDashForce = dashForce;
        losingSpeedStacks = false;
        baseSpeed = speed;
        speedPowerUpStacks = 0;
        isPaused = false;
        //dashStackAmount = 0;
        isChargingStacks = false;
        isDead = false;
        rigidbody = GetComponent<Rigidbody>();
        attackFail = GetComponents<AudioSource>()[0];
        attackSuccess = GetComponents<AudioSource>()[1];
        deathSound = GetComponents<AudioSource>()[2];
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        baseFieldOfView = camera.fieldOfView;
        crosshairSuccess = GameObject.Find("Crosshair_Success").GetComponent<Image>();
        crosshairFail = GameObject.Find("Crosshair_Fail").GetComponent<Image>();
        //pauseMenu = GameObject.Find("PauseMenu");
        // canSkipText = true;

    }

    void Update()
    {
        //attackFail = GameObject.FindGameObjectWithTag("Player").GetComponents<AudioSource>()[0];
        //attackSuccess = GameObject.FindGameObjectWithTag("Player").GetComponents<AudioSource>()[1];
        //deathSound = GameObject.FindGameObjectWithTag("Player").GetComponents<AudioSource>()[2];

        //Only able to do this is 
        if (!isPaused && !isDead)
        {

            //If talking to someone, cannot move, move the camera, or do anything related to casting rays
            if (dialogText.text == "")
            {
                //Updates the ray casted onto the screen for various purposes
                ray = new Ray(camera.transform.position, camera.transform.forward * 2);

                Physics.Raycast(ray.origin, ray.direction, out hit);

                if (movementEnabled)
                {
                    //Moves the player
                    translate = new Vector3(Input.GetAxis(HORIZONTAL) * speed * Time.deltaTime, 0,
                                        Input.GetAxis(VERTICAL) * speed * Time.deltaTime);
                    transform.Translate(translate);

                    //if(translate.Equals)
                }

                if (cameraMovementEnabled)
                {
                    //So the player is able to rotate relative to the y axis with the camera. This is setup this way so its local axis is synchronized
                    //with what the player sees
                    transform.localEulerAngles = new Vector3(0, yRot, 0);

                    //So only the camera rotates relative to the x axis, not the player
                    camera.transform.localEulerAngles = new Vector3(xRot, 0, 0);

                    //Because vertical camera rotation is relative to the in-game x axis
                    xRot += Input.GetAxis(MOUSE_Y) * sensitivity * -1;

                    //Because horizontal camera rotation is relative to the in-game y axis
                    yRot += Input.GetAxis(MOUSE_X) * sensitivity;

                    xRot = Mathf.Clamp(xRot, -55, 35);
                }

                if (canInteract && hit.collider != null && hit.collider.tag == "Interactive" && hit.distance < interactionRange)
                {
                    //if (hit.collider != null && hit.collider.tag == "Interactive" && hit.distance < interactionRange)
                    //{
                        hit.collider.gameObject.SendMessage("UpdateInteractionText");

                        if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0))
                        {
                            hit.collider.gameObject.SendMessage("DoAction");
                        }
                    //}

//                    else if (interactText.GetComponent<Text>().text != "")
  //                  {
    //                    CleanInteractionText();
      //              }
                }

                else if (interactText.GetComponent<Text>().text != "")
                {
                    CleanInteractionText();
                }

                //Receive input for attack
                //if (Input.GetMouseButton(0))
                //{
                //    StartCoroutine(Attack());
                //}
                else if (Input.GetMouseButton(0))
                {
                    StartCoroutine(Attack());
                }

                //Receive input for jump
                if (Input.GetKey(KeyCode.Space))
                {
                    Jump();
                }

                //Receive input for dash
                Dash();

                //Charge dash
                if (CanChargeStacks() && !isChargingStacks)
                {
                    StartCoroutine(ChargeStacks());
                }

                if (Input.GetKeyDown(KeyCode.LeftControl))
                {
                    //Debug.Log("visor effect active: " + isVisorOn);
                    if (!isVisorOn)
                    {
                        StartCoroutine(ActivateEnergyVission());
                    }

                    else
                    {
                        ActivateVission(false);
                        isVisorOn = false;
                        contrastEnhance.intensity = 0;
                        //visorEffect.SetActive(false);
                    }
                }
            }

            else if (introducingText )
            {
                //if F is pressed when talking to someone
                if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0))
                {
                    shouldSkipText = true;
                }

            }

            //A conversation is active in a conversation, so it must continue when the player presses F
            else if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0))
            {
                ContinueConversation();
            }

            if (dialogText.text != "")
            {
                CleanInteractionText();
            }

        }


        //Get input for pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }

        //Receive input for retry
        if (isDead)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                Retry();
            }
        }
    }

    public static int GetAmountOfEnemiesKilled()
    {
        return amountOfEnemiesKilled;
    }

    public static void InformEnemyDeath()
    {
        amountOfEnemiesKilled++;
        if(amountOfEnemiesKilled == 1)
        {
            AnalyticsUtils.InformFirstEnemyDeath();
        }
    }

    public void Reset()
    {
        TutorialController.SetCurrentEvent(0);
    }

    // Should stacks be charging?
    bool CanChargeStacks()
    {
        return Time.timeScale > 0 && dashStackAmount < dashMaxStacks;
    }

    public void UpdateInteractionText(string text)
    {
        interactText.GetComponent<Text>().text = "[F] " + text;
    }

    void CleanInteractionText()
    {
        interactText.GetComponent<Text>().text = "";
    }

    IEnumerator ActivateEnergyVission()
    {
        float timeShiftHasBeenPressed = 0.0f;
        //float originalAlpha = visorEffect.GetComponent<Image>().color.a;
        //Color tempColor = visorEffect.GetComponent<Image>().color;
        //tempColor.a = 0;
        //visorEffect.GetComponent<Image>().color = tempColor;
        //visorEffect.SetActive(true);

        bool done = false;
        ////If the player stops pressing X, the energy vission stops activating
       while (Input.GetKey(KeyCode.LeftControl) && !done)
        {
            yield return new WaitForSeconds(0.01f);
            timeShiftHasBeenPressed += 0.01f;
            done = timeShiftHasBeenPressed > timeToActivateVission;
            //    //Percentage of the time that has charged is translated to alpha percentage and then assigned to the effect
            //    //The 0.5f at the end is so the maximum alpha to be achieved before activation is 50% the original
            //    tempColor.a = (timeShiftHasBeenPressed / timeToActivateVission) * originalAlpha * 0.8f;
            //    visorEffect.GetComponent<Image>().color = tempColor;
            contrastEnhance.intensity = (timeShiftHasBeenPressed / timeToActivateVission) * (contrastInBattleMode + 5f);
            //Debug.Log("intensity: " + contrastEnhance.intensity);
        }


        if (done)
        {
            contrastEnhance.intensity = contrastInBattleMode + 2f;
            //contrastEnhance.intensity = 0.1f;
            sfxContainer.GetComponents<AudioSource>()[2].Play();
            //tempColor.a = originalAlpha;
            //visorEffect.GetComponent<Image>().color = tempColor;
            ActivateVission(true);
            
            yield return new WaitForSeconds(0.2f);
            //Debug.Log("started with intensity " + contrastEnhance.intensity);
            float rate = contrastEnhance.intensity - contrastInBattleMode;
            while (contrastEnhance.intensity > contrastInBattleMode)
            {
               // Debug.Log("passed with intensity " + contrastEnhance.intensity);
                contrastEnhance.intensity -= rate * 0.05f;
                yield return new WaitForSeconds(0.01f);
            }
            //contrastEnhance.intensity = 2.6f;
            
            isVisorOn = true;
            tutorialController.PressedLeftCtrl();
            yield return new WaitForSeconds(0.2f);
        }

        else
        {
            isVisorOn = false;
            //visorEffect.SetActive(false);
            contrastEnhance.intensity = 0;
        }

    }

    void ActivateVission(bool activate)
    {
        //Identify GameObjects with Physics.OverlapSphere to identify colliders
        Collider[] colliders = Physics.OverlapSphere(transform.position, vissionRadius);

        //Cycles through all colliders
        for (int i = 0; i < colliders.Length; i++)
        {
            //Identifies spawners
            if (colliders[i].gameObject.tag == "PowerUpPlate_Spawn")
            {
                //Spawns PowerUpPlates where spawners are
                colliders[i].gameObject.GetComponent<PowerUpPlateSpawner>().SpawnPowerUpPlate(activate);
            }
        }
       // contrastEnhance.intensity = contrastInBattleMode + 0.5f;
    }

    IEnumerator ChargeStacks()
    {
        //Updates time counter every 1/100 of a second, and because floating point numbers can't be 
        //compared to maximum precision, this means it will take at most an extra 1/100 of a second
        //charging 

        Debug.Log("dash stacks while charging: " + dashStackAmount);
        float accumulatedStackChargingTime = 0.0f;
        isChargingStacks = true;

        while (accumulatedStackChargingTime < dashStackChargingTime)
        {
            yield return new WaitForSeconds(0.01f);
            accumulatedStackChargingTime += 0.01f;
            dashStackLoadingCounter.fillAmount = Mathf.Max(0, (float)accumulatedStackChargingTime / dashStackChargingTime);
        }
        
        if(dashStackAmount < dashMaxStacks)
        {
        dashStackAmount += 1;
        }
        accumulatedStackChargingTime = 0.0f;
        isChargingStacks = false;
        updateDashGraphicalInfo();
    }

    IEnumerator Attack()
    {
        if (Time.time - lastAttack > attackCooldown)
        {
            lastAttack = Time.time;

            if (hit.collider != null && hit.collider.tag == ENEMY_TAG && hit.distance < attackRange)
            {
                crosshairSuccess.enabled = true;
                attackSuccess.Play();
                force = hit.collider.gameObject.transform.position - hit.point;

                if (force.y < 0)
                {
                    force.y *= -1;
                }

                //force.y = Random.Range(0.1f, 1);
                force *= Random.Range(500, 1000) * effect;
                //Debug.Log("hit with force: " + force);
                //force = force.
                //hit.collider.gameObject.GetComponent<BasicEnemyController>().TakeDamageWithEffect(force, attack);
                // hit.collider.gameObject.GetComponent<BasicEnemyController>().TakeDamage(attack);
                hit.collider.gameObject.SendMessage("TakeDamage", attack);
                yield return new WaitForSeconds(0.2f);
                crosshairSuccess.enabled = false;
            }

            else
            {
                crosshairFail.enabled = true;
                attackFail.Play();
                yield return new WaitForSeconds(0.2f);
                crosshairFail.enabled = false;
            }
        }
    }

    void Jump()
    {
        if (isGrounded)
        {
            isGrounded = false;
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    IEnumerator DisplayDamageEffect()
    {
        damageEffect.SetActive(true);
        AudioSource audio = damageEffect.GetComponent<AudioSource>();
        audio.pitch = Random.Range(0.9f, 1.1f);
        audio.Play();
        yield return new WaitForSeconds(0.2f);
        damageEffect.SetActive(false);
    }

    public int GetMaxHealthPoints()
    {
        return maxHealthPoints;
    }

    public int GetHealthPoints()
    {
        return healthPoints;
    }

    //Should be called everytime the player takes damage
    public bool TakeDamage(int damage)
    {
        if (!isDead)
        {
            if (!IsInvulnerable())
            {
                StartCoroutine(DisplayDamageEffect());

                //'Max' it doesn't take negative values
                ModifyHealth(-damage);

                if (healthPoints <= 0)
                {
                    Die();
                }

                return true;
            }

            else
            {
                blockedAttacks++;
                return false;
            }
        }
        return false;
    }

    public int GetBlockedAttacks()
    {
        return blockedAttacks;
    }

    //Adds the value passed as a parameter to the current health
    void ModifyHealth(int modifyValue)
    {

        healthPoints += modifyValue;
        healthPoints = Mathf.Clamp(healthPoints, 0, maxHealthPoints);
        StartCoroutine(smoothHealthUpdate());
    }

    IEnumerator smoothHealthUpdate()
    {
        float targetFillAmount = Mathf.Max(0, (float)healthPoints / maxHealthPoints);
        float rateOfChange = (healthBar.fillAmount - targetFillAmount) * (1 / (healthUpdateTime * 100));

        //Because comparing floating points is hard, != would work strangely. That's why there is a case for > and another for <
        if (healthBar.fillAmount < targetFillAmount)
        {
            rateOfChange = rateOfChange * -1;
            while (healthBar.fillAmount < targetFillAmount)
            {
                healthBar.fillAmount += rateOfChange;
                yield return new WaitForSeconds(0.01f);
            }
        }

        else
        {
            while (healthBar.fillAmount > targetFillAmount)
            {
                healthBar.fillAmount -= rateOfChange;
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    //Is the player invulnerable at the moment?
    bool IsInvulnerable()
    {
        return Time.time - lastDash < dashInvulnerabilityTime;
    }

    void Die()
    {
        isDead = true;
        deathSound.Play();
        deathUI.SetActive(true);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(ENEMY_TAG);
        GameObject[] enemySpawners = GameObject.FindGameObjectsWithTag(ENEMY_SPAWNER_TAG);
        int length = enemies.Length;
        for (int i = 0; i < length; i++)
        {
            enemies[i].SetActive(false);
            
        }
        length = enemySpawners.Length;
        for (int k = 0; k <  length; k++)
        {
            for (int i = 0; i < length; i++)
            {
                enemySpawners[i].SetActive(false);

            }
        }

        

        amountOfTimesDied++;
    }

    public static int GetAmountOfTimesDied()
    {
        return amountOfTimesDied;
    }

    void Retry()
    {
        ReplenishHealth();
        isDead = false;
        transform.position = GameObject.Find("Respawn").transform.position;
        GameObject.Find("BlueP").GetComponent<TutorialController>().Retry();
        deathUI.SetActive(false);
    }

    //This tipe of dash uses instantenous stop rather tan deceleration. Feels weird, so not using it

    //IEnumerator DashCor()
    //{
    //    float effect = 1 + speedStackEffect * speedPowerUpStacks;
    //    speed *= 3f;
    //    updateDashInfo();
    //    yield return new WaitForSeconds(0.5f);
    //    if(speedPowerUpStacks > 0)
    //    {
    //    speed = baseSpeed * speedPowerUpStacks * effect;
    //    }
    //    else
    //    {
    //        speed = baseSpeed;
    //    }
    //}

    void Dash()
    {
        if (canDash())
        {
           // if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.R))
           if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetMouseButtonDown(1))
            {
                //StartCoroutine(DashCor());
                if (translate != Vector3.zero)
                {
                    rigidbody.AddRelativeForce(translate.normalized * dashForce, ForceMode.VelocityChange);
                }
                else
                {
                    rigidbody.AddRelativeForce(new Vector3(0, 0, dashForce), ForceMode.VelocityChange);
                }
                updateDashInfo();
                StartCoroutine(FadeIntoInvulnerability());
            }
        }
    }

    IEnumerator FadeIntoInvulnerability()
    {
        //Debug.Log("was invulnerable at start: " + IsInvulnerable());
        Image dashImage = dashInvulnerabilityEffect.GetComponent<Image>();
        Color targetColor = dashImage.color;
        Color tempColor = targetColor;
        tempColor.a = 0;
        dashImage.color = tempColor;
        dashInvulnerabilityEffect.SetActive(true);
        AudioSource audioCopy = dashInvulnerabilityEffect.GetComponent<AudioSource>();
        audioCopy.pitch = Random.Range(0.95f, 1.05f);
        audioCopy.Play();
        while (tempColor != targetColor)
        {
            tempColor.a += 0.1f * targetColor.a;
            dashImage.color = tempColor;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(dashInvulnerabilityTime - 0.16f);

        //Fade out in 0.05 secs
        Color originalColor = tempColor;
        targetColor.a = 0;
        float difference = tempColor.a;
        while (tempColor != targetColor)
        {
            tempColor.a -= difference * 0.2f;
            dashImage.color = tempColor;
            yield return new WaitForSeconds(0.01f);
        }
        dashInvulnerabilityEffect.SetActive(false);
        dashImage.color = originalColor;
        //Debug.Log("was invulnerable at end: " + IsInvulnerable());
    }

    void updateDashInfo()
    {
        lastDash = Time.time;
        dashStackAmount -= 1;
        updateDashGraphicalInfo();
    }

    void updateDashGraphicalInfo()
    {
        dashNumberText.text = dashStackAmount.ToString();
    }

    public void Pause()
    {
        //Assumes this is the component that has the background music
        GameObject bgMusic = GameObject.Find("BackgroundMusicContainer");
        if (isPaused)
        {
            Time.timeScale = 1;
            Cursor.visible = false;
            pauseMenu.SetActive(false);
            isPaused = false;
            if (bgMusic != null)
            {
                bgMusic.GetComponent<AudioSource>().volume /= 0.3f;
            }
        }

        else
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
            isPaused = true;
            Cursor.visible = true;
            if (bgMusic != null)
            {
                bgMusic.GetComponent<AudioSource>().volume *= 0.3f;
            }
        }
    }

    bool canDash()
    {
        return Time.time - lastDash > dashCooldown && Time.timeScale > 0 && dashStackAmount > 0;
    }

    //To receive power ups from the plates
    public void ReceivePowerUp(string type)
    {
        if (type == Utils.POWER_UP_SPEED)
        {
            ModifySpeedStacks(1);
            tutorialController.PlayerTouchedPowerUp();
        }
    }

    IEnumerator LoseSpeedStacks()
    {
        float accumulatedTime = 0.0f;
        losingSpeedStacks = true;

        while (accumulatedTime < speedStackLoseTime)
        {
            accumulatedTime += 0.01f;
            yield return new WaitForSeconds(0.01f);
            if (resetSpeedStackLoss)
            {
                accumulatedTime = 0;
                resetSpeedStackLoss = false;
            }
            speedStackForeground.fillAmount = 1 - accumulatedTime / speedStackLoseTime;
        }

        losingSpeedStacks = false;
        ModifySpeedStacks(-1);
    }

    public void ReplenishHealth()
    {
        ModifyHealth(maxHealthPoints);
    }

    //Will sum the parameter to the current speed stacks
    void ModifySpeedStacks(int sum)
    {
        //with this, field of view will decrease gradually
        //if (sum < 0)
        //{
        //    StartCoroutine(FieldOfViewChange(fieldOfViewAugment/speedPowerUpStacks * -1));
        //}

        if (sum > 0)
        {
            StartCoroutine(SetFieldOfView(baseFieldOfView + 4f));
        }

        speedPowerUpStacks += sum;

        //with this, field of view will decrease only when all stacks are lost
        if (speedPowerUpStacks == 0)
        {
            StartCoroutine(SetFieldOfView(baseFieldOfView));
        }

        if (speedPowerUpStacks > maxSpeedStacks)
        {
            speedPowerUpStacks = maxSpeedStacks;
        }

        effect = 1 + speedStackEffect * speedPowerUpStacks;

        speed = baseSpeed * effect;
        dashStackChargingTime = baseDashChargingTime / effect;
        //camera.fieldOfView = baseFieldOfView * effect; 
        

        //Modify dash force in the same way speed is
        //forwardDashForce = baseForwardDashForce * effect;
        //lateralDashForce = baseLateralDashForce * effect;

        //Speed already included in dash force
        //dashForce = baseDashForce * effect;

   //     if(speedPowerUpStacks > 0)
    //    {
            speedStackText.text = speedPowerUpStacks.ToString();
      //  }

        //else
        //{
        //    speedStackText.text = "";
        //}

        //Resets 
        if (speedPowerUpStacks > 0)
        {
            speedStackBackground.SetActive(true);
            
            if (!losingSpeedStacks)
            {
                StartCoroutine(LoseSpeedStacks());
            }

            else
            {
                resetSpeedStackLoss = true;
            }
        }

        else
        {
            //speedStackBackground.SetActive(false);
            //StartCoroutine(FieldOfViewReset());
        }
    }

    IEnumerator SetFieldOfView(float field)
    {
        float totalChange = 0;
        float change =field - camera.fieldOfView;
        //Debug.Log("change: " + change);
        float rate = 0.05f;
        if (change > 0 && camera.fieldOfView < maxFieldOfView)
        {
            while (totalChange < change)
            {
                totalChange += change * rate;
          //      Debug.Log("total change: " + totalChange);
                camera.fieldOfView += change * rate;
                yield return new WaitForSeconds(0.01f);
            }
        }

        else if (change < 0)
        {
            while (totalChange > change)
            {
                totalChange += change * rate;
              //  Debug.Log("total change: " + totalChange);
                camera.fieldOfView += change * rate;
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    IEnumerator FieldOfViewChange(float change)
    {
        float totalChange = 0;
        Debug.Log("change: " + change);
        float rate = 0.05f;
        if(change > 0 && camera.fieldOfView < maxFieldOfView)
        {
            while (totalChange < change)
            {
                totalChange += change * rate;
                Debug.Log("total change: " + totalChange);
                camera.fieldOfView += change * rate;
                yield return new WaitForSeconds(0.01f);
            }
        }

        else if (change < 0)
        {
            while (totalChange > change)
            {
                totalChange += change * rate;
                Debug.Log("total change: " + totalChange);
                camera.fieldOfView += change * rate;
                yield return new WaitForSeconds(0.01f);
            }
        }

    }

    void UpdateStacksGraphicalInfo()
    {
        //Updates UI for stacks
    }

    void IncreaseSensitivity()
    {
        sensitivity += 1;
        sensitivity = Mathf.Clamp(sensitivity, 1, 10);
        GameObject.Find("MenuController").SendMessage("UpdateSensitivityText", sensitivity.ToString());
    }

    void DecreaseSensitivity()
    {
        sensitivity -= 1;
        sensitivity = Mathf.Clamp(sensitivity, 1, 10);
        GameObject.Find("MenuController").SendMessage("UpdateSensitivityText", sensitivity.ToString());
    }

    public void SetInteractivity(bool newValue)
    {
        canInteract = newValue;
    }

    public void SetMovementEnabled(bool newValue)
    {
        movementEnabled = newValue;
    }

    public void SetCameraMovementEnabled(bool newValue)
    {
        cameraMovementEnabled = newValue;
    }

    public void RegisterDummyPunch()
    {
        timesDummyWasPunched++;

        if (timesDummyWasPunched > 5)
        {
            DisplayTip("This dummy is immortal");
        }
    }

    public int GetTimesDummyWasPunched()
    {
        return timesDummyWasPunched;
    }
    //public IEnumerator IntroduceText(string textToIntroduce, AudioSource writingSoundEffect, int numberOfPlays)
    //{
    //    yield return new WaitForSeconds(0.01f);
    //    //textBeingIntroduced = textToIntroduce;
    //    //canInteract = false;
    //    //introduceTextCoroutineInstance = PlayerUtils.IntroduceText(textToIntroduce, dialogText, textWriteDelay, writingSoundEffect, numberOfPlays);
    //    //yield return StartCoroutine(introduceTextCoroutineInstance);
    //    //introduceTextCoroutineInstance = null;
    //}

    public IEnumerator IntroduceNewText(string textToIntroduce, AudioSource writingSoundEffect, float characterWriteDelay, float timeBetweenPlays, GameObject talkingWith)
    {
        //Waiting time added because when it is not there, sometimes dialog would just appear as if textWriteDelay was 0
        yield return new WaitForSeconds(0.01f);
        //Debug.Log("parameter passed by npc: " + talkingWith);
        objectTalkingTo = talkingWith;
        introducingText = true;
        crosshair.SetActive(false);
        DisableInteraction();
        ClearText();

        //Separated by time
        string[] textGroup = textToIntroduce.Split('|');

        char[] textInChar = null;
        //float counterForPlaying = 0.0f;
        bool shouldPlay = writingSoundEffect != null;
        string finalText = "";
        for (int j = 0; j < textGroup.Length; j++)
        {
            textInChar = textGroup[j].ToCharArray();
            string buffer = "";
            finalText += textGroup[j];
            for (int i = 0; i < textInChar.Length; i++)
            {
                //Debug.Log("iteration: " + i + " .. buffer: " + buffer);
				if (buffer != "" && buffer.ToCharArray()[0] == '%')
                {
                    if(textInChar[i] == '%')
                    {
                        dialogText.text += buffer.Substring(1, (buffer.Length - 1));
                        buffer = "";
                        yield return new WaitForSeconds(characterWriteDelay);
                        continue;
                    }

                    //implicit else
                        buffer += textInChar[i];
                        continue;
                }

                if(textInChar[i] == '%')
                {
                    buffer += textInChar[i];
                    continue;
                }

                if (shouldSkipText)
                {
                    dialogText.text = finalText;
                    shouldSkipText = false;
                    break;
                }

                if (shouldPlay)
                {
                    writingSoundEffect.Play();
                    shouldPlay = false;
                }

                //else
                //{
                 //   counterForPlaying += characterWriteDelay;
                //}
                
               // shouldPlay = counterForPlaying > timeBetweenPlays;
                dialogText.text += textInChar[i];
                yield return new WaitForSeconds(characterWriteDelay);
            }
            //Assumes that if there's a |, it closes and what's between it is a floating number which meaning is to
            //wait for that many seconds before continuing the dialog.
            if (j + 1 < textGroup.Length)
            {
                //canSkipText = false;
                yield return new WaitForSeconds(float.Parse(textGroup[j + 1]));
                shouldSkipText = false;
                //Because the for loop does the other one
                j = j + 1;
                //writingSoundEffect.Play();
            }
        }
        introducingText = false;
        EnableInteraction();
    }

    void ContinueConversation()
    {
        objectTalkingTo.SendMessage("ContinueConversation");
    }

    void ClearText()
    {
        dialogText.text = "";
    }

    public void FinishConversation()
    {
        dialogText.text = "";
        objectTalkingTo = null;
        crosshair.SetActive(true);
    }

    public void SetTutorialText(string newText)
    {
        tutorialText.GetComponent<Text>().text = newText;
    }

    public void DisplayWarning(string warning)
    {
        tutorialText.color = PlayerUtils.warningColor;
        tutorialText.text = "<color=yellow><size=26>!</size></color>  " + warning;
    }

    public void DisplayTip(string tip)
    {
        tutorialText.color = PlayerUtils.normalTutorialColor;
        tutorialText.text = tip;
    }

    public bool HasMaxHealth()
    {
        return healthPoints == maxHealthPoints;
    }

    public void DisableInteraction()
    {
        canInteract = false;
    }

    public void EnableInteraction()
    {
        canInteract = true;
    }

    public void SetDashStacks(int newDashes)
    {
        dashStackAmount = newDashes;
        updateDashGraphicalInfo();
    }

    void OnApplicationQuit()
    {
        //Debug.Log("Supuestamente enviado");
        AnalyticsUtils.RegisterQuitEvent();
    }
}
