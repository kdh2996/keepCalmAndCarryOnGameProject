using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.UI;

public class PlayerThirdPersonInputMobile : MonoBehaviour {

    protected ThirdPersonUserControl userControl;
    protected Player player;

    public FixedJoystick leftJoyStick;

    public FixedButton MeleeAtkButton;
    public FixedButton RangedButton;
    public FixedButton ReloadAtkButton;

    public FixedButton RollButton;

    public FixedButton SpellButton;
    public FixedButton ShieldButton;

    public FixedButton TornadoBlazeButton;
    public FixedButton KnockBackButton;
    public FixedButton TauntButton;
    public FixedButton RoarButton;
    public FixedButton HeavySmashButton;

    public FixedButton RushButton;

    public Text SpellShieldLabel;
    public bool textinit = true;

    void Start ()
    {
        userControl = GetComponent<ThirdPersonUserControl>();
        player = GetComponent<Player>();

        player.isMobileInput = true;
    }
	
	void Update ()
    {
        userControl.mobileInput_h = leftJoyStick.inputVector.x;
        userControl.mobileInput_v = leftJoyStick.inputVector.y;

        if(player.playerWarriorController.isDefense_On == true && textinit == true)
        {
            textinit = false;

            SpellShieldLabel.text = "Shield";
        }


        /* 오른쪽 모바일 버튼 제어 */

        // 근거리 무기 버튼
        if(MeleeAtkButton.Pressed == true)
        {
            player.isPressedMeleeAtkButton = true;
        }
        else
        {
            player.isPressedMeleeAtkButton = false;
        }

        // 원거리 무기 버튼
        if (RangedButton.Pressed == true)
        {
            player.isPressedRangedAtkButton = true;
        }
        else
        {
            player.isPressedRangedAtkButton = false;
        }

        // 재장전 버튼
        if (ReloadAtkButton.Pressed == true)
        {
            player.isPressedReloadButton = true;
        }
        else
        {
            player.isPressedReloadButton = false;
        }

        // 구르기 & 순간이동 버튼
        if (RollButton.Pressed == true)
        {
            player.isPressedRollButton = true;
        }
        else
        {
            player.isPressedRollButton = false;
        }

        // 스펠 버튼
        if (SpellButton.Pressed == true)
        {
            player.isPressedSpellButton = true;
        }
        else
        {
            player.isPressedSpellButton = false;
        }

        // 방패 버튼
        if (ShieldButton.Pressed == true)
        {
            player.isPressedShieldButton = true;
        }
        else
        {
            player.isPressedShieldButton = false;
        }

        // 회오리 불꽃 버튼
        if (TornadoBlazeButton.Pressed == true)
        {
            player.isPressedTornadoBlazeButton = true;
        }
        else
        {
            player.isPressedTornadoBlazeButton = false;
        }


        // 스킬 버튼 제어

        // 넉백 버튼
        if (KnockBackButton.Pressed == true)
        {
            player.isPressedKnockBackButton = true;
        }
        else
        {
            player.isPressedKnockBackButton = false;
        }

        // 도발 버튼
        if (TauntButton.Pressed == true)
        {
            player.isPressedTauntButton = true;
        }
        else
        {
            player.isPressedTauntButton = false;
        }

        // 포효 버튼
        if (RoarButton.Pressed == true)
        {
            player.isPressedRoarButton = true;
        }
        else
        {
            player.isPressedRoarButton = false;
        }

        // 육중한 강타 버튼
        if (HeavySmashButton.Pressed == true)
        {
            player.isPressedHeavySmashButton = true;
        }
        else
        {
            player.isPressedHeavySmashButton = false;
        }

        // 주문불꽃 UI 이동에 대해 특수 처리
        if(player.playerMageController.keyDownForSpellBlaze == true)
        {
            player.mobile_h = leftJoyStick.inputVector.x;
            player.mobile_v = leftJoyStick.inputVector.y;
        }

        if(RushButton.Pressed == true)
        {
            player.isPressedRushButton = true;
        }
        else
        {
            player.isPressedRushButton = false;
        }

    }

}
