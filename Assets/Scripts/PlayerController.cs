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

    public GameObject dashPrefab;

    public Image healthBar;

    //Maximum dash stacks 
    public int dashMaxStacks;

    //Time a dash stack takes to charge
    public float dashStackChargingTime;

    public Image dashStackLoadingCounter;

    public Text dashNumberText;

    public GameObject pauseMenu;

    //Percentage of speed each speed stack adds
    public float speedStackEffect;

    //Time shift has to be pressed to activate Vission
    public float timeToActivateVission;

    //Time a speed stack takes to dissapear
    public float speedStackLoseTime;

    //Vission radius
    public float vissionRadius;

    public GameObject visorUI;

    public Image speedStackForeground;

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

    //--------------------------------------------------------------------------------------------------------------
    //Functions
    //--------------------------------------------------------------------------------------------------------------

    void Start()
    {
        losingSpeedStacks = false;
        baseSpeed = speed;
        speedPowerUpStacks = 0;
        isPaused = false;
        dashStackAmount = 0;
        isChargingStacks = false;
        isDead = false;
        rigidbody = GetComponent<Rigidbody>();
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        crosshairSuccess = GameObject.Find("Crosshair_Success").GetComponent<Image>();
        crosshairFail = GameObject.Find("Crosshair_Fail").GetComponent<Image>();
        attackFail = GameObject.FindGameObjectWithTag("Player").GetComponents<AudioSource>()[0];
        attackSuccess = GameObject.FindGameObjectWithTag("Player").GetComponents<AudioSource>()[1];
    }

    void Update()
    {
        //So the player is able to rotate relative to the y axis with the camera. This is setup this way so its local axis is synchronized
        //with what the player sees
        transform.localEulerAngles = new Vector3(0, yRot, 0);

        //So only the camera rotates relative to the x axis, not the player
        camera.transform.localEulerAngles = new Vector3(xRot, 0, 0);

        speedStackForeground.fillAmount -= 0.01f;

        //Only able to do this is 
        if (!isPaused && !isDead)
        {
            //Updates the ray casted onto the screen for various purposes
            ray = new Ray(camera.transform.position, camera.transform.forward * attackRange);

            //Moves the player
            Vector3 mov = new Vector3(Input.GetAxis("Horizontal") * speed * Time.deltaTime, 0,
                                Input.GetAxis("Vertical") * speed * Time.deltaTime);

            transform.Translate(mov);

            //Because vertical camera rotation is relative to the in-game x axis
            xRot += Input.GetAxis("Mouse Y") * sensitivity * -1;

            //Because horizontal camera rotation is relative to the in-game y axis
            yRot += Input.GetAxis("Mouse X") * sensitivity;

            xRot = Mathf.Clamp(xRot, -55, 35);
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

            if(Input.GetKeyDown(KeyCode.X))
            {
                StartCoroutine(ActivateEnergyVission());
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
            if (Input.GetKeyDown(KeyCode.R))
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

    IEnumerator ActivateEnergyVission()
    {
        float timeShiftHasBeenPressed = 0.0f;

        //If the player stops pressing X, the energy vission stops activating
        while (Input.GetKey(KeyCode.X))
        {
            yield return new WaitForSeconds(0.01f);
            timeShiftHasBeenPressed += 0.01f;
            if (timeShiftHasBeenPressed > timeToActivateVission)
            {
                visorUI.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                ActivateVission();
                visorUI.SetActive(false);
                yield break;
            }
        }
    }

    void ActivateVission()
    {
        //Identify GameObjects with Physics.OverlapSphere to identify colliders
        Collider[] colliders = Physics.OverlapSphere(transform.position, vissionRadius);
        for(int i = 0; i < colliders.Length; i++)
        {
            Debug.Log(colliders[i].gameObject.tag);
            if (colliders[i].gameObject.tag == "PowerUpPlate_Spawn")
            {
                colliders[i].gameObject.SendMessage("SpawnPowerUpPlate");
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
            RaycastHit hit;

            Physics.Raycast(ray.origin, ray.direction, out hit);

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

    //Should be called everytime the player takes damage
    void TakeDamage(int damage)
    {
        healthPoints -= damage;

        //'Max' it doesn't take negative values
        healthBar.fillAmount = Mathf.Max(0, (float)healthPoints / maxHealthPoints);

        if (healthPoints <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        GameObject.Find("img_GameOverScreen").GetComponent<Image>().enabled = true;
        GameObject.Find("txt_GameOverText").GetComponent<Text>().enabled = true;
        GameObject.Find("txt_RetryText").GetComponent<Text>().enabled = true;
    }

    void Retry()
    {
        SceneManager.LoadScene("Demo");
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

    //Should organize code into a method that updates all graphical elements
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

    void receivePowerUp(string type)
    {
        if (type == Utils.POWER_UP_SPEED)
        {
            modifySpeedStacks(1);
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
        }
    }

    //Will sum the parameter to the current speed stacks
    void modifySpeedStacks(int sum)
    {
        speedPowerUpStacks += sum;
        speed = baseSpeed * (1 + speedStackEffect * speedPowerUpStacks);

        //Resets 
        if (speedPowerUpStacks > 0)
        {
            if (!losingSpeedStacks)
            {
                StartCoroutine(LoseSpeedStacks());
            }

            else
            {
                resetSpeedStackLoss = true;
            }
        }
    }

    void updateStacksGraphicalInfo()
    {
        //Updates UI for stacks
    }

}
