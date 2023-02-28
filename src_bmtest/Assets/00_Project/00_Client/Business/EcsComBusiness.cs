using System;

namespace Client
{
    [System.Serializable]
    struct EcsComBusiness
    {
        public int ID; //For save/read data
        public string BusinessName;
        public float BusinessProgressEarning; //0F...1F
        public int Level;

        //Upgrade 1
        public bool IsUpgrade1Applyed;
        public string UpgradeFirstName;
        public int UpgradeFirstEarn�ultiplier; // 100% , 200% 
        public int UpgradeFirstPrice;

        //Upgrade 2
        public bool IsUpgrade2Applyed;
        public string UpgradeSecondName;
        public int UpgradeSecondEarn�ultiplier; // 100% , 200%
        public int UpgradeSecondPrice;

        public ViewUiOneBusiness UiView;

        //������� ���������
        public int BasePrice;

        //��������� c��������� ����� ��� 
        public int CurrentLevelUpPrice;


        //������� �����
        public int EarnBaseVal;
        //�����
        public int EarnVal;
        //���t���� ������
        public float EarnDelay;
        public float CurrentTimer;
    }
}