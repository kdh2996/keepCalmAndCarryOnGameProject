using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour {

    public void OnStartButtonClicked()
    {
        //Application.LoadLevel("Game_Modeling");

        // 1초후 게임 씬을 불러옴.
        Invoke("LoadGameScene", 1.0f);
    }

    public void OnBossButtonClicked()
    {
        // 1초후 게임 씬을 불러옴.
        Invoke("LoadBossScene", 1.0f);
    }

    public void OnCharacterButtonClicked()
    {
        // 1초후 캐릭터 씬을 불러옴.
        Invoke("LoadCharacterScene", 1.0f);
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

    // 캐릭터 씬을 불러오는 메소드
    public void LoadCharacterScene()
    {
        SceneManager.LoadScene("Character");
    }
}
