using System.Collections;
using UnityEngine;
using Mirror;

public class MP_PlayerCombat : NetworkBehaviour
{
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public GameObject MageFirePrefab;

    private string hero_name;
    private float time_until_attack;
    private Vector2 knight_attack_position = new Vector2(1.7f, 0);
    private Vector2 knight_extra_attack_position = new Vector2(2f, 0);
    private Vector2 mage_attack_position = new Vector2(3.8f, 0.2f);
    private Vector2 mage_extra_attack_position = new Vector2(3.8f, 1f);
    private Vector2 rogue_attack_position = new Vector2(1.6f, 0);
    private Vector2 rogue_extra_attack_position = new Vector2(1.6f, 0);
    private Vector2 current_attack_position;
    private float attack_radius = 1.8f;      // ******
    //private Collider2D[] hit_enemies;

    private int attack_damage = 20;
    private int extra_attack_damage;
    private GameObject MageFire_clone;
    MageFire mageFire;
    private bool is_extra_attack;
    private bool wait_for_extra_attack = false;
    private Animator animator;
    private AnimatorStateInfo animatorState;


    // Start is called before the first frame update
    public override void OnStartLocalPlayer()
    {
        hero_name = PlayerPrefs.GetString("Hero", "Knight");

    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (wait_for_extra_attack && !animator.GetCurrentAnimatorStateInfo(0).IsName("jump"))
        {
            wait_for_extra_attack = false;
            ExtraAttack();
        }
    }

    public void startAttack()
    {
        if (!isLocalPlayer)
            return;

        animatorState = animator.GetCurrentAnimatorStateInfo(0);
        if (animatorState.IsName("land") || animatorState.IsName("stand") || animatorState.IsName("jump") ||
            animatorState.IsName("walk") || animatorState.IsName("run"))
        {
            Attack();
        }
    }

    public void startExtraAttack()
    {
        if (!isLocalPlayer)
            return;

        animatorState = animator.GetCurrentAnimatorStateInfo(0);
        if (animatorState.IsName("jump"))
            wait_for_extra_attack = true;
        if (animatorState.IsName("land") || animatorState.IsName("stand") ||
            animatorState.IsName("walk") || animatorState.IsName("run"))
        {
            ExtraAttack();
        }
    }

    private void Attack()
    {
        if (!isLocalPlayer)
            return;

        switch (hero_name)
        {
            case "Knight":
                attack_radius = 2.9f;
                time_until_attack = 0.2f;
                attack_damage = 20;
                StartCoroutine(MeleeAttack(get_AttackPosition(knight_attack_position), attack_radius));
                break;

            case "Mage":
                time_until_attack = 0.4f;
                is_extra_attack = false;
                StartCoroutine(WaitTo_FireAttack(get_AttackPosition(mage_attack_position)));
                break;

            case "Rogue":
                attack_radius = 2.7f;
                time_until_attack = 0.1f;
                attack_damage = 25;
                StartCoroutine(MeleeAttack(get_AttackPosition(rogue_attack_position), attack_radius));

                time_until_attack = 0.3f;
                StartCoroutine(MeleeAttack(get_AttackPosition(rogue_attack_position), attack_radius));
                break;
        }
    }
    private void ExtraAttack()
    {
        if (!isLocalPlayer)
            return;

        switch (hero_name)
        {
            case "Knight":
                attack_radius = 5.5f;
                time_until_attack = 0.62f;
                attack_damage = 40;
                StartCoroutine(MeleeAttack(get_AttackPosition(knight_extra_attack_position), attack_radius));
                break;

            case "Mage":
                time_until_attack = 0.62f;
                is_extra_attack = true;
                StartCoroutine(WaitTo_FireAttack(get_AttackPosition(mage_extra_attack_position)));
                break;

            case "Rogue":
                attack_radius = 3.3f;
                time_until_attack = 0.3f;
                attack_damage = 30;
                StartCoroutine(MeleeAttack(get_AttackPosition(rogue_extra_attack_position), attack_radius));

                time_until_attack = 0.5f;
                StartCoroutine(MeleeAttack(get_AttackPosition(rogue_extra_attack_position), attack_radius));
                break;
        }
    }

    private IEnumerator MeleeAttack(Vector2 position, float radius)
    {
        //Debug.Log("MeleeAttack starts... ");
        //Debug.Log("position, radius: " + position + "  " + radius);
        yield return new WaitForSecondsRealtime(time_until_attack);
        //Debug.Log(" after wait");

        Cmd_MeleeAttack(position, radius);

        yield return null;
    }

    [Command]
    private void Cmd_MeleeAttack(Vector2 position, float radius)
    {
        Collider2D[] hit_enemies = Physics2D.OverlapCircleAll(position, radius, enemyLayers);
        Debug.Log(" hit_enemies:" + hit_enemies + "   langth: " + hit_enemies.Length);
        foreach (Collider2D enemy in hit_enemies)
        {
            Debug.Log("MeleeAttack hit: " + enemy.name);
            enemy.GetComponent<MP_Enemy>().TakeDamage(attack_damage);
        }
    }

    private IEnumerator WaitTo_FireAttack(Vector2 position)
    {
        yield return new WaitForSecondsRealtime(time_until_attack);
        if (is_extra_attack)
            Cmd_ExtraFireAttack(position);
        else
            Cmd_FireAttack(position);
        yield return null;
    }

    [Command]
    private void Cmd_FireAttack(Vector2 position)
    {
        MageFire_clone = (GameObject)Instantiate(MageFirePrefab, position, attackPoint.rotation);
        mageFire = MageFire_clone.GetComponent<MageFire>();
        mageFire.set_attack_damage(attack_damage);
        mageFire.is_MP = true;
        NetworkServer.Spawn(MageFire_clone);
    }

    [Command]
    private void Cmd_ExtraFireAttack(Vector2 position)
    {
        MageFire_clone = (GameObject)Instantiate(MageFirePrefab, position, attackPoint.rotation);
        mageFire = MageFire_clone.GetComponent<MageFire>();
        mageFire.set_extraFire();
        mageFire.set_attack_damage(attack_damage * 2);
        mageFire.is_MP = true;
        NetworkServer.Spawn(MageFire_clone);
    }

    private Vector2 get_AttackPosition(Vector2 position)  // Get the right position of the attack according to the transform rotation
    {
        //Debug.Log("position:    " + position);
        Vector2 tmp_pos = position;
        tmp_pos.Scale(new Vector2(attackPoint.rotation.y + attackPoint.rotation.w, 1));
        //Debug.Log("tmp_pos:    " + tmp_pos);
        //Debug.Log("attackPoint.position: " + attackPoint.position);
        //current_attack_position = (Vector2)attackPoint.position + tmp_pos;
        return (Vector2)attackPoint.position + tmp_pos;
    }

    void OnDrawGizmosSelected()
    {
        Vector2 tmp_pos = knight_attack_position;
        tmp_pos.Scale(new Vector2(attackPoint.rotation.y + attackPoint.rotation.w, 1));
        current_attack_position = (Vector2)attackPoint.position + tmp_pos;
        Gizmos.DrawWireSphere(current_attack_position, attack_radius);
    }

    public void setAnimator(Animator anim)
    {
        animator = anim;
    }
}