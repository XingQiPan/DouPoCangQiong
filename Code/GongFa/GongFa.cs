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
        // 1. 功法等级枚举
        public enum GongFaRank
        {
            黄阶, // 黄
            玄阶,  // 玄
            地阶,    // 地
            天阶   // 天
        }

        public class GongFaData
        {
            public string name;
            public GongFaRank rank = GongFaRank.黄阶;
            public int grade = 9; // 9品最低, 1品最高
            public int ceng = 0; //功法层数，最低一层最高十层
            public int maxceng = 10;
            public int exp = 0; //修炼层数经验
            public int max_exp = 100;
            public float deduction = 0; //推演品阶，如果当前已经10层圆满，则要推演，推演成功后层数+1
            public float max_deduction = 1;
            public int curr_skills = 0;
            public int max_skills = 0;
            public float mod_talent = 0.01f; //提升层数则会
            public GongFaData(string name)
            {
                this.name = name;
            }
            
        }

    }
}
