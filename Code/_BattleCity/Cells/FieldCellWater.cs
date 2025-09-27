using Celeste;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using Celeste.Mod.Entities;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class FieldCellWater : FieldCell
    {
        Solid solid;
        DreamBlock dreamblock;
        JumpthruPlatform platform;

        public FieldCellWater(Vector2 position, MTexture texture = null) 
            : base(position, BCEnum_CellType.Water, texture)
        {
        }

        public override void Awake(Scene scene)
        {
            SetMoveThrough(ProgressController.PlayerCanMoveThroughWater());
        }

        public void SetMoveThrough(bool value)
        {
            Level level = SceneAs<Level>();
            if (MeliHelperModule.Settings.Testing_MoveThroughWaterisDreamblock)
            {
                // Dreamblock should be nicer, but i'm worrying field is too small for a dreamblocks
                if (value)
                {
                    // Add dreamblock to the level, remove solid
                    if (solid != null) level.Remove(solid);
                    if (dreamblock == null)
                    {
                        dreamblock = new DreamBlock(Position, 8, 8, null, false, false);
                        dreamblock.Add(new MeliHelperActualParentComponent(this));
                    }
                    if (!level.Contains(dreamblock)) level.Add(dreamblock);
                    dreamblock.Visible = false;
                }
                else
                {
                    // Add solid to the level, remove dreamblock
                    if (dreamblock != null) level.Remove(dreamblock);
                    CreateSolidAndAddToTheLevel(level);
                }
            }
            else
            {
                // First variant, simple jumpthru platform
                if (value)
                {
                    // Add platform to the level, remove solid
                    if (solid != null) level.Remove(solid);
                    if (platform == null) platform = new JumpthruPlatform(Position, 8, "");
                    if (!level.Contains(platform)) level.Add(platform);
                    platform.Visible = false;
                }
                else
                {
                    // Add solid to the level, remove platform
                    if (platform != null) level.Remove(platform);
                    CreateSolidAndAddToTheLevel(level);
                }
            }
        }

        void CreateSolidAndAddToTheLevel(Level level)
        {
            if (solid == null)
            {
                solid = new Solid(Position, 8, 8, true);
                solid.Add(new MeliHelperActualParentComponent(this));
            }
            if (!level.Contains(solid)) level.Add(solid);
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
            if (solid != null) solid.RemoveSelf();
            if (platform != null) platform.RemoveSelf();
        }




    }
}
