using DdouPoCangPong;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DdouPoCangPong.Code.Abstract
{
    public abstract class ExtendLibrary<TAsset, T>
    where TAsset : Asset, new() where T : ExtendLibrary<TAsset, T>
    {
        private readonly List<TAsset> _assets_added = new();
        protected ReadOnlyCollection<TAsset> assets_added;
        protected AssetLibrary<TAsset> cached_library;
        protected TAsset t;

        protected ExtendLibrary()
        {
            cached_library =
                AssetManager._instance._list.Find(x => x is (AssetLibrary<TAsset>)) as AssetLibrary<TAsset>;
            assets_added = _assets_added.AsReadOnly();
        }

        public void Init()
        {
            OnInit();
        }

        public virtual void OnReload()
        {
        }

        public virtual void PostInit(TAsset asset)
        {
        }

        protected void RegisterAssets(string prefix = "")
        {
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
            foreach (PropertyInfo prop in props)
                if (prop.PropertyType == typeof(TAsset))
                {
                    TAsset item;
                    var item_id = $"{prefix}.{prop.Name}";
                    var clone_source_attr = prop.GetCustomAttribute<CloneSourceAttribute>();
                    var asset_id_attr = prop.GetCustomAttribute<AssetIdAttribute>();
                    if (asset_id_attr != null)
                    {
                        item_id = asset_id_attr.AssetId;
                    }
                    if (clone_source_attr != null)
                        item = Clone(item_id, clone_source_attr.clone_source_id);
                    else
                        item = Add(new TAsset
                        {
                            id = item_id
                        });

                    prop.SetValue(null, item);
                    PostInit(item);
                    Main.LogInfo($"({typeof(T).Name}) Initializes {item_id}");
                }
        }

        protected abstract void OnInit();

        protected virtual TAsset Add(TAsset asset)
        {
            t = cached_library.add(asset);
            _assets_added.Add(t);
            return t;
        }

        protected virtual TAsset Clone(string new_id, string from_id)
        {
            t = cached_library.clone(new_id, from_id);
            _assets_added.Add(t);
            return t;
        }
    }
}
