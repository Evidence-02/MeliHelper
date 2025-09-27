using Celeste;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using Celeste.Mod.Entities;
using System.Collections;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    [Tracked]
    [CustomEntity("MeliHelper/BattleCityFlag")]
    class Flag : Solid
    {
        Sprite sprite;
        Color death_effect_color;
        bool is_destroyed, is_create_death_effect, is_kill_player_instead;

        public Flag(EntityData data, Vector2 offset) : base(data.Position + offset - new Vector2(8, 8), 16, 16, true) 
        {
            is_create_death_effect = data.Bool("createDeathEffect");
            death_effect_color = Methods.GetColorFromString(data.Attr("deathEffectColor"));
            is_kill_player_instead = data.Bool("killPlayerOnFail", false);

            Add(sprite = GFX.SpriteBank.Create(data.Attr("sprite", "MeliHelper_BC_Flag")));
            sprite.Origin = new Vector2(8, 8);
            sprite.Position += new Vector2(8, 8);
            if (data.Bool("damageOnDash", true))
                this.OnDashCollide += onDashCollide;
            Depth = DepthController.BC_FLAG;
        }

        protected virtual DashCollisionResults onDashCollide(Player player, Vector2 dir)
        {
            if (Field.Instance != null)
            {
                Destroy();
                return DashCollisionResults.NormalCollision;
            }

            return DashCollisionResults.Ignore;
        }

        public void Destroy()
        {
            if (!is_destroyed)
            {
                Audio.Play(SoundController.BC_PLAYER_TANK_DESTROYED);
                sprite.Play("destroy");
                if (is_create_death_effect)
                    Add(new DeathEffect(death_effect_color));
                is_destroyed = true;

                if (is_kill_player_instead) Add(new Coroutine(MadelineFuckingDies()));
                else Field.Instance.SetState(BCEnum_GameState.Gameover);
            }
        }

        public IEnumerator MadelineFuckingDies()
        {
            Level level = SceneAs<Level>();
            Player player = level.Tracker.GetEntity<Player>();
            if (Methods.PlayerIsAlive(player))
                Methods.PlayerLock(player, true);

            Field.Instance.SetState(BCEnum_GameState.Pause);
            yield return 0.7f;

            if (Methods.PlayerIsAlive(player))
                player.Die(Vector2.Normalize(player.Center - this.Center));
        }



        public override void Render()
        {
            Draw.Rect(this.Collider, Color.Black);
            Draw.Rect(new Rectangle((int)Position.X - 4, (int)Position.Y - 4, (int)Width + 8, (int)Height + 8), Color.Black * 0.3f);
            Draw.Rect(new Rectangle((int)Position.X - 8, (int)Position.Y - 8, (int)Width + 16, (int)Height + 16), Color.Black * 0.3f);
            base.Render();

        }
    }
}
