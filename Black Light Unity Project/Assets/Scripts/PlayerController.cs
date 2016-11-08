using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
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

    public float lateralDashForce;

    public float forwardDashForce;

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
    //--------------------------------------------------------------------------------------------------------------
    //Private variables
    //--------------------------------------------------------------------------------------------------------------

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

    //Is the game paused?
    private bool isPaused;

    //Number of stacks of speed power up
    private int speedPowerUpStacks;

    //If true, the time a speed stack takes to be lost will reset 
    private bool resetSpeedStackLoss;

    //Is the player losing speed stacks right now?
    private bool losingSpeedStacks;

    private float baseForwardDashForce;

    private float baseLateralDashForce;

    //Sound effect played at death
    private AudioSource deathSound;

    //Should interactions be possible?
    private bool canInteract;

    private RaycastHit hit;

    //Pause menu
    private GameObject pauseMenu;

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

    //--------------------------------------------------------------------------------------------------------------
    //Functions
    //--------------------------------------------------------------------------------------------------------------

    void Start()
    {
        xRot = transform.rotation.eulerAngles.x;
        yRot = transform.rotation.eulerAngles.y;
        canInteract = true;
        objectTalkingTo = null;
        introducingText = false;
        movementEnabled = true;
        cameraMovementEnabled = true;
        maxHealthPoints = healthPoints;
        baseForwardDashForce = forwardDashForce;
        baseLateralDashForce = lateralDashForce;
        losingSpeedStacks = false;
        baseSpeed = speed;
        speedPowerUpStacks = 0;
        isPaused = false;
        dashStackAmount = 0;
        isChargingStacks = false;
        isDead = false;
        rigidbody = GetComponent<Rigidbody>();
        attackFail = GetComponents<AudioSource>()[0];
        attackSuccess = GetComponents<AudioSource>()[1];
        deathSound = GetComponents<AudioSource>()[2];
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        crosshairSuccess = GameObject.Find("Crosshair_Success").GetComponent<Image>();
        crosshairFail = GameObject.Find("Crosshair_Fail").GetComponent<Image>();
        pauseMenu = GameObject.Find("PauseMenu");
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
                ray = new Ray(camera.transform.position, camera.transform.forward * attackRange);

                Physics.Raycast(ray.origin, ray.direction, out hit);

                if (movementEnabled)
                {
                    //Moves the player
                    transform.Translate(new Vector3(Input.GetAxis("Horizontal") * speed * Time.deltaTime, 0,
                                        Input.GetAxis("Vertical") * speed * Time.deltaTime));
                }

                if (cameraMovementEnabled)
                {
                    //So the player is able to rotate relative to the y axis with the camera. This is setup this way so its local axis is synchronized
                    //with what the player sees
                    transform.localEulerAngles = new Vector3(0, yRot, 0);

                    //So only the camera rotates relative to the x axis, not the player
                    camera.transform.localEulerAngles = new Vector3(xRot, 0, 0);

                    //Because vertical camera rotation is relative to the in-game x axis
                    xRot += Input.GetAxis("Mouse Y") * sensitivity * -1;

                    //Because horizontal camera rotation is relative to the in-game y axis
                    yRot += Input.GetAxis("Mouse X") * sensitivity;

                    xRot = Mathf.Clamp(xRot, -55, 35);
                }

                if (canInteract)
                {
                    if (hit.collider != null && hit.collider.tag == "Interactive" && hit.distance < interactionRange)
                    {
                        hit.collider.gameObject.SendMessage("UpdateInteractionText");

                        if (Input.GetKeyDown(KeyCode.F))
                        {
                            hit.collider.gameObject.SendMessage("DoAction");
                        }
                    }

                    else if (interactText.GetComponent<Text>().text != "")
                    {
                        CleanInteractionText();
                    }
                }



                //Receive input for attack
                if (Input.GetMouseButton(0))
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

                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    StartCoroutine(ActivateEnergyVission());
                }
            }

            else if (introducingText && Input.GetKey(KeyCode.F))
            {
                //if F is pressed when talking to someone
                if (Input.GetKeyDown(KeyCode.F))
                {
                    shouldSkipText = true;
                }
            }

            //A conversation is active in a conversation, so it must continue when the player presses F
            else if (Input.GetKeyDown(KeyCode.F))
            {
                ContinueConversation();
            }

            if (dialogText.text != "")
            {
                CleanInteractionText();
            }

            //Get input for pause
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Pause();
            }

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
        float originalAlpha = visorEffect.GetComponent<Image>().color.a;
        Color tempColor = visorEffect.GetComponent<Image>().color;
        tempColor.a = 0;
        visorEffect.GetComponent<Image>().color = tempColor;
        visorEffect.SetActive(true);

        bool done = false;
        //If the player stops pressing X, the energy vission stops activating
        while (Input.GetKey(KeyCode.LeftShift) && !done)
        {
            yield return new WaitForSeconds(0.01f);
            timeShiftHasBeenPressed += 0.01f;
            done = timeShiftHasBeenPressed > timeToActivateVission;
            //Percentage of the time that has charged is translated to alpha percentage and then assigned to the effect
            //The 0.5f at the end is so the maximum alpha to be achieved before activation is 50% the original
            tempColor.a = (timeShiftHasBeenPressed / timeToActivateVission) * originalAlpha * 0.5f;
            visorEffect.GetComponent<Image>().color = tempColor;
        }

        if (done)
        {
            tempColor.a = originalAlpha;
            visorEffect.GetComponent<Image>().color = tempColor;
            ActivateVission();
            visorEffect.GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(0.2f);
        }

        visorEffect.SetActive(false);
    }

    void ActivateVission()
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
                colliders[i].gameObject.GetComponent<PowerUpPlateSpawner>().SpawnPowerUpPlate();
            }
        }
    }

    IEnumerator ChargeStacks()
    {
        //Updates time counter every 1/100 of a second, and because floating point numbers can't be 
        //compared to maximum precision, this means it will take at most an extra 1/100 of a second
        //charging 
        float accumulatedStackChargingTime = 0.0f;
        isChargingStacks = true;

        while (accumulatedStackChargingTime < dashStackChargingTime)
        {
            yield return new WaitForSeconds(0.01f);
            accumulatedStackChargingTime += 0.01f;
            dashStackLoadingCounter.fillAmount = Mathf.Max(0, (float)accumulatedStackChargingTime / dashStackChargingTime);
        }

        dashStackAmount += 1;
        accumulatedStackChargingTime = 0.0f;
        isChargingStacks = false;
        updateDashGraphicalInfo();
    }

    IEnumerator Attack()
    {
        if (Time.time - lastAttack > attackCooldown)
        {
            lastAttack = Time.time;

            if (hit.collider == null || hit.collider.tag != "Enemy")
            {
                crosshairFail.enabled = true;
                attackFail.Play();
                yield return new WaitForSeconds(0.2f);
                crosshairFail.enabled = false;
            }

            else
            {
                crosshairSuccess.enabled = true;
                attackSuccess.Play();
                hit.collider.gameObject.SendMessage("TakeDamage", attack);
                yield return new WaitForSeconds(0.2f);
                crosshairSuccess.enabled = false;
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
        yield return new WaitForSeconds(0.2f);
        damageEffect.SetActive(false);
    }

    //Should be called everytime the player takes damage
    public bool TakeDamage(int damage)
    {
        bool ans = IsInvulnerable();
        if (!isDead)
        {
            if (!ans)
            {
                StartCoroutine(DisplayDamageEffect());

                //'Max' it doesn't take negative values
                ModifyHealth(-damage);

                if (healthPoints <= 0)
                {
                    Die();
                }
            }
        }
        return ans;
    }

    //Adds the value passed as a parameter to the current health
    void ModifyHealth(int modifyValue)
    {
        healthPoints += modifyValue;
        healthPoints = Mathf.Clamp(healthPoints, 0, maxHealthPoints);
        healthBar.fillAmount = Mathf.Max(0, (float)healthPoints / maxHealthPoints);
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

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].SetActive(false);
        }
    }

    void Retry()
    {
        ReplenishHealth();
        isDead = false;
        transform.position = GameObject.Find("Respawn").transform.position;
        GameObject.Find("Baroth").GetComponent<TutorialController>().Retry();
        deathUI.SetActive(false);
    }

    void Dash()
    {
        if (canDash())
        {
            if (Input.GetKey(KeyCode.Q))
            {
                //Dash left
                rigidbody.AddRelativeForce(new Vector3(-lateralDashForce, 0, 0), ForceMode.VelocityChange);
                updateDashInfo();
            }

            else if (Input.GetKey(KeyCode.E))
            {
                //Dash to right
                rigidbody.AddRelativeForce(new Vector3(lateralDashForce, 0, 0), ForceMode.VelocityChange);
                updateDashInfo();
            }

            else if (Input.GetKey(KeyCode.R) || Input.GetMouseButton(2))
            {
                //Dash forward
                rigidbody.AddRelativeForce(new Vector3(0, 0, forwardDashForce), ForceMode.VelocityChange);
                updateDashInfo();
            }
        }
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

    void Pause()
    {
        //Pauses if not paused, unpause if paused.
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0;
        }

        else
        {
            Time.timeScale = 1;
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
        speedPowerUpStacks += sum;

        if (speedPowerUpStacks > maxSpeedStacks)
        {
            speedPowerUpStacks = maxSpeedStacks;
        }

        float effect = 1 + speedStackEffect * speedPowerUpStacks;

        speed = baseSpeed * effect;

        //Modify dash force in the same way speed is
        forwardDashForce = baseForwardDashForce * effect;
        lateralDashForce = baseLateralDashForce * effect;
        speedStackText.text = speedPowerUpStacks.ToString();

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
            speedStackBackground.SetActive(false);
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
        objectTalkingTo = talkingWith;
        introducingText = true;
        ClearText();

        //Separated by time
        string[] textGroup = textToIntroduce.Split('|');

        char[] textInChar = textGroup[0].ToCharArray();
        float counterForPlaying = 0.0f;
        bool shouldPlay = writingSoundEffect != null;

        for (int j = 0; j < textGroup.Length; j++)
        {
            for (int i = 0; i < textInChar.Length; i++)
            {
                if (shouldSkipText)
                {
                    dialogText.text = textToIntroduce;
                    shouldSkipText = false;
                    break;
                }

                if (shouldPlay)
                {
                    writingSoundEffect.Play();
                }

                else
                {
                    counterForPlaying += characterWriteDelay;
                }
                shouldPlay = counterForPlaying > timeBetweenPlays;
                dialogText.text += textInChar[i];
                yield return new WaitForSeconds(characterWriteDelay);
            }
        }
        introducingText = false;
    }

    void ContinueConversation()
    {
        objectTalkingTo.SendMessage("ContinueConversation");
    }

    void ClearText()
    {
        dialogText.text = "";
    }

    void FinishConversation()
    {
        dialogText.text = "";
        objectTalkingTo = null;
    }

    public void SetTutorialText(string newText)
    {
        tutorialText.GetComponent<Text>().text = newText;
    }

    public void DisplayWarning(string warning)
    {
        tutorialText.color = PlayerUtils.warningColor;
        tutorialText.text = "<color=yellow><size=24>!</size></color>  " + warning;
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
}
