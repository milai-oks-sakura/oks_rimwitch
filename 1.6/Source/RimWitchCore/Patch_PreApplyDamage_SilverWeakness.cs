using HarmonyLib;
using RimWorld;
using Verse;



namespace oks
{

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.PreApplyDamage))]
    public class Patch_PreApplyDamage_SilverWeakness
    {
        static private float WEAKNESS = 0.1f;
        static bool Prefix(Pawn __instance, ref DamageInfo dinfo, ref bool absorbed)
        {
            // 1. 检查被攻击者是否是模组怪物
            if (!__instance.def.HasModExtension<MonsterModExtension>())
            {
                return true;
            }

            Pawn attacker = dinfo.Instigator as Pawn;

            if (attacker == null)
            {
                return true;
            }

            // 2. 检查攻击者手中的武器
            ThingWithComps weapon = attacker.equipment?.Primary;
            if (weapon == null)
            {
                // 空手/牙齿爪子：视为非银剑
                dinfo.SetAmount(dinfo.Amount * 0.1f);
                return true;
            }

            bool isSilverSwordExtension = weapon.def.HasModExtension<SilverSwordModExtension>();
            bool isSilverSwordOrigin = weapon.Stuff?.defName == "Silver";
            

            // 4. 非银剑：伤害 × 0.1
            if (!(isSilverSwordExtension || isSilverSwordOrigin))
            {
                dinfo.SetAmount(dinfo.Amount * WEAKNESS);
            }

            return true; // 继续走原版伤害流程（护甲、部位分配等）
        }
    }
}
