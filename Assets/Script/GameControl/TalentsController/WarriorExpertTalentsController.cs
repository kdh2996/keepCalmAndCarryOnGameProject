using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WarriorExpertTalentsController : TalentsController
{

    /* 특성 관련 적용 변수 */

    // Warrior - Guardian

    Button GuardianHammerSecuritiesButton;
    bool GuardianHammerSecurities_On = false;

    Button GuardianHeavySmashButton;
    bool GuardianHeavySmash_On = false;


    /* 씬 이동 관련 메소드들 */

    public void OnWarriorNormalButtonClicked()
    {
        //Application.LoadLevel("Game_Modeling");

        // 1초후 노말 특성 씬을 불러옴.
        Invoke("LoadWarriorNormalTalentsScene", 1.0f);
    }

    // 워리어의 노말 특성 씬을 불러오는 메소드
    public void LoadWarriorNormalTalentsScene()
    {
        SceneManager.LoadScene("Warrior_NormalTalents");
    }


    /* 특성 적용 메소드들 */
    // 해당 메소드들을 통해 변환된 값은, 
    // 다음 씬에서도 유지되어 변경사항이 반영된다.

    // 최종적으로 특성 적용 메소드들을 모두 모아, 
    // 다음 씬에 반영 하도록 하는 메소드
    public void OnApplyButtonClicked()
    {
        // (4.2.2.6A.1)
        // Guardian 망치 경호대 특성 적용
        if (GuardianHammerSecurities_On == true)
        {
            GuardianHammerSecuritiesButton.interactable = true;

            // Warrior의 특성 반영 사항을 전달
            InGameTalentsDB.InGameTalents_SingleTon().warriorPlayer.guardianExpertTalents_DB.GuardianExpertTalents_HammerSecurities_On = true;
        }

        // (4.2.2.6A.3)
        // Guardian 육중한 강타 특성 적용
        if (GuardianHeavySmash_On == true)
        {
            GuardianHeavySmashButton.interactable = true;

            // Warrior의 특성 반영 사항을 전달
            InGameTalentsDB.InGameTalents_SingleTon().warriorPlayer.guardianExpertTalents_DB.GuardianExpertTalents_HeavySmash_On = true;
        }
    }

    // Guardian Hammer Securities 스킬 On Button
    public void OnGuardianHammerSecuritiesButtonCliked(Button thisButton)
    {
        GuardianHammerSecuritiesButton = thisButton;
        thisButton.interactable = false;

        GuardianHammerSecurities_On = true;
    }

    // Guardian Heavy Smash 스킬 On Button
    public void OnGuardianHeavySmashButtonCliked(Button thisButton)
    {
        GuardianHeavySmashButton = thisButton;
        thisButton.interactable = false;

        GuardianHeavySmash_On = true;
    }

}
