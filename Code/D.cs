using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdouPoCangPong.Code
{
    public class D
    {
        /// <summary>
        /// 每个小境界增幅
        /// </summary>
        public static List<float> additions = new List<float>()
        {
            0.01f,  // 1.00%
            0.02f,  // 2.00%
            0.05f,  // 5.00%
            0.1f,   // 10.00%
            0.1f,   // 10.00%
            0.15f,  // 15.00%
            0.2f,   // 20.00%
            0.25f,  // 25.00%
            0.5f,   // 50.00%
            0.8f,   // 80.00%
            1.0f    // 100.00%
        };

        public static List<float> chaces = new List<float>()
        {
            1f,1f,1f,9.95f,0.9f,0.8f,0.75f,0.7f,0.6f,0.5f,0.25f
        };

        public static List<int> Ages = new List<int>()
        {
        0,
        0,
        20,
        40,
        60,
        80,
        100,
        150,
        250,
        1200,
        8000
        };

    }
}
