using NeoModLoader.General.UI.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using DdouPoCangPong.Code.Abstract;

namespace DdouPoCangPong.Code.UI.UIPerfeb
{
    public class LineTitleValue : APrefabPreview<LineTitleValue>
    {
        public Text Title => title;
        public Text Value => value;
        [SerializeField]
        private Text title;
        [SerializeField]
        private Text value;

        public void Setup(string title_str, string value_str)
        {
            Title.text = title_str;
            Value.text = value_str;
        }
        private static void _init()
        {
            var obj = Main.NewPrefabPreview(nameof(LineTitleValue), typeof(HorizontalLayoutGroup));

            var title_obj = new GameObject(nameof(Title), typeof(Text));
            var value_obj = new GameObject(nameof(Value), typeof(Text));

            title_obj.transform.SetParent(obj.transform);
            value_obj.transform.SetParent(obj.transform);

            var title = title_obj.GetComponent<Text>();
            var value = value_obj.GetComponent<Text>();

            title.font = LocalizedTextManager.current_font;
            value.font = LocalizedTextManager.current_font;

            title.alignment = TextAnchor.MiddleLeft;
            value.alignment = TextAnchor.MiddleRight;

            title.color = Color.white;
            value.color = Color.white;

            title.resizeTextForBestFit = true;
            value.resizeTextForBestFit = true;

            title.resizeTextMaxSize = 12;
            value.resizeTextMaxSize = 12;

            title.resizeTextMinSize = 1;
            value.resizeTextMinSize = 1;

            Prefab = obj.AddComponent<LineTitleValue>();
            Prefab.title = title;
            Prefab.value = value;
        }
    }
}
