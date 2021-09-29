using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameTalentsDB : MonoBehaviour {

    // 씬 이동 간에도 파괴되지 않는 SingleTon 생성
    public static InGameTalentsDB SingleTon_Talents = null;


    public static InGameTalentsDB InGameTalents_SingleTon()
    {
        if(!SingleTon_Talents)
        {
            GameObject tempSingleTon = new GameObject();

            // 싱글톤 인스턴스 생성.
            SingleTon_Talents = tempSingleTon.AddComponent<InGameTalentsDB>();
            SingleTon_Talents.name = typeof(InGameTalentsDB).ToString();

            DontDestroyOnLoad(SingleTon_Talents);
        }

        return SingleTon_Talents;
    }

    // 현재 플레이어의 데이터
    public CurrentPlayer curPlayer = new CurrentPlayer();


    // 아처로 플레이할 경우의 데이터
    public ArcherPlayer archerPlayer = new ArcherPlayer();

    // 워리어로 플레이할 경우의 데이터
    public WarriorPlayer warriorPlayer = new WarriorPlayer();

    // 메이지로 플레이할 경우의 데이터
    public MagePlayer magePlayer = new MagePlayer();


    // 현재 플레이어의 특징을 판별하는 변수
    public bool isArcher = false;
    public bool isWarrior = false;
    public bool isMage = false;


    // 현재 플레이어의 데이터를 특정 직업군 데이터로 변경.
    public static void ChagneCurPlayerToArcher()
    {
        SingleTon_Talents.curPlayer.curArcherPlayer = SingleTon_Talents.archerPlayer;
        SingleTon_Talents.curPlayer.curWarriorPlayer = new WarriorPlayer();
        SingleTon_Talents.curPlayer.curMagePlayer = new MagePlayer();
    }

    public static void ChagneCurPlayerToWarrior()
    {
        SingleTon_Talents.curPlayer.curArcherPlayer = new ArcherPlayer();
        SingleTon_Talents.curPlayer.curWarriorPlayer = SingleTon_Talents.warriorPlayer;
        SingleTon_Talents.curPlayer.curMagePlayer = new MagePlayer();
    }

    public static void ChagneCurPlayerToMage()
    {
        SingleTon_Talents.curPlayer.curArcherPlayer = new ArcherPlayer();
        SingleTon_Talents.curPlayer.curWarriorPlayer = new WarriorPlayer();
        SingleTon_Talents.curPlayer.curMagePlayer = SingleTon_Talents.magePlayer;
    }


    public static void AllResetData_SingleTone()
    {
        if(SingleTon_Talents == null)
        {
            InGameTalents_SingleTon();
        }
        else
        {
            SingleTon_Talents.curPlayer.curArcherPlayer = new ArcherPlayer();
            SingleTon_Talents.curPlayer.curWarriorPlayer = new WarriorPlayer();
            SingleTon_Talents.curPlayer.curMagePlayer = new MagePlayer();

            /*
            SingleTon_Talents.curPlayer.archerTalents_DB = new Archer();
            SingleTon_Talents.curPlayer.engineerStyleTalents_DB = new EngineerStyleTalents();
            SingleTon_Talents.curPlayer.engineerCoreTalents_DB = new EngineerCoreTalents();
            SingleTon_Talents.curPlayer.engineerExpertTalents_DB = new EngineerExpertTalents();

            SingleTon_Talents.curPlayer.warriorTalents_DB = new Warrior();
            SingleTon_Talents.curPlayer.guardianStyleTalents_DB = new GuardianStyleTalents();
            SingleTon_Talents.curPlayer.guardianCoreTalents_DB = new GuardianCoreTalents();
            SingleTon_Talents.curPlayer.guardianExpertTalents_DB = new GuardianExpertTalents();

            SingleTon_Talents.curPlayer.mageTalents_DB = new Mage();
            SingleTon_Talents.curPlayer.spellCasterStyleTalents_DB = new SpellCasterStyleTalents();
            SingleTon_Talents.curPlayer.spellCasterCoreTalents_DB = new SpellCasterCoreTalents();
            SingleTon_Talents.curPlayer.spellCasterExpertTalents_DB = new SpellCasterExpertTalents();
            */
        }
    }


    [System.Serializable]
    public class CurrentPlayer
    {

        // Archer 계열 특성 DB
        public ArcherPlayer curArcherPlayer = new ArcherPlayer();

        // Warrior 계열 특성 DB
        public WarriorPlayer curWarriorPlayer = new WarriorPlayer();

        // Mage 계열 특성 DB
        public MagePlayer curMagePlayer = new MagePlayer();

    }

    [System.Serializable]
    public class ArcherPlayer
    {
        // Archer 계열 특성 DB
        public Archer archerTalents_DB = new Archer();

        public EngineerStyleTalents engineerStyleTalents_DB = new EngineerStyleTalents();
        public EngineerCoreTalents engineerCoreTalents_DB = new EngineerCoreTalents();
        public EngineerExpertTalents engineerExpertTalents_DB = new EngineerExpertTalents();

        public ScoutStyleTalents scoutStyleTalents_DB = new ScoutStyleTalents();
    }

    [System.Serializable]
    public class WarriorPlayer
    {
        // Warrior 계열 특성 DB

        public Warrior warriorTalents_DB = new Warrior();

        public GuardianStyleTalents guardianStyleTalents_DB = new GuardianStyleTalents();
        public GuardianCoreTalents guardianCoreTalents_DB = new GuardianCoreTalents();
        public GuardianExpertTalents guardianExpertTalents_DB = new GuardianExpertTalents();
    }

    [System.Serializable]
    public class MagePlayer
    {
        // Mage 계열 특성 DB

        public Mage mageTalents_DB = new Mage();

        public SpellCasterStyleTalents spellCasterStyleTalents_DB = new SpellCasterStyleTalents();
        public SpellCasterCoreTalents spellCasterCoreTalents_DB = new SpellCasterCoreTalents();
        public SpellCasterExpertTalents spellCasterExpertTalents_DB = new SpellCasterExpertTalents();
    }


}
