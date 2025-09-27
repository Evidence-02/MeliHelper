using Celeste;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    [CustomEntity("MeliHelper/BattleCityField")]
    class Field : Entity
    {
        public static Field Instance;
        public const int PIX_CELL = 16;
        public const int PIX_TILE = 4;

        Level level;
        FieldCell[,] mass_cells;
        BCEnum_GameState state;
        BCEnum_Goal goal;
        BCEnum_FinishEvent event_finish;
        BCEnum_BackgroundType background_type;
        MTexture texture1, texture2;
        string name, type_ui, next_level, return_level;
        int id, w, h;
        float background_opacity;
        bool is_show_ui, is_show_intro, is_have_dirt;

        EventUI ui_events;
        FieldEnemiesComponent component_enemies;
        FieldItemComponent component_items;
        //FieldBzzzzComponent component_bzzzz;

        public Field(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            Instance = this;
            id = data.Int("levelID", 1);
            //name = data.Attr("name", Dialog.Get("EVIDENCE02_BATTLECITY_LEVELNAME_DEFAULT").Replace("#ID", id.ToString()));
            name = data.Attr("name", "");
            if (name == "") name = Dialog.Get("EVIDENCE02_BATTLECITY_LEVELNAME_DEFAULT");
            name = name.Replace("#ID", id.ToString());

            next_level = data.Attr("nextLevel", "a-00");
            return_level = data.Attr("returnLevel", "a-00");
            background_opacity = data.Float("backgroundOpacity", 0.75f);
            background_type = (BCEnum_BackgroundType)Enum.Parse(typeof(BCEnum_BackgroundType), data.Attr("backgroundType", "Default"));
            type_ui = data.Attr("typeUI", "Default");
            is_show_ui = data.Bool("showUI", true);
            is_show_intro = data.Bool("showIntro", true);
            texture1 = GFX.Game["Evidence02/objects_bc/tiles/shadowBrick01"];
            texture2 = GFX.Game["Evidence02/objects_bc/tiles/shadowBrick02"];

            goal = BCEnum_Goal.Nothing;
            switch (data.Attr("goal", "Kill enemies"))
            {
                case "Kill enemies":    goal = BCEnum_Goal.KillEnemies; break;
                case "Collect storby":  goal = BCEnum_Goal.CollectStorby; break;
            }
            
            event_finish = BCEnum_FinishEvent.Nothing;
            switch (data.Attr("finishEvent", "Default"))
            {
                case "Default":
                case "Endscreen": event_finish = BCEnum_FinishEvent.Endscreen; break;
                case "Fast teleport": event_finish = BCEnum_FinishEvent.FastTeleport; break;
            }

            component_enemies = new FieldEnemiesComponent(this, data);
            //Add(component_enemy = new FieldEnemyComponent(this, data));
            //Add(component_bzzzz = new FieldBzzzzController());
            Add(component_items = new FieldItemComponent());
            //Add(component_test = new FieldTestComponent());
            
            w = (PIX_CELL / PIX_TILE) * data.Int("fieldWidth",  13);
            h = (PIX_CELL / PIX_TILE) * data.Int("fieldHeight", 13);
            mass_cells = new FieldCell[w, h];
            Depth = DepthController.BC_FIELD_BACKGROUND;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = scene as Level;
            level.Add(ui_events = new EventUI(type_ui));
            ui_events.Visible = is_show_ui;
            Add(component_enemies);

            //level.Add(new FieldDebugPlayerTracker(this));
            //level.Add(new FieldDebugCellTracker(this));
            
            //FontController.Load();
            TextureController.Load();
            BonusesController.SetDefault();
            EnemyTypesController.ResetToDefault();
            MeliHelperModule.Instance.Session.BattleCity_CustomRules = null;
            BCController.SetHooksLoaded();

            if (background_type == BCEnum_BackgroundType.Default)
            {
                level.Add(new Solid(Position + new Vector2(-16, -16), (w + 8) * PIX_TILE, 16, false));
                level.Add(new Solid(Position + new Vector2(-16, h * PIX_TILE), (w + 8) * PIX_TILE, 16, false));
                level.Add(new Solid(Position + new Vector2(-16, 0), 16, h * PIX_TILE, false));
                level.Add(new Solid(Position + new Vector2(w * PIX_TILE, 0), 16, h * PIX_TILE, false));
            }
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            if (is_show_intro && !MeliHelperModule.Instance.Session.BattleCity_StartedLevelsID.Contains(this.id) || MeliHelperModule.Settings.Debug_IntroEverytime)
            {
                MeliHelperModule.Instance.Session.BattleCity_StartedLevelsID.Add(this.id);
                level.Add(new BCCutsceneIntro(this, level.Tracker.GetEntity<Player>(), name));
                SetState(BCEnum_GameState.Pause);   // debug!!! // why is it not working on Cutscene ?
            }
            ProgressController.LoadProgress();
        }

        public override void Update()
        {
            component_enemies.Update();
            if (is_have_dirt)
            {
                foreach (Player player in level.Tracker.GetEntities<Player>())
                    if (Methods.PlayerIsAlive(player) && level.Entities.FindAll<FieldCellDirt>().Exists(t => t.isCollideEntity(player)))
                        player.Speed *= (1f + 4.5f * Engine.DeltaTime);

                foreach (Enemy enemy in level.Entities.FindAll<Enemy>())
                    if (level.Entities.FindAll<FieldCellDirt>().Exists(t => t.isCollideEntity(enemy)))
                        enemy.UpdateOnDirt();
            }
        }

        public override void Render()
        {
            base.Render();
            if (background_type == BCEnum_BackgroundType.Default)
            {
                Draw.Rect(GetTilePosition(0, 0), w * PIX_TILE, h * PIX_TILE, Color.Black * background_opacity);
                for (int i = -4; i < w + 4; i++)
                {
                    // Top and bottom
                    for (int j = -4; j < 0; j++) DrawBackgroundTile(i, j);
                    for (int j = h; j < h + 4; j++) DrawBackgroundTile(i, j);
                }

                for (int j = 0; j < h; j++)
                {
                    // Left and right
                    for (int i = -4; i < 0; i++) DrawBackgroundTile(i, j);
                    for (int i = w; i < w + 4; i++) DrawBackgroundTile(i, j);
                }
            }
        }

        void DrawBackgroundTile(int i, int j)
        {
            if ((i + j) % 2 == 0) texture1.Draw(GetTilePosition(i, j));
            else texture2.Draw(GetTilePosition(i, j));
        }

        public void SetState(BCEnum_GameState state)
        {
            this.state = state;
            if (state == BCEnum_GameState.Gameover)
                level.Add(new BCCutsceneWin(this, level.Tracker.GetEntity<Player>(), name, return_level, true));
                //level.Add(new BCCutsceneGameover(this, return_level));
        }

        public EventUI GetEventUI
        {
            get
            {
                return ui_events;
            }
        }

        public FieldEnemiesComponent GetEnemiesComponent
        {
            get
            {
                return component_enemies;
            }
        }

        public FieldItemComponent GetItemComponent
        {
            get
            {
                return component_items;
            }
        }

        public Vector2 GetPositionForBonus()
        {
            int tx, ty, counter = 10;
            do
            {
                tx = 4 * Calc.Random.Next(1, w / 4 - 1);
                ty = 4 * Calc.Random.Next(1, h / 4 - 1);
            }
            while (    counter-- > 0 
                    && isKindaUnreachable(tx, ty) 
                    && isKindaUnreachable(tx-2, ty) 
                    && isKindaUnreachable(tx, ty-2)
                    && isKindaUnreachable(tx-2, ty-2)
                    );

            if (counter <= 0)
            {
                // Too much empty tries, create map of the field and find at least 1 good cell
                List<int> list_cells = new List<int>();
                for (int i = 4; i <= w - 4; i += 4)
                    for (int j = 4; j <= h - 4; j += 4)
                        if (!isKindaUnreachable(i, j) || !isKindaUnreachable(i-2, j) || !isKindaUnreachable(i, j-2) || !isKindaUnreachable(i-2, j-2))
                            list_cells.Add(i * 1000 + j);
                    
                if (list_cells.Count == 0)
                    ShowDebugError("Can't find good cell for bonus");
                else
                {
                    int id = Methods.GetRandomizer().Next(0, list_cells.Count);
                    tx = list_cells[id] / 1000;
                    ty = list_cells[id] % 1000;
                }
            }
            return GetTilePosition(tx, ty);
        }

        public BCEnum_CellType GetCellType(int tx, int ty)
        {
            return (mass_cells[tx, ty] != null) ? mass_cells[tx, ty].GetCellType : BCEnum_CellType.Empty;
        }

        public bool isActualSolid(int tx, int ty)
        {
            return isActualSolid(GetCellType(tx, ty));
        }

        public bool isActualSolid(BCEnum_CellType type)
        {
            return type == BCEnum_CellType.Brick || type == BCEnum_CellType.Steel || type == BCEnum_CellType.Blocked;
        }

        public bool isWalkable(BCEnum_CellType type)
        {
            return type == BCEnum_CellType.Empty || type == BCEnum_CellType.Grass || type == BCEnum_CellType.Dirt;
        }

        public bool isKindaUnreachable(int tx, int ty)
        {
            return isKindaUnreachable(GetCellType(tx, ty));
        }

        public bool isKindaUnreachable(BCEnum_CellType type)
        {
            return type == BCEnum_CellType.Steel || type == BCEnum_CellType.Water || type == BCEnum_CellType.Blocked;
        }

        public void CheckFinish(BCEnum_Goal check_goal)
        {
            if (this.goal == check_goal)
                switch (event_finish)
                {
                    case BCEnum_FinishEvent.Endscreen:    level.Add(new BCCutsceneWin(this, level.Tracker.GetEntity<Player>(), name, next_level)); break;
                    case BCEnum_FinishEvent.FastTeleport: level.Add(new CutsceneRoomTeleport(next_level, Vector2.Zero, Player.IntroTypes.None)); break;
                }
        }

        public BCEnum_GameState GetGameState
        {
            get
            {
                return state;
            }
        }

        public void ShowDebugError(string err)
        {
            ui_events.SetError(err);
        }

        public void ShowDebugInfo(string err, bool is_first_label = true)
        {
            if (is_first_label) ui_events.SetInfo(err);
            else                ui_events.SetInfo2(err);
        }



        #region Coordinates

        // default field size 
        // in tiles: 52x52 
        // in cells: 13x13
        public Vector2 GetTilePosition(int cx, int cy)
        {
            return Position + PIX_TILE * new Vector2(cx, cy);
        }

        public int GetTileCX(Vector2 position)
        {
            return (int)((position.X - this.Position.X) / PIX_TILE);
        }

        public int GetTileCY(Vector2 position)
        {
            return (int)((position.Y - this.Position.Y) / PIX_TILE);
        }

        public Vector2 GetCellPosition(int cx, int cy)
        {
            return Position + PIX_CELL * new Vector2(cx, cy);
        }

        public int GetCellCX(Vector2 position)
        {
            return (int)((position.X - this.Position.X) / PIX_CELL);
        }

        public int GetCellCY(Vector2 position)
        {
            return (int)((position.Y - this.Position.Y) / PIX_CELL);
        }

        public bool isInField(int tx, int ty)
        {
            // w, h - sizes in tiles
            return tx >= 0 && tx < w && ty >= 0 && ty < h;
        }

        public bool isInField(Vector2 position)
        {
            return isInField(
                (int)((position.X - this.Position.X) / PIX_TILE),
                (int)((position.Y - this.Position.Y) / PIX_TILE));
        }

        public int GetW
        {
            get
            {
                return w;
            }
        }

        public int GetH
        {
            get
            {
                return h;
            }
        }

        #endregion

        #region Cells
        
        public void AddCell(BCEnum_CellType cell_type, Vector2 position)
        {
            int tx = GetTileCX(position);
            int ty = GetTileCY(position);
            if (!isInField(tx, ty))
                return;

            if (cell_type == BCEnum_CellType.Brick)
            {
                for (int i = 0; i < 2; i++)
                    for (int j = 0; j < 2; j++)
                    {
                        FieldCell cell = new FieldCellBrick(
                            GetTilePosition(tx + i, ty + j),
                            TextureController.GetBrickTile((tx + i + ty + j) % 2 == 0));
                        mass_cells[tx + i, ty + j] = cell;
                        level.Add(cell);
                    }
            }
            else
            {
                Vector2 pos = GetTilePosition(tx, ty);
                FieldCell cell = null;
                switch (cell_type)
                {
                    case BCEnum_CellType.Steel: cell = new FieldCellSteel(pos); break;
                    case BCEnum_CellType.Grass: cell = new FieldCellGrass(pos); break;
                    case BCEnum_CellType.Water: cell = new FieldCellWater(pos); break;
                    case BCEnum_CellType.Dirt:    cell = new FieldCellDirt(pos); is_have_dirt = true;  break;
                    case BCEnum_CellType.Blocked: cell = new FieldCellBlocked(pos); break;
                }

                if (cell != null)
                {
                    level.Add(cell);
                    for (int i = 0; i < 2; i++)
                        for (int j = 0; j < 2; j++)
                            mass_cells[tx + i, ty + j] = cell;
                }
            }
        }

        public void AddCellBrickTile(int tx, int ty, MTexture texture = null)
        {
            if (!isInField(tx, ty))
                return;
            if (texture == null)
                texture = TextureController.GetBrickTile((tx + ty) % 2 == 0);
            
            FieldCell cell = new FieldCellBrick(GetTilePosition(tx, ty), texture);
            mass_cells[tx, ty] = cell;
            level.Add(cell);
        }

        public void AddCellAnyTile(BCEnum_CellType cell_type, int tx, int ty, MTexture texture = null)
        {
            if (!isInField(tx, ty))
                return;
            if (texture == null) texture = TextureController.GetCellTypeTexture(cell_type);

            Vector2 pos = GetTilePosition(tx, ty);
            FieldCell cell = null;
            switch (cell_type)
            {
                case BCEnum_CellType.Steel: cell = new FieldCellSteel(pos, texture); break;
                case BCEnum_CellType.Grass: cell = new FieldCellGrass(pos, texture); break;
                case BCEnum_CellType.Water: cell = new FieldCellWater(pos, texture); break;
                case BCEnum_CellType.Dirt: cell = new FieldCellDirt(pos, texture); is_have_dirt = true; break;
                //case BCEnum_CellType.Blocked: cell = new FieldCellBlocked(pos); break;
            }
            if (cell != null)
            {
                level.Add(cell);
                for (int i = 0; i < 2; i++)
                    for (int j = 0; j < 2; j++)
                        mass_cells[tx + i, ty + j] = cell;
            }
        }

        public void RemoveCell(Vector2 position)
        {
            int tx = GetTileCX(position);
            int ty = GetTileCY(position);
            for (int dx = 0; dx < 2; dx++)
                for (int dy = 0; dy < 2; dy++)
                    if (mass_cells[tx + dx, ty + dy] != null)
                    {
                        mass_cells[tx + dx, ty + dy].RemoveSelf();
                        mass_cells[tx + dx, ty + dy] = null;
                    }
        }

        public void DestroyCells(Rectangle rect)
        {
            for (int tx = rect.Left; tx < rect.Right; tx++)
                for (int ty = rect.Top; ty < rect.Bottom; ty++)
                    if (mass_cells[tx, ty] != null)
                    {
                        mass_cells[tx, ty].RemoveSelf();
                        mass_cells[tx, ty] = null;
                    }
        }

        #endregion

        #region Wall collisions
        
        public Rectangle GetWallCollisionsToRender(Vector2 brick_center, Vector2 dir, bool is_can_destroy_steel)
        {
            Rectangle rect = CheckWallCollisions(brick_center, dir, is_can_destroy_steel);
            return new Rectangle(
                (int)Position.X + PIX_TILE * rect.X,
                (int)Position.Y + PIX_TILE * rect.Y,
                rect.Width * PIX_TILE,
                rect.Height * PIX_TILE);
        }

        public Rectangle CheckWallCollisionsByDash(Vector2 player_center, Vector2 dir, int depth = 5)
        {
            int tx = GetTileCX(player_center);
            int ty = GetTileCY(player_center);
            int dx = (int)dir.X;
            int dy = (int)dir.Y;

            // Check if collided?
            BCEnum_CellType cell = BCEnum_CellType.Empty;
            for (int i = 0; i < depth; i++)
            {
                if (!isInField(tx, ty)) return Rectangle.Empty;
                if (GetCellType(tx, ty) == BCEnum_CellType.Brick || GetCellType(tx, ty) == BCEnum_CellType.Steel && ProgressController.isPlayerCanDestroySteel())
                {
                    cell = GetCellType(tx, ty);
                    break;
                }

                tx += dx;
                ty += dy;
            }
            if (cell == BCEnum_CellType.Empty)
                return Rectangle.Empty;


            return CheckWallCollisions(GetTilePosition(tx, ty), dir, cell == BCEnum_CellType.Steel);
        }

        public Rectangle CheckWallCollisions(Vector2 brick_center, Vector2 dir, bool is_steel)
        {
            int tx = GetTileCX(brick_center);
            int ty = GetTileCY(brick_center);

            BCEnum_CellType cell = is_steel ? BCEnum_CellType.Steel : BCEnum_CellType.Brick;
            int mg = 4;
            int tw = 1;
            int th = 1;
            if (dir.X == 0)
            {
                // Vertical dash/shoot 

                // Check left cells
                int ddy = dir.Y < 0 ? +1 : -1;
                int move_left = (brick_center.X % mg < mg / 2) ? 2 : 1;
                for (int i = 0; i < move_left; i++)
                    if (isInField(tx - 1, ty) && GetCellType(tx - 1, ty) == cell && !isActualSolid(tx - 1, ty + ddy))
                    {
                        tx--;
                        tw++;
                    }

                // Move to the right cells, need 4 to destroy
                for (int i = tw; i < 4; i++)
                    if (isInField(tx + tw, ty) && GetCellType(tx + tw, ty) == cell && !isActualSolid(tx + tw, ty + ddy))
                        tw++;

                // Check left again, if needed
                for (int i = tw; i < 4; i++)
                    if (isInField(tx - 1, ty) && GetCellType(tx - 1, ty) == cell && !isActualSolid(tx - 1, ty + ddy))
                    {
                        tx--;
                        tw++;
                    }

            }
            else
            {
                // Horizontal dash/shoot 

                // Literally the same code, but horiz and vert are inverted
                // Check top cells
                int ddx = dir.X < 0 ? +1 : -1;
                int move_up = (brick_center.Y % mg < mg / 2) ? 2 : 1;
                for (int i = 0; i < move_up; i++)
                    if (isInField(tx, ty - 1) && GetCellType(tx, ty - 1) == cell && !isActualSolid(tx + ddx, ty - 1))
                    {
                        ty--;
                        th++;
                    }

                // Move to the bottom cells, need 4 to destroy
                for (int i = th; i < 4; i++)
                    if (isInField(tx, ty + th) && GetCellType(tx, ty + th) == cell && !isActualSolid(tx + ddx, ty + th))
                        th++;

                // Check top again, if needed
                for (int i = th; i < 4; i++)
                    if (isInField(tx, ty - 1) && GetCellType(tx, ty - 1) == cell && !isActualSolid(tx + ddx, ty - 1))
                    {
                        ty--;
                        th++;
                    }
            }

            // Destroy the whole cell
            if (is_steel)
            {
                if (tx % 2 > 0)
                {
                    tw += tx % 2;
                    tx -= tx % 2;
                }
                if (ty % 2 > 0)
                {
                    th += ty % 2;
                    ty -= ty % 2;
                }

                if (tw % 2 > 0) tw += 2 - tw % 2;
                if (th % 2 > 0) th += 2 - th % 2;
            }

            // Correct to the whole cell
            if (MeliHelperModule.Instance.Session.BattleCity_CustomRules != null && MeliHelperModule.Instance.Session.BattleCity_CustomRules.isShootOnlyCenter)
            {
                tx -= tx % 4;
                ty -= ty % 4;
                tw = 4;
                th = 4;
            }

            return new Rectangle(tx, ty, tw, th);
        }





        public bool isZoneFree(int tx, int ty, int sx = 4, int sy = 4)
		{
			// Check current object position
			for (int i = 0; i < sx; i++)
				for (int j = 0; j < sy; j++)
					if (!isInField(tx + i, ty + j) || !isWalkable(GetCellType(tx + i, ty + j)))
						return false;
			
			return true;
		}
		
        #endregion
        
    }
}
