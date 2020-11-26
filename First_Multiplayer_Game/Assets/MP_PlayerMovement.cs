using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;   //using UnityEngine.Networking;

public class MP_PlayerMovement : NetworkBehaviour
{
    public MP_CharacterController controller;
    public GameObject game_Buttons;
    public GameObject pause_Menu;

    public float run_Speed = 55f;
    public float climb_Speed = 20f;
    public Joystick joystick;
    public GameObject txt_START;


    //int chosen_hero_index;
    string chosen_hero_name;
    SpriteRenderer _spriteRenderer;
    private MP_PlayerCombat playerCombat;
    private MP_PlayerHealth playerHealth;
    float horizontal_speed = 0;
    float vertical_speed = 0;
    bool is_jump = false;
    bool is_high_jump = false;
    bool can_high_jump = false;
    bool is_attack = false;
    bool is_extra_attack = false;
    bool is_dead = false;

    private Animator animator;
    private int speed_HashId = Animator.StringToHash("speed");
    //private int is_attack_HashId = Animator.StringToHash("is_attack");
    private int attack_trigger_HashId = Animator.StringToHash("attack_trigger");
    private int extra_attack_trigger_HashId = Animator.StringToHash("extra_attack_trigger");

    public bool game_is_ON { get; set; }  // whether the game has started
    bool is_firstTouch = true;
    public bool game_is_PAUSED { get; set; }  // whether the game is paused

    private RectTransform rectTransform;



    // Start is called before the first frame update
    public override void OnStartLocalPlayer()
    {
        //chosen_hero_index = PlayerPrefs.GetInt("Hero", 0);
        chosen_hero_name = PlayerPrefs.GetString("Hero", "Knight");
        Debug.Log("chosen hero name: " + chosen_hero_name);
        game_is_ON = false;
        game_is_PAUSED = false;

        _spriteRenderer = transform.GetComponent<SpriteRenderer>();

        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = Resources.Load("Animations/Animator Controllers/Player Animators/" + chosen_hero_name + " Animator") as RuntimeAnimatorController;
        controller.setAnimator(animator);

        playerCombat = GetComponent<MP_PlayerCombat>();
        playerCombat.setAnimator(animator);

        playerHealth = GetComponent<MP_PlayerHealth>();
        playerHealth.setAnimator(animator);

        rectTransform = joystick.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x * 2, rectTransform.sizeDelta.y);

        game_Buttons.SetActive(false);
        pause_Menu.SetActive(false);

        //Debug.Log("chosen Hero: " + chosen_hero_index);


        StartCoroutine(FadeIn());
    }

    void Update()  // Update is called once per frame
    {
        if (!isLocalPlayer)
            return;

        if (joystick.gameObject.activeSelf)
        {
            horizontal_speed = joystick.Horizontal * run_Speed;
            //vertical_speed = joystick.Vertical;
        }

        if (game_is_ON)  // the game is on
        {
            if (Math.Abs(joystick.Horizontal) > 0.85)
                horizontal_speed = run_Speed * Math.Sign(joystick.Horizontal);
            if (Math.Abs(joystick.Horizontal) < 0.25)
                horizontal_speed = 0;

            if (joystick.Vertical > 0.9)
                is_jump = true;
            //if (Math.Abs(joystick.Vertical) < 0.15)
            //    vertical_speed = 0;
        }
        else  // the game is paused or not started yet
        {
            if (is_firstTouch && horizontal_speed != 0)  // detecting the first touch
                is_firstTouch = false;
            else if (!is_firstTouch && horizontal_speed == 0 && !is_dead)  // starting the game
            {
                game_is_ON = true;
                Destroy(txt_START);
                //txt_START.SetActive(false);  // hiding the text

                //rectTransform = joystick.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x / 2, rectTransform.sizeDelta.y);

                game_Buttons.SetActive(true);
            }
        }
        //Debug.Log("horizontal_speed: " + horizontal_speed);
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        if (game_is_ON && !game_is_PAUSED)
        {
            controller.Move(horizontal_speed * Time.fixedDeltaTime, is_jump, is_high_jump, is_attack, is_extra_attack);
            animator.SetFloat(speed_HashId, Math.Abs(horizontal_speed / run_Speed));
            is_jump = false;
            is_high_jump = false;
        }
        else if (is_dead)
            controller.Move(0, false, false, false, false);
    }

    private IEnumerator FadeIn()
    {
        Color tmpcolor = _spriteRenderer.color;
        tmpcolor.a = 0;
        while (tmpcolor.a < 1)
        {
            tmpcolor.a += Time.deltaTime;
            _spriteRenderer.color = tmpcolor;
            yield return null;
        }
    }

    public string get_HeroName() { return chosen_hero_name; }

    public void set_is_Jump()
    {
        if (!isLocalPlayer)
            return;

        is_jump = true;
        //Debug.Log("<color=red> set_is_Jump : </color>");
        //if (can_high_jump)
        //    is_high_jump = true;
        //can_high_jump = true;
        //if (is_high_jump)
        //    can_high_jump = false;
        //StartCoroutine(DisableHighJump());
    }

    //private IEnumerator DisableHighJump()
    //{
    //    yield return new WaitForSecondsRealtime(0.7f);
    //    can_high_jump = false;
    //}

    public void set_is_attack(bool attack)
    {
        if (!isLocalPlayer)
            return;

        is_attack = attack;

        if (is_attack)
        {
            animator.SetTrigger(attack_trigger_HashId);
            playerCombat.startAttack();
        }
    }

    public void set_is_extra_attack(bool extra_attack)
    {
        if (!isLocalPlayer)
            return;

        is_extra_attack = extra_attack;

        if (is_extra_attack)
        {
            animator.SetTrigger(extra_attack_trigger_HashId);
            playerCombat.startExtraAttack();
        }
    }

    public void Land()
    {
        //Debug.Log("landed 2");
        //if (controller.is_extra_attack)
        //{
        //    set_is_extra_attack(true);
        //    controller.is_extra_attack = false;
        //}
    }

    public void set_Death()
    {
        if (!isLocalPlayer)
            return;
        is_dead = true;
        game_is_ON = false;
        joystick.gameObject.SetActive(false);
        game_Buttons.SetActive(false);
        //GameObject.FindGameObjectWithTag("joystick").SetActive(false);
        //joystick.GetComponent<FloatingJoystick>().enabled = false;
        horizontal_speed = 0;
        //vertical_speed = 0;
    }

    public void set_PauseGame(bool is_pause)
    {
        game_is_PAUSED = is_pause;
        joystick.gameObject.SetActive(!is_pause);
        game_Buttons.SetActive(!is_pause);
        pause_Menu.SetActive(is_pause);
    }

}