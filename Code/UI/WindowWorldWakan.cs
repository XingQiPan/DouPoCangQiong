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

        public static int GetRealmIndex(int level)
        {
            if (level <= 0) return -1;

            int realmIndex = (level - 1) / 9;

            // 斗帝是最高境界，所有超过斗圣9星(90级)的都归为斗帝
            if (realmIndex >= 10) return 10; // 斗帝索引为10

            return realmIndex;
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
