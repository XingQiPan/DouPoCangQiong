using NeoModLoader.api;
using NeoModLoader.api.attributes;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using DdouPoCangPong.Code.Abstract;
using DdouPoCangPong.Code.UI.UIPerfeb;
using DdouPoCangPong.Code.Tool;

namespace DdouPoCangPong.Code.UI
{
    public class WindowCultiStats : AbstractWindow<WindowCultiStats>
    {
        protected override void Init()
        {
            var vertical_layout_group = ContentTransform.gameObject.AddComponent<VerticalLayoutGroup>();
            vertical_layout_group.childAlignment = TextAnchor.UpperCenter;
            vertical_layout_group.childControlHeight = true;
            vertical_layout_group.childControlWidth = true;
            vertical_layout_group.childForceExpandHeight = false;
            vertical_layout_group.childForceExpandWidth = true;
            vertical_layout_group.padding = new RectOffset(20, 20, 10, 10);

            var fitter = ContentTransform.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            BackgroundTransform.Find("Scroll View").GetComponent<RectTransform>().sizeDelta = new(200, 250f);
           

            _line_pool = new MonoObjPool<LineTitleValue>(LineTitleValue.Prefab, ContentTransform, x =>
            {
                x.Title.resizeTextForBestFit = false;
                x.Title.fontSize = 8;
                x.Value.resizeTextForBestFit = false;
                x.Value.fontSize = 8;
            });
        }

        private MonoObjPool<LineTitleValue> _line_pool;

        private void AddLineValue(string title, int count)
        {
            var line = _line_pool.GetNext();
            line.Setup(title, count.ToString());
        }

        private int GetRealmIndex(int level)
        {
            if (level < 0) return -1;
            if (level <= 8) return 0;  // 斗之气
            if (level <= 18) return 1; // 斗者
            if (level <= 28) return 2; // 斗师
            if (level <= 38) return 3; // 大斗师
            if (level <= 48) return 4; // 斗灵
            if (level <= 58) return 5; // 斗王
            if (level <= 68) return 6; // 斗皇
            if (level <= 78) return 7; // 斗宗
            if (level <= 88) return 8; // 斗尊
            if (level <= 98) return 9; // 斗圣
            return 10; // 斗帝 (99)
        }

        [Hotfixable]
        public override void OnNormalEnable()
        {
            _line_pool.Clear();

            // 获取所有生物
            var all_actors = World.world.units.getSimpleList();

            // 统计各个境界的人数
            int[] counts = new int[11];
            foreach (var actor in all_actors)
            {
                var level = actor.GetCultisysLevel();
                var realmIndex = GetRealmIndex(level);
                if (realmIndex >= 0)
                {
                    counts[realmIndex]++;
                }

            }

            // 显示统计结果
            string[] realm_names = {
            "斗之气",
            "斗者",
            "斗师",
            "大斗师",
            "斗灵",
            "斗王",
            "斗皇",
            "斗宗",
            "斗尊",
            "斗圣",
            "斗帝"
        };

            for (int i = 0; i < realm_names.Length; i++)
            {
                AddLineValue(realm_names[i], counts[i]);
            }

            // 显示总人数
            AddLineValue("修炼者总数", counts.Sum());
        }
    }
}
