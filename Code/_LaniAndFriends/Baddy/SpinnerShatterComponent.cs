using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class SpinnerShatterComponent : Component
    {
        Level level;
        Vector2 center, accet;
        float radius;

        public SpinnerShatterComponent(Vector2 accet, float radius) 
            : base(true, true)
        {
            this.accet = accet;
            this.radius = radius;
        }

        public override void Added(Entity entity)
        {
            base.Added(entity);
            level = entity.SceneAs<Level>();
        }

        public override void Update()
        {
            base.Update();
            center = Entity.Center + accet;


            // Default: spinners
            List<CrystalStaticSpinner> list_spinners = Scene.Entities.OfType<CrystalStaticSpinner>().ToList()
                .FindAll(t => Vector2.Distance(t.Center, center) <= radius);
            if (list_spinners.Count > 0)
            {
                //Audio.Play("event:/game/06_reflection/boss_spikes_burst");
                foreach (CrystalStaticSpinner spinner in list_spinners)
                    spinner.Destroy();
            }
            
            // Remove trash from hotel (literally)
            List<DustStaticSpinner> list_blocks_bunnies = level.Entities.FindAll<DustStaticSpinner>()
                .FindAll(t => Vector2.Distance(t.Center, center) <= radius);
            foreach (DustStaticSpinner bnuuy in list_blocks_bunnies)
                bnuuy.RemoveSelf();

            List<DustTrackSpinner> list_tracks_bunnies = level.Entities.FindAll<DustTrackSpinner>()
                .FindAll(t => Vector2.Distance(t.Center, center) <= radius);
            foreach (DustTrackSpinner bnuuy in list_tracks_bunnies)
                bnuuy.RemoveSelf();

            List<DustRotateSpinner> list_rotate_bunnies = level.Entities.FindAll<DustRotateSpinner>()
                .FindAll(t => Vector2.Distance(t.Center, center) <= radius);
            foreach (DustRotateSpinner bnuuy in list_rotate_bunnies)
                bnuuy.RemoveSelf();




        }

        public void SetRadius(int radius)
        {
            this.radius = radius;
        }
    }
}
