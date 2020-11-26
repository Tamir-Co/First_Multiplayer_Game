using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageFire : MonoBehaviour
{
    public float speed = 20f;

    private Rigidbody2D rb;
    private Animator animator;
    private CircleCollider2D circleCollider = null;
    private bool is_extraAttack = false;  // Weather is this the extra_attack or not
    private int fire_HashId = Animator.StringToHash("mage's_fire");
    private int extra_fire_HashId = Animator.StringToHash("mage's_extra_fire");
    private int attack_damage;

    [HideInInspector] public bool is_MP = false;


    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("fire_HashId : " + fire_HashId);
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        animator = GetComponent<Animator>();
        animator.Play(fire_HashId);  // Play the "mage's_fire" animation
        if (is_extraAttack)
        {
            animator.Play(extra_fire_HashId);  // Play the "mage's_extra_fire" animation
            circleCollider = GetComponent<CircleCollider2D>();
            circleCollider.offset = new Vector2(-0.8f, -0.6f);
            circleCollider.radius = 1.4f;
        }
        StartCoroutine(WaitUntilStop());
        //StartCoroutine(WaitUntilDestroy());
        Destroy(gameObject, 0.9f);  // The animation is 0.9 sec long 
    }

    // Update is called once per frame
    //void Update() { }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("MageFire collision : " + collision.name);
        if (is_MP)  // If the cuurent game is multyplayer
        {
            MP_Enemy mp_enemy = collision.GetComponent<MP_Enemy>();
            if (mp_enemy != null)
            {
                mp_enemy.TakeDamage(attack_damage);
                Destroy(gameObject);
            }
        }
        else  // If the cuurent game is singleplayer
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(attack_damage);
                Destroy(gameObject);
            }
        }
    }

    public void set_attack_damage(int damage)
    {
        attack_damage = damage;
    }

    public void set_extraFire()
    {
        is_extraAttack = true;
    }

    private IEnumerator WaitUntilStop()
    {
        yield return new WaitForSecondsRealtime(0.5f);  // The fire stops moving after 0.5 sec
        rb.velocity = new Vector2(0, 0);
    }

    private IEnumerator WaitUntilDestroy()
    {
        yield return new WaitForSecondsRealtime(0.9f);  // The animations are 0.9 sec long 
        Destroy(gameObject);
    }
}