using DdouPoCangPong.Code.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DdouPoCangPong.Code.Skills
{
    /// <summary>
    /// 统一管理所有 Actor 的技能组件
    /// </summary>
    internal static class ActorSkillManager
    {
        private static Dictionary<Actor, ActorSkillComponent> components = new Dictionary<Actor, ActorSkillComponent>();

        /// <summary>
        /// 为一个 Actor 添加技能组件. 通常在 Actor 初始化时调用.
        /// </summary>
        public static void AddComponent(Actor pActor)
        {
            if (!components.ContainsKey(pActor))
            {
                components.Add(pActor, new ActorSkillComponent(pActor));
            }
        }

        /// <summary>
        /// 移除一个 Actor 的技能组件. 通常在 Actor 死亡或被销毁时调用.
        /// </summary>
        public static void RemoveComponent(Actor pActor)
        {
            if (components.ContainsKey(pActor))
            {
                components.Remove(pActor);
            }
        }

        /// <summary>
        /// 获取一个 Actor 的技能组件.
        /// </summary>
        public static ActorSkillComponent GetComponent(Actor pActor)
        {
            components.TryGetValue(pActor, out ActorSkillComponent component);
            return component;
        }

        /// <summary>
        /// 更新所有技能组件的逻辑，主要用于计算冷却时间.
        /// 这个方法应该由游戏的主更新循环来调用.
        /// </summary>
        public static void Update(float pElapsed)
        {
            var actors = new List<Actor>(components.Keys);
            foreach (var actor in actors)
            {
                if (!actor.isAlive())
                {
                    RemoveComponent(actor);
                    continue;
                }

                if (components.TryGetValue(actor, out ActorSkillComponent component))
                {
                    component.Update(pElapsed);
                }
            }
        }
    }
}
