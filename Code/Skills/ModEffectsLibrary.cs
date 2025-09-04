using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdouPoCangPong.Code.Skills
{
    /// <summary>
    /// 存放所有 Mod 自定义的特效。
    /// 通过 Harmony 补丁在游戏启动时加载。
    /// </summary>
    internal static class ModEffectsLibrary
    {
        public static void init()
        {
            // 注册 “斗气爆发” 特效
            // 这个特效可以用于各种技能的起手式
            var douqi_burst = new EffectAsset
            {
                id = "douqi_burst_effect", // 特效的唯一ID
                // 注意：这里的 sprite_path 指向你的Mod资源包中的图片路径
                // 你需要确保有一张名为 "douqi_burst_effect_t" 的序列帧图片放在 "effects" 文件夹下
                sprite_path = "cultiway/effect/fire_polo",
                use_basic_prefab = true,       // 使用游戏自带的基础特效预设体
                sorting_layer_id = "EffectsBack", // 在角色身后渲染
                draw_light_area = true,        // 产生光照
                draw_light_size = 0.4f,        // 光照范围
                limit = 60                     // 同屏最大数量限制
            };
            AssetManager.effects_library.add(douqi_burst);

            // 注册 “圣光闪烁” 特效
            // 这是一个用于治疗法术的示例特效
            //var holy_sparkle = new EffectAsset
            //{
            //    id = "holy_sparkle_effect",
            //    // 同样，你需要提供对应的特效图片资源
            //    sprite_path = "effects/holy_sparkle_effect_t",
            //    use_basic_prefab = true,
            //    sorting_layer_id = "EffectsTop", // 在角色身前渲染
            //    limit = 50
            //};
            //AssetManager.effects_library.add(holy_sparkle);
        }
    }
}