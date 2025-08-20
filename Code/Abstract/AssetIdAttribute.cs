using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdouPoCangPong.Code.Abstract
{
    public class AssetIdAttribute: Attribute
    {
        public readonly string AssetId;
        public AssetIdAttribute(string assetId) {
        AssetId = assetId;
        }
    }
}
