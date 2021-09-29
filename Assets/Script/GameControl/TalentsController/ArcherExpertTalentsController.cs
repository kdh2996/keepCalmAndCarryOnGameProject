using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ArcherExpertTalentsController : TalentsController
{

    /* 특성 관련 적용 변수 */

    // Archer - Engineer
    Button EngineerSwordOffGunButton;
    bool EngineerSwordOffGunApply_On;


    /* 씬 이동 관련 메소드들 */

    public void OnArcherNormalButtonClicked()
    {
        //Application.LoadLevel("Game_Modeling");

        // 1초후 전문가 특성 씬을 불러옴.
        Invoke("LoadArcherNormalTalentsScene", 1.0f);
    }

    // 아처의 노말 특성 씬을 불러오는 메소드
    public void LoadArcherNormalTalentsScene()
    {
        SceneManager.LoadScene("Archer_NormalTalents");
    }


    /* 특성 적용 메소드들 */
    // 해당 메소드들을 통해 변환된 값은, 
    // 다음 씬에서도 유지되어 변경사항이 반영된다.

    // 최종적으로 특성 적용 메소드들을 모두 모아, 
    // 다음 씬에 반영 하도록 하는 메소드
    public void OnApplyButtonClicked()
    {
        // (3.2.2.6A.1)
        // Engineer 소드 오프 건 특성 적용
        if (EngineerSwordOffGunApply_On == true)
        {
            EngineerSwordOffGunButton.interactable = true;

            // Archer의 특성 반영 사항을 전달
            InGameTalentsDB.InGameTalents_SingleTon().archerPlayer.engineerExpertTalents_DB.EngineerExpertTalents_SwordOffBowGun_On = true;
        }

    }

    // Engineer SwordOffGun 스킬 On Button
    public void OnEngineerSwordOffGunButtonCliked(Button thisButton)
    {
        EngineerSwordOffGunButton = thisButton;
        thisButton.interactable = false;

        // Archer 기본 특성 반영.
        EngineerSwordOffGunApply_On = true;
    }


}