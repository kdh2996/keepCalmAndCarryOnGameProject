using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Player curPlayer_Object;

    public Player acherPlayer_Object;
    public Player warriorPlayer_Object;
    public Player magePlayer_Object;

    /* 플레이어 킬 관련 변수들 */
    public int totPlayerKillCount = 0;
    public int playerKillCount = 0;



    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(playerKillCount > 0)
        {
            playerKillCount = 0;
            totPlayerKillCount++;

            print("==Player Kills Enemy ==");
            print("** Total Kill Count : " + totPlayerKillCount + " **");

            // 분노 관련 처리
            if (curPlayer_Object.rage_On == true && totPlayerKillCount % 3 == 0)
            {
                curPlayer_Object.Rage_StackCount_Inc();
            }

            // 위압 관련 처리
            if (curPlayer_Object.coercion_On == true)
            {
                curPlayer_Object.Coercion_StackCount_Inc();
            }
        }
    }


    // 플레이어의 킬 수를 올려주는 메소드
    public void KillCount_Inc()
    {
        playerKillCount++;
    }

    public void OnArcherButtonClicked()
    {
        if(InGameTalentsDB.InGameTalents_SingleTon().archerPlayer.archerTalents_DB.ArcherBaseTalents_On == false)
        {
            // Archer의 기본 특성을 자동으로 반영.
            InGameTalentsDB.InGameTalents_SingleTon().archerPlayer.archerTalents_DB.ArcherBaseTalents_On = true;
        }

        InGameTalentsDB.ChagneCurPlayerToArcher();

        curPlayer_Object.CallSetTalentsScenseDB();

        // Archer로 변함에 따라 호출되는 메소드
        curPlayer_Object.CallArcherTranfer_Update();

        // 솔선 스택에 따른 처리를 위해 호출되는 메소드
        curPlayer_Object.CallLeadStackTranfer_Update();

        curPlayer_Object.isArcher = true;
        curPlayer_Object.isWarrior = false;
        curPlayer_Object.isMage = false;

    }

    public void OnWarriorButtonClicked()
    {
        if(InGameTalentsDB.InGameTalents_SingleTon().warriorPlayer.warriorTalents_DB.WarriorBaseTalents_On == false)
        {
            // Warrior의 기본 특성을 자동으로 반영.
            InGameTalentsDB.InGameTalents_SingleTon().warriorPlayer.warriorTalents_DB.WarriorBaseTalents_On = true;
        }

        InGameTalentsDB.ChagneCurPlayerToWarrior();

        curPlayer_Object.CallSetTalentsScenseDB();

        curPlayer_Object.isArcher = false;
        curPlayer_Object.isWarrior = true;
        curPlayer_Object.isMage = false;
    }

    public void OnMageButtonClicked()
    {
        if(InGameTalentsDB.InGameTalents_SingleTon().magePlayer.mageTalents_DB.MageBaseTalents_On == false)
        {
            // Mage의 기본 특성을 자동으로 반영.
            InGameTalentsDB.InGameTalents_SingleTon().magePlayer.mageTalents_DB.MageBaseTalents_On = true;
        }

        InGameTalentsDB.ChagneCurPlayerToMage();

        curPlayer_Object.CallSetTalentsScenseDB();

        // 솔선 스택에 따른 처리를 위해 호출되는 메소드
        curPlayer_Object.CallLeadStackTranfer_Update();

        curPlayer_Object.isArcher = false;
        curPlayer_Object.isWarrior = false;
        curPlayer_Object.isMage = true;
    }

}
