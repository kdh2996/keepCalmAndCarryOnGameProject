using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TalentsController : MonoBehaviour {

    /* 씬 변환 메소드들 */

    public void OnTitletButtonClicked()
    {
        //Application.LoadLevel("Game_Modeling");

        // 1초후 타이틀 씬을 불러옴.
        Invoke("LoadTitleScene", 1.0f);
    }

    public void OnCharacterSelectorButtonClicked()
    {
        //Application.LoadLevel("Game_Modeling");

        // 1초후 캐릭터 씬을 불러옴.
        Invoke("LoadCharacterSelectorsScene", 1.0f);
    }

    public void OnStartButtonClicked()
    {
        //Application.LoadLevel("Game_Modeling");

        // 1초후 게임 씬을 불러옴.
        Invoke("LoadGameScene", 1.0f);
    }

    public void OnBossButtonClicked()
    {
        //Application.LoadLevel("Game_Modeling");

        // 1초후 게임 씬을 불러옴.
        Invoke("LoadBossScene", 1.0f);
    }

    // 타이틀 씬을 불러오는 메소드
    public void LoadTitleScene()
    {
        SceneManager.LoadScene("Title");
    }

    // 캐릭터 선택 씬을 불러오는 메소드
    public void LoadCharacterSelectorsScene()
    {
        SceneManager.LoadScene("Character");
    }

    // 게임 씬을 불러오는 메소드
    public void LoadGameScene()
    {
        SceneManager.LoadScene("Game_Modeling");
    }

    // 보스 씬을 불러오는 메소드
    public void LoadBossScene()
    {
        SceneManager.LoadScene("boss1_InGame");
    }

}
