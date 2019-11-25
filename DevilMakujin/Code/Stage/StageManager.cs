using DevilMakujin.Code.Entity;
using DevilMakujin.Code.Graphics;
using DevilMakujin.Properties;

namespace DevilMakujin.Code
{
    static public class StageManager
    {
        /// <summary>
        /// Acts mostly as MusicPlayer and MapDraw argument, having zero impact on gameplay.
        /// </summary>
        public enum Stage
        {
            None,
            OuterRing,
            MediumRing,
            InnerRing,
            Center,
        }

        static public Stage CurrentStage { get; private set; }

        static public void SetStartingStage(BattleScreen battle)
        {
            battle.AddEnemies(EnemySet.debugSet1);
            CurrentStage = (Stage)Settings.Default.StartingStage; // Stage.OuterRing;  //Stage.Center; // DEBUG Stage.OuterRing;
            PlayerPhysics.InitPosOnStage(StageManager.CurrentStage, battle.playerDrawOffset);
        }

        static public void NextStage(BattleScreen battle)
        {
            CurrentStage++;
            PlayerEquipInfo.HealBetweenStages();
            PlayerPhysics.InitPosOnStage(StageManager.CurrentStage, battle.playerDrawOffset);

            battle.ClearEntityList();
            AddEnemiesOnCurrentStage(battle);
        }

        public static void AddEnemiesOnCurrentStage(BattleScreen battle)
        {
            if (CurrentStage < Stage.Center)
            {
                switch (CurrentStage)
                {
                    case Stage.MediumRing:
                        battle.AddEnemies(EnemySet.debugSet2);
                        break;
                    case Stage.InnerRing:
                        battle.AddEnemies(EnemySet.debugSet3);
                        break;
                    case Stage.OuterRing:
                        battle.AddEnemies(EnemySet.debugSet1);
                        break;
                }
            }
            else
            {
                battle.AddEnemies(EnemySet.bossSet);
            }
        }

        static public void SetbackStage(BattleScreen battle)
        {
            CurrentStage--;
            NextStage(battle);
        }
    }
}
