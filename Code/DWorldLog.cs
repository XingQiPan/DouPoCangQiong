using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdouPoCangPong.Code
{
    internal class DWorldLog
    {
        public static DWorldLog instance;
        public DWorldLog() => DWorldLog.instance = this;

        public static void logUplevelActor(Actor actor)
        {
            new WorldLogMessage(WorldLogLibrary.alliance_dissolved, actor.name, null, null)
            {
                color_special1 = actor.getColor().getColorText()
            }.add();
        }
    }
}
