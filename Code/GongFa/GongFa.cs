using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DdouPoCangPong.Code.GongFa
{
    public class GongFa
    {
        // 1. 功法等级枚举 (新增超阶)
        public enum GongFaRank
        {
            黄阶,
            玄阶,
            地阶,
            天阶,
            超阶
        }

        public class GongFaData
        {
            // 基础信息
            public string id; // 功法的唯一ID，用于区分不同家族的功法
            public string name;
            public GongFaRank rank = GongFaRank.黄阶;
            public int grade = 9; // 9品最低, 1品最高

            // 修炼进度
            public int ceng = 0;
            public int maxceng = 10;
            public int exp = 0;
            public int max_exp = 100;
            public float cultivation_speed_mod = 1.0f; // 修炼速度加成，初始为1

            // 推演相关
            public float deduction_progress = 0;
            public float max_deduction = 250; // 推演所需总经验

            // 新增：功法解锁的斗技ID列表
            public List<string> granted_skill_ids = new List<string>();

            /// <summary>
            /// 新增：根据功法等阶，获取可容纳的最大斗技数量
            /// </summary>
            public int GetMaxSkillsCapacity()
            {
                switch (rank)
                {
                    case GongFaRank.黄阶: return 1;
                    case GongFaRank.玄阶: return 2;
                    case GongFaRank.地阶: return 3;
                    case GongFaRank.天阶: return 4;
                    case GongFaRank.超阶: return 5;
                    default: return 0;
                }
            }

            /// <summary>
            /// 新增：获取当前功法等阶的突破概率
            /// </summary>
            public float GetBreakthroughChance()
            {
                switch (rank)
                {
                    case GongFaRank.黄阶: return 0.5f; // 50%
                    case GongFaRank.玄阶: return 0.4f; // 40%
                    case GongFaRank.地阶: return 0.3f; // 30%
                    case GongFaRank.天阶: return 0.2f; // 20%
                    case GongFaRank.超阶: return 0.1f; // 10%
                    default: return 0.0f;
                }
            }


            private int _initialWuxingBonus = -1; // 私有字段，用于存储初始随机悟性

            public GongFaData(string pName, bool isNewFamilyGongFa = false)
            {
                this.name = pName;
                if (isNewFamilyGongFa)
                {
                    // 为新的家族功法创建一个唯一ID
                    this.id = Guid.NewGuid().ToString();
                }
            }

            /// <summary>
            /// 获取角色的初始随机悟性值，只在第一次获取时随机生成
            /// </summary>
            public int GetInitialWuxingBonus(Actor actor)
            {
                if (_initialWuxingBonus == -1)
                {
                    _initialWuxingBonus = Randy.randomInt(0, 100);
                }
                return _initialWuxingBonus;
            }

            /// <summary>
            /// 创建当前功法数据的一个浅拷贝副本。用于继承。
            /// </summary>
            public GongFaData Clone()
            {
                return (GongFaData)this.MemberwiseClone();
            }
        }
    }
}
