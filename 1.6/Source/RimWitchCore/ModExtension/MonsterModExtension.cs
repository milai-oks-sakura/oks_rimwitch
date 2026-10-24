using Verse;

namespace oks
{
    /// <summary>
    ///  怪物类型
    /// </summary>
    public enum MonsterType
    {   
        BEAST, ///< 野兽
        CuRsedOne, ///< 受诅咒者
        INSECTOID, ///< 类虫生物
        OGROID, ///< 食人魔类
        SPECTER, ///< 恶灵
        RELICT, ///< 残物
        HYBRID, ///< 混合兽
        VAMPIRE, ///< 吸血鬼
        DRACONID, ///< 龙类
        UNKNOWN, ///< 未知
    }
    /// <summary>
    /// 怪物相关拓展
    /// </summary>
    public class MonsterModExtension : DefModExtension
    {
        public MonsterType monsterType;
    };
}
