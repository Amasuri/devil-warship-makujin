using DevilMakujin.Code.Graphics;
using DevilMakujin.Code.Music;

namespace DevilMakujin.Code.Entity
{
    /// <summary>
    /// Will be scrapped into player class (together with PlayerSprite variables)
    /// </summary>
    public static class PlayerEquipInfo
    {
        public static Bullet.BulletType gun = Bullet.BulletType.Blaster;
        public static ShieldType shield = ShieldType.None;

        public const int maxHits = 3;
        public static int Hits { get; private set; } = maxHits;

        /// <summary>
        /// <para>
        /// Щиты:
        /// 1. Окружает весь корабль силовым полем, держит всего пару-тройку выстрелов, но через несколько секунд регенерируется.
        /// 2. Находится спереди корабля, держит гораздо больше выстрелов, но регенерируется медленнее.Как в Gradius.
        /// 3. Шар, который вращается вокруг корабля и блокирует встретившиеся вражеские выстрелы выстрелы/наносит повреждения ближайшим врагам. В Aleste было что-то такое.
        /// </summary>
        public enum ShieldType
        {
            None,
            Field,
            StrongForward,
            Rotating
        }

        public static float GetEquippedGunSpeed()
        {
            switch (gun)
            {
                //For comparison: player MaxSpeed is 20.0f
                case Bullet.BulletType.Blaster:
                    return 30.0f;

                case Bullet.BulletType.SpeedBlaster:
                    return 45.0f;

                case Bullet.BulletType.LongBlaster:
                    return 60.0f;

                default:
                    return 30.0f;
            }
        }

        /// <summary>
        /// Isn't only for getting current gun usage, but also for checking info on gun placement dialogue.
        /// Hence it's in info: it's more about getting information on any gun type, not on current one.
        /// </summary>
        public static int GetGunSpace(Bullet.BulletType type)
        {
            return (int)type;
        }

        /// <summary>
        /// Isn't only for getting current shield usage, but also for checking info on shield placement dialogue.
        /// Hence it's in info: it's more about getting information on any shield type, not on current one.
        /// </summary>
        public static int GetShieldSpace(ShieldType type)
        {
            return (type == ShieldType.None) ? 0 : 2;
        }

        public static int GetShootTime(Bullet.BulletType type)
        {
            if (type != Bullet.BulletType.Blaster)
                return 5;
            else
                return 10;
        }

        /// <summary>
        /// Bug debug update todo this is ugly; place player info somewhere in separate class
        /// </summary>
        public static bool HitAndCheckDead(PlayerSprite player, Music.MusicPlayer music)
        {
            //If cheating
            if(player.isCheatInvisibilityActive)
            {
                return Hits <= 0;
            }

            //Recharge shield and don't change dmg
            if(shield != ShieldType.None && player.IsShieldActive)
            {
                player.SetShieldRecharge();
                player.SetInvisibility();
                music.PlaySound(Music.SoundEvent.ShieldGone);
                return Hits <= 0;
            }

            //Dmg is here
            if (!player.IsInvisible())
            {
                Hits--;
                player.SetInvisibility();
            }

            //Play different sound on damage/death
            if (Hits > 0)
                music.PlaySound(SoundEvent.PlayerDamage);
            else
                music.PlaySound(SoundEvent.PlayerDeath);

            return Hits <= 0;
        }

        public static void HealBetweenStages()
        {
            Hits = maxHits;
        }
    }
}
