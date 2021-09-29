using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class WarriorNormalTalentsController : TalentsController
{

    /* 특성 관련 적용 변수 */

    // Warrior
    Button WarriorBasicTalentsButton;
    bool WarriorBasicTalentsApply_On = false;

    // Warrior - Guardian
    Button GuardianKnockBackButton;
    bool GuardianKnockBackApply_On = false;

    Button GuardianTauntButton;
    bool GuardianTauntApply_On = false;

    Button GuardianRoarButton;
    bool GuardianRoarApply_On = false;


    /* 씬 변환 메소드들 */

    public void OnWarriorExpertButtonClicked()
    {
        //Application.LoadLevel("Game_Modeling");

        // 1초후 전문가 특성 씬을 불러옴.
        Invoke("LoadWarriorExpertTalentsScene", 1.0f);
    }

    // 워리어의 전문가 특성 씬을 불러오는 메소드
    public void LoadWarriorExpertTalentsScene()
    {
        SceneManager.LoadScene("Warrior_ExpertTalents_Guardian");
    }


    /* 특성 적용 메소드들 */
    // 해당 메소드들을 통해 변환된 값은, 
    // 다음 씬에서도 유지되어 변경사항이 반영된다.

    // 최종적으로 특성 적용 메소드들을 모두 모아, 
    // 다음 씬에 반영 하도록 하는 메소드
    public void OnApplyButtonClicked()
    {
        // Warrior 기본 베이스 특성 적용
        if (WarriorBasicTalentsApply_On == true)
        {
            WarriorBasicTalentsButton.interactable = true;

            // Warrior의 특성 반영 사항을 전달
            InGameTalentsDB.InGameTalents_SingleTon().warriorPlayer.warriorTalents_DB.WarriorBaseTalents_On = true;
        }

        // (4.2.2.2)
        // Guardian 넉백 특성 적용
        if (GuardianKnockBackApply_On == true)
        {
            GuardianKnockBackButton.interactable = true;

            // Warrior의 특성 반영 사항을 전달
            InGameTalentsDB.InGameTalents_SingleTon().warriorPlayer.guardianStyleTalents_DB.GuardianStyleTalents_KnockBack_On = true;
        }

        // (4.2.2.6-1)
        // Guardian 도발 특성 적용
        if (GuardianTauntApply_On == true)
        {
            GuardianTauntButton.interactable = true;

            // Warrior의 특성 반영 사항을 전달
            InGameTalentsDB.InGameTalents_SingleTon().warriorPlayer.guardianCoreTalents_DB.GuardianCoreTalents_Taunt_On = true;
        }

        // (4.2.2.6-2)
        // Guardian 포효 특성 적용
        if (GuardianRoarApply_On == true)
        {
            GuardianRoarButton.interactable = true;

            // Warrior의 특성 반영 사항을 전달
            InGameTalentsDB.InGameTalents_SingleTon().warriorPlayer.guardianCoreTalents_DB.GuardianCoreTalents_Roar_On = true;
        }

    }

    // Warrior Base 스킬 전부 On Button
    public void OnWarriorBasicTalentsButtonCliked(Button thisButton)
    {
        WarriorBasicTalentsButton = thisButton;
        thisButton.interactable = false;

        // Warrior 기본 특성 반영.
        WarriorBasicTalentsApply_On = true;
    }

    // Guardian KnockBack 스킬 On Button
    public void OnGuardianKnockBackButtonCliked(Button thisButton)
    {
        GuardianKnockBackButton = thisButton;
        thisButton.interactable = false;

        GuardianKnockBackApply_On = true;
    }

    // Guardian Taunt 스킬 On Button
    public void OnGuardianTauntButtonCliked(Button thisButton)
    {
        GuardianTauntButton = thisButton;
        thisButton.interactable = false;

        GuardianTauntApply_On = true;
    }

    // Guardian Roar 스킬 On Button
    public void OnGuardianRoarButtonCliked(Button thisButton)
    {
        GuardianRoarButton = thisButton;
        thisButton.interactable = false;

        GuardianRoarApply_On = true;
    }

}
