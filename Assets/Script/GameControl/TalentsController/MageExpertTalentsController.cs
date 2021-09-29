using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MageExpertTalentsController : TalentsController
{

    /* 특성 관련 적용 변수 */

    // Mage - SpellCaster
    Button SpellCasterCoreMedium;
    bool SpellCasterCoreMediumApply_On = false;

    Button SpellCasterTornadoBlazeButton;
    bool SpellCasterTornadoBlazeApply_On = false;


    /* 씬 이동 관련 메소드들 */

    public void OnMageNormalButtonClicked()
    {
        //Application.LoadLevel("Game_Modeling");

        // 1초후 노말 특성 씬을 불러옴.
        Invoke("LoadMageNormalTalentsScene", 1.0f);
    }

    // 메이지의 노말 특성 씬을 불러오는 메소드
    public void LoadMageNormalTalentsScene()
    {
        SceneManager.LoadScene("Mage_NormalTalents");
    }


    /* 특성 적용 메소드들 */
    // 해당 메소드들을 통해 변환된 값은, 
    // 다음 씬에서도 유지되어 변경사항이 반영된다.

    // 최종적으로 특성 적용 메소드들을 모두 모아, 
    // 다음 씬에 반영 하도록 하는 메소드
    public void OnApplyButtonClicked()
    {
        // (5.2.1.6C.1)
        // SpellCaster 핵심 매개체 특성 적용
        if (SpellCasterCoreMediumApply_On == true)
        {
            SpellCasterCoreMedium.interactable = true;

            // Mage의 특성 반영 사항을 전달
            InGameTalentsDB.InGameTalents_SingleTon().magePlayer.spellCasterExpertTalents_DB.SpellCasterExpertTalents_CoreMedium_On = true;
        }

        // (5.2.1.6A.2)
        // SpellCaster 회오리 불꽃 특성 적용
        if (SpellCasterTornadoBlazeApply_On == true)
        {
            SpellCasterTornadoBlazeButton.interactable = true;

            // Mage의 특성 반영 사항을 전달
            InGameTalentsDB.InGameTalents_SingleTon().magePlayer.spellCasterExpertTalents_DB.SpellCasterExpertTalents_TornadoBlaze_On = true;
        }

    }

    // SpellCaster CoreMedium 스킬 On Button
    public void OnSpellCasterCoreMediumButtonCliked(Button thisButton)
    {
        SpellCasterCoreMedium = thisButton;
        thisButton.interactable = false;

        SpellCasterCoreMediumApply_On = true;
    }

    // SpellCaster Tornado Blaze 스킬 On Button
    public void OnSpellCasterTornadoBlazeButtonCliked(Button thisButton)
    {
        SpellCasterTornadoBlazeButton = thisButton;
        thisButton.interactable = false;

        SpellCasterTornadoBlazeApply_On = true;
    }

}

