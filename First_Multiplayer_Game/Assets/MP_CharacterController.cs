using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class MP_CharacterController : NetworkBehaviour
{
	[SerializeField] private float m_JumpVelocity = 25f;                           // Amount of force added when the player jumps.
	//[SerializeField] private float highjump_velocity = 7;
	//[SerializeField] [Range(0, 1)] private float m_CrouchSpeed = 0.36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
	[SerializeField] [Range(0, 0.3f)] private float m_MovementSmoothing = 0.05f; // How much to smooth out the movement
	[SerializeField] private bool m_AirControl = true;                           // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;                           // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                            // A position marking where to check if the player is grounded.
																				 //[SerializeField] private Transform m_CeilingCheck;                           // A position marking where to check for ceilings
																				 //[SerializeField] private Collider2D m_CrouchDisableCollider;                 // A collider that will be disabled when crouching

	private Rigidbody2D m_Rigidbody2D;
	private Animator animator;
	private Vector3 m_Velocity = Vector3.zero;
	private bool m_FacingRight = true;   // For determining which way the player is currently facing.
	private bool m_Grounded;             // Whether or not the player is grounded.
	const float k_GroundedRadius = 0.35f; // Radius of the overlap circle to determine if grounded
										  //const float k_CeilingRadius = 0.2f;  // Radius of the overlap circle to determine if the player can stand up

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	bool wasGrounded;
	Collider2D[] colliders;
	private int jump_trigger_HashId = Animator.StringToHash("jump_trigger");
	private int high_jump_trigger_HashId = Animator.StringToHash("high_jump_trigger");
	private int has_landed_HashId = Animator.StringToHash("has_landed");
	//private int is_attack_HashId = Animator.StringToHash("is_attack");
	bool is_jump = false;
	//[HideInInspector] public bool is_extra_attack { get; set; }


	public override void OnStartLocalPlayer()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		//playerCombat = GetComponent<PlayerCombat>();
		//is_extra_attack = false;

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();
	}

	private void FixedUpdate()
	{
		wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (!is_jump && colliders[i].gameObject != gameObject)  // if the player is not starting a jump & standing on some object
			{
				m_Grounded = true;
				if (!wasGrounded)
				{
					OnLandEvent.Invoke();
					//Debug.Log("OnLandEvent.Invoke()");
				}
			}
		}
		if (!m_Grounded)  // an extra check if the player has detached from the ground
			is_jump = false;
	}

	//private void OnDrawGizmosSelected()
	//{
	//	Gizmos.DrawWireSphere(m_GroundCheck.position, k_GroundedRadius);
	//}


	public void Move(float move_speed, bool jump, bool high_jump, bool attack, bool extra_attack)
	{
		// only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{
			// Disable one of the colliders when crouching
			//if (m_CrouchDisableCollider != null)
			//	  m_CrouchDisableCollider.enabled = false;


			Vector3 targetVelocity = new Vector2(move_speed * 10f, m_Rigidbody2D.velocity.y);  // Move the character by finding the target velocity
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);  // And then smoothing it out and applying it to the character

			//if (attack)
			//{
			//	animator.SetBool(is_attack_HashId, true);
			//	playerCombat.startAttack();
			//}

			if (!attack && ((move_speed > 0 && !m_FacingRight) || (move_speed < 0 && m_FacingRight)))  // If the input is moving the player to another direction he is facing, then flip
				Flip();  // Flip the player.

			//if (!m_Grounded)  // If the player is jumping then wait until he is landing and then attack (extra attack)
			//	is_extra_attack = extra_attack;
		}

		//if (is_jump)
		//	Debug.Log(" m_Grounded + attack : " + m_Grounded + attack);
		//if (jump && is_jump && !m_Grounded && !attack)  // high jump
		//{
		//	//float highjump_velocity = m_Rigidbody2D.velocity.y + m_JumpVelocity   ______  m_Rigidbody2D.velocity.y + m_JumpVelocity /2
		//	m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, m_JumpVelocity + highjump_velocity);
		//	//m_Grounded = false;
		//	animator.SetTrigger(high_jump_trigger_HashId);
		//	is_jump = false;   //***
		//}

		//if (jump)
		//	Debug.Log("<color=red> move Jump : </color>");
		//if (m_Grounded)
		//	Debug.Log("<color=red> move m_Grounded : </color>");
		//if (!attack)
		//	Debug.Log("<color=red> move !attack : </color>");

		if (jump && m_Grounded && !attack)  // If the player should jump
		{
			// Add a vertical force to the player.
			//m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
			//Debug.Log(" <color=red>jump : </color>");
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, m_JumpVelocity);
			m_Grounded = false;
			is_jump = true;
			animator.SetBool(has_landed_HashId, false);
			animator.SetTrigger(jump_trigger_HashId);
			//StartCoroutine(DisableHighJump());
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Coin"))
		{
			//Debug.Log("<color=red> Trigger Enter </color>");
			Destroy(collision.gameObject);
		}
	}

	//private void OnCollisionEnter2D(Collision2D collision)
	//{
	//    if (collision.gameObject.CompareTag("Coin"))
	//	{
	//		Debug.Log("<color=blue> Collision Enter </color>");
	//		Destroy(collision.gameObject);
	//    }
	//}

	private IEnumerator DisableHighJump()
	{
		yield return new WaitForSecondsRealtime(0.7f);
		is_jump = false;
	}

	public void Land()
	{
		//Debug.Log("landed");
		animator.SetBool(has_landed_HashId, true);
	}

	private void Flip()  // Switch the way the player is labelled as facing.
	{
		m_FacingRight = !m_FacingRight;
		transform.Rotate(0, 180f, 0);
		RectTransform[] rects = GetComponentsInChildren<RectTransform>();
		foreach (RectTransform oneRect in rects)
			if (oneRect.name == "Canvas")
				oneRect.Rotate(0, 180f, 0);
	}

	public void setAnimator(Animator anim)
	{
		animator = anim;
	}
}