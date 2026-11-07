using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;

namespace Celeste.Mod.MeliHelper
{
    [CustomEntity("MeliHelper/RubikDashSchemeEntity")]
    class RubikDashSchemeEntity : Entity
    {
        public delegate void RubikDashSchemeAction(RubikCube cube);

        public RCMEnum_ControlScheme GetScheme { get; private set; }
        public string GetSchemeName { get; private set; }
        Dictionary<Vector2, RubikDashSchemeAction> dict_dashes;

        public RubikDashSchemeEntity(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            GetScheme = (RCMEnum_ControlScheme)Enum.Parse(typeof(RCMEnum_ControlScheme), data.Attr("scheme"));
            GetSchemeName = data.Attr("name");
            dict_dashes = new Dictionary<Vector2, RubikDashSchemeAction>();
            
            string[] mass_directions = { "Up", "Down", "Left", "Right", "UpLeft", "UpRight", "DownLeft", "DownRight" };
            foreach (string dash_direction in mass_directions)
            {
                RubikDashSchemeAction action;
                string act = data.Attr("dash" + dash_direction);
                switch (act)
                {
                    case "Change direction":  action = (cube) => cube.ChangeDefaultDirection(); break;
                    case "Go to main scheme": action = (cube) => DashController.TryLoadScheme(cube.SceneAs<Level>(), RCMEnum_ControlScheme.Main);    break;
                    case "Go to scheme 1":    action = (cube) => DashController.TryLoadScheme(cube.SceneAs<Level>(), RCMEnum_ControlScheme.Scheme1); break;
                    case "Go to scheme 2":    action = (cube) => DashController.TryLoadScheme(cube.SceneAs<Level>(), RCMEnum_ControlScheme.Scheme2); break;
                    case "Go to scheme 3":    action = (cube) => DashController.TryLoadScheme(cube.SceneAs<Level>(), RCMEnum_ControlScheme.Scheme3); break;
                    case "None": action = (cube) => { }; break;
                    default:
                        RCMEnum_AxisType edge = (RCMEnum_AxisType)Enum.Parse(typeof(RCMEnum_AxisType), act);
                        string rotate_direction = data.Attr("dash" + dash_direction + "Direction");
                        action = (cube) => cube.TryRotateAxis(edge, rotate_direction);
                        break;
                }

                int dx = 0, dy = 0;
                switch (dash_direction)
                {
                    case "UpLeft":    dy--; dx--; break;
                    case "Up":        dy--;       break;
                    case "UpRight":   dy--; dx++; break;
                    case "Left":            dx--; break;
                    case "Right":           dx++; break;
                    case "DownLeft":  dy++; dx--; break;
                    case "Down":      dy++;       break;
                    case "DownRight": dy++; dx++; break;
                }
                Vector2 dir = Vector2.Normalize(new Vector2(dx, dy));
                dict_dashes[dir] = action;
            }
        }

        public void Dash(RubikCube cube, Vector2 v)
        {
            if (dict_dashes.ContainsKey(v))
                dict_dashes[v].Invoke(cube);
        }
    }
}
