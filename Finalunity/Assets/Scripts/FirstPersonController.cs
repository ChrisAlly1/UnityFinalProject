using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : NetworkBehaviour {
    private bool m_IsWalking;
    [SerializeField] private float m_WalkSpeed;
    [SerializeField] private float m_RunSpeed;
    [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
    [SerializeField] private float m_JumpSpeed;
    [SerializeField] private float m_StickToGroundForce;
    [SerializeField] private float m_GravityMultiplier;
    [SerializeField] private MouseLook m_MouseLook;
    [SerializeField] private bool m_UseFovKick;
    [SerializeField] private FOVKick m_FovKick = new FOVKick();
    [SerializeField] private bool m_UseHeadBob;
    [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
    [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
    [SerializeField] private float m_StepInterval;

    private bool chatUp;
    private Camera m_Camera;
    private bool m_Jump;
    private float m_YRotation;
    private Vector2 m_Input;
    private Vector3 m_MoveDir = Vector3.zero;
    private CharacterController m_CharacterController;
    private CollisionFlags m_CollisionFlags;
    private bool m_PreviouslyGrounded;
    private Vector3 m_OriginalCameraPosition;
    private float m_StepCycle;
    private float m_NextStep;
    private bool m_Jumping;
    private Health health;
    private Inventory inventory;
    private bool powerActive, doubleDamageVar;
    private Dictionary<PowerUp, System.Action> powerEffect = new Dictionary<PowerUp, System.Action>();

    [HideInInspector]
    public enum PowerUp { NONE, FAST_MOVE, DOUBLE_DAMAGE, FAST_FIRE, JUMP_HIGH, INVINCIBLE };

    //variables to know which gun is equiped
    private bool smgE = false;
    private bool pistolE = true;

    public GameObject projectile, SMG, pistol;
    public Transform bulletSpawn;
    private Transform attachedCamera;
    private Text powerUpTimerText;

    //timer for power ups
    private float powerUpTimer = 10.0f;
    private PowerUp currentPower;

    public float speed = 20;
    public float nextFireP = 0.7f;
    public float nextFireS = 0.1f;
    public float myTime = 0.0f;
    public float fireSpeed = 15.0f;

    private MouseLook[] mous;
    // Use this for initialization
    private void Start() {
        if (isLocalPlayer) {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_Jumping = false;
            m_MouseLook.Init(transform, m_Camera.transform);

            currentPower = PowerUp.NONE;
            health = gameObject.GetComponent<Health>();
            inventory = gameObject.GetComponent<Inventory>();
            powerActive = false; doubleDamageVar = false;
            pistol.SetActive(true); SMG.SetActive(false);
            chatUp = false;

            //Attaches the gun to the camera
            foreach (Transform child in transform) {
                if (child.CompareTag("Rifle")) {
                    child.SetParent(Camera.main.transform, false);
                    Vector3 newPos = child.position;
                    newPos.y -= 0.5f;
                    newPos.z -= 0.05f;
                    child.position = newPos;
                    break;
                }
            }

            powerUpTimerText = GameObject.FindGameObjectWithTag("Timer").GetComponent<Text>();
            powerUpTimerText.text = "";

            powerEffect.Add(PowerUp.DOUBLE_DAMAGE, doubleDamage);
            powerEffect.Add(PowerUp.FAST_FIRE, fastFire);
            powerEffect.Add(PowerUp.FAST_MOVE, fastWalk);
            powerEffect.Add(PowerUp.JUMP_HIGH, jumpHigh);
            powerEffect.Add(PowerUp.INVINCIBLE, invincible);
        }
       
    }

    // Update is called once per frame
    private void Update() {
        if (!isLocalPlayer) {
            return;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Camera.main.transform.position = transform.position + transform.up * 0.5f;
        Camera.main.transform.parent = transform;

        if (Input.GetKeyDown(KeyCode.Escape)) {
            chatUp = !chatUp;
        }

        if (!chatUp) {
            //equip smg
            if (Input.GetKeyDown(KeyCode.O)) {
                Debug.Log("SMG equipped");
                smgE = true; pistolE = false;
                pistol.SetActive(false); SMG.SetActive(true);
                //equip pistol
            } else if (Input.GetKeyDown(KeyCode.P)) {
                Debug.Log("Pistol equipped");
                smgE = false; pistolE = true;
                pistol.SetActive(true); SMG.SetActive(false);
            }

            //for activating power ups
            //testing purposes ONLY
            /*if (Input.GetKeyDown(KeyCode.I)) {
                inventory.AddPowerUp(PowerUp.FAST_MOVE);
            }
            else if (Input.GetKeyDown(KeyCode.G)) {
                inventory.AddPowerUp(PowerUp.DOUBLE_DAMAGE);
            }
            else if (Input.GetKeyDown(KeyCode.F)) {
                inventory.AddPowerUp(PowerUp.FAST_FIRE);
            }
            else if (Input.GetKeyDown(KeyCode.B)) {
                inventory.AddPowerUp(PowerUp.INVINCIBLE);
            }
            else if (Input.GetKeyDown(KeyCode.J)) {
                inventory.AddPowerUp(PowerUp.JUMP_HIGH);
            }*/

            //Check if the player wishes to use a power up
            if (!powerActive) {
                if (Input.GetButtonDown("Inventory1")) {
                    currentPower = inventory.getPowerUp(1);
                    if (currentPower != PowerUp.NONE) {
                        inventory.RemovePowerUp(1);
                        powerEffect[currentPower]();
                        powerActive = true;
                    }
                } else if (Input.GetButtonDown("Inventory2")) {
                    currentPower = inventory.getPowerUp(2);
                    if (currentPower != PowerUp.NONE) {
                        inventory.RemovePowerUp(2);
                        powerEffect[currentPower]();
                        powerActive = true;
                    }
                } else if (Input.GetButtonDown("Inventory3")) {
                    currentPower = inventory.getPowerUp(3);
                    if (currentPower != PowerUp.NONE) {
                        inventory.RemovePowerUp(3);
                        powerEffect[currentPower]();
                        powerActive = true;
                    }
                } else if (Input.GetButtonDown("Inventory4")) {
                    currentPower = inventory.getPowerUp(4);
                    if (currentPower != PowerUp.NONE) {
                        inventory.RemovePowerUp(4);
                        powerEffect[currentPower]();
                        powerActive = true;
                    }
                } else if (Input.GetButtonDown("Inventory5")) {
                    currentPower = inventory.getPowerUp(5);
                    if (currentPower != PowerUp.NONE) {
                        inventory.RemovePowerUp(5);
                        powerEffect[currentPower]();
                        powerActive = true;
                    }
                }
            } else {
                powerUpTimer -= Time.deltaTime;
                int g = (int)powerUpTimer;
                powerUpTimerText.text = g.ToString();
            }

            if (powerUpTimer <= 0.0f) {
                Debug.Log("Power up over");
                if (currentPower == PowerUp.INVINCIBLE) {
                    health.SwitchInvincibility();
                }

                powerUpTimerText.text = "";
                m_WalkSpeed = 8; m_JumpSpeed = 6;
                nextFireP = 0.7f; nextFireS = 0.1f;
                powerUpTimer = 10.0f;
                currentPower = PowerUp.NONE;
                doubleDamageVar = false; powerActive = false;
            }

            //countdown timer to allow the player to shoot again its just always subtracting and resets when the player shoots
            nextFireP -= Time.deltaTime;
            nextFireS -= Time.deltaTime;

            //used to fire the guns
            if (Input.GetButton("Fire1")) {
                if (smgE && nextFireS <= 0.0f) {
                    CmdfireBullet();
                    //timer until can shoot again
                    if (currentPower == PowerUp.FAST_FIRE) {
                        nextFireS = 0.05f;
                    }
                    else {
                        nextFireS = 0.1f;
                    }
                } else if (pistolE && nextFireP <= 0.0f) {
                    CmdfireBullet();
                    //timer until can shoot again
                    if (currentPower == PowerUp.FAST_FIRE) {
                        nextFireP = 0.35f;
                    }
                    else {
                        nextFireP = 0.7f;
                    }
                }
            }

            RotateView();

            // the jump state needs to read here to make sure it is not missed
            if (!m_Jump) {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded) {
                StartCoroutine(m_JumpBob.DoBobCycle());
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }

            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded) {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }
    }

    [Command]
    void CmdfireBullet() {
        GameObject bullet = (GameObject)Instantiate(projectile, bulletSpawn.position, bulletSpawn.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * fireSpeed;

        if (doubleDamageVar) {
            bullet.GetComponent<Projectile>().SetDoubleDamage();
        }

        NetworkServer.Spawn(bullet);

        Destroy(bullet, 2.0f);
    }

    //checking for collision with power ups
    void OnTriggerEnter(Collider c) {
        if (c.gameObject.tag == "FastMove") {
            inventory.AddPowerUp(PowerUp.FAST_MOVE);
            Destroy(c.gameObject);
        } else if (c.gameObject.tag == "FastFire") {
            inventory.AddPowerUp(PowerUp.FAST_FIRE);
            Destroy(c.gameObject);
        } else if (c.gameObject.tag == "DoubleDamage") {
            inventory.AddPowerUp(PowerUp.DOUBLE_DAMAGE);
            Destroy(c.gameObject);
        } else if (c.gameObject.tag == "JumpHigh") {
            inventory.AddPowerUp(PowerUp.JUMP_HIGH);
            Destroy(c.gameObject);
        } else if (c.gameObject.tag == "Invincibility") {
            inventory.AddPowerUp(PowerUp.INVINCIBLE);
            Destroy(c.gameObject);
        }
    }

    //checking if player is in the snow
    void OnTriggerStay(Collider c) {
        if (!isLocalPlayer) {
            return;
        }

        if (c.gameObject.tag == "Snow") {
            Debug.Log("lower speed");
            if (currentPower == PowerUp.FAST_MOVE) {
                m_WalkSpeed = 8;
            } else {
                m_WalkSpeed = 4;
            }
        }
    }

    //reset movement speed when exit snow
    void OnTriggerExit() {
        if (!isLocalPlayer) {
            return;
        }

        if (currentPower == PowerUp.FAST_MOVE) {
            m_WalkSpeed = 16;
        } else {
            m_WalkSpeed = 8;
        }
    }
    private void FixedUpdate() {
        if (!isLocalPlayer) {
            return;
        }

        if (!chatUp) {
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x * speed;
            m_MoveDir.z = desiredMove.z * speed;

            if (m_CharacterController.isGrounded) {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump) {
                    m_MoveDir.y = m_JumpSpeed;
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else {
                m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
            }

            m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            m_MouseLook.UpdateCursorLock();
        }
    }

    private void ProgressStepCycle(float speed) {
        if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0)) {
            m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) *
                         Time.fixedDeltaTime;
        }

        if (!(m_StepCycle > m_NextStep)) {
            return;
        }

        m_NextStep = m_StepCycle + m_StepInterval;
    }

    private void UpdateCameraPosition(float speed) {
        Vector3 newCameraPosition;
        if (!m_UseHeadBob) {
            return;
        }

        if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded) {
            m_Camera.transform.localPosition =
                m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                  (speed * (m_IsWalking ? 1f : m_RunstepLenghten)));
            newCameraPosition = m_Camera.transform.localPosition;
            newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
        }
        else {
            newCameraPosition = m_Camera.transform.localPosition;
            newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
        }
        m_Camera.transform.localPosition = newCameraPosition;
    }

    private void GetInput(out float speed) {
        // Read input
        float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        float vertical = CrossPlatformInputManager.GetAxis("Vertical");

        bool waswalking = m_IsWalking;

        // On standalone builds, walk/run speed is modified by a key press.
        // keep track of whether or not the character is walking or running
        m_IsWalking = !Input.GetKey(KeyCode.LeftShift);

        // set the desired speed to be walking or running
        speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
        m_Input = new Vector2(horizontal, vertical);

        // normalize input if it exceeds 1 in combined length:
        if (m_Input.sqrMagnitude > 1) {
            m_Input.Normalize();
        }

        // handle speed change to give an fov kick
        // only if the player is going to a run, is running and the fovkick is to be used
        if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0) {
            StopAllCoroutines();
            StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
        }
    }

    private void RotateView() {
        m_MouseLook.LookRotation(transform, m_Camera.transform);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        Rigidbody body = hit.collider.attachedRigidbody;
        //dont move the rigidbody if the character is on top of it
        if (m_CollisionFlags == CollisionFlags.Below) {
            return;
        }

        if (body == null || body.isKinematic) {
            return;
        }
        body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
    }

    public override void OnStartLocalPlayer() {
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    void fastWalk() {
        Debug.Log("Fast Move power up");
        m_WalkSpeed = 16;
    }

    void doubleDamage() {
        Debug.Log("Double Damage power up");
        doubleDamageVar = true;
    }

    void fastFire() {
        Debug.Log("Fast Fire power up");
        nextFireP = 0.35f;
        nextFireS = 0.05f;
    }

    void invincible() {
        Debug.Log("Invincibility power up");
        health.SwitchInvincibility();
    }

    void jumpHigh() {
        Debug.Log("Jump height up");
        m_JumpSpeed = 10;
    }
}