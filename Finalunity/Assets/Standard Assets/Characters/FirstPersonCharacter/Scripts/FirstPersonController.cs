using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using UnityEngine.Networking;

namespace UnityStandardAssets.Characters.FirstPerson {
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : NetworkBehaviour {
        [SerializeField] private bool m_IsWalking;
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

        //variables to know which gun is equiped
        public bool smgE = false;
        public bool pistolE = true;

        public GameObject projectile;
        public Transform bulletSpawn;
        private Transform attachedCamera;

        //varriables for the first power up - bonus movementspeed
        public int powerup1 = 0;
        public float powerup1Timer = 10.0f;
        public bool powerup1Active = false;

        //variables fro second power up - double damage
        public int powerup2 = 0;
        public float powerup2Timer = 10.0f;
        public bool powerup2Active = false;
       

        public float speed = 20;
        public float nextFireP = 0.5f;
        public float nextfireS = 0.1f;
        public float myTime = 0.0f;
        public float fireSpeed = 15.0f;

        // Use this for initialization
        private void Start() {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_Jumping = false;
			m_MouseLook.Init(transform , m_Camera.transform);

            if (isLocalPlayer) {
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
            }
        }

        // Update is called once per frame
        private void Update() {
            if(!isLocalPlayer) {          
                return;
            } else {
                //Camera.main.transform.position = transform.position - transform.forward * 10 + transform.up * 3;
                Camera.main.transform.position = transform.position + transform.up * 0.5f;
                //Camera.main.transform.LookAt(transform.position);
                Camera.main.transform.parent = transform;
            }
            //equip smg
            if(Input.GetKeyDown(KeyCode.O))
            {
                smgE = true;
                pistolE = false;
            }
            //equip pistol
            if(Input.GetKeyDown(KeyCode.P))
            {
                smgE = false;
                pistolE = true;
            }
            //used to activated powerup 1
            if(Input.GetKeyDown(KeyCode.I))
            {
                if(powerup1 >=1)
                {                   
                    powerup1 = powerup1 - 1;
                    powerup1Active = true;
                }
            }
           
            //effects of powerup1
            if(powerup1Active == true)
            {
                m_WalkSpeed = 16;
                powerup1Timer -= Time.deltaTime;
            }
            //turn off powerup 1
            if(powerup1Timer <= 0)
            {
                powerup1Active = false;
                powerup1Timer = 10.0f;
                m_WalkSpeed = 8;
            }

            //countdown timer to allow the player to shoot again its just always subtracting and resets when the player shoots
            nextFireP -= Time.deltaTime;
            nextfireS -= Time.deltaTime;


            //used to fire pistol
            if (Input.GetButtonDown("Fire1")) {
                if (pistolE == true && nextFireP <=0)
                {
                    CmdfireBullet();
                    //timer until can shoot again
                    nextFireP = 0.5f;
                }         
            }
            //used to fire the smg
            if(Input.GetButton("Fire1"))
            {
                if (smgE == true && nextfireS <= 0)
                {
                    CmdfireBullet();
                    //timer until can shoot again
                    nextfireS = 0.1f;
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

        [Command]
        void CmdfireBullet() {
            GameObject bullet = (GameObject)Instantiate(projectile, bulletSpawn.position, bulletSpawn.rotation);
            //bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * fireSpeed, ForceMode.Impulse);
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * fireSpeed;        
            NetworkServer.Spawn(bullet);

            Destroy(bullet, 2);
        } 

        //checking for collision with power ups
        void OnTriggerEnter(Collider c)
        {
            if(c.gameObject.tag == "Powerup1")
            {
                Debug.Log("working power up");
                powerup1 = powerup1 + 1;
                Destroy(c.gameObject);
            }
        }
        //checking if player is in the snow
        void OnTriggerStay(Collider c)
        {
            if (!isLocalPlayer)
            {
               return;
            }
            if (c.gameObject.tag == "Snow")
            {
                Debug.Log("lowerspped");
                m_WalkSpeed = 4;
            }
        }
        //reset movement speed when exit snow
        void OnTriggerExit()
        {
            if (!isLocalPlayer)
            {
                return;
            }
            m_WalkSpeed = 8;
        }
        private void FixedUpdate()
        {
            if (!isLocalPlayer)
            {
                return;
            }
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;


            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            m_MouseLook.UpdateCursorLock();
        }
        void OnTriggerEnter()
        {
            
        }
        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;
        }

        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
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
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }

        private void RotateView() {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
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
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }
        public override void OnStartLocalPlayer()
        {
            GetComponent<MeshRenderer>().material.color = Color.blue;
        }
    }
}