using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class ArcherNormalTalentsController : TalentsController {

    /* 특성 관련 적용 변수 */

    // Archer
    Button ArcherBasicTalentsButton;
    bool ArcherBasicTalentsApply_On = false;

    // Archer - Engineer
    Button EngineerReloadleverButton;
    bool EngineerReloadleverApply_On = false;

    Button EngineerBulkBoltMagazineButton;
    bool EngineerBulkBoltMagazineApply_On = false;

    Button EngineerSwordOffGunButton;
    bool EngineerSwordOffGunApply_On = false;


    /* 씬 이동 관련 메소드들 */

    public void OnArcherExpertButtonClicked()
    {
        //Application.LoadLevel("Game_Modeling");

        // 1초후 전문가 특성 씬을 불러옴.
        Invoke("LoadArcherExpertTalentsScene", 1.0f);
    }

    // 아처의 전문가 특성 씬을 불러오는 메소드
    public void LoadArcherExpertTalentsScene()
    {
        SceneManager.LoadScene("Archer_ExpertTalents_Engineer");
    }


    /* 특성 적용 메소드들 */
    // 해당 메소드들을 통해 변환된 값은, 
    // 다음 씬에서도 유지되어 변경사항이 반영된다.

    // 최종적으로 특성 적용 메소드들을 모두 모아, 
    // 다음 씬에 반영 하도록 하는 메소드
    public void OnApplyButtonClicked()
    {
        // Archer 기본 베이스 특성 적용
        if(ArcherBasicTalentsApply_On == true)
        {
            ArcherBasicTalentsButton.interactable = true;

            // Archer의 특성 반영 사항을 전달
            InGameTalentsDB.InGameTalents_SingleTon().archerPlayer.archerTalents_DB.ArcherBaseTalents_On = true;

            //InGameTalentsDB.InGameTalents_SingleTon().archerTalents_DB.ArcherBaseTalents_On = true;
        }

        // (3.2.2.1)
        // Engineer 장전 지랫대 특성 적용
        if (EngineerReloadleverApply_On == true)
        {
            EngineerReloadleverButton.interactable = true;

            // Archer의 특성 반영 사항을 전달
            InGameTalentsDB.InGameTalents_SingleTon().archerPlayer.engineerStyleTalents_DB.EngineerStyleTalents_ReloadLever_On = true;

            //InGameTalentsDB.InGameTalents_SingleTon().engineerStyleTalents_DB.EngineerStyleTalents_ReloadLever_On = true;
        }

        // (3.2.2.6)
        // Engineer 대용량 볼트 탄창 특성 적용
        if (EngineerBulkBoltMagazineApply_On == true)
        {
            EngineerBulkBoltMagazineButton.interactable = true;

            // Archer의 특성 반영 사항을 전달
            InGameTalentsDB.InGameTalents_SingleTon().archerPlayer.engineerCoreTalents_DB.EngineerCoreTalents_BulkBoltMagazie_On = true;

            //InGameTalentsDB.InGameTalents_SingleTon().engineerCoreTalents_DB.EngineerCoreTalents_BulkBoltMagazie_On = true;
        }
    }

    // Archer Base 스킬 전부 On Button
    public void OnArcherBasicTalentsButtonCliked(Button thisButton)
    {
        ArcherBasicTalentsButton = thisButton;
        thisButton.interactable = false;

        // Archer 기본 특성 반영.
        ArcherBasicTalentsApply_On = true;
    }

    // Engineer Reloadlever 스킬 On Button
    public void OnEngineerReloadleverButtonCliked(Button thisButton)
    {
        EngineerReloadleverButton = thisButton;
        thisButton.interactable = false;

        // Archer 기본 특성 반영.
        EngineerReloadleverApply_On = true;
    }

    // Engineer BulkBoltMagazine 스킬 On Button
    public void OnEngineerBulkBoltMagazineButtonCliked(Button thisButton)
    {
        EngineerBulkBoltMagazineButton = thisButton;
        thisButton.interactable = false;

        // Archer 기본 특성 반영.
        EngineerBulkBoltMagazineApply_On = true;
    }
}
