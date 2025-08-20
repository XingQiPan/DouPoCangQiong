using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using DdouPoCangPong.Code.Trait;

namespace DdouPoCangPong.Code
{
    internal class UnitWindowHelper
    {
        //private static TraitsGrid _traits_grid;
        //private static ObjectPoolGenericMono<ActorTraitButton> _trait_button_pool;

        class WudaoTraitContainer : MonoBehaviour
        {
            private UnitWindow _window;

            private void Awake()
            {
                _window = GetComponentInParent<UnitWindow>();
            }

            //public void OnEnable()
            //{
            //    int trait_count = 0;
            //    foreach (var trait in _window.actor.traits)
            //    {
            //        if (!Traits.all_traits.Contains(trait.id)) continue;
            //        var button = _trait_button_pool.getNext();
            //        button.load(trait);
            //        trait_count++;
            //    }
            //    _traits_grid.transform.gameObject.SetActive(trait_count > 0);
            //}

            //private void OnDisable()
            //{
            //    _trait_button_pool.clear();
            //}
        }
        public static bool Initialize(UnitWindow unitWindow)
        {
            //var trait_grid = unitWindow.transform.GetComponentInChildren<TraitsGrid>(true);
            //if (trait_grid == null) return false;

            //var content_meta = unitWindow.transform.FindRecursive("content_meta");

            //var container = new GameObject("content_wudao_traits", typeof(VerticalLayoutGroup));
            //container.transform.SetParent(content_meta.parent);
            //container.transform.localScale = Vector3.one;
            //container.transform.SetSiblingIndex(content_meta.GetSiblingIndex());
            //container.GetComponent<RectTransform>().sizeDelta =
            //    new(210, container.GetComponent<RectTransform>().sizeDelta.y);
            //var layout_group = container.GetComponent<VerticalLayoutGroup>();
            //layout_group.childControlHeight = true;
            //layout_group.childControlWidth = true;
            //layout_group.childForceExpandWidth = true;
            //layout_group.childForceExpandHeight = false;

            //_traits_grid = UnityEngine.Object.Instantiate(trait_grid, container.transform);
            //_trait_button_pool = new(unitWindow.transform.GetComponentInChildren<ActorTraitsContainer>(true)._prefab_trait,
            //    _traits_grid.transform);
            //foreach (var component in _traits_grid.GetComponentsInChildren<ActorTraitButton>(true))
            //{
            //    _trait_button_pool._elements_total.Add(component);
            //}
            //_trait_button_pool.clear();

            //container.AddComponent<WudaoTraitContainer>();
            //unitWindow.scroll_window.tabs.tab_default.tab_elements.Add(container.transform);


            var mask = unitWindow._avatar_element.gameObject.transform.Find("Mask");
            var tip_button = mask.gameObject.AddComponent<TipButton>();
            tip_button.hoverAction = () => { tooltip_action_cultisys(unitWindow._avatar_element.gameObject, unitWindow.actor); };
            return true;
        }
        private static void tooltip_action_cultisys(GameObject obj, Actor actor)
        {
            Tooltip.show(obj, Tooltips.cultisys.id, new TooltipData
            {
                actor = actor
            });
        }
    }
}
