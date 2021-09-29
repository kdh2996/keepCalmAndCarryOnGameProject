using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MageNormalTalentsController : TalentsController
{

    /* 특성 관련 적용 변수 */

    // Mage
    Button MageBasicTalentsButton;
    bool MageBasicTalentsApply_On = false;

    // Mage - SpellCaster
    Button SpellCasterRuneCastingButton;
    bool SpellCasterRuneCastingApply_On;

    Button SpellCasterBlinkButton;
    bool SpellCasterBlinkApply_On;


    /* 씬 변환 메소드들 */

    public void OnMageExpertButtonClicked()
    {
        //Application.LoadLevel("Game_Modeling");

        // 1초후 전문가 특성 씬을 불러옴.
        Invoke("LoadMageExpertTalentsScene", 1.0f);
    }

    // 메이지의 전문가 특성 씬을 불러오는 메소드
    public void LoadMageExpertTalentsScene()
    {
        SceneManager.LoadScene("Mage_ExpertTalent_SpellCaster");
    }


    /* 특성 적용 메소드들 */
    // 해당 메소드들을 통해 변환된 값은, 
    // 다음 씬에서도 유지되어 변경사항이 반영된다.

    // 최종적으로 특성 적용 메소드들을 모두 모아, 
    // 다음 씬에 반영 하도록 하는 메소드
    public void OnApplyButtonClicked()
    {
        // Mage 기본 베이스 특성 적용
        if (MageBasicTalentsApply_On == true)
        {
            MageBasicTalentsButton.interactable = true;

            // Mage의 특성 반영 사항을 전달
            InGameTalentsDB.InGameTalents_SingleTon().magePlayer.mageTalents_DB.MageBaseTalents_On = true;
        }

        // (5.2.1.6)
        // SpellCaster 룬 캐스팅 특성 적용
        if (SpellCasterRuneCastingApply_On == true)
        {
            SpellCasterRuneCastingButton.interactable = true;

            // Mage의 특성 반영 사항을 전달
            InGameTalentsDB.InGameTalents_SingleTon().magePlayer.spellCasterCoreTalents_DB.SpellCasterCoreTalents_RuneCasiting_On = true;
        }

        // (5.2.1.6.1)
        // SpellCaster 순간 이동 특성 적용
        if (SpellCasterBlinkApply_On == true)
        {
            SpellCasterBlinkButton.interactable = true;

            // Mage의 특성 반영 사항을 전달
            InGameTalentsDB.InGameTalents_SingleTon().magePlayer.spellCasterCoreTalents_DB.SpellCasterCoreTalents_Blink_On = true;
        }

    }

    // Mage Base 스킬 전부 On Button
    public void OnMageBasicTalentsButtonCliked(Button thisButton)
    {
        MageBasicTalentsButton = thisButton;
        thisButton.interactable = false;

        // Mage 기본 특성 반영.
        MageBasicTalentsApply_On = true;
    }

    // SpellCaster Rune Casting 스킬 On Button
    public void OnSpellCasterRuneCastingButtonCliked(Button thisButton)
    {
        SpellCasterRuneCastingButton = thisButton;
        thisButton.interactable = false;

        SpellCasterRuneCastingApply_On = true;
    }

    // SpellCaster Blink 스킬 On Button
    public void OnSpellCasterBlinkButtonCliked(Button thisButton)
    {
        SpellCasterBlinkButton = thisButton;
        thisButton.interactable = false;

        SpellCasterBlinkApply_On = true;
    }

}

