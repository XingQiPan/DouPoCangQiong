using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdouPoCangPong.Code.Skills
{
    /// <summary>
    /// 代表一个角色已学会的技能实例，包含熟练度等个人化数据
    /// </summary>
    public class LearnedSkill
    {
        public string id; // 技能ID, 对应 SkillAsset
        public GongFa.GongFa.GongFaRank quality; // 品质，由授予它的功法决定
        public int grade = 9; // 等级, 9品最低, 1品最高

        public int proficiency = 0; // 当前熟练度
        public int max_proficiency = 100; // 升级所需熟练度

        public LearnedSkill(string pId, GongFa.GongFa.GongFaRank pQuality)
        {
            this.id = pId;
            this.quality = pQuality;
        }

        public SkillAsset GetAsset()
        {
            return SkillsLibrary.get(id);
        }

        /// <summary>
        /// 增加熟练度并处理升级
        /// </summary>
        public void AddProficiency(int amount)
        {
            if (grade == 1) return; // 满级后不再增加

            proficiency += amount;
            while (proficiency >= max_proficiency && grade > 1)
            {
                grade--; // 品级提升 (数值减小)
                proficiency -= max_proficiency;
                // 升级后，大幅增加最大熟练度需求
                max_proficiency = (int)(max_proficiency * 1.8f);
            }
        }
    }
}