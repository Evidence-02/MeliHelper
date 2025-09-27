using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._Minesweeper
{
    [CustomEntity("MeliHelper/MinesweeperCell")]
    class MinesweeperCell : Entity
    {
        MinesweeperField field;
        Solid solid;
        MTexture texture;
        Minesweeper_CellMark mark;
        bool is_exploded;
        public bool isOpened { get; set; }
        public bool isBomb { get; set; }
        public int GetNeighbors { get; set; }

        public MinesweeperCell(EntityData data, Vector2 offset) : base(data.Position + offset - new Vector2(8, 8))
        {
            isOpened = data.Bool("opened");
            if (data.Bool("bomb", false)) isBomb = true;
            else GetNeighbors = data.Int("neighbors");

            // TODO: find field and register bomb there
            //if (field == null) field = scene.Entities.FindFirst<MinesweeperField>(); // TODO: find first field where cell is inside
        }

        public MinesweeperCell(MinesweeperField field, Vector2 position) : base(position)
        {
            this.field = field;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            (scene as Level).Add(solid = new Solid(Position, 16, 16, false));
            solid.Add(new MeliHelperActualParentComponent(this));
            mark = Minesweeper_CellMark.None;
            UpdateTexture();

            //MeliHelperModule.Instance.Session.RegisteredSolid[solid] = "MinesweeperCell";
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            if (isOpened) Open();
            else solid.OnDashCollide += onDashCollide; 
        }

        public override void Render()
        {
            base.Render();
            texture.Draw(Position);
        }

        protected DashCollisionResults onDashCollide(Player player, Vector2 dir)
        {
            if (field != null)
                return field.RegisterPlayerDash(player, dir, this);
            return PlayerDashCollideResult(player);
        }

        public DashCollisionResults PlayerDashCollideResult(Player player)
        {
            player.RefillDash();
            player.RefillStamina();
            if (MeliHelperModule.Instance.Session.Minesweeper_CellMarker == Minesweeper_CellMark.Flag)
                InvertFlagMarker();
            else if (!isMarkedAsFlag)
            {
                Open();
                return DashCollisionResults.Bounce;
            }

            return DashCollisionResults.NormalCollision;
        }

        public void InvertFlagMarker()
        {
            SetMark(isMarkedAsFlag ? Minesweeper_CellMark.None : Minesweeper_CellMark.Flag);
        }

        public void SetMark(Minesweeper_CellMark mark)
        {
            if (field != null)
                field.RegisterMarkedFlag((isMarkedAsFlag ? +1 : 0) + (mark == Minesweeper_CellMark.Flag ? -1 : 0));

            this.mark = mark;
            UpdateTexture();
        }

        public void Open(Player player = null)
        {
            isOpened = true;
            solid.RemoveSelf();
            if (isBomb)
            {
                is_exploded = true;
                if (player == null) player = Methods.GetPlayerOnScene(Scene);
                if (Methods.PlayerIsAlive(player)) player.Die(Vector2.Normalize(player.Center - this.Center));
                if (field != null)
                    field.Kaboom();
            }
            else
            {
                //texture = GFX.Game["Evidence02/objects_melihelper/minesweeper/cell0" + GetNeighbors];
                if (field != null)
                    field.RegisterOpenedCell();
                if (GetNeighbors == 0)
                    field.OpenAllNeighborCells(this);
            }
            UpdateTexture();
        }

        public void Reveal()
        {
            isOpened = true;
            UpdateTexture();
        }

        public void UpdateTexture()
        {
            string name;
            if (isOpened)
            {
                if (isBomb)
                    name = (is_exploded) ? "cellBombExploded" : (isMarkedAsFlag) ? "cellFlag" : "cellBomb";
                else if (isMarkedAsFlag)
                    name = "cellFlagError";
                else 
                    name = "cell0" + GetNeighbors;
            }
            else
            {
                name = (isMarkedAsFlag) ? "cellFlag" : "cellFull";
            }
            texture = GFX.Game["Evidence02/objects_melihelper/minesweeper/" + name];
        }

        public bool isCollide(Vector2 point)
        {
            return !isOpened && solid.CollidePoint(point);
        }

        public bool isMarkedAsFlag
        {
            get
            {
                return mark == Minesweeper_CellMark.Flag;
            }
        }
    }
}
