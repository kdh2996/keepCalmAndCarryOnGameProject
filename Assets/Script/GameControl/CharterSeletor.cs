using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharterSeletor : MonoBehaviour {

    public void OnTitletButtonClicked()
    {
        //Application.LoadLevel("Game_Modeling");

        // 1초후 타이틀 씬을 불러옴.
        Invoke("LoadTitleScene", 1.0f);
    }

    public void OnArcherButtonClicked()
    {
        //Application.LoadLevel("Game_Modeling");

        // 1초후 게임 씬을 불러옴.
        Invoke("LoadArcherNormalTalentsScene", 1.0f);
    }

    public void OnWarriorButtonClicked()
    {
        //Application.LoadLevel("Game_Modeling");

        // 1초후 게임 씬을 불러옴.
        Invoke("LoadWarriorNormalTalentsScene", 1.0f);
    }

    public void OnMageButtonClicked()
    {
        //Application.LoadLevel("Game_Modeling");

        // 1초후 게임 씬을 불러옴.
        Invoke("LoadMageNormalTalentsScene", 1.0f);
    }

    // 타이틀 씬을 불러오는 메소드
    public void LoadTitleScene()
    {
        SceneManager.LoadScene("Title");
    }

    // 아처의 노말 특성 씬을 불러오는 메소드
    public void LoadArcherNormalTalentsScene()
    {
        SceneManager.LoadScene("Archer_NormalTalents");
    }

    // 워리어의 노말 특성 씬을 불러오는 메소드
    public void LoadWarriorNormalTalentsScene()
    {
        SceneManager.LoadScene("Warrior_NormalTalents");
    }

    // 메이지의 노말 특성 씬을 불러오는 메소드
    public void LoadMageNormalTalentsScene()
    {
        SceneManager.LoadScene("Mage_NormalTalents");
    }

}
