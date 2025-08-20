using HarmonyLib;
using DdouPoCangPong.Code;
using NeoModLoader.api;
using NeoModLoader.General;
using NeoModLoader.General.UI.Tab;
using NeoModLoader.ui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using DdouPoCangPong.Code.UI;

namespace DdouPoCangPong
{
    public class Main : BasicMod<Main>
    {
        internal const string asset_id_prefix = "xingqipan.custommodt001";
        protected override void OnModLoad()
        {
            Config.isEditor = true;
            Stats.Init();
            Debug.Log("斗气属性加载成功……");
            //TraitGroup.Init();
            Debug.Log("斗气特质加载成功……");
            new Tooltips().Init();
            try
            {
                Harmony.CreateAndPatchAll(typeof(Patches));
                Debug.Log("斗气机制加载成功……");
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            // 创建空白窗口
            ScrollWindow empty_window = WindowCreator.CreateEmptyWindow("xingqipan.custommodt001", "xingqipan.custommodt001");
            empty_window.scrollRect.gameObject.SetActive(true);

            // 获取并设置内容区域
            var content = empty_window.scrollRect.transform.Find("Viewport/Content").GetComponent<RectTransform>();
            content.pivot = new Vector2(0.5f, 1);
            content.sizeDelta = new Vector2(180, 0);

            // 添加布局组件
            var layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.childControlHeight = false;
            layout.childControlWidth = false;
            layout.childAlignment = TextAnchor.UpperCenter;

            // 添加大小适配
            content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // 显示窗口
            ScrollWindow.showWindow(empty_window.screen_id);
            var tab = TabManager.CreateTab(asset_id_prefix, asset_id_prefix, "",
                SpriteTextureLoader.getSprite("icon.png"));
            tab.SetLayout(new List<string>()
        {
            "tab"
        });

            WindowCultiStats.CreateAndInit($"{asset_id_prefix}.UI.{nameof(WindowCultiStats)}");
            tab.AddPowerButton("tab", PowerButtonCreator.CreateSimpleButton($"{asset_id_prefix}.allLevel", () =>
            {
                ScrollWindow.showWindow($"{asset_id_prefix}.UI.{nameof(WindowCultiStats)}");
            }, SpriteTextureLoader.getSprite("icon.png")));

            

            tab.UpdateLayout();
        }

        public static GameObject NewPrefabPreview(string name, params Type[] types)
        {
            var obj = new GameObject(name, types);
            obj.transform.SetParent(Instance.PrefabLibrary);
            return obj;
        }
    }
}