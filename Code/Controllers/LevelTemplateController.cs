using Celeste.Editor;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class LevelTemplateController
    {
        static Dictionary<LevelTemplate, List<CustomWallInfo>> dict_levels;

        public static void Load()
        {
            dict_levels = new Dictionary<LevelTemplate, List<CustomWallInfo>>();

            On.Celeste.Editor.LevelTemplate.ctor_LevelData += OnLevelTemplate_ctor;
            On.Celeste.Editor.LevelTemplate.RenderContents += OnLevelTemplate_RenderContents;
        }

        public static void Unload()
        {
            On.Celeste.Editor.LevelTemplate.ctor_LevelData -= OnLevelTemplate_ctor;
            On.Celeste.Editor.LevelTemplate.RenderContents -= OnLevelTemplate_RenderContents;
        }

        private static void OnLevelTemplate_RenderContents(On.Celeste.Editor.LevelTemplate.orig_RenderContents orig,
            LevelTemplate self, Monocle.Camera camera, List<LevelTemplate> allLevels)
        {
            orig(self, camera, allLevels);
            if (dict_levels.ContainsKey(self))
            {
                foreach (CustomWallInfo info in dict_levels[self])
                    Draw.Rect(info.rect, info.clr);
            }
        }

        private static void OnLevelTemplate_ctor(On.Celeste.Editor.LevelTemplate.orig_ctor_LevelData orig, LevelTemplate self, LevelData data)
        {
            orig(self, data);
            if (!dict_levels.ContainsKey(self))
            {
                List<CustomWallInfo> list_rectangles = new List<CustomWallInfo>();
                List<EntityData> list_fields = data.Entities.FindAll(t => t.Name.StartsWith("MeliHelper/BattleCityField") && t.Attr("backgroundType", "Default") == "Default");
                foreach (EntityData entity_data in list_fields)
                {
                    int x = (int)entity_data.Position.X / 8 + 1;
                    int y = (int)entity_data.Position.Y / 8 + 1;
                    int w = 2 * entity_data.Int("fieldWidth", 13), h = 2 * entity_data.Int("fieldHeight", 13);
                    list_rectangles.Add(new CustomWallInfo(new Rectangle(self.X + x, self.Y + y, w, h), Color.Gray * 0.15f));

                    //Color clr = Color.Red * 0.3f;
                    //list_rectangles.Add(new CustomWallInfo(new Rectangle(self.X + x, self.Y + y, w, 1), clr));
                    //list_rectangles.Add(new CustomWallInfo(new Rectangle(self.X + x, self.Y + y + h, w, 1), clr));
                    //list_rectangles.Add(new CustomWallInfo(new Rectangle(self.X + x, self.Y + y, 1, h), clr));
                    //list_rectangles.Add(new CustomWallInfo(new Rectangle(self.X + x + h, self.Y + y, 1, h + 1), clr));
                }

                List<EntityData> list_cells = data.Entities.FindAll(t => t.Name.StartsWith("MeliHelper/BattleCityCell"));
                foreach (EntityData entity_data in list_cells)
                {
                    int x = (int)entity_data.Position.X / 8;
                    int y = (int)entity_data.Position.Y / 8;
                    int w = 2, h = 2;
                    if (entity_data.Name.Contains("Wall"))
                    {
                        w = entity_data.Width / 8;
                        h = entity_data.Height / 8;
                    }
                    else
                    {
                        x--; y--;
                        switch (entity_data.Attr("fill", "Full"))
                        {
                            case "Left":   w--; break;
                            case "Right":  w--; x++; break;
                            case "Top":    h--; break;
                            case "Bottom": h--; y++; break;
                        }
                    }


                    if (x < 0)
                    {
                        w += x;
                        x = 0;
                    }
                    if (y < 0)
                    {
                        h += y;
                        y = 0;
                    }

                    if (w > self.Width - x)
                        w = self.Width - x;
                    if (h > self.Height - y)
                        h = self.Height - y;

                    if (w > 0 && h > 0)
                    {
                        Color clr = Color.White;
                        string walltype = entity_data.Attr("cellType");
                        switch (walltype)
                        {
                            case "Steel": clr = Color.SlateGray; break;
                            case "Water": clr = Color.RoyalBlue; break;
                            case "Grass": clr = Color.LightGreen; break;
                            case "Dirt":  clr = Color.Gainsboro; break;
                        }
                        list_rectangles.Add(new CustomWallInfo(new Rectangle(self.X + x, self.Y + y, w, h), clr));
                    }
                }


                // Any non-typical blocks
                List<EntityData> list_blocks = data.Entities.FindAll(t => t.Name.StartsWith("MeliHelper/LaniIceBlock"));
                foreach (EntityData entity_data in list_blocks)
                    list_rectangles.Add(new CustomWallInfo(new Rectangle(
                        self.X + (int)entity_data.Position.X / 8, 
                        self.Y + (int)entity_data.Position.Y / 8, 
                        entity_data.Width / 8, entity_data.Height / 8), Color.White * 0.8f));


                dict_levels[self] = list_rectangles;
            }
        }

        class CustomWallInfo
        {
            public Rectangle rect { get; set; }
            public Color clr { get; set; }

            public CustomWallInfo(Rectangle rect, Color clr)
            {
                this.rect = rect;
                this.clr = clr;
            }
        }

    }
}
