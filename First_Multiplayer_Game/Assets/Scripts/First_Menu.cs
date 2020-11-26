using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class First_Menu : MonoBehaviour
{
    //static int chosen_hero_index;
    private string chosen_hero_name = "Knight";  // default hero
    [Header("Hero animators:")]
    public Animator Knight_animator;
    public Animator Mage_animator;
    public Animator Rogue_animator;


    //Animation anim;

    void Start()
    {
        //anim = GetComponent<Animation>();
    }

    public void btn_Start_SP()  // Starts the single-player game
    {
        //PlayerPrefs.SetInt("Hero", chosen_hero_index);
        PlayerPrefs.SetString("Hero", chosen_hero_name);
        SceneManager.LoadScene("Game_Scene");
    }

    public void btn_Start_MP()  // Opens the menu to multiplayer game
    {
        //PlayerPrefs.SetInt("Hero", chosen_hero_index);
        PlayerPrefs.SetString("Hero", chosen_hero_name);
        SceneManager.LoadScene("EnterMultiPlayer_Scene");
    }

    public void btn_choose_Knight()
    {
        //chosen_hero_index = 0;  // Knight
        chosen_hero_name = "Knight";

        Knight_animator.speed = 1;
        Mage_animator.speed = 0;
        Rogue_animator.speed = 0;
        Knight_animator.Play("idle");
        Mage_animator.Play("stand");
        Rogue_animator.Play("stand");
    }
    public void btn_choose_Mage()
    {
        //chosen_hero_index = 1;  // Mage
        chosen_hero_name = "Mage";

        Knight_animator.speed = 0;
        Mage_animator.speed = 1;
        Rogue_animator.speed = 0;
        Knight_animator.Play("stand");
        Mage_animator.Play("idle");
        Rogue_animator.Play("stand");
    }
    public void btn_choose_Rogue()
    {
        //chosen_hero_index = 2;  // Rogue
        chosen_hero_name = "Rogue";

        Knight_animator.speed = 0;
        Mage_animator.speed = 0;
        Rogue_animator.speed = 1;
        Knight_animator.Play("stand");
        Mage_animator.Play("stand");
        Rogue_animator.Play("idle");
    }
}