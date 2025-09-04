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
