﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss1_GameController : MonoBehaviour {

    public Player player;

    void Update()
    {
        // 게임 오버 (게임 패배 조건)
        if (player.GetHealthPoint() <= 0)
        {
            // 게임 컨트롤러 업데이트 종료.
            enabled = false;

            // 1초 후에 타이틀 화면으로 회귀하는 메소드 호출.
            Invoke("ReturnToTitle", 1.0f);
        }

    }

    // 타이틀 화면 돌아가기
    void ReturnToTitle()
    {
        // 타이틀 씬으로 직접 회귀.
        SceneManager.LoadScene("Title");
    }

    // 특성 선택 화면 돌아가기
    void ReturnToTalentsPage()
    {
        // 특성 모두 초기화
        InGameTalentsDB.AllResetData_SingleTone();

        // 특성 페이지 씬으로 직접 회귀.
        SceneManager.LoadScene("Character");
    }

    // 기본 스테이지로 이동
    void LoadDefaultStageScene()
    {
        // 특성 페이지 씬으로 직접 회귀.
        SceneManager.LoadScene("Game_Modeling");
    }


    // TitleButton 클릭시 이벤트
    public void OnTitletButtonClicked()
    {
        //Application.LoadLevel("Game_Modeling");

        // 1초후 타이틀 씬을 불러옴.
        Invoke("ReturnToTitle", 1.0f);
    }

    // TalentsButton 클릭시 이벤트
    public void OnTalentsButtonClicked()
    {
        //Application.LoadLevel("Game_Modeling");

        // 1초후 타이틀 씬을 불러옴.
        Invoke("ReturnToTalentsPage", 1.0f);
    }

    // DefaultStageButton 클릭시 이벤트
    public void OnDefaultStageButtonClicked()
    {
        //Application.LoadLevel("Game_Modeling");

        // 1초후 타이틀 씬을 불러옴.
        Invoke("LoadDefaultStageScene", 1.0f);
    }
}
