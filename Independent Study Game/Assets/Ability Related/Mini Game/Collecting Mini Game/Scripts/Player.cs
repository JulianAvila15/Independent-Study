using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [Range(0, 10)] [SerializeField] private float m_Speed = 2;
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    public bool m_Grounded;            // Whether or not the player is grounded.
    private Rigidbody2D m_Rigidbody2D;
    private Animator m_Animator;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;
   private Vector3 playerTargetVelocity = new Vector2(0,0);

    public float move = 0;
    private bool jump = false;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent, OnLandEventInTutorial;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public bool playerCanMoveInPDTutorial;

    public bool playerCanJumpInPDTutorial;

    [SerializeField] private AbilityTutorialProgressiveDisclosureHandler abilityPDHandler;

    public Vector3 playerSpawnPosition = new Vector3(-24.61891f, 4.247815f, -23.54283f), playerPositionDuringDemo;

    [SerializeField] private GameObject triggerWarp;

    public CoinSpawner coinSpawner;
    public bool canMoveRight=true, canMoveLeft=true;

 [SerializeField]   ManagerofManagers managerHub;

    float rawInput;


    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();

        if (OnLandEventInTutorial == null)
            OnLandEventInTutorial = new UnityEvent();


       
    }

    private void Start()
    {
        playerPositionDuringDemo = new Vector3(-23.6000004f, playerSpawnPosition.y,playerSpawnPosition.z);
    }

    private void Update()
    {
         rawInput = Input.GetAxisRaw("Horizontal");

        if (PlayerCanMove()&&((rawInput > 0 && canMoveRight) || (rawInput < 0 && canMoveLeft)))
            move = rawInput;
        else
            move = 0;

        m_Animator.SetBool("Moving", move != 0);

        // Have to make sure it's not already playing the animation, otherwise the trigger will be set twice
        //Fetch the current Animation clip information for the base layer
        AnimatorClipInfo[] currentClipInfo = m_Animator.GetCurrentAnimatorClipInfo(0);
        //Access the Animation clip name
        string clipName = currentClipInfo[0].clip.name;

        if (PlayerCanJump())
        {

            jump = Input.GetButton("Jump");
            if (jump && m_Grounded)
            {

                m_Animator.SetTrigger("Jump");
            }
            else if (!m_Grounded && clipName == "ChefJumpUp")
            {
                m_Animator.ResetTrigger("Jump");
            }

            // without the if, the animator will interrupt the first jump animation because InAir is true
            if (clipName != "ChefJumpUp")
            {
                m_Animator.SetBool("InAir", !m_Grounded);
            }
        }
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!wasGrounded && m_Rigidbody2D.velocity.y <= 0.01f)
                {
                    if (playerCanJumpInPDTutorial && AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered)
                    {
                        OnLandEventInTutorial.Invoke();
                    }

                        if(!coinSpawner.startedSpawning)
                        OnLandEvent.Invoke();

                    if (triggerWarp.gameObject.activeInHierarchy)
                        triggerWarp.gameObject.SetActive(false);
                }
            }
        }

        Move(move, jump);
    }

	


	public void Move(float move, bool jump)
	{
        if (PlayerCanMove())
        {
            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {
                // Move the character by finding the target velocity


                playerTargetVelocity.x = move * m_Speed;
                playerTargetVelocity.y = m_Rigidbody2D.velocity.y;

                // And then smoothing it out and applying it to the character
                m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, playerTargetVelocity, ref m_Velocity, m_MovementSmoothing);

                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }
            // If the player should jump...
            if (m_Grounded && jump)
            {
              
                    // Add a vertical force to the player.
                    m_Grounded = false;
                    m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
                    m_Animator.SetTrigger("Jump");
                
            }
        }
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

    public IEnumerator WaitForLanding()
    {
        yield return new WaitUntil(() => m_Grounded);
        abilityPDHandler.miniGamePDHandler.CheckIfCanContinueAfterCollectingCoin();
    }

    private void OnEnable()
    {
        if (CanSpawnPlayerNormally())
            gameObject.transform.position = playerSpawnPosition;
        else
            gameObject.transform.position = playerPositionDuringDemo;
    }

    bool PlayerCanJump()
    {
        return managerHub.gameManager.tutorialType != GameManager.TutorialType.progressiveDisclosure || abilityPDHandler.completedTutorials.Contains("Collecting") || playerCanJumpInPDTutorial;
    }

    bool PlayerCanMove()
    {
        return managerHub.gameManager.tutorialType != GameManager.TutorialType.progressiveDisclosure || abilityPDHandler.completedTutorials.Contains("Collecting") || playerCanMoveInPDTutorial;
    }

    bool CanSpawnPlayerNormally()
    {
        return (!AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered || managerHub.abilityPDManager.GetStepTutorialType() != TutorialStepType.demonstrateLosing);
    }
}