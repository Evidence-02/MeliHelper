using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Celeste.SummitCheckpoint;

namespace Celeste.Mod.MeliHelper
{
    [CustomEntity("MeliHelper/CheckpointSaveFlag")]
    class SaveCheckpointFlag : Entity
    {
        Level level;
        Sprite sprite;
        bool is_active, is_confetti;
        int id;

        public SaveCheckpointFlag(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            Add(sprite = GFX.SpriteBank.Create(data.Attr("sprite", "MeliHelper_CheckpointSaveFlag")));
            Add(new PlayerCollider(OnPlayer, new Hitbox(24, 24, -12, -12)));
            this.id = data.ID;
            this.is_confetti = data.Bool("createConfetti", true);
            SetActive(MeliHelperModule.Instance.Session.LaniActiveFlagID == id, false);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = scene as Level;

            Player player = level.Tracker.GetEntity<Player>();
            if (player != null)
                Depth = player.Depth + 1;
        }

        void OnPlayer(Player player)
        {
            if (!is_active)
            {
                if (is_confetti)
                {
                    Audio.Play(SFX.game_07_checkpointconfetti);
                    level.Add(new ConfettiRenderer(this.Center));
                }

                foreach (SaveCheckpointFlag flag_active in level.Entities.FindAll<SaveCheckpointFlag>().FindAll(t => t.is_active))
                    flag_active.SetActive(false);
                SetActive(true);
            }
        }

        public void SetActive(bool value, bool is_set_respawn_point_if_active = true)
        {
            is_active = value;
            sprite.Play(is_active ? "active" : "idle");
            if (value && is_set_respawn_point_if_active)
            {
                level.Session.RespawnPoint = level.GetSpawnPoint(this.Center);
                MeliHelperModule.Instance.Session.LaniActiveFlagID = id;
            }
        }
    }
}
