using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DevilMakujin.Code.Entity
{
    static public class EnemySet
    {
        static public List<GenericEnemy> debugSet1;
        public static List<GenericEnemy> debugSet2;
        public static List<GenericEnemy> debugSet3;
        public static List<GenericEnemy> bossSet;

        static public void InitEnemyListings(DevimakuGame game)
        {
            debugSet1 = new List<GenericEnemy>
            {
                new ShipRocketeer(Vector2.Zero + new Vector2(0, -100*10)),
                new ShipRocketeer(Vector2.Zero + new Vector2(-100*10, -100)),
                new ShipGunner(Vector2.Zero + new Vector2(0, 100*10)),
                new ShipGunner(Vector2.Zero + new Vector2(100*10, 100)),
            };

            debugSet2 = new List<GenericEnemy>
            {
                new ShipGunner(Vector2.Zero + new Vector2(0, -100*10)),
                new ShipGunner(Vector2.Zero + new Vector2(-100*10, -100)),
                new ShipGunner(Vector2.Zero + new Vector2(0, 100*10)),
                new ShipGunner(Vector2.Zero + new Vector2(100*10, 100)),

                new ShipGunner(Vector2.Zero + new Vector2(0, -100*15)),
                new ShipGunner(Vector2.Zero + new Vector2(-100*15, -100)),
                new ShipGunner(Vector2.Zero + new Vector2(0, 100*15)),
                new ShipGunner(Vector2.Zero + new Vector2(100*15, 100)),
            };

            debugSet3 = new List<GenericEnemy>
            {
                new ShipGunner(Vector2.Zero + new Vector2(0, -100*10)),
                new ShipGunner(Vector2.Zero + new Vector2(-100*10, -100)),
                new ShipGunner(Vector2.Zero + new Vector2(0, 100*10)),
                new ShipGunner(Vector2.Zero + new Vector2(100*10, 100)),

                new ShipGunner(Vector2.Zero + new Vector2(0, -100*15)),
                new ShipGunner(Vector2.Zero + new Vector2(-100*15, -100)),
                new ShipGunner(Vector2.Zero + new Vector2(0, 100*15)),
                new ShipGunner(Vector2.Zero + new Vector2(100*15, 100)),

                new ShipGunner(Vector2.Zero + new Vector2(0, -100*20)),
                new ShipGunner(Vector2.Zero + new Vector2(-100*20, -100)),
                new ShipGunner(Vector2.Zero + new Vector2(0, 100*20)),
                new ShipGunner(Vector2.Zero + new Vector2(100*20, 100)),
            };

            bossSet = new List<GenericEnemy> //Or boss entity for sweet functions? or foreach will cover that if we foreach(Boss... .where(... is Boss))?
            {
                new BossEntity(game, Vector2.Zero),
            };
        }
    }
}
