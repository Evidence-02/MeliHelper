using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class BonusDefault : Bonus
    {
        float ttl;

        public BonusDefault(Vector2 center, BCEnum_BonusType type, float ttl) : base(center, type)
        {
            this.ttl = ttl;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Audio.Play(SoundController.BC_POWERUP_APPEARED);
            Depth = DepthController.BC_BONUS;
        }

        protected override void onPlayer(Player player)
        {
            base.onPlayer(player);
            ProgressController.AddPoints(500);
            RemoveSelf();
        }

        public override void Update()
        {
            base.Update();

            ttl -= Engine.DeltaTime;
            if (ttl <= 0) RemoveSelf();
            if (ttl <= 2.56f)
            {
                // 0    0.32    0.64    0.96    1.28     1.60     1.92   2.24    2.56     2.88    3.20   3.52      3.84   4.16
                // -------       ---------       ----------        --------       ----------       --------         --------
                image.Visible = (ttl % 0.64 <= 0.32f);
                //image.Color = Methods.GetColorBetween(Color.White, Color.Red, ttl) * (ttl / TIME_DISAPPEARING);
            }
        }
    }
}
