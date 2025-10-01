using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    [CustomEntity("MeliHelper/DateTimeMessage")]
    class DateTimeMessage : Entity
    {
        Color color_text;
        string format;
        float alpha;
        int size;
        bool is_outline;

        public DateTimeMessage(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            format = data.Attr("dateTimeFormat");
            color_text = Methods.GetColorFromString(data.Attr("colorText", "FFFFFF"));
            alpha = data.Float("opacity", 1f);
            size = data.Int("size", 1);
            is_outline = data.Bool("outline", false);
        }

        public override void Render()
        {
            base.Render();
            if (is_outline)
                FontControllerOutline.DrawText(Position, DateTime.Now.ToString(format), color_text * alpha, size);
            else
                FontControllerOutline.DrawTextWhite(Position, DateTime.Now.ToString(format), color_text * alpha, size);
        }


    }
}
