using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle_Controller : BaseEntity
{
    /*
    This is a pretty standard controller, but it's edited specifically to the fact that the background is always moving past you,
    so moving left is the same as pressing on the brakes.
    */
    //Inspector stuff
    public float accleration = 1f;
    public float brakeForce = 2f;
    public float maxSpeed = 7f;
    [Tooltip("Used for a higher max speed when going backwards due to breaking, raher than acceleration.")]
    public float breakMaxMultiplier = 1.5f;
    [Tooltip("Body drag is used to bring the car to a stop faster for more precise movements.")]
    public float bodyDrag = 3f;
    public float rollSensitivity = 2f;
    [Tooltip("A value, in seconds. The rate at which gas is expended.")]
    public int gasSpendRate = 1;
    public float bounceForce = 2f;
    //End inspector stuff
    float maxBrakeSpeed;
    float curSpeed = 0f;
    float maxDistFromFloor = 1.5f;
    float wheelRotSpeed = 20.0f;


    Rigidbody2D rb;
    GameObject[] wheels;
    PolygonCollider2D col;
    //Relating to damage
    float damageInterval = 1.5f;
    bool canDamage = true;

    public GameObject[] Wheels
    {
        get {return wheels;}
        set {wheels = value;}
    }
    bool grounded;
    public LayerMask thisCar;

    //This is for world borders
    [Header("Screen edge properties. No running away!")]
    public int[] edgePoints = new int[2];
    public float edgeDeadZone = 5f;

    //This is for being cool programmer and not making a variable public :)
    
    // Start is called before the first frame update
    IEnumerator GasDownTimer(int seconds)
    {
        while(true)
        {
            yield return new WaitForSeconds(seconds);
            if(Gas > 0)
            {
                ExpendGas(1);
                Debug.Log("Gas expend" + Gas.ToString());
            }
        }
        
    }
    void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
        maxBrakeSpeed = -maxSpeed * breakMaxMultiplier;
        col = GetComponent<PolygonCollider2D>();
        StartCoroutine(GasDownTimer(gasSpendRate));
        
    }
    //Define some funcs broh
    //
    void Bounce(float multiplier)
    {
        //Used for jumping and bouncing when taking damage from hitting the ground.
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + (bounceForce * multiplier));
    }
    IEnumerator DamageCoolDown(float seconds)
    {
        //Avoid constant collision calls and instant death on collision.
        canDamage = false;
        yield return new WaitForSeconds(seconds);
        canDamage = true;
    }
    void PlayDamageSound()
    {
        string[] soundList = {"metal_hit1","metal_hit2"};
        FindObjectOfType<AudioManager>().PlayRandomFromList(soundList);
    }

    // Update is called once per frame
    void Update()
    {
        //World Checks
        if(Health < 0)
        {
            GameController.instance.CurState = GameController.playerState.Dead;
            Kill();
        }
        
        if(Gas <= 0)
        {
            MapBuilder.instance.slowing = true;
            if(MapBuilder.instance.speed <= 0)
            {
                GameController.instance.GameState = GameController.gameState.Ended;
            }
        }
        else if(Gas > 0 && MapBuilder.instance.slowing)
        {
            MapBuilder.instance.slowing = false;
        }
        //Car controller logics
        grounded = Physics2D.Raycast(transform.position, -transform.up, maxDistFromFloor, thisCar);
        float xInput = Input.GetAxis("Horizontal");
        if(transform.position.x > edgePoints[0] && transform.position.x < edgePoints[1])
        {
            if(grounded)
            {
                if(!AudioManager.instance.GetSound("engine_constant").source.isPlaying)
                {
                    AudioManager.instance.Play("engine_constant");
                    AudioManager.instance.Pause("engine_air_redline");
                    AudioManager.instance.Pause("engine_air_start");
                }


                if(Input.GetButtonDown("Jump"))
                {
                    Bounce(1.0f);
                }
                if(wheels != null)
                {
                    foreach(GameObject wheel in wheels)
                    {
                        wheel.transform.eulerAngles = new Vector3(0,0, wheel.transform.eulerAngles.z + wheelRotSpeed * 20);
                    }
                }
                if(curSpeed > maxBrakeSpeed && curSpeed < maxSpeed)
                {
                    if(xInput > 0)
                    {
                        curSpeed += accleration * xInput * Time.deltaTime;
                    }
                    else if(xInput < 0)
                    {
                        curSpeed += accleration * xInput * brakeForce * Time.deltaTime;
                    }
                }
                else if(curSpeed < maxBrakeSpeed || curSpeed > maxSpeed)
                {
                    if(curSpeed < maxBrakeSpeed)
                    {
                        curSpeed = maxBrakeSpeed;
                    }
                    else
                    {
                        curSpeed = maxSpeed;
                    }
                }
                if(xInput == 0 && curSpeed <= maxSpeed && curSpeed >= maxBrakeSpeed)
                {
                    float deceleration = accleration * Time.deltaTime * bodyDrag;
                    if(curSpeed > 0)
                    {
                        curSpeed -= deceleration;
                    }
                    else if(curSpeed < 0)
                    {
                        curSpeed += deceleration;
                    }
                }
            }
            else
            {
                //What happens when in the air:
                if(!AudioManager.instance.GetSound("engine_air_redline").source.isPlaying)
                {   
                    AudioManager.instance.Pause("engine_constant");
                    AudioManager.instance.Play("engine_air_start");
                    AudioManager.instance.DelayPlay("engine_air_redline", AudioManager.instance.GetSound("engine_air_start").clip.length);

                }

                Vector3 rotation = new Vector3(0,0,transform.localEulerAngles.z - (xInput * rollSensitivity * Time.deltaTime));
                transform.localEulerAngles = rotation;
            }
        }
        else
        {
            if(transform.position.x < edgePoints[0])
            {
                float distanceToEdge = edgePoints[0] - transform.position.x;
                curSpeed = distanceToEdge + 1;
            }
            if(transform.position.x > edgePoints[1])
            {
                float distanceToEdge = edgePoints[1] - transform.position.x;
                curSpeed = distanceToEdge - 1;
            }
        }
        
        rb.velocity = new Vector2(curSpeed, rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(canDamage)
        {
            Vector2 hitPoint = other.GetContact(0).point;
            Vector2 hitPointLocal = transform.InverseTransformPoint(hitPoint);
            if(hitPointLocal.x > 0 && hitPointLocal.y > 0 || hitPointLocal.y > 0 || hitPointLocal.x > 2 && hitPointLocal.y > -0.45)
            {
                //Getting knocked
                
                Damage(10);
                PlayDamageSound();
                StartCoroutine(DamageCoolDown(damageInterval));
                Bounce(1f);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Item")
        {
            ItemMain item = other.GetComponent<ItemMain>();
            if(item.healthInc > 0)
            {
                AudioManager.instance.Play("repair");
            }
            if(item.gasInc > 0)
            {
                AudioManager.instance.Play("fillup");
            }
            Heal(item.healthInc);
            GasUp(item.gasInc);
            Destroy(other.gameObject);
        }
    }
    
}
