using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MP_PlayerHealth : NetworkBehaviour
{
    public HealthBar health_bar;
    public int max_health = 150;
    [SyncVar] private int current_health;

    private Animator animator;
    private MP_PlayerMovement playerMovement;
    [HideInInspector] public bool is_hurt { get; set; }
    private bool is_dead;


    // Start is called before the first frame update
    public override void OnStartLocalPlayer()
    {
        current_health = max_health;
        health_bar.set_MaxHealth(max_health);
        is_hurt = false;
        playerMovement = GetComponent<MP_PlayerMovement>();
    }

    // Update is called once per frame
    //void Update() { }

    public void TakeDamage(int damage)
    {
        if (!this.isLocalPlayer) { return; }
        //if (!this.isServer) { }

        if (!is_dead && !is_hurt)
        {
            is_hurt = true;
            Debug.Log("player Damage: " + damage);
            current_health -= damage;
            animator.SetTrigger("hurt_trigger");
            Cmd_hurt_trigger();
            //StartCoroutine(Hurt());
            if (current_health <= 0)
            {
                current_health = 0;
                Die();
            }
            else
                StartCoroutine(Hurt());
            health_bar.set_Health(current_health);
            //NetworkAnimator netanim = GetComponent<NetworkAnimator>();     //   **********
            //netanim.SetTrigger("dvdvd");
        }
    }

    [Command]
    void Cmd_hurt_trigger()
    {
        Rpc_hurt_trigger();
    }

    [ClientRpc]
    void Rpc_hurt_trigger()
    {
        if (!isLocalPlayer)
            animator.SetTrigger("hurt_trigger");
    }

    private IEnumerator Hurt()
    {
        if (this.isLocalPlayer)
        {
            yield return new WaitForSecondsRealtime(0.45f);  // The animation is 0.5 sec long
            is_hurt = false;
        }
    }

    //public void FinishHurt() { is_hurt = false; }

    private void Die()
    {
        if (this.isLocalPlayer)
        {
            animator.SetBool("is_dead", true);
            is_dead = true;
            playerMovement.set_Death();
            //yield return new WaitForSecondsRealtime(1.25f);  // The animation is 1.25 sec long
            Destroy(gameObject, 1.25f);
        }
    }

    public void setAnimator(Animator anim)
    {
        if (this.isLocalPlayer)
            animator = anim;
    }
}