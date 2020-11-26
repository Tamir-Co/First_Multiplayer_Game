using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MP_Enemy : NetworkBehaviour
{
    public float speed = 5f;
    public Vector2 attack_offset;
    public float attack_radius = 3;
    public LayerMask player_Layer;
    public Transform edge_right, edge_left;
    public bool is_facicg_right = true;

    public HealthBar health_bar;
    public int max_health = 100;
    private int current_health = 100;

    public GameObject CoinPrefab;

    public int index { get; set; }  // The index of the enemy in the spawner
    private Rigidbody2D rb;
    private SpriteRenderer sprite_renderer;
    private RectTransform canvas;
    private Vector3 scale;
    private Animator animator;
    private readonly int is_walking_HashId = Animator.StringToHash("is_walking");

    private bool is_dead = false;
    private int attack_damage = 15;
    private Vector2 attack_position;
    private bool is_attacking = false;
    private float time_between_attacks = 1.4f;

    private bool is_walking = false;
    private bool is_moving_right;
    private string ChooseDirection_coroutine = "ChooseDirection";

    private Transform player;
    private Vector2 target_pos;
    private float distance;  // The distance betweeen the player and the enemy

    //private bool game_is_ON = false;
    private MP_PlayerMovement playerMovement;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite_renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        RectTransform[] rects = GetComponentsInChildren<RectTransform>();
        foreach (RectTransform oneRect in rects)
            if (oneRect.name == "Canvas")
                canvas = oneRect;

        health_bar.set_MaxHealth(max_health);

        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerMovement = player.GetComponent<MP_PlayerMovement>();

        StartCoroutine(ChooseDirection_coroutine);
    }


    void FixedUpdate()
    {
        if (!is_attacking && !is_dead && playerMovement.game_is_ON)
        {
            distance = Vector2.Distance(transform.position, player.position);
            if (distance < 15)
            {
                LookAtPlayer();
                if (distance < 5)   // Attack
                {
                    animator.SetBool(is_walking_HashId, false);
                    animator.SetTrigger("attack_trigger");
                }
                else  // Walk towards the player
                {
                    if ((transform.position.x > edge_left.position.x && !is_facicg_right) ||
                        (transform.position.x < edge_right.position.x && is_facicg_right))     // If the enemy is between the 2 edges
                    {
                        target_pos = new Vector2(player.position.x, rb.position.y);
                        rb.MovePosition(Vector2.MoveTowards(rb.position, target_pos, speed * Time.fixedDeltaTime));
                        animator.SetBool(is_walking_HashId, true);
                    }
                }
            }
            else  // idle and walk
            {
                //animator.SetBool(is_walking_HashId, is_walking);
                if (is_walking)
                {
                    if ((is_moving_right && !is_facicg_right) || (!is_moving_right && is_facicg_right))
                        Flip();

                    if ((transform.position.x > edge_right.position.x && is_facicg_right) ||  // If he reaches the right edge
                        (transform.position.x < edge_left.position.x && !is_facicg_right))  // If he reaches the left edge
                    {
                        //Debug.Log("transform.right.x before:  " + transform.right.x);
                        Flip();
                        is_moving_right = !is_moving_right;
                        //Debug.Log("transform.right.x after:  " + transform.right.x);
                    }


                    target_pos = new Vector2(rb.position.x + transform.right.x, rb.position.y);
                    rb.MovePosition(Vector2.MoveTowards(rb.position, target_pos, speed * Time.fixedDeltaTime));
                }
            }
        }
    }

    public void DecideWalkOrIdle()  // Called by the "lizard_walk" & "lizard_idle" animations
    {
        if (!is_dead && playerMovement.game_is_ON)
        {
            is_walking = (Random.value < 0.5);
            animator.SetBool(is_walking_HashId, is_walking);
            //if (is_walking)
            //    StartCoroutine(ChooseDirection_coroutine);
        }
        else
        {
            is_walking = false;
            animator.SetBool(is_walking_HashId, is_walking);
        }
    }

    private IEnumerator ChooseDirection()  // Choosing the direction in which the player is walking
    {
        is_moving_right = (Random.value < 0.5);
        yield return new WaitForSecondsRealtime(Random.Range(1.5f, 2.5f));

        if (is_walking)  // Reset the coroutine
        {
            StopCoroutine(ChooseDirection_coroutine);
            StartCoroutine(ChooseDirection_coroutine);
        }

    }

    public void Attack()  // Called by the "lizard_attack" animation
    {
        is_attacking = true;

        attack_position = transform.position;
        attack_position += (Vector2)transform.right * attack_offset.x;
        attack_position += (Vector2)transform.up * attack_offset.y;

        Collider2D collider = Physics2D.OverlapCircle(attack_position, attack_radius, player_Layer);
        if (collider != null)
        {
            Debug.Log("Lizard Attack on: " + collider.name);
            collider.GetComponent<MP_PlayerHealth>().TakeDamage(attack_damage);
        }
        StartCoroutine(WaitUntilNextAttack());
    }

    private IEnumerator WaitUntilNextAttack()  // Wait until it can attack again
    {
        yield return new WaitForSecondsRealtime(time_between_attacks);
        is_attacking = false;
    }

    public void TakeDamage(int damage)
    {
        if (!is_dead)
        {
            Debug.Log("TakeDamage: " + damage);
            current_health -= damage;
            health_bar.set_Health(current_health);
            animator.SetTrigger("hurt_trigger");
            if (current_health <= 0)
                StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        animator.SetBool("is_dead", true);
        is_dead = true;
        canvas.GetComponent<Canvas>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;

        int[] list_of_amount = { 0, 1, 1, 2 };
        int amount_of_coins = list_of_amount[Random.Range(0, list_of_amount.Length)];
        for (int i = 0; i < amount_of_coins; i++)
            Instantiate(CoinPrefab, transform.position, Quaternion.identity);

        yield return new WaitForSecondsRealtime(1f);  // The animation is 1 sec long
        Color tmpcolor = sprite_renderer.color;
        while (tmpcolor.a > 0)  // Slowly fades out
        {
            tmpcolor.a -= Time.deltaTime;
            sprite_renderer.color = tmpcolor;
            yield return null;
        }
        //Destroy(gameObject);
        StartCoroutine(ComeBackToLife());
    }

    private IEnumerator ComeBackToLife()
    {
        yield return new WaitForSecondsRealtime(15f);
        animator.SetBool("is_dead", false);
        animator.SetBool(is_walking_HashId, false);
        animator.Play("lizard_idle");

        Color tmpcolor = sprite_renderer.color;
        while (tmpcolor.a < 1)  // Slowly fades in
        {
            tmpcolor.a += Time.deltaTime;
            sprite_renderer.color = tmpcolor;
            yield return null;
        }
        yield return new WaitForSecondsRealtime(2.5f);
        is_dead = false;
        current_health = max_health;
        health_bar.set_Health(current_health);
        canvas.GetComponent<Canvas>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
    }

    private void LookAtPlayer()
    {
        if (transform.position.x > player.position.x && is_facicg_right)
            Flip();
        if (transform.position.x < player.position.x && !is_facicg_right)
            Flip();
    }

    private void Flip()
    {
        is_facicg_right = !is_facicg_right;
        scale = transform.localScale;
        scale.z *= -1;
        transform.localScale = scale;
        transform.Rotate(0, 180, 0);
        canvas.Rotate(0, 180, 0);
    }

    void OnDrawGizmosSelected()
    {
        //Debug.Log("----------------------------------------------start");
        //tmp_pos.Scale(new Vector2(attackPoint.rotation.y + attackPoint.rotation.w, 1));
        //current_attack_position = (Vector2)attackPoint.position + tmp_pos;
        //Debug.Log("right, up: " + transform.right + " , " + transform.up);
        //transform.Rotate(0, 180f, 0);
        //Debug.Log("right, up: " + transform.right + " , " + transform.up);
        //Debug.Log("-----------------------------------------------end");
        Gizmos.DrawWireSphere(attack_position, attack_radius);
    }
}