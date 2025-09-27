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
    [CustomEntity("MeliHelper/MinesweeperField")]
    class MinesweeperField : Entity
    {
        Player player;
        CustomVirtualButtonChecker button_change_mode;
        MinesweeperBombCounter ui_counter;
        MinesweeperFace ui_face;
        MinesweeperTimer ui_timer;
        MinesweeperCell[,] mass_cells;
        List<MinesweeperCell> list_reserved_cells;
        int w, h, count_bombs, count_empty_cells, max_start_cell;
        string diagonal_dashes;
        bool is_place_player_inside;
        string room_win;

        public MinesweeperField(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            w = data.Int("fieldWidth");
            h = data.Int("fieldHeight");
            count_bombs = data.Int("bombs");
            count_empty_cells = w * h - count_bombs + 1;
            is_place_player_inside = data.Bool("placePlayerInside");
            room_win = data.Attr("roomTeleportOnWin");
            max_start_cell = data.Int("maxStartCell", 1);
            diagonal_dashes = data.Attr("diagonalDashesOpenCells", "OnlyThroughFlags");

            list_reserved_cells = new List<MinesweeperCell>();

            button_change_mode = new CustomVirtualButtonChecker(MeliHelperModule.Settings.Minesweeper_ChangeDashMode);
            MeliHelperModule.Instance.Session.Minesweeper_CellMarker = Minesweeper_CellMark.None;
            Depth = DepthController.DEFAULT_UI;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);

            Level level = scene as Level;
            mass_cells = new MinesweeperCell[w, h];
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                    level.Add(mass_cells[i, j] = new MinesweeperCell(this, Position + 16 * new Vector2(i, j)));

            
            char[,] cells_generated = MinesweeperFieldGenerator.GenerateField(w, h, count_bombs);
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    if (cells_generated[x, y] == 'B')
                    {
                        mass_cells[x, y].isBomb = true;
                        for (int dx = -1; dx <= 1; dx++)
                            for (int dy = -1; dy <= 1; dy++)
                                if (isInField(x + dx, y + dy))
                                    mass_cells[x + dx, y + dy].GetNeighbors++;
                    }

            Calc.PopRandom();
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);

            Level level = scene as Level;
            List<MinesweeperBombCounter> list_bomb_counters = level.Entities.FindAll<MinesweeperBombCounter>();
            if (list_bomb_counters.Count > 0)
            {
                ui_counter = list_bomb_counters.OrderBy(t => Vector2.Distance(t.Position, this.Position)).First();
                ui_counter.GetCountBombs = count_bombs;
            }

            List<MinesweeperTimer> list_timers = level.Entities.FindAll<MinesweeperTimer>();
            if (list_timers.Count > 0)
                ui_timer = list_timers.OrderBy(t => Vector2.Distance(t.Position, this.Position)).First();

            List<MinesweeperFace> list_faces = level.Entities.FindAll<MinesweeperFace>();
            if (list_faces.Count > 0)
                ui_face = list_faces.OrderBy(t => Vector2.Distance(t.Position, this.Position)).First();


            player = Methods.GetPlayerOnScene(scene);
            foreach (MinesweeperCell cell in mass_cells)
                if (cell.Depth < player.Depth + 1)
                    cell.Depth = player.Depth + 1;

            // TODO: find cell with 0 or 1 neighbors (or more if needed), open it and then "player.Center = cell.Position;"
            if (is_place_player_inside && Methods.PlayerIsAlive(player))
            {
                int max_neighbors = max_start_cell;

                int min_neighbors = 9;
                foreach (MinesweeperCell cell in mass_cells)
                    if (!cell.isBomb && cell.GetNeighbors < min_neighbors)
                        min_neighbors = cell.GetNeighbors;
                if (max_neighbors < min_neighbors)
                    max_neighbors = min_neighbors;
                if (min_neighbors == 9)
                {
                    SoundController.PlayDebugSound01();
                    return;
                }


                List<MinesweeperCell> list_good_cells = new List<MinesweeperCell>();
                foreach (MinesweeperCell cell in mass_cells)
                    if (!cell.isBomb && cell.GetNeighbors <= max_neighbors)
                        list_good_cells.Add(cell);

                if (list_good_cells.Count > 0)
                {
                    Calc.PushRandom(MeliHelperModule.Instance.SaveData.MinesweeperFieldID);
                    MinesweeperCell cell_place = list_good_cells[Calc.Random.Next(0, list_good_cells.Count)];
                    Calc.PushRandom(MeliHelperModule.Instance.SaveData.MinesweeperFieldID);
                    cell_place.Open();
                    player.Center = cell_place.Center + new Vector2(8, 8);
                }
            }

            if (MeliHelperModule.Settings.Debug_MinesweeperSolvedFromStart)
            {
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                        if (!mass_cells[x, y].isBomb)
                            list_reserved_cells.Add(mass_cells[x, y]);
            }
        }

        public override void Update()
        {
            base.Update();
            if (button_change_mode.OhItsReallyFuckingPressedIsntIt())
            {
                Minesweeper_CellMark next = 
                    (MeliHelperModule.Instance.Session.Minesweeper_CellMarker == Minesweeper_CellMark.None) 
                    ? Minesweeper_CellMark.Flag 
                    : Minesweeper_CellMark.None;

                MeliHelperModule.Instance.Session.Minesweeper_CellMarker = next;
                Audio.Play(SFX.char_mad_backpack_drop);
            }

            if (list_reserved_cells.Count > 0 && Scene.OnInterval(MeliHelperModule.Settings.Debug_MinesweeperSolvedFromStart ? 0.01f : 0.06f))
            {
                list_reserved_cells[0].Open();
                list_reserved_cells.RemoveAt(0);
            }
        }

        public override void Render()
        {
            base.Render();
            if (MeliHelperModule.Instance.Session.Minesweeper_CellMarker != Minesweeper_CellMark.None && Methods.PlayerIsAlive(player))
            {
                // Draw flag marker above the player head
                GFX.Game["Evidence02/objects_melihelper/minesweeper/mode" + MeliHelperModule.Instance.Session.Minesweeper_CellMarker].DrawCentered(player.Center - new Vector2(0, 16));
            }
        }

        public DashCollisionResults RegisterPlayerDash(Player player, Vector2 dir, MinesweeperCell cell_dashed)
        {
            int px = (int)((player.Center.X - Position.X) / 16);
            int py = (int)((player.Center.Y - Position.Y) / 16);
            int dx = Math.Sign(player.DashDir.X);
            int dy = Math.Sign(player.DashDir.Y);
            int cx = px + dx;
            int cy = py + dy;
            //SceneAs<Level>().Add(new FloatyWordsEntity(player.Center, $"x={player.DashDir.X}, y={player.DashDir.Y}", Color.White));
            
            // Try dash diagonal (need two flagged cells on both axises)
            if (diagonal_dashes != "None")
            {
                if (dx != 0 && dy != 0 && isInField(cx, cy)
                      && !mass_cells[cx, py].isOpened && (mass_cells[cx, py].isMarkedAsFlag || diagonal_dashes == "Always")
                      && !mass_cells[px, cy].isOpened && (mass_cells[px, cy].isMarkedAsFlag || diagonal_dashes == "Always")
                      && !mass_cells[cx, cy].isOpened)
                    return mass_cells[cx, cy].PlayerDashCollideResult(player);
            }

            // Dash horizontal/vertical as normal
            cx = px + Math.Sign(dir.X);
            cy = py + Math.Sign(dir.Y);
            if (isInField(cx, cy) && !mass_cells[cx, cy].isOpened)
                return mass_cells[cx, cy].PlayerDashCollideResult(player);

            // No cell? Probably player is on the edge of cell. Try open it!
            if (Math.Sign(dx) + Math.Sign(dy) == 1)
                return cell_dashed.PlayerDashCollideResult(player);

            return DashCollisionResults.NormalCollision;
        }

        public void Kaboom()
        {
            // TODO:
            // 1+ set smile face to sad face
            // 2+ stop timer
            // 3. open every unmarked bombs
            // 4. cross every marked free cell as failed
            // 5. kill her lol

            if (ui_face  != null) ui_face.SetState("Sad");
            if (ui_timer != null) ui_timer.Stop();

            foreach (MinesweeperCell cell in mass_cells)
                if (cell.isBomb || cell.isMarkedAsFlag)
                    cell.Reveal();
        }

        public void RegisterMarkedFlag(int add_value)
        {
            if (ui_counter != null)
                ui_counter.GetCountBombs += add_value;
        }

        public void RegisterOpenedCell()
        {
            count_empty_cells--;
            if (count_empty_cells <= 0 && !MeliHelperModule.Settings.Debug_MinesweeperSolvedFromStart)
            {
                // You're win!
                // Teleport yourself out bitch


                // TODO: 
                // 1+ set smile face to win face
                // 2+ stop nearest timer
                // 3+. mark all unmarked bomb cells as flag
                // 4+. if room != "" then teleport yourself

                if (ui_timer != null) ui_timer.Stop();
                if (ui_face != null) ui_face.SetState("Win");

                foreach (var item in mass_cells)
                    if (item.isBomb)
                        item.SetMark(Minesweeper_CellMark.Flag);

                if (room_win != "")
                    SceneAs<Level>().Add(new CutsceneRoomTeleport(room_win, Vector2.Zero));
            }
        }

        public void OpenAllNeighborCells(MinesweeperCell cell)
        {
            int x = -1, y = -1;
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                    if (mass_cells[i, j] == cell)
                    {
                        x = i;
                        y = j;
                        break;
                    }

            if (x >= 0)
            {
                // Found, open it!
                for (int i = -1; i <= 1; i++)
                    for (int j = -1; j <= 1; j++)
                        if (isInField(x + i, y + j) && !list_reserved_cells.Contains(mass_cells[x + i, y + j]) && !mass_cells[x + i, y + j].isOpened)
                            list_reserved_cells.Add(mass_cells[x + i, y + j]);
            }
        }

        bool isInField(int x, int y)
        {
            return x >= 0 && x < w && y >= 0 && y < h;
        } 

    }
}
