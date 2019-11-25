using static DevilMakujin.Code.Entity.Bullet;
using static DevilMakujin.Code.Entity.PlayerEquipInfo;

namespace DevilMakujin.Code.Entity
{
    /// <summary>
    /// NOT an inventory holder, just a comparison method bundle.
    /// </summary>
    static public class InvUtils
    {
        public const int maxSpace = 9;

        //todo you're forgetting about possibility of several items together
        //kinda make a list of tuples type+space, and if space>9 then can't take more

        /// <summary>
        /// Internal only. For self use, for drawings use EquipInfos.
        /// </summary>
        private static int GetCurrentLength(BulletType gun, ShieldType shield)
        {
            return PlayerEquipInfo.GetShieldSpace(shield) + PlayerEquipInfo.GetGunSpace(gun);
        }

        public static bool CanEquipTogether(BulletType gun, ShieldType shield)
        {
            return GetCurrentLength(gun, shield) <= maxSpace;
        }
    }
}
