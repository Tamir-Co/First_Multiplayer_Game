using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameButtons : MonoBehaviour
{
    public PlayerMovement playerMovement;
    //private GameObject gameObject;

    private Button btn_jump;
    private Button btn_attack;
    private Button btn_extra_attack;

    private Image img_jump_icon;
    private Image img_attack_icon;
    private Image img_extra_attack_icon;

    private string hero_name;
    private float attack_duration = 0.7f;
    private float extra_attack_duration = 3f;

    //private bool was_paused { get; set; }
    private float time_until_enable_attack;
    private float current_time;

    private PlayerHealth playerHealth;

    //private bool is_paused = false;

    // Start is called before the first frame update
    void Start()
    {
        btn_jump = GameObject.Find("btn_jump").GetComponent<Button>();
        btn_attack = GameObject.Find("btn_attack").GetComponent<Button>();
        btn_extra_attack = GameObject.Find("btn_extra_attack").GetComponent<Button>();

        hero_name = playerMovement.get_HeroName();
        if (hero_name.Equals("Knight"))
            attack_duration = 0.5f;

        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();

        //was_paused = false;
        //time_until_enable_attack = 0;

        set_Icons();
        //Debug.Log("image name" +  img_attack_icon.sprite.name);

    }

    // Update is called once per frame
    //void Update()
    //{
    //
    //}

    private void set_Icons()
    {
        img_jump_icon = GameObject.Find("btn_jump/img_jump_icon").GetComponent<Image>();
        img_attack_icon = GameObject.Find("btn_attack/img_attack_icon").GetComponent<Image>();
        img_extra_attack_icon = GameObject.Find("btn_extra_attack/img_extra_attack_icon").GetComponent<Image>();

        img_jump_icon.sprite = Resources.Load<Sprite>("Icons for buttons/" + hero_name + "_jump_icon");
        img_attack_icon.sprite = Resources.Load<Sprite>("Icons for buttons/" + hero_name + "_attack_icon");
        img_extra_attack_icon.sprite = Resources.Load<Sprite>("Icons for buttons/" + hero_name + "_extra_attack_icon");

        {
            //switch (playerMovement.get_HeroIndex())
            //{
            //    case 0:  // knight
            //        img_jump_icon.sprite = Resources.Load<Sprite>("Icons for buttons/knight_jump_icon");
            //        img_attack_icon.sprite = Resources.Load<Sprite>("Icons for buttons/knight_attack_icon");
            //        img_extra_attack_icon.sprite = Resources.Load<Sprite>("Icons for buttons/knight_extra_attack_icon");
            //        break;
            //
            //    case 1:  // mage
            //        img_jump_icon.sprite = Resources.Load<Sprite>("Icons for buttons/mage_jump_icon");
            //        img_attack_icon.sprite = Resources.Load<Sprite>("Icons for buttons/mage_attack_icon");
            //        img_extra_attack_icon.sprite = Resources.Load<Sprite>("Icons for buttons/mage_extra_attack_icon");
            //        break;
            //
            //    case 2:  // rogue
            //        img_jump_icon.sprite = Resources.Load<Sprite>("Icons for buttons/rogue_jump_icon");
            //        img_attack_icon.sprite = Resources.Load<Sprite>("Icons for buttons/rogue_attack_icon");
            //        img_extra_attack_icon.sprite = Resources.Load<Sprite>("Icons for buttons/rogue_extra_attack_icon");
            //        break;
            //}
        }
    }

    public void btn_Pause()
    {
        //if (!is_paused)
        //{
        Time.timeScale = 0;
        playerMovement.set_PauseGame(true);
        //}
        //is_paused = !is_paused;
    }

    public void btn_Jump()
    {
        if (!playerHealth.is_hurt)
            playerMovement.set_is_Jump();
    }

    public void btn_Attack()
    {
        if (!playerHealth.is_hurt)
        {
            playerMovement.set_is_attack(true);
            img_attack_icon.fillAmount = 0;
            StartCoroutine(DisableAttack());
        }
    }
    public void btn_Extra_Attack()
    {
        if (!playerHealth.is_hurt)
        {
            playerMovement.set_is_extra_attack(true);
            img_extra_attack_icon.fillAmount = 0;
            StartCoroutine(DisableExtraAttack());
        }
    }

    private IEnumerator DisableAttack()
    {
        //time_until_enable_attack = attack_duration;
        btn_attack.interactable = false;
        while (img_attack_icon.fillAmount < 1)
        {
            img_attack_icon.fillAmount += 1 / attack_duration * Time.deltaTime;
            yield return null;
        }
        playerMovement.set_is_attack(false);
        btn_attack.interactable = true;
    }

    private IEnumerator DisableExtraAttack()
    {
        btn_extra_attack.interactable = false;
        while (img_extra_attack_icon.fillAmount < 1)
        {
            img_extra_attack_icon.fillAmount += 1 / extra_attack_duration * Time.deltaTime;
            yield return null;
        }
        playerMovement.set_is_extra_attack(false);
        btn_extra_attack.interactable = true;
    }

    //private void OnDisable()
    //{
    //    if (playerMovement.game_is_ON)
    //    {
    //
    //        //time_until_enable_attack = 5.5f;
    //    }
    //}

    private void OnEnable()
    {
        if (playerMovement.game_is_ON)
        {
            Start();
            //Debug.Log("     *****     OnEnable **** : " + btn_attack);
            StartCoroutine(DisableAttack());
            StartCoroutine(DisableExtraAttack());
        }
    }
}