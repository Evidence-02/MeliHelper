using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System.Collections;

namespace Celeste.Mod.MeliHelper
{
    [CustomEntity("MeliHelper/RubikCube")]
    class RubikCube : Entity
    {
        RCMEnum_CubeType type;
        RCMEnum_CubeState state;
        List<Axis> list_axis;
        List<Face> list_faces;
        List<Sticker> list_stickers;
        CubeProjection projection;
        RotationGrid grid;
        Color COLOR_UP, COLOR_DOWN, COLOR_LEFT, COLOR_RIGHT, COLOR_FRONT, COLOR_BACK, COLOR_BORDER, COLOR_INSIDE;
        Journal journal;
        RotationEvent event_rotate;
        string id;
        bool is_can_rotate_x, is_can_rotate_y, is_can_rotate_z, is_rotate_clockwise;
        bool is_generate_borders, is_generate_insides, is_flickering, is_short_name_in_journal, is_solved_from_start;
        int id_scramble, count_points_on_solve;
        float rotate_time_manual, rotate_time_scramble, cube_size, delay_scramble;
        int[] scramble_rotates;

        public RubikCube(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            DashController.Load();
            list_axis = new List<Axis>();
            list_faces = new List<Face>();
            list_stickers = new List<Sticker>();
            journal = new Journal();

            COLOR_UP     = Calc.HexToColor(data.Attr("colorUp",    "FFFFFF"));   // White
            COLOR_DOWN   = Calc.HexToColor(data.Attr("colorDown",  "FFFF00"));   // Yellow
            COLOR_LEFT   = Calc.HexToColor(data.Attr("colorLeft",  "FF8800"));   // Orange
            COLOR_RIGHT  = Calc.HexToColor(data.Attr("colorRight", "FF0000"));   // Red
            COLOR_FRONT  = Calc.HexToColor(data.Attr("colorFront", "00FF00"));   // Green
            COLOR_BACK   = Calc.HexToColor(data.Attr("colorBack",  "0000FF"));   // Blu
            COLOR_BORDER = Calc.HexToColor(data.Attr("colorBorders", "222222"));   // Black
            COLOR_INSIDE = Calc.HexToColor(data.Attr("colorInside",  "000000"));   // Black
            is_generate_borders = data.Bool("generateBorders", true);
            is_generate_insides = data.Bool("generateInsides", false);
            is_flickering       = data.Bool("flickering",      false);

            rotate_time_manual   = data.Float("rotateTime", 0.40f);
            rotate_time_scramble = data.Float("scrambleRotateTime", 0.08f);
            int count_scrambles_max = data.Int("scrambleRotates", 50);
            cube_size = data.Float("cubeSize", 50);
            count_points_on_solve = data.Int("pointsOnSolve", 100);
            delay_scramble = data.Float("delayScramble", 1.2f);
            scramble_rotates = new int[count_scrambles_max];

            this.type = RCMEnum_CubeType.Cube333;
            switch (data.Attr("cubeType"))
            {
                case "2x2x2": type = RCMEnum_CubeType.Cube222; break;
                case "2x2x3": type = RCMEnum_CubeType.Cube223; break;
                case "2x3x3": type = RCMEnum_CubeType.Cube233; break;
                case "3x3x3": type = RCMEnum_CubeType.Cube333; break;
                case "Mirror 2x2":     type = RCMEnum_CubeType.Mirror22;      break;
                case "Mirror 3x3":     type = RCMEnum_CubeType.Mirror;        break;
                case "Mirror Floppy":  type = RCMEnum_CubeType.MirrorFloppy;  break;
                case "Floppy Cube":    type = RCMEnum_CubeType.FloppyCube;    break;
                case "Dino Cube":      type = RCMEnum_CubeType.DinoCube;      break;
                case "Skewb":          type = RCMEnum_CubeType.Skewb;         break;
                case "Clover Cube":    type = RCMEnum_CubeType.CloverCube;    break;
                case "Petal Pyraminx": type = RCMEnum_CubeType.PetalPyraminx; break;
                case "Axis Cube":      type = RCMEnum_CubeType.AxisCube;      break;
                case "Kilominx":       type = RCMEnum_CubeType.Kilominx;      break;
                case "Megaminx":       type = RCMEnum_CubeType.Megaminx;      break;
            }

            this.id = data.Attr("cubeID");
            if (this.id == "") this.id = type.ToString() + "_" + data.ID;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Level level = scene as Level;
            DashController.TryLoadScheme(level, RCMEnum_ControlScheme.Main);
            SeedController.Initialize();

            /*
            //////////
            // load //
            bool restorePrevious = MeliHelperModule.Instance.Session.CubeInfo.ContainsKey(id)
                                && (MeliHelperModule.Instance.Session.CubeInfo[id].isCubeSolved
                                    || MeliHelperModule.Instance.Session.RestoreCubesOnRoomEnter && MeliHelperModule.Instance.Session.CubeInfo[id].GetSeed == SeedController.GetCurrentSeed());
            if (!restorePrevious)
                MeliHelperModule.Instance.Session.CubeInfo[id] = new CubeSessionInfo() { GetSeed = SeedController.GetCurrentSeed(), isCubeSolved = false, GetJournal = new Journal() };

            CubeSessionInfo info = MeliHelperModule.Instance.Session.CubeInfo[id];
            if (info.isCubeSolved) { ... }
            else if (restorePrevious && false)
            {
                state = CubeStateEnum.RestorePrevious;
                journal = info.GetJournal;
                foreach (JournalMove move in info.GetJournal.GetListMoves)
                {
                }
                state = CubeStateEnum.Normal;
            }
            else { ... }
             */
            
            is_solved_from_start = MeliHelperModule.Instance.Session.CubeInfo.ContainsKey(id) && MeliHelperModule.Instance.Session.CubeInfo[id].isCubeSolved;
            if (!is_solved_from_start)
                MeliHelperModule.Instance.Session.CubeInfo[id] = new CubeSessionInfo() { GetSeed = SeedController.GetCurrentSeed(), isCubeSolved = false, GetJournal = new JournalSession() };

            if (is_solved_from_start)
            {
                CubeSessionInfo info = MeliHelperModule.Instance.Session.CubeInfo[id];
                Clear();
                foreach (JournalSessionMove move in info.GetJournal.GetListMoves)
                    this.journal.AddMove(move);
                state = RCMEnum_CubeState.Solved;
            }
            else
            {
                id_scramble = 0;
                Clear();
                state = RCMEnum_CubeState.Scramble;

                SeedController.RestartRandomizer();
                for (int i = 0; i < scramble_rotates.Length; i++)
                {
                    int id = Calc.Random.Next(list_axis.Count);
                    bool is_clockwise = Calc.Random.NextDouble() < 0.5f;
                    scramble_rotates[i] = id * (is_clockwise ? 1 : -1);
                }
                SeedController.StopRandomizer();
            }
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);

            Level level = scene as Level;
            Methods.StayBehindPlayer(level, this);
            if (is_solved_from_start)
            {
                foreach (var item in level.Entities.FindAll<RubikMoveTrigger>())
                    item.RemoveSelf();
                foreach (var item in level.Entities.FindAll<RubikCubeProjectionEntity>())
                    item.RemoveSelf();
            }
        }

        public void Descramble()
        {
            if (state == RCMEnum_CubeState.Normal)
                state = RCMEnum_CubeState.Descramble;
        }
        
        public void Clear()
        {
            list_axis.Clear();
            list_faces.Clear();
            list_stickers.Clear();
            journal.Clear();
            state = RCMEnum_CubeState.Normal;
            grid = null;
            is_can_rotate_x = true;
            is_can_rotate_y = true;
            is_can_rotate_z = true;
            is_short_name_in_journal = (this.type == RCMEnum_CubeType.MirrorFloppy || this.type == RCMEnum_CubeType.CloverCube);

            bool is_create_main_axises = false;
            switch (this.type)
            {
                case RCMEnum_CubeType.DinoCube:      CreateDinoCube();      is_create_main_axises = true; break;
                case RCMEnum_CubeType.Skewb:         CreateSkewb();         is_create_main_axises = true; break;
                case RCMEnum_CubeType.CloverCube:    CreateCloverCube();    is_create_main_axises = true; break;
                case RCMEnum_CubeType.AxisCube:      CreateAxisCube();      break;
                case RCMEnum_CubeType.PetalPyraminx: CreatePetalPyraminx(); break;
                case RCMEnum_CubeType.Kilominx:      CreateKilominx();      break;
                case RCMEnum_CubeType.Megaminx:      CreateKilominx();      break;
                case RCMEnum_CubeType.FloppyCube:    CreateFloppyCube();    break;
                case RCMEnum_CubeType.Cube222:       CreateCube(2, 2, 2);   is_create_main_axises = true; break;
                case RCMEnum_CubeType.Cube223:       CreateCube(2, 3, 2);   is_create_main_axises = true; break;
                case RCMEnum_CubeType.Cube233:       CreateCube(3, 2, 3);   is_create_main_axises = true; break;
                case RCMEnum_CubeType.Cube333:       CreateCube(3, 3, 3);   is_create_main_axises = true; break;
                //case CubeTypeEnum.Cube234: GenerateCube(3, 2, 4); break;    
                //case CubeTypeEnum.Cube555: GenerateCube(5, 5, 5); break;
                case RCMEnum_CubeType.Mirror22:     CreateMirror222();      is_create_main_axises = true; break;    
                case RCMEnum_CubeType.Mirror:       CreateMirror333();      is_create_main_axises = true; break;
                case RCMEnum_CubeType.MirrorFloppy: CreateMirrorFloppy(); break;
            }

            if (is_create_main_axises)
            {
                list_axis.Add(new AxisSurface(Vector3.Zero, new Vector3(0, 1, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0), RCMEnum_AxisType.AxisX, RotateCountForAxis('Y'), RCMEnum_AxisDirection.All));
                list_axis.Add(new AxisSurface(Vector3.Zero, new Vector3(0, 0, 1), new Vector3(1, 0, 0), new Vector3(0, 1, 0), RCMEnum_AxisType.AxisY, RotateCountForAxis('Z'), RCMEnum_AxisDirection.All));
                list_axis.Add(new AxisSurface(Vector3.Zero, new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1), RCMEnum_AxisType.AxisZ, RotateCountForAxis('X'), RCMEnum_AxisDirection.All));
            }

            foreach (var item in list_stickers)
                Add(item);
            
            // FACES
            GenerateFaces();
            

            Color recolor = Color.Black;
            if (type == RCMEnum_CubeType.Mirror || type == RCMEnum_CubeType.Mirror22) recolor = Color.White;
            if (type == RCMEnum_CubeType.MirrorFloppy) recolor = Color.Gold;
            if (recolor != Color.Black)
                foreach (var item in list_stickers.FindAll(t => t.isFaceSticker))
                    item.SetColor(recolor);



            



            if ((type == RCMEnum_CubeType.Mirror22 || type == RCMEnum_CubeType.Mirror)) projection = new CubeProjection1(this.Center, list_stickers);
            else                                                                projection = new CubeProjection1(this.Center, list_stickers);
            
            //if (type == CubeTypeEnum.Mirror22 || type == CubeTypeEnum.Mirror)
            //    foreach (StickerProjection proj in projection.GetStickerProjections)
            //        proj.SetSlices(0.005f * MeliHelperModule.Settings.CubeSizeKoef0005 * cube_size);
        }

        #region cubes

        void CreateCube(int lenx, int leny, int lenz, bool is_generate_axis = true)
        {
            Vector3[,,] list_coords = Methods.GenerateVectorArray(Vector3.Zero, cube_size / leny, lenx + 1, leny + 1, lenz + 1);
            CreateCube(list_coords, is_generate_axis: is_generate_axis);
        }

        void CreateCube(Vector3[,,] list_coords, bool is_generate_axis = true)
        {
            int lenx = list_coords.GetLength(0) - 1;
            int leny = list_coords.GetLength(1) - 1;
            int lenz = list_coords.GetLength(2) - 1;
            is_can_rotate_x = (lenx == leny);
            is_can_rotate_y = (leny == lenz);
            is_can_rotate_z = (lenz == lenx);

            for (int x = 0; x < lenx; x++)
                for (int y = 0; y < leny; y++)
                {
                    list_stickers.Add(new Sticker(COLOR_FRONT, list_coords[x, y, lenz], list_coords[x + 1, y, lenz], list_coords[x + 1, y + 1, lenz], list_coords[x, y + 1, lenz]));
                    list_stickers.Add(new Sticker(COLOR_BACK,  list_coords[x, y, 0],    list_coords[x + 1, y, 0],    list_coords[x + 1, y + 1, 0],    list_coords[x, y + 1, 0]));
                }

            for (int x = 0; x < lenx; x++)
                for (int z = 0; z < lenz; z++)
                {
                    list_stickers.Add(new Sticker(COLOR_UP,   list_coords[x, leny, z], list_coords[x + 1, leny, z], list_coords[x + 1, leny, z + 1], list_coords[x, leny, z + 1]));
                    list_stickers.Add(new Sticker(COLOR_DOWN, list_coords[x, 0, z],    list_coords[x + 1, 0, z],    list_coords[x + 1, 0, z + 1],    list_coords[x, 0, z + 1]));
                }

            for (int y = 0; y < leny; y++)
                for (int z = 0; z < lenz; z++)
                {
                    list_stickers.Add(new Sticker(COLOR_RIGHT, list_coords[lenx, y, z], list_coords[lenx, y, z + 1], list_coords[lenx, y + 1, z + 1], list_coords[lenx, y + 1, z]));
                    list_stickers.Add(new Sticker(COLOR_LEFT,  list_coords[0, y, z],    list_coords[0, y, z + 1],    list_coords[0, y + 1, z + 1],    list_coords[0, y + 1, z]));
                }

            if (is_generate_insides)
            {
                float ki1 = 0.01f, ki2 = 1 - ki1;
                for (int z = 1; z < lenz; z++)
                    for (int x = 0; x < lenx; x++)
                        for (int y = 0; y < leny; y++)
                        {
                            list_stickers.Add(new Sticker(COLOR_INSIDE, 
                                ki1 * list_coords[x,     y,     z + 1] + ki2 * list_coords[x,     y,     z],
                                ki1 * list_coords[x + 1, y,     z + 1] + ki2 * list_coords[x + 1, y,     z],
                                ki1 * list_coords[x + 1, y + 1, z + 1] + ki2 * list_coords[x + 1, y + 1, z],
                                ki1 * list_coords[x,     y + 1, z + 1] + ki2 * list_coords[x,     y + 1, z]
                                )
                            { isFaceSticker = false });
                            list_stickers.Add(new Sticker(COLOR_INSIDE, 
                                ki1 * list_coords[x,     y,     z - 1] + ki2 * list_coords[x,     y,     z],
                                ki1 * list_coords[x + 1, y,     z - 1] + ki2 * list_coords[x + 1, y,     z],
                                ki1 * list_coords[x + 1, y + 1, z - 1] + ki2 * list_coords[x + 1, y + 1, z],
                                ki1 * list_coords[x,     y + 1, z - 1] + ki2 * list_coords[x,     y + 1, z]
                                )
                            { isFaceSticker = false });
                        }

                for (int y = 1; y < leny; y++)
                    for (int x = 0; x < lenx; x++)
                        for (int z = 0; z < lenz; z++)
                        {
                            list_stickers.Add(new Sticker(COLOR_INSIDE,
                                ki1 * list_coords[x,     y + 1, z    ] + ki2 * list_coords[x,     y, z],
                                ki1 * list_coords[x + 1, y + 1, z    ] + ki2 * list_coords[x + 1, y, z],
                                ki1 * list_coords[x + 1, y + 1, z + 1] + ki2 * list_coords[x + 1, y, z + 1],
                                ki1 * list_coords[x,     y + 1, z + 1] + ki2 * list_coords[x,     y, z + 1]
                                )
                            { isFaceSticker = false });
                            list_stickers.Add(new Sticker(COLOR_INSIDE,
                                ki1 * list_coords[x,     y - 1, z    ] + ki2 * list_coords[x,     y, z],
                                ki1 * list_coords[x + 1, y - 1, z    ] + ki2 * list_coords[x + 1, y, z],
                                ki1 * list_coords[x + 1, y - 1, z + 1] + ki2 * list_coords[x + 1, y, z + 1],
                                ki1 * list_coords[x,     y - 1, z + 1] + ki2 * list_coords[x,     y, z + 1]
                                )
                            { isFaceSticker = false });
                        }

                for (int x = 1; x < lenx; x++)
                    for (int y = 0; y < leny; y++)
                        for (int z = 0; z < lenz; z++)
                        {
                            list_stickers.Add(new Sticker(COLOR_INSIDE,
                                ki1 * list_coords[x + 1, y,     z    ] + ki2 * list_coords[x, y,     z],
                                ki1 * list_coords[x + 1, y,     z + 1] + ki2 * list_coords[x, y,     z + 1],
                                ki1 * list_coords[x + 1, y + 1, z + 1] + ki2 * list_coords[x, y + 1, z + 1],
                                ki1 * list_coords[x + 1, y + 1, z    ] + ki2 * list_coords[x, y + 1, z]
                                )
                            { isFaceSticker = false });
                            list_stickers.Add(new Sticker(COLOR_INSIDE,
                                ki1 * list_coords[x - 1, y,     z    ] + ki2 * list_coords[x, y,     z],
                                ki1 * list_coords[x - 1, y,     z + 1] + ki2 * list_coords[x, y,     z + 1],
                                ki1 * list_coords[x - 1, y + 1, z + 1] + ki2 * list_coords[x, y + 1, z + 1],
                                ki1 * list_coords[x - 1, y + 1, z    ] + ki2 * list_coords[x, y + 1, z]
                                )
                            { isFaceSticker = false });
                        }
            }

            if (is_generate_axis)
            {
                float x1 = list_coords[1, 0, 0].X;
                float x2 = list_coords[lenx - 1, 0, 0].X;
                float y1 = list_coords[0, 1, 0].Y;
                float y2 = list_coords[0, leny - 1, 0].Y;
                float z1 = list_coords[0, 0, lenz - 1].Z;
                float z2 = list_coords[0, 0, 1].Z;
                int d = 5;

                list_axis.Add(new AxisSurface(new Vector3(0, y2, 0), new Vector3(d, y2, 0), new Vector3(0, y2, d), new Vector3((x1 + x2) / 2, y2 + d, (z1 + z2) / 2), RCMEnum_AxisType.Up,    RotateCountForAxis('Z')));
                list_axis.Add(new AxisSurface(new Vector3(0, y1, 0), new Vector3(0, y1, d), new Vector3(d, y1, 0), new Vector3((x1 + x2) / 2, y1 + d, (z1 + z2) / 2), RCMEnum_AxisType.Down,  RotateCountForAxis('Z')));

                list_axis.Add(new AxisSurface(new Vector3(x1, 0, 0), new Vector3(x1, d, 0), new Vector3(x1, 0, d), new Vector3(x1 + d, (y1 + y2) / 2, (z1 + z2) / 2), RCMEnum_AxisType.Left,  RotateCountForAxis('Y')));
                list_axis.Add(new AxisSurface(new Vector3(x2, 0, 0), new Vector3(x2, 0, d), new Vector3(x2, d, 0), new Vector3(x2 + d, (y1 + y2) / 2, (z1 + z2) / 2), RCMEnum_AxisType.Right, RotateCountForAxis('Y')));

                list_axis.Add(new AxisSurface(new Vector3(0, 0, z1), new Vector3(0, d, z1), new Vector3(d, 0, z1), new Vector3((x1 + x2) / 2, (y1 + y2) / 2, z1 + d), RCMEnum_AxisType.Front, RotateCountForAxis('X')));
                list_axis.Add(new AxisSurface(new Vector3(0, 0, z2), new Vector3(d, 0, z2), new Vector3(0, d, z2), new Vector3((x1 + x2) / 2, (y1 + y2) / 2, z2 + d), RCMEnum_AxisType.Back,  RotateCountForAxis('X')));


                /*
                t = dist * (lenz - 2) / 2;
                list_axis.Add(new AxisSurface(new Vector3(0, 0, +t), new Vector3(0, tv, +t), new Vector3(tv, 0, +t), new Vector3(0, 0, +t + tv), RotateCountForAxis('X'), AxisDirectionEnum.Bottom, key: "F"));
                list_axis.Add(new AxisSurface(new Vector3(0, 0, -t), new Vector3(0, tv, -t), new Vector3(tv, 0, -t), new Vector3(0, 0, -t + tv), RotateCountForAxis('X'), AxisDirectionEnum.Top, key: "B"));

                t = dist * (lenx - 2) / 2;
                list_axis.Add(new AxisSurface(new Vector3(+t, 0, 0), new Vector3(+t, tv, 0), new Vector3(+t, 0, tv), new Vector3(+t + tv, 0, 0), RotateCountForAxis('Y'), AxisDirectionEnum.Top, key: "R"));
                list_axis.Add(new AxisSurface(new Vector3(-t, 0, 0), new Vector3(-t, tv, 0), new Vector3(-t, 0, tv), new Vector3(-t + tv, 0, 0), RotateCountForAxis('Y'), AxisDirectionEnum.Bottom, key: "L"));

                t = dist * (leny - 2) / 2;
                list_axis.Add(new AxisSurface(new Vector3(0, +t, 0), new Vector3(0, +t, tv), new Vector3(tv, +t, 0), new Vector3(0, +t + tv, 0), RotateCountForAxis('Z'), AxisDirectionEnum.Top, key: "U"));
                list_axis.Add(new AxisSurface(new Vector3(0, -t, 0), new Vector3(0, -t, tv), new Vector3(tv, -t, 0), new Vector3(0, -t + tv, 0), RotateCountForAxis('Z'), AxisDirectionEnum.Bottom, key: "D"));
                */
            }
        }

        void CreateMirror222_Test()
        {
            // X: 17 33
            // Y: 13 37
            // Z: 21 29
            float dist = cube_size / 2;
            Vector3[,,] list_coords = Methods.GenerateVectorArray(
                coords_x: new float[] { -0.6f * dist, 0, 1.4f * dist },
                coords_y: new float[] { -0.4f * dist, 0, 1.6f * dist },
                coords_z: new float[] { -0.8f * dist, 0, 1.2f * dist }
                );
            CreateCube(list_coords);
            
            //------//
            // Test //
            float ddddd = 0.05f * dist;
            foreach (Sticker sticker in list_stickers)
            {
                for (int i = 0; i < sticker.GetVertices.Length; i++)
                {
                    Vector3 v = sticker.GetCentralPoint();
                    sticker.GetVertices[i]      += ddddd * new Vector3(Math.Sign(v.X), Math.Sign(v.Y), Math.Sign(v.Z));
                    sticker.GetVerticesSaved[i] += ddddd * new Vector3(Math.Sign(v.X), Math.Sign(v.Y), Math.Sign(v.Z));
                }
            }
        }

        void CreateMirror222()
        {
            // X: 17 33
            // Y: 13 37
            // Z: 21 29
            float dist = cube_size / 2;
            Vector3[,,] list_coords = Methods.GenerateVectorArray(
                coords_x: new float[] { -0.6f * dist, 0, 1.4f * dist },
                coords_y: new float[] { -0.4f * dist, 0, 1.6f * dist },
                coords_z: new float[] { -0.8f * dist, 0, 1.2f * dist }
                );
            CreateCube(list_coords);

            List<float> list_grids = new List<float>();
            for (int i = -8; i <= 8; i++)
            {
                list_grids.Add(i * 0.2f * dist);
                //list_grids.Add(i * 0.198f * dist);
                //list_grids.Add(i * 0.202f * dist);
            }
            grid = new RotationGrid(list_grids.ToArray());
        }

        void CreateMirror333()
        {
            // X: 11 19 27
            // Y:  7 19 31
            // Z: 15 19 23
            float dist = cube_size / 2;
            Vector3[,,] list_coords = Methods.GenerateVectorArray(
                coords_x: new float[] { -0.8f * dist, -0.4f * dist, 0.4f * dist, 1.6f * dist },
                coords_y: new float[] { -0.6f * dist, -0.4f * dist, 0.4f * dist, 1.8f * dist },
                coords_z: new float[] { -1.0f * dist, -0.4f * dist, 0.4f * dist, 1.4f * dist }
                );
            CreateCube(list_coords);

            List<float> list_grids = new List<float>();
            for (int i = -10; i <= 10; i++)
            {
                list_grids.Add(i * 0.2f * dist);
                //list_grids.Add(i * 0.198f * dist);
                //list_grids.Add(i * 0.202f * dist);
            }
            grid = new RotationGrid(list_grids.ToArray());
        }

        void CreateMirrorFloppy()
        {
            int len = 5;

            Vector3[] vC = new Vector3[len];
            Vector3[] vL = new Vector3[len];
            Vector3[] vR = new Vector3[len];
            Vector3[] vCorner = new Vector3[4];
            double[] A = new double[len];
            double[] B = new double[len];

            float dist = 0.75f * cube_size;
            float height = 0.125f * dist;
            double dist_center = 0.55f * dist;
            double angle_start = 0.273f * Math.PI;
            for (int i = 0; i < len; i++)
            {
                double angle = angle_start + i * 2 * Math.PI / 5;
                double angle_perp = angle + Math.PI / 2;
                double x0 = dist_center * Math.Cos(angle);
                double y0 = dist_center * Math.Sin(angle);

                // y - y0 = k(x - x0), где k = tg(a)
                // y = kx - k * x0 + y0
                // A = tg(a)
                // B = y0 - tg(a) * x0
                A[i] = Math.Tan(angle_perp);
                B[i] = y0 - x0 * Math.Tan(angle_perp);
            }

            for (int i = 0; i < len; i++)
            {
                // y = ax + b
                // a1 * x + b1 = a2 * x + b2
                // (a2 - a1) * x = b1 - b2
                // x = (b1 - b2) / (a2 - a1)
                int i2 = (i + 1) % len;
                double xC = (B[i] - B[i2]) / (A[i2] - A[i]);
                double yC = A[i] * xC + B[i];
                vC[i] = new Vector3((float)xC, (float)yC, 0);
            }

            for (int i = 0; i < len; i++)
            {
                Vector3[] v_temp = new Vector3[4];
                double a = A[i], b = B[i];
                double x, y;

                x = dist;
                y = a * x + b;
                if (Math.Abs(y) <= dist)
                    v_temp[0] = new Vector3((float)x, (float)y, 0);

                x = -dist;
                y = a * x + b;
                if (Math.Abs(y) <= dist)
                    v_temp[1] = new Vector3((float)x, (float)y, 0);


                // y = a * x + b      >>>      x = (y - b) / a  
                y = dist;
                x = (y - b) / a;
                if (Math.Abs(x) <= dist)
                    v_temp[2] = new Vector3((float)x, (float)y, 0);

                y = -dist;
                x = (y - b) / a;
                if (Math.Abs(x) <= dist)
                    v_temp[3] = new Vector3((float)x, (float)y, 0);

                vL[i] = v_temp.First(t => t != Vector3.Zero);
                vR[i] = v_temp.Last(t => t != Vector3.Zero);
            }


            Vector3 vU = new Vector3(0, 0, -height);
            Vector3 vD = new Vector3(0, 0, +height);
            vCorner[0] = new Vector3(dist, dist, 0);
            vCorner[1] = new Vector3(dist, -dist, 0);
            vCorner[2] = new Vector3(-dist, -dist, 0);
            vCorner[3] = new Vector3(-dist, dist, 0);




            foreach (Vector3 del in new Vector3[] { vU, vD })
            {
                // center
                Color clr = (del.Z > 0) ? COLOR_UP : COLOR_DOWN;
                list_stickers.Add(new Sticker(clr, vC[0] + del, vC[1] + del, vC[2] + del, vC[3] + del, vC[4] + del) { isFaceSticker = false });

                // star
                list_stickers.Add(new Sticker(clr, vC[0] + del, vC[1] + del, vL[2] + del, vR[0] + del));
                list_stickers.Add(new Sticker(clr, vC[1] + del, vC[2] + del, vR[3] + del, vL[1] + del));
                list_stickers.Add(new Sticker(clr, vC[2] + del, vC[3] + del, vR[4] + del, vR[2] + del));
                list_stickers.Add(new Sticker(clr, vC[3] + del, vC[4] + del, vL[0] + del, vL[3] + del));
                list_stickers.Add(new Sticker(clr, vC[4] + del, vC[0] + del, vR[1] + del, vCorner[0] + del, vL[4] + del));

                // edges
                list_stickers.Add(new Sticker(clr, vC[0] + del, vR[0] + del, vR[1] + del));
                list_stickers.Add(new Sticker(clr, vC[1] + del, vL[1] + del, vCorner[3] + del, vL[2] + del));
                list_stickers.Add(new Sticker(clr, vC[2] + del, vR[2] + del, vCorner[2] + del, vR[3] + del));
                list_stickers.Add(new Sticker(clr, vC[3] + del, vL[3] + del, vCorner[1] + del, vR[4] + del));
                list_stickers.Add(new Sticker(clr, vC[4] + del, vL[4] + del, vL[0] + del));
            }

            // 2. Axises
            RCMEnum_AxisType[] edges = { RCMEnum_AxisType.UpLeft, RCMEnum_AxisType.DownLeft, RCMEnum_AxisType.Down, RCMEnum_AxisType.DownRight, RCMEnum_AxisType.UpRight };
            for (int i = 0; i < len; i++)
                list_axis.Add(new AxisSurface(
                    vC[i] + vU,
                    vC[i] + vD,
                    vC[(i + 1) % len] + vD,
                    (vC[i] + vC[(i + 1) % len]) / 2,
                    edges[i], 2));

            // 3. Borders
            Vector3[] mass_borders = {
                vR[0], vR[1], vCorner[0], vL[4], vL[0], vL[3], vCorner[1], vR[4], vR[2], vCorner[2], vR[3], vL[1], vCorner[3], vL[2], vR[0]
            };
            Color[] mass_colors = {
                COLOR_BACK, COLOR_BACK, COLOR_RIGHT, COLOR_RIGHT, COLOR_RIGHT, COLOR_RIGHT, COLOR_FRONT, COLOR_FRONT, COLOR_FRONT, COLOR_LEFT, COLOR_LEFT, COLOR_LEFT, COLOR_BACK, COLOR_BACK
            };
            for (int i = 0; i < mass_borders.Length - 1; i++)
            {
                list_stickers.Add(new Sticker(mass_colors[i], 
                    mass_borders[i] + vU,
                    mass_borders[i] + vD,
                    mass_borders[i + 1] + vD,
                    mass_borders[i + 1] + vU
                    ));
            }


            // 4. Y axis for lulz
            list_axis.Add(new AxisSurface(Vector3.Zero, new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1), RCMEnum_AxisType.AxisY, 5, RCMEnum_AxisDirection.All));
            //list_axis.Add(new AxisSurface(Vector3.Zero, new Vector3(0, 1, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0), EdgeTypeEnum.AxisX, RotateCountForAxis('Y'), AxisDirectionEnum.All));
            //list_axis.Add(new AxisSurface(Vector3.Zero, new Vector3(0, 0, 1), new Vector3(1, 0, 0), new Vector3(0, 1, 0), EdgeTypeEnum.AxisY, RotateCountForAxis('Z'), AxisDirectionEnum.All));
            //list_axis.Add(new AxisSurface(Vector3.Zero, new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1), EdgeTypeEnum.AxisZ, RotateCountForAxis('X'), AxisDirectionEnum.All));
        }

        void CreateDinoCube()
        {
            Color clr;
            float dist = cube_size / 2;
            Vector3 v000 = Vector3.Zero, v001 = Vector3.Zero, v010 = Vector3.Zero, v011 = Vector3.Zero,
                    v100 = Vector3.Zero, v101 = Vector3.Zero, v110 = Vector3.Zero, v111 = Vector3.Zero;
            Methods.GenerateCubeVectors(Vector3.Zero, dist, ref v000, ref v001, ref v010, ref v011, ref v100, ref v101, ref v110, ref v111);
            int cnt_turns = 3;

            list_axis.Add(new AxisSurface(v010, v111, v001, rotate_point: v011, type: RCMEnum_AxisType.UpLeft,    cnt_turns: cnt_turns));
            list_axis.Add(new AxisSurface(v110, v101, v011, rotate_point: v111, type: RCMEnum_AxisType.UpRight,   cnt_turns: cnt_turns));
            list_axis.Add(new AxisSurface(v000, v011, v101, rotate_point: v001, type: RCMEnum_AxisType.DownLeft,  cnt_turns: cnt_turns));
            list_axis.Add(new AxisSurface(v100, v001, v111, rotate_point: v101, type: RCMEnum_AxisType.DownRight, cnt_turns: cnt_turns));
            //list_axis.Add(new AxisSurface(v011, v000, v110, rotate_point: v010, cnt_turns: cnt_turns));
            //list_axis.Add(new AxisSurface(v111, v010, v100, rotate_point: v110, cnt_turns: cnt_turns));
            //list_axis.Add(new AxisSurface(v001, v100, v010, rotate_point: v000, cnt_turns: cnt_turns));
            //list_axis.Add(new AxisSurface(v101, v110, v000, rotate_point: v100, cnt_turns: cnt_turns));

            Vector3 vcz1 = new Vector3(0, 0, +dist);
            Vector3 vcz0 = new Vector3(0, 0, -dist);
            Vector3 vcy1 = new Vector3(0, +dist, 0);
            Vector3 vcy0 = new Vector3(0, -dist, 0);
            Vector3 vcx1 = new Vector3(+dist, 0, 0);
            Vector3 vcx0 = new Vector3(-dist, 0, 0);

            clr = COLOR_FRONT;
            list_stickers.Add(new Sticker(clr, v001, v101, vcz1));
            list_stickers.Add(new Sticker(clr, v011, v001, vcz1));
            list_stickers.Add(new Sticker(clr, v111, v011, vcz1));
            list_stickers.Add(new Sticker(clr, v101, v111, vcz1));

            clr = COLOR_BACK;
            list_stickers.Add(new Sticker(clr, v000, v100, vcz0));
            list_stickers.Add(new Sticker(clr, v010, v000, vcz0));
            list_stickers.Add(new Sticker(clr, v110, v010, vcz0));
            list_stickers.Add(new Sticker(clr, v100, v110, vcz0));

            clr = COLOR_UP;
            list_stickers.Add(new Sticker(clr, v010, v011, vcy1));
            list_stickers.Add(new Sticker(clr, v111, v110, vcy1));
            list_stickers.Add(new Sticker(clr, v110, v010, vcy1));
            list_stickers.Add(new Sticker(clr, v011, v111, vcy1));

            clr = COLOR_DOWN;
            list_stickers.Add(new Sticker(clr, v000, v001, vcy0));
            list_stickers.Add(new Sticker(clr, v101, v100, vcy0));
            list_stickers.Add(new Sticker(clr, v100, v000, vcy0));
            list_stickers.Add(new Sticker(clr, v001, v101, vcy0));

            clr = COLOR_LEFT;
            list_stickers.Add(new Sticker(clr, v000, v001, vcx0));
            list_stickers.Add(new Sticker(clr, v011, v010, vcx0));
            list_stickers.Add(new Sticker(clr, v010, v000, vcx0));
            list_stickers.Add(new Sticker(clr, v001, v011, vcx0));

            clr = COLOR_RIGHT;
            list_stickers.Add(new Sticker(clr, v100, v101, vcx1));
            list_stickers.Add(new Sticker(clr, v111, v110, vcx1));
            list_stickers.Add(new Sticker(clr, v110, v100, vcx1));
            list_stickers.Add(new Sticker(clr, v101, v111, vcx1));
        }

        void CreateSkewb()
        {
            float dist = 0.5f * cube_size;
            Vector3[,,] list_coords = Methods.GenerateVectorArray(Vector3.Zero, dist, 3, 3, 3);
            Vector3 temp;
            Color clr;
            int cnt_turns = 3;

            temp = list_coords[0, 2, 2];
            list_axis.Add(new AxisSurface(
                temp + new Vector3(3 * dist, 0, 0),
                temp + new Vector3(0, -3 * dist, 0),
                temp + new Vector3(0, 0, -3 * dist),
                temp, RCMEnum_AxisType.UpLeft, cnt_turns));

            temp = list_coords[2, 2, 2];
            list_axis.Add(new AxisSurface(
                temp + new Vector3(-3 * dist, 0, 0),
                temp + new Vector3(0, 0, -3 * dist),
                temp + new Vector3(0, -3 * dist, 0),
                temp, RCMEnum_AxisType.UpRight, cnt_turns));

            temp = list_coords[2, 2, 0];
            list_axis.Add(new AxisSurface(
                temp + new Vector3(-3 * dist, 0, 0),
                temp + new Vector3(0, 0, 3 * dist),
                temp + new Vector3(0, -3 * dist, 0),
                temp, RCMEnum_AxisType.DownLeft, cnt_turns));

            temp = list_coords[2, 0, 2];
            list_axis.Add(new AxisSurface(
                temp + new Vector3(-3 * dist, 0, 0),
                temp + new Vector3(0, 3 * dist, 0),
                temp + new Vector3(0, 0, -3 * dist),
                temp, RCMEnum_AxisType.DownRight, cnt_turns));

            //temp = list_coords[0, 2, 0];
            //list_axis.Add(new AxisSurface(
            //    temp + new Vector3(3 * dist, 0, 0),
            //    temp + new Vector3(0, 0, 3 * dist),
            //    temp + new Vector3(0, -3 * dist, 0),
            //    rotate_point: temp,
            //    cnt_turns: cnt_turns
            //    ));

            //temp = list_coords[0, 0, 2];
            //list_axis.Add(new AxisSurface(
            //    temp + new Vector3(3 * dist, 0, 0),
            //    temp + new Vector3(0, 3 * dist, 0),
            //    temp + new Vector3(0, 0, -3 * dist),
            //    rotate_point: temp,
            //    cnt_turns: cnt_turns
            //    ));

            //temp = list_coords[0, 0, 0];
            //list_axis.Add(new AxisSurface(
            //    temp + new Vector3(3 * dist, 0, 0),
            //    temp + new Vector3(0, 3 * dist, 0),
            //    temp + new Vector3(0, 0, 3 * dist),
            //    rotate_point: temp,
            //    cnt_turns: cnt_turns
            //    ));

            //temp = list_coords[2, 0, 0];
            //list_axis.Add(new AxisSurface(
            //    temp + new Vector3(-3 * dist, 0, 0),
            //    temp + new Vector3(0, 0, 3 * dist),
            //    temp + new Vector3(0, 3 * dist, 0),
            //    rotate_point: temp,
            //    cnt_turns: cnt_turns
            //    ));


            clr = COLOR_FRONT;
            list_stickers.Add(new Sticker(clr, list_coords[0, 0, 2], list_coords[1, 0, 2], list_coords[0, 1, 2]));
            list_stickers.Add(new Sticker(clr, list_coords[2, 0, 2], list_coords[2, 1, 2], list_coords[1, 0, 2]));
            list_stickers.Add(new Sticker(clr, list_coords[0, 2, 2], list_coords[0, 1, 2], list_coords[1, 2, 2]));
            list_stickers.Add(new Sticker(clr, list_coords[2, 2, 2], list_coords[1, 2, 2], list_coords[2, 1, 2]));
            //list_stickers.Add(new Sticker(clr, list_coords[1, 0, 2], list_coords[0, 1, 2], list_coords[1, 2, 2], list_coords[2, 1, 2]));
            list_stickers.Add(new Sticker(clr, list_coords[2, 1, 2], list_coords[1, 2, 2], list_coords[0, 1, 2], list_coords[1, 0, 2]));

            clr = COLOR_BACK;
            list_stickers.Add(new Sticker(clr, list_coords[0, 0, 0], list_coords[1, 0, 0], list_coords[0, 1, 0]));
            list_stickers.Add(new Sticker(clr, list_coords[2, 0, 0], list_coords[2, 1, 0], list_coords[1, 0, 0]));
            list_stickers.Add(new Sticker(clr, list_coords[0, 2, 0], list_coords[0, 1, 0], list_coords[1, 2, 0]));
            list_stickers.Add(new Sticker(clr, list_coords[2, 2, 0], list_coords[1, 2, 0], list_coords[2, 1, 0]));
            list_stickers.Add(new Sticker(clr, list_coords[2, 1, 0], list_coords[1, 2, 0], list_coords[0, 1, 0], list_coords[1, 0, 0]));

            clr = COLOR_UP;
            list_stickers.Add(new Sticker(clr, list_coords[0, 2, 0], list_coords[1, 2, 0], list_coords[0, 2, 1]));
            list_stickers.Add(new Sticker(clr, list_coords[2, 2, 0], list_coords[2, 2, 1], list_coords[1, 2, 0]));
            list_stickers.Add(new Sticker(clr, list_coords[0, 2, 2], list_coords[0, 2, 1], list_coords[1, 2, 2]));
            list_stickers.Add(new Sticker(clr, list_coords[2, 2, 2], list_coords[1, 2, 2], list_coords[2, 2, 1]));
            list_stickers.Add(new Sticker(clr, list_coords[2, 2, 1], list_coords[1, 2, 2], list_coords[0, 2, 1], list_coords[1, 2, 0]));

            clr = COLOR_DOWN;
            list_stickers.Add(new Sticker(clr, list_coords[0, 0, 0], list_coords[1, 0, 0], list_coords[0, 0, 1]));
            list_stickers.Add(new Sticker(clr, list_coords[2, 0, 0], list_coords[2, 0, 1], list_coords[1, 0, 0]));
            list_stickers.Add(new Sticker(clr, list_coords[0, 0, 2], list_coords[0, 0, 1], list_coords[1, 0, 2]));
            list_stickers.Add(new Sticker(clr, list_coords[2, 0, 2], list_coords[1, 0, 2], list_coords[2, 0, 1]));
            list_stickers.Add(new Sticker(clr, list_coords[2, 0, 1], list_coords[1, 0, 2], list_coords[0, 0, 1], list_coords[1, 0, 0]));

            clr = COLOR_RIGHT;
            list_stickers.Add(new Sticker(clr, list_coords[2, 0, 0], list_coords[2, 1, 0], list_coords[2, 0, 1]));
            list_stickers.Add(new Sticker(clr, list_coords[2, 2, 0], list_coords[2, 2, 1], list_coords[2, 1, 0]));
            list_stickers.Add(new Sticker(clr, list_coords[2, 0, 2], list_coords[2, 0, 1], list_coords[2, 1, 2]));
            list_stickers.Add(new Sticker(clr, list_coords[2, 2, 2], list_coords[2, 1, 2], list_coords[2, 2, 1]));
            list_stickers.Add(new Sticker(clr, list_coords[2, 2, 1], list_coords[2, 1, 2], list_coords[2, 0, 1], list_coords[2, 1, 0]));

            clr = COLOR_LEFT;
            list_stickers.Add(new Sticker(clr, list_coords[0, 0, 0], list_coords[0, 1, 0], list_coords[0, 0, 1]));
            list_stickers.Add(new Sticker(clr, list_coords[0, 2, 0], list_coords[0, 2, 1], list_coords[0, 1, 0]));
            list_stickers.Add(new Sticker(clr, list_coords[0, 0, 2], list_coords[0, 0, 1], list_coords[0, 1, 2]));
            list_stickers.Add(new Sticker(clr, list_coords[0, 2, 2], list_coords[0, 1, 2], list_coords[0, 2, 1]));
            list_stickers.Add(new Sticker(clr, list_coords[0, 2, 1], list_coords[0, 1, 2], list_coords[0, 0, 1], list_coords[0, 1, 0]));

        }
        
        void CreateCloverCube()
        {
            float dist = 0.5f * cube_size;

            Vector3[,,] corners = new Vector3[2, 2, 2];
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                    for (int k = 0; k < 2; k++)
                        corners[i, j, k] = new Vector3((i == 0 ? -1 : 1) * dist, (j == 0 ? -1 : 1) * dist, (k == 0 ? -1 : 1) * dist);

            // Z
            CreateCloverCubeEdge(COLOR_BACK,  new Vector3[] { corners[0, 0, 0], corners[1, 0, 0], corners[1, 1, 0], corners[0, 1, 0] }, RCMEnum_AxisType.Back);
            CreateCloverCubeEdge(COLOR_FRONT, new Vector3[] { corners[0, 0, 1], corners[1, 0, 1], corners[1, 1, 1], corners[0, 1, 1] }, RCMEnum_AxisType.Front);

            // Y
            CreateCloverCubeEdge(COLOR_DOWN,  new Vector3[] { corners[0, 0, 0], corners[1, 0, 0], corners[1, 0, 1], corners[0, 0, 1] }, RCMEnum_AxisType.Down);
            CreateCloverCubeEdge(COLOR_UP,    new Vector3[] { corners[0, 1, 0], corners[1, 1, 0], corners[1, 1, 1], corners[0, 1, 1] }, RCMEnum_AxisType.Up);

            // X
            CreateCloverCubeEdge(COLOR_LEFT,  new Vector3[] { corners[0, 0, 0], corners[0, 1, 0], corners[0, 1, 1], corners[0, 0, 1] }, RCMEnum_AxisType.Left);
            CreateCloverCubeEdge(COLOR_RIGHT, new Vector3[] { corners[1, 0, 0], corners[1, 1, 0], corners[1, 1, 1], corners[1, 0, 1] }, RCMEnum_AxisType.Right);


            // Axises
            CreateCloverCubeAxis(corners[0, 0, 0], corners[1, 0, 0], "BD");
            CreateCloverCubeAxis(corners[0, 1, 0], corners[0, 0, 0], "BL");
            CreateCloverCubeAxis(corners[0, 0, 0], corners[0, 0, 1], "DL");

            CreateCloverCubeAxis(corners[1, 0, 0], corners[1, 1, 0], "BR");
            CreateCloverCubeAxis(corners[1, 0, 1], corners[1, 0, 0], "DR");
            CreateCloverCubeAxis(corners[1, 1, 0], corners[0, 1, 0], "BU");
            CreateCloverCubeAxis(corners[0, 1, 1], corners[0, 1, 0], "UL");
            CreateCloverCubeAxis(corners[1, 0, 1], corners[0, 0, 1], "FD", RCMEnum_AxisType.Down);
            CreateCloverCubeAxis(corners[0, 0, 1], corners[0, 1, 1], "FL", RCMEnum_AxisType.Left);

            CreateCloverCubeAxis(corners[1, 1, 0], corners[1, 1, 1], "UR");
            CreateCloverCubeAxis(corners[1, 1, 1], corners[1, 0, 1], "FR", RCMEnum_AxisType.Right);
            CreateCloverCubeAxis(corners[0, 1, 1], corners[1, 1, 1], "FU", RCMEnum_AxisType.Up);

            //// Connect places to axises
            //foreach (StickerPlace place in list_places)
            //{
            //    List<Axis> list_axis_sticker = list_axis.FindAll(t => t.GetDirection != AxisDirectionEnum.All && t.isTouchable && t.isPointOnAxis(place.GetCentralPoint));
            //    if (list_axis_sticker.Count == 1)
            //        place.GetAxisMain = list_axis_sticker[0];
            //}
        }
        
        void CreateCloverCubeEdge(Color clr, Vector3[] vertices, RCMEnum_AxisType type)
        {
            int len = 4;
            Vector3 center = Vector3.Zero;
            for (int i = 0; i < len; i++) center += vertices[i];
            center /= len;

            int kk = 5;
            Vector3[] vD = new Vector3[len];
            Vector3[] vLP = new Vector3[len];
            Vector3[] vRP = new Vector3[len];
            Vector3[,] vL = new Vector3[len, kk];
            Vector3[,] vR = new Vector3[len, kk];
            for (int i = 0; i < len; i++)
            {
                Vector3 dd = vertices[i] - center;
                vD[i] = dd;
                if (dd.X == 0)
                {
                    vLP[i] = new Vector3(0, -dd.Z, dd.Y);
                    vRP[i] = new Vector3(0, dd.Z, -dd.Y);
                }
                else if (dd.Y == 0)
                {
                    vLP[i] = new Vector3(-dd.Z, 0, dd.X);
                    vRP[i] = new Vector3(dd.Z, 0, -dd.X);
                }
                else if (dd.Z == 0)
                {
                    vLP[i] = new Vector3(-dd.Y, dd.X, 0);
                    vRP[i] = new Vector3(dd.Y, -dd.X, 0);
                }
                else
                    throw new Exception("???");
            }




            for (int i = 0; i < len; i++)
            {
                for (int k = 0; k < kk; k++)
                {
                    float koef1 = (k + 1f) / (kk + 1);
                    float koef2 = 0.25f - (koef1 - 0.5f) * (koef1 - 0.5f);
                    vL[i, k] = center + koef1 * vD[i] + koef2 * vLP[i];
                    vR[i, k] = center + koef1 * vD[i] + koef2 * vRP[i];
                }
            }



            for (int i = 0; i < len; i++)
            {
                Vector3[] mass_vertices = new Vector3[2 * kk + 2];
                mass_vertices[0] = center;
                for (int k = 0; k < kk; k++)
                    mass_vertices[k + 1] = vL[i, k];
                mass_vertices[kk + 1] = vertices[i];
                for (int k = 0; k < kk; k++)
                    mass_vertices[kk + k + 2] = vR[i, kk - k - 1];
                list_stickers.Add(new Sticker(clr, mass_vertices));

                
                int i2 = (i + 1) % len;
                mass_vertices = new Vector3[2 * kk + 3];
                mass_vertices[0] = center;
                for (int k = 0; k < kk; k++)
                    mass_vertices[k + 1] = vL[i, k];
                mass_vertices[kk + 1] = vertices[i];
                mass_vertices[kk + 2] = vertices[i2];
                for (int k = 0; k < kk; k++)
                    mass_vertices[kk + k + 3] = vR[i2, kk - k - 1];
                list_stickers.Add(new Sticker(clr, mass_vertices));
            }

            //// Inner
            //for (int i = 0; i < len; i++)
            //    vertices[i] *= 0.9f;
            //list_places.Add(new StickerPlace(Color.White, null, vertices));
        }

        void CreateCloverCubeAxis(Vector3 start, Vector3 end, string description, RCMEnum_AxisType type = RCMEnum_AxisType.None)
        {
            Vector3 center = (start + end) / 2;
            Vector3 v0 = center, v1 = center;
            if (v0.X != 0) v0.X = 0; else if (v0.Y != 0) v0.Y = 0; else v0.Z = 0;
            if (v1.Z != 0) v1.Z = 0; else if (v1.Y != 0) v1.Y = 0; else v1.X = 0;
            list_axis.Add(new AxisSphere3(start, end, 2 * v0,
                center,
                1.4f * center, 1.0136f * center.Length(), type, 2));
        }

        void CreateAxisCube()
        {
            float dist = cube_size / 2;
            Vector3 v000 = Vector3.Zero, v001 = Vector3.Zero, v010 = Vector3.Zero, v011 = Vector3.Zero,
                    v100 = Vector3.Zero, v101 = Vector3.Zero, v110 = Vector3.Zero, v111 = Vector3.Zero;
            Methods.GenerateCubeVectors(Vector3.Zero, dist, ref v000, ref v001, ref v010, ref v011, ref v100, ref v101, ref v110, ref v111);
            int cnt_turns = 3;
            
            CreateAxisEdge(COLOR_FRONT, v001, v011, v101, v111);
            CreateAxisEdge(COLOR_BACK, v110, v100, v010, v000);
            CreateAxisEdge(COLOR_UP, v010, v011, v110, v111);
            CreateAxisEdge(COLOR_DOWN, v101, v100, v001, v000);
            CreateAxisEdge(COLOR_LEFT, v011, v001, v010, v000);
            CreateAxisEdge(COLOR_RIGHT, v100, v110, v101, v111);

            float axis_koef = 0.25f;
            list_axis.Add(new AxisSurface(v001, v010, (v110 + v111) / 2,
                ((1 - axis_koef) * v011 + axis_koef * v111), RCMEnum_AxisType.Front, cnt_turns));
            list_axis.Add(new AxisSurface(v110, (v001 + v000) / 2, v101,
                ((1 - axis_koef) * v100 + axis_koef * v000), RCMEnum_AxisType.Back, cnt_turns));

            list_axis.Add(new AxisSurface(v010, v100, (v011 + v111) / 2,
                ((1 - axis_koef) * v110 + axis_koef * v111), RCMEnum_AxisType.Up, cnt_turns));
            list_axis.Add(new AxisSurface(v101, (v100 + v000) / 2, v011,
                ((1 - axis_koef) * v001 + axis_koef * v000), RCMEnum_AxisType.Down, cnt_turns));


            list_axis.Add(new AxisSurface(v001, (v011 + v111) / 2, v100,
                ((1 - axis_koef) * v101 + axis_koef * v111), RCMEnum_AxisType.Right, cnt_turns));
            list_axis.Add(new AxisSurface(v011, (v001 + v000) / 2, v110,
                ((1 - axis_koef) * v010 + axis_koef * v000), RCMEnum_AxisType.Left, cnt_turns));

        }

        void CreateAxisEdge(Color clr, Vector3 v00, Vector3 v01, Vector3 v10, Vector3 v11)
        {
            Vector3 ft = (v01 + v11) / 2;
            Vector3 fr = (v10 + v11) / 2;
            Vector3 ct = 0.666f * v01 + 0.333f * v10;
            Vector3 cr = 0.333f * v01 + 0.666f * v10;

            list_stickers.Add(new Sticker(clr, v00, v01, ct));
            list_stickers.Add(new Sticker(clr, ft, v01, ct));

            list_stickers.Add(new Sticker(clr, v00, ct, cr));
            list_stickers.Add(new Sticker(clr, ct, cr, fr, ft));

            list_stickers.Add(new Sticker(clr, v00, cr, v10));
            list_stickers.Add(new Sticker(clr, fr, cr, v10));

            list_stickers.Add(new Sticker(clr, ft, fr, v11));

            //list_stickers.Add(new Sticker(clr, v00, v01, ft));
            //list_stickers.Add(new Sticker(clr, v00, ft,  fr));
            //list_stickers.Add(new Sticker(clr, v00, fr, v10));
            //list_stickers.Add(new Sticker(clr, ft,  fr, v11));
        }
        
        void CreateFloppyCube()
        {
            float dist = cube_size / 3f;
            int lenx = 3, leny = 1, lenz = 3;
            is_can_rotate_x = (lenx == leny);
            is_can_rotate_y = (leny == lenz);
            is_can_rotate_z = (lenz == lenx);

            Vector3[,,] list_coords = Methods.GenerateVectorArray(Vector3.Zero, dist, lenx + 1, leny + 1, lenz + 1);
            for (int x = 0; x < lenx; x++)
                for (int y = 0; y < leny; y++)
                {
                    list_stickers.Add(new Sticker(COLOR_FRONT, list_coords[x, y, lenz], list_coords[x + 1, y, lenz], list_coords[x + 1, y + 1, lenz], list_coords[x, y + 1, lenz]));
                    list_stickers.Add(new Sticker(COLOR_BACK, list_coords[x, y, 0], list_coords[x + 1, y, 0], list_coords[x + 1, y + 1, 0], list_coords[x, y + 1, 0]));
                }

            for (int x = 0; x < lenx; x++)
                for (int z = 0; z < lenz; z++)
                {
                    list_stickers.Add(new Sticker(COLOR_UP, list_coords[x, leny, z], list_coords[x + 1, leny, z], list_coords[x + 1, leny, z + 1], list_coords[x, leny, z + 1]));
                    list_stickers.Add(new Sticker(COLOR_DOWN, list_coords[x, 0, z], list_coords[x + 1, 0, z], list_coords[x + 1, 0, z + 1], list_coords[x, 0, z + 1]));
                }

            for (int y = 0; y < leny; y++)
                for (int z = 0; z < lenz; z++)
                {
                    list_stickers.Add(new Sticker(COLOR_RIGHT, list_coords[lenx, y, z], list_coords[lenx, y, z + 1], list_coords[lenx, y + 1, z + 1], list_coords[lenx, y + 1, z]));
                    list_stickers.Add(new Sticker(COLOR_LEFT, list_coords[0, y, z], list_coords[0, y, z + 1], list_coords[0, y + 1, z + 1], list_coords[0, y + 1, z]));
                }


            // Generate insides
            if (is_generate_insides)
            {
                float ki1 = 0.02f, ki2 = 1 - ki1;
                for (int x = 0; x < lenx; x++)
                {
                    list_stickers.Add(new Sticker(COLOR_INSIDE,
                        ki1 * list_coords[x,     0, lenz] + ki2 * list_coords[x,     0, lenz - 1],
                        ki1 * list_coords[x + 1, 0, lenz] + ki2 * list_coords[x + 1, 0, lenz - 1],
                        ki1 * list_coords[x + 1, 1, lenz] + ki2 * list_coords[x + 1, 1, lenz - 1],
                        ki1 * list_coords[x,     1, lenz] + ki2 * list_coords[x,     1, lenz - 1]
                        )
                    { isFaceSticker = false });
                    list_stickers.Add(new Sticker(COLOR_INSIDE,
                        ki1 * list_coords[x,     0, lenz - 2] + ki2 * list_coords[x,     0, lenz - 1],
                        ki1 * list_coords[x + 1, 0, lenz - 2] + ki2 * list_coords[x + 1, 0, lenz - 1],
                        ki1 * list_coords[x + 1, 1, lenz - 2] + ki2 * list_coords[x + 1, 1, lenz - 1],
                        ki1 * list_coords[x,     1, lenz - 2] + ki2 * list_coords[x,     1, lenz - 1]
                        )
                    { isFaceSticker = false });

                    list_stickers.Add(new Sticker(COLOR_INSIDE,
                        ki1 * list_coords[x,     0, 0] + ki2 * list_coords[x,     0, 1],
                        ki1 * list_coords[x + 1, 0, 0] + ki2 * list_coords[x + 1, 0, 1],
                        ki1 * list_coords[x + 1, 1, 0] + ki2 * list_coords[x + 1, 1, 1],
                        ki1 * list_coords[x,     1, 0] + ki2 * list_coords[x,     1, 1]
                        )
                    { isFaceSticker = false });
                    list_stickers.Add(new Sticker(COLOR_INSIDE,
                        ki1 * list_coords[x,     0, 2] + ki2 * list_coords[x,     0, 1],
                        ki1 * list_coords[x + 1, 0, 2] + ki2 * list_coords[x + 1, 0, 1],
                        ki1 * list_coords[x + 1, 1, 2] + ki2 * list_coords[x + 1, 1, 1],
                        ki1 * list_coords[x,     1, 2] + ki2 * list_coords[x,     1, 1]
                        )
                    { isFaceSticker = false });
                }

                for (int z = 0; z < lenz; z++)
                {
                    list_stickers.Add(new Sticker(COLOR_INSIDE,
                        ki1 * list_coords[lenx, 0, z    ] + ki2 * list_coords[lenx - 1, 0, z],
                        ki1 * list_coords[lenx, 0, z + 1] + ki2 * list_coords[lenx - 1, 0, z + 1],
                        ki1 * list_coords[lenx, 1, z + 1] + ki2 * list_coords[lenx - 1, 1, z + 1],
                        ki1 * list_coords[lenx, 1, z    ] + ki2 * list_coords[lenx - 1, 1, z]
                        )
                    { isFaceSticker = false });
                    list_stickers.Add(new Sticker(COLOR_INSIDE,
                        ki1 * list_coords[lenx - 2, 0, z    ] + ki2 * list_coords[lenx - 1, 0, z],
                        ki1 * list_coords[lenx - 2, 0, z + 1] + ki2 * list_coords[lenx - 1, 0, z + 1],
                        ki1 * list_coords[lenx - 2, 1, z + 1] + ki2 * list_coords[lenx - 1, 1, z + 1],
                        ki1 * list_coords[lenx - 2, 1, z    ] + ki2 * list_coords[lenx - 1, 1, z]
                        )
                    { isFaceSticker = false });

                    list_stickers.Add(new Sticker(COLOR_INSIDE,
                        ki1 * list_coords[0, 0, z    ] + ki2 * list_coords[1, 0, z],
                        ki1 * list_coords[0, 0, z + 1] + ki2 * list_coords[1, 0, z + 1],
                        ki1 * list_coords[0, 1, z + 1] + ki2 * list_coords[1, 1, z + 1],
                        ki1 * list_coords[0, 1, z    ] + ki2 * list_coords[1, 1, z]
                        )
                    { isFaceSticker = false });
                    list_stickers.Add(new Sticker(COLOR_INSIDE,
                        ki1 * list_coords[2, 0, z    ] + ki2 * list_coords[1, 0, z],
                        ki1 * list_coords[2, 0, z + 1] + ki2 * list_coords[1, 0, z + 1],
                        ki1 * list_coords[2, 1, z + 1] + ki2 * list_coords[1, 1, z + 1],
                        ki1 * list_coords[2, 1, z    ] + ki2 * list_coords[1, 1, z]
                        )
                    { isFaceSticker = false });
                }
            }


            // Generate axises
            float x1 = list_coords[1, 0, 0].X        - 0.01f * dist;
            float x2 = list_coords[lenx - 1, 0, 0].X + 0.01f * dist;
            float y1 = list_coords[0, 1, 0].Y        - 0.01f * dist;
            float y2 = list_coords[0, leny - 1, 0].Y + 0.01f * dist;
            float z1 = list_coords[0, 0, lenz - 1].Z + 0.01f * dist;
            float z2 = list_coords[0, 0, 1].Z        - 0.01f * dist;
            int d = 5;
                
            list_axis.Add(new AxisSurface(new Vector3(x1, 0, 0), new Vector3(x1, d, 0), new Vector3(x1, 0, d), new Vector3(x1 + d, (y1 + y2) / 2, (z1 + z2) / 2), RCMEnum_AxisType.Left,  4));
            list_axis.Add(new AxisSurface(new Vector3(x2, 0, 0), new Vector3(x2, 0, d), new Vector3(x2, d, 0), new Vector3(x2 + d, (y1 + y2) / 2, (z1 + z2) / 2), RCMEnum_AxisType.Right, 4));
            list_axis.Add(new AxisSurface(new Vector3(0, 0, z1), new Vector3(0, d, z1), new Vector3(d, 0, z1), new Vector3((x1 + x2) / 2, (y1 + y2) / 2, z1 + d), RCMEnum_AxisType.Down,  4));
            list_axis.Add(new AxisSurface(new Vector3(0, 0, z2), new Vector3(d, 0, z2), new Vector3(0, d, z2), new Vector3((x1 + x2) / 2, (y1 + y2) / 2, z2 + d), RCMEnum_AxisType.Up,    4));
            list_axis.Add(new AxisSurface(Vector3.Zero, new Vector3(0, 0, 1), new Vector3(1, 0, 0), new Vector3(0, 1, 0), RCMEnum_AxisType.AxisZ, 4, RCMEnum_AxisDirection.All));
        }

        void CreatePetalPyraminx()
        {
            float dist = 0.666f * cube_size;
            Vector3 del = dist * new Vector3(0, 0, 35f / 99);
            Vector3 vA = del + dist * new Vector3((float)Math.Cos(3 * Math.PI / 6), (float)Math.Sin(3 * Math.PI / 6), 0);
            Vector3 vB = del + dist * new Vector3((float)Math.Cos(7 * Math.PI / 6), (float)Math.Sin(7 * Math.PI / 6), 0);
            Vector3 vC = del + dist * new Vector3((float)Math.Cos(11 * Math.PI / 6), (float)Math.Sin(11 * Math.PI / 6), 0);
            Vector3 vD = del + dist * new Vector3(0, 0, -(float)Math.Sqrt(2));
            CreatePetalPyraminxEdge(Color.Red, vA, vB, vC);
            CreatePetalPyraminxEdge(Color.DodgerBlue, vA, vB, vD);
            CreatePetalPyraminxEdge(Color.Yellow, vA, vD, vC);
            CreatePetalPyraminxEdge(Color.LawnGreen, vD, vB, vC);

            int count_rotates = 3;
            float koef_surface = 0.33f;
            list_axis.Add(new AxisSurface(
                koef_surface * vA + (1 - koef_surface) * vB,
                koef_surface * vA + (1 - koef_surface) * vD,
                koef_surface * vA + (1 - koef_surface) * vC,
                vA, RCMEnum_AxisType.Up, count_rotates
                ));
            list_axis.Add(new AxisSurface(
                koef_surface * vB + (1 - koef_surface) * vC,
                koef_surface * vB + (1 - koef_surface) * vD,
                koef_surface * vB + (1 - koef_surface) * vA,
                vB, RCMEnum_AxisType.Left, count_rotates
                ));
            list_axis.Add(new AxisSurface(
                koef_surface * vC + (1 - koef_surface) * vD,
                koef_surface * vC + (1 - koef_surface) * vB,
                koef_surface * vC + (1 - koef_surface) * vA,
                vC, RCMEnum_AxisType.Right, count_rotates
                ));
            list_axis.Add(new AxisSurface(
                koef_surface * vD + (1 - koef_surface) * vA,
                koef_surface * vD + (1 - koef_surface) * vB,
                koef_surface * vD + (1 - koef_surface) * vC,
                vD, RCMEnum_AxisType.Back, count_rotates
                ));



            list_axis.Add(new AxisSphere(vA, vC, vB, (vA + vB + vC) / 3, 0.37f * dist, RCMEnum_AxisType.RotateCenter, cnt_turns: 3));
            //list_axis.Add(new AxisSphere(vD, vC, vB, (vD + vB + vC) / 3, 0.4f * dist, cnt_turns: 3));
            //list_axis.Add(new AxisSphere(vA, vD, vB, (vA + vB + vD) / 3, 0.4f * dist, cnt_turns: 3));
            //list_axis.Add(new AxisSphere(vA, vC, vD, (vA + vD + vC) / 3, 0.4f * dist, cnt_turns: 3));

            Vector3 vc = (vA + vB + vC + vD) / 4;
            list_axis.Add(new AxisSurface(vB, vC, vD, vA, RCMEnum_AxisType.AxisX, 3, RCMEnum_AxisDirection.All));
            list_axis.Add(new AxisSurface(vD, vA, vB, vC, RCMEnum_AxisType.AxisY, 3, RCMEnum_AxisDirection.All));
            list_axis.Add(new AxisSurface(vC, vA, vD, vB, RCMEnum_AxisType.AxisZ, 3, RCMEnum_AxisDirection.All));
        }

        void CreatePetalPyraminxEdge(Color clr, Vector3 vA, Vector3 vB, Vector3 vC)
        {
            Vector3 center = (vA + vB + vC) / 3;
            AxisSurface axis = new AxisSurface(vA, vB, vC, center, RCMEnum_AxisType.None, 1); 
            Vector3 rotate_point = 0.4f * vA + 0.6f * center;
            Vector3[] mass_circle = new Vector3[18];
            for (int i = 0; i < mass_circle.Length; i++)
                mass_circle[i] = axis.RotatePointAroundAxis(rotate_point, i * 2 * (float)Math.PI / 18);


            list_stickers.Add(new Sticker(clr, mass_circle[17], mass_circle[0], mass_circle[1], center));
            list_stickers.Add(new Sticker(clr, mass_circle[1], mass_circle[2], mass_circle[3], mass_circle[4], mass_circle[5], center));
            list_stickers.Add(new Sticker(clr, mass_circle[5], mass_circle[6], mass_circle[7], center));
            list_stickers.Add(new Sticker(clr, mass_circle[7], mass_circle[8], mass_circle[9], mass_circle[10], mass_circle[11], center));
            list_stickers.Add(new Sticker(clr, mass_circle[11], mass_circle[12], mass_circle[13], center));
            list_stickers.Add(new Sticker(clr, mass_circle[13], mass_circle[14], mass_circle[15], mass_circle[16], mass_circle[17], center));


            //for (int i = 0; i < 9; i++)
            //    list_stickers.Add(new Sticker(clr,
            //        mass_circle[(2 * i + 17) % 18],
            //        mass_circle[(2 * i +  0) % 18],
            //        mass_circle[(2 * i +  1) % 18],
            //        center));


            float dist_koef = 0.85f;
            Vector3 vAB = dist_koef * vA + (1 - dist_koef) * vB;
            Vector3 vBA = dist_koef * vB + (1 - dist_koef) * vA;
            Vector3 vAC = dist_koef * vA + (1 - dist_koef) * vC;
            Vector3 vCA = dist_koef * vC + (1 - dist_koef) * vA;
            Vector3 vBC = dist_koef * vB + (1 - dist_koef) * vC;
            Vector3 vCB = dist_koef * vC + (1 - dist_koef) * vB;
            list_stickers.Add(new Sticker(clr, vA, vAB, mass_circle[1], mass_circle[0], mass_circle[17], vAC));
            list_stickers.Add(new Sticker(clr, vB, vBC, mass_circle[7], mass_circle[6], mass_circle[5], vBA));
            list_stickers.Add(new Sticker(clr, vC, vCA, mass_circle[13], mass_circle[12], mass_circle[11], vCB));


            // short version //

            //list_stickers.Add(new Sticker(clr, vBA, mass_circle[4], mass_circle[3], mass_circle[2], vAB));
            //list_stickers.Add(new Sticker(clr, vCB, mass_circle[10], mass_circle[9], mass_circle[8], vBC));
            //list_stickers.Add(new Sticker(clr, vAC, mass_circle[16], mass_circle[15], mass_circle[14], vCA));

            // ------------- //

            // canonical
            //list_stickers.Add(new Sticker(clr, vAB, mass_circle[1], mass_circle[2], mass_circle[3], mass_circle[4], mass_circle[5], vBA));
            //list_stickers.Add(new Sticker(clr, vBC, mass_circle[7], mass_circle[8], mass_circle[9], mass_circle[10], mass_circle[11], vCB));
            //list_stickers.Add(new Sticker(clr, vCA, mass_circle[13], mass_circle[14], mass_circle[15], mass_circle[16], mass_circle[17], vAC));

            // canonical
            list_stickers.Add(new Sticker(clr, mass_circle[1], mass_circle[2], mass_circle[3], mass_circle[4], mass_circle[5], vBA, vAB));
            list_stickers.Add(new Sticker(clr, mass_circle[7], mass_circle[8], mass_circle[9], mass_circle[10], mass_circle[11], vCB, vBC));
            list_stickers.Add(new Sticker(clr, mass_circle[13], mass_circle[14], mass_circle[15], mass_circle[16], mass_circle[17], vAC, vCA));

            // get rotated, idiot
            //list_stickers.Add(new Sticker(clr, vAB, mass_circle[2], mass_circle[3], mass_circle[4], vBA));
            //list_stickers.Add(new Sticker(clr, vBC, mass_circle[8], mass_circle[9], mass_circle[10], vCB));
            //list_stickers.Add(new Sticker(clr, vCA, mass_circle[14], mass_circle[15], mass_circle[16], vAC));
        }

        void CreateKilominx()
        {
            Vector3[] mv1 = new Vector3[5];
            Vector3[] mv2 = new Vector3[5];
            Vector3[] mv3 = new Vector3[5];
            Vector3[] mv4 = new Vector3[5];

            float dist = cube_size / 1.6f;
            float angle_del_x = 2 * (float)Math.PI / 5;
            float angle_del_z = (float)Math.PI / 6;
            float ax, az;
            for (int i = 0; i < 5; i++)
            {
                float kk1 = 1.754f;
                float kk2 = 0.3603f;

                ax = (i - 0.5f) * angle_del_x;
                az = kk1 * angle_del_z;
                mv1[i] = dist * new Vector3((float)(Math.Sin(ax) * Math.Cos(az)), (float)(Math.Cos(ax) * Math.Cos(az)), (float)Math.Sin(az));

                ax = (i - 0.5f) * angle_del_x;
                az = kk2 * angle_del_z;
                mv2[i] = dist * new Vector3((float)(Math.Sin(ax) * Math.Cos(az)), (float)(Math.Cos(ax) * Math.Cos(az)), (float)Math.Sin(az));

                ax = (i + 0f) * angle_del_x;
                az = -kk2 * angle_del_z;
                mv3[i] = dist * new Vector3((float)(Math.Sin(ax) * Math.Cos(az)), (float)(Math.Cos(ax) * Math.Cos(az)), (float)Math.Sin(az));

                ax = (i + 0f) * angle_del_x;
                az = -kk1 * angle_del_z;
                mv4[i] = dist * new Vector3((float)(Math.Sin(ax) * Math.Cos(az)), (float)(Math.Cos(ax) * Math.Cos(az)), (float)Math.Sin(az));
            }



            CreateKilominxEdge(Color.White, new Vector3[] { mv1[0], mv1[1], mv1[2], mv1[3], mv1[4] }, RCMEnum_AxisType.Front, additional_axis: RCMEnum_AxisType.AxisZ);
            CreateKilominxEdge(Color.Gray,  new Vector3[] { mv4[0], mv4[1], mv4[2], mv4[3], mv4[4] }, RCMEnum_AxisType.Back,  is_axis_reversed: true);
            Color[] mass_colors1 = { Color.Yellow, Color.DarkBlue, Color.Red, Color.DarkGreen, Color.Purple };
            Color[] mass_colors2 = { Color.LightGreen, Color.Pink, Color.Beige, Color.LightBlue, Color.Orange };
            RCMEnum_AxisType[] mass_types1 = { RCMEnum_AxisType.Up, RCMEnum_AxisType.UpRight, RCMEnum_AxisType.DownRight, RCMEnum_AxisType.DownLeft, RCMEnum_AxisType.UpLeft };
            RCMEnum_AxisType[] mass_types2 = { RCMEnum_AxisType.None, RCMEnum_AxisType.None, RCMEnum_AxisType.None, RCMEnum_AxisType.None, RCMEnum_AxisType.None };
            //EdgeTypeEnum[] mass_types1 = { EdgeTypeEnum.UpRight, EdgeTypeEnum.DownRight, EdgeTypeEnum.Down, EdgeTypeEnum.DownLeft, EdgeTypeEnum.UpLeft };
            //EdgeTypeEnum[] mass_types2 = { EdgeTypeEnum.None, EdgeTypeEnum.None, EdgeTypeEnum.None, EdgeTypeEnum.None, EdgeTypeEnum.None };

            for (int k1 = 0; k1 < 5; k1++)
            {
                int k2 = (k1 + 1) % 5;
                CreateKilominxEdge(mass_colors1[k1], new Vector3[] { mv1[k1], mv1[k2], mv2[k2], mv3[k1], mv2[k1] }, mass_types1[k1], is_axis_reversed: true, additional_axis: k1 == 0 ? RCMEnum_AxisType.AxisX : RCMEnum_AxisType.None);
                CreateKilominxEdge(mass_colors2[k1], new Vector3[] { mv4[k1], mv4[k2], mv3[k2], mv2[k2], mv3[k1] }, mass_types2[k1]);
            }


            //list_axis.Add(new AxisSurface(mv2[0], mv2[1], mv2[2], (mv1[0] + mv1[1] + mv1[2] + mv1[3] + mv1[4]) / 5, 5));
            //list_axis.Add(new AxisSurface(mv3[0], mv3[1], mv3[2], (mv4[0] + mv4[1] + mv4[2] + mv4[3] + mv4[4]) / 5, 5));


        }

        void CreateKilominxEdge(Color clr, Vector3[] vertices, RCMEnum_AxisType type,
            bool is_axis_reversed = false, RCMEnum_AxisType additional_axis = RCMEnum_AxisType.None)
        {
            int len = 5;

            Vector3 center = Vector3.Zero;
            for (int i = 0; i < len; i++) center += vertices[i];
            center /= len;

            switch (this.type)
            {
                case RCMEnum_CubeType.Kilominx:
                    Vector3[] edges_50 = new Vector3[len];
                    for (int i = 0; i < len; i++) edges_50[i] = (vertices[i] + vertices[(i + 1) % len]) / 2;
                    for (int i = 0; i < len; i++)
                        list_stickers.Add(new Sticker(clr, edges_50[(i + len - 1) % len], vertices[i], edges_50[i], center));
                    break;

                case RCMEnum_CubeType.Megaminx:
                    float koef = 0.7f;
                    Vector3[] edges_L = new Vector3[len];
                    Vector3[] edges_R = new Vector3[len];
                    Vector3[] edges_I = new Vector3[len];
                    for (int i = 0; i < len; i++)
                    {
                        edges_L[i] = koef * vertices[i] + (1 - koef) * vertices[(i + 1) % len];
                        edges_R[i] = koef * vertices[i] + (1 - koef) * vertices[(i + len - 1) % len];
                    }
                    for (int i = 0; i < len; i++)
                        edges_I[i] = 0.45f * vertices[i] + 0.55f * center;
                    for (int i = 0; i < len; i++)
                    {
                        list_stickers.Add(new Sticker(clr, vertices[i], edges_L[i], edges_I[i], edges_R[i]));
                        list_stickers.Add(new Sticker(clr, edges_L[i], edges_R[(i + 1) % len], edges_I[(i + 1) % len], edges_I[i]));
                    }
                    list_stickers.Add(new Sticker(clr, edges_I));
                    break;
            }

            
            float koef_axis = 0.7f;
            if (!is_axis_reversed) list_axis.Add(new AxisSurface(koef_axis * vertices[0], koef_axis * vertices[1], koef_axis * vertices[2], center, type, 5));
            else                   list_axis.Add(new AxisSurface(koef_axis * vertices[0], koef_axis * vertices[2], koef_axis * vertices[1], center, type, 5));

            if (additional_axis != RCMEnum_AxisType.None)
                list_axis.Add(new AxisSurface(vertices[0], vertices[1], vertices[2], center, additional_axis, 5, RCMEnum_AxisDirection.All));
        }
        
        #endregion

        void GenerateFaces()
        {
            Dictionary<Color, Face> dict_faces = new Dictionary<Color, Face>();
            float koef_sticker_shift = (type == RCMEnum_CubeType.Kilominx || type == RCMEnum_CubeType.Megaminx) ? 0.1f 
                                     : (type == RCMEnum_CubeType.MirrorFloppy) ? 0.005f 
                                     : 0.001f;
            foreach (Sticker sticker in list_stickers.FindAll(t => t.isFaceSticker))
            {
                Color key = sticker.GetColor;
                if (!dict_faces.ContainsKey(key))
                    dict_faces[key] = new Face(key, koef_sticker_shift);
                dict_faces[key].AddSticker(sticker);
            }

            list_faces.Clear();
            foreach (var item in dict_faces)
                list_faces.Add(item.Value);
        }

        int RotateCountForAxis(char axis_char)
        {
            switch (axis_char)
            {
                case 'X': return is_can_rotate_x ? 4 : 2;
                case 'Y': return is_can_rotate_y ? 4 : 2;
                case 'Z': return is_can_rotate_z ? 4 : 2;
                default: return 4;
            }
        }

        public void ChangeDefaultDirection()
        {
            this.is_rotate_clockwise = !is_rotate_clockwise;
        }

        public bool TryRotateAxis(RCMEnum_AxisType type, string rotate_direction, bool is_manual = true)
        {
            Axis axis = list_axis.FirstOrDefault(t => t.GetAxisType == type);
            bool is_clockwise = (rotate_direction == "Clockwise" 
                              || rotate_direction == "Default" && is_rotate_clockwise);
            return (axis != null && TryRotateAxis(axis, is_clockwise, is_manual));
        }
        
        bool TryRotateAxis(Axis axis, bool is_clockwise, bool is_manual = true)
        {
            if (   state == RCMEnum_CubeState.Solved
                || state == RCMEnum_CubeState.Scramble && is_manual)
                return false;
            if (event_rotate != null && !event_rotate.isFinished)
                event_rotate.Finish();
            //if (event_rotate != null && !event_rotate.isFinished)
            //    return false;
            
            List<Sticker> list_stickers_rotate = new List<Sticker>();
            foreach (Sticker sticker in list_stickers)
                if (axis.isStickerOnAxis(sticker))
                    list_stickers_rotate.Add(sticker);
            event_rotate = 
                (grid == null) ? new RotationEvent(axis, list_stickers_rotate, (is_clockwise ? 1 : -1) * (2 * (float)Math.PI / axis.GetCountRotates), millis_ttl: is_manual ? rotate_time_manual : rotate_time_scramble, type_moves: "start") 
                               : new RotationEvent(axis, list_stickers_rotate, (is_clockwise ? 1 : -1) * (2 * (float)Math.PI / axis.GetCountRotates), millis_ttl: is_manual ? rotate_time_manual : rotate_time_scramble, grid: grid);
            if (is_manual && state != RCMEnum_CubeState.RestorePrevious)
                journal.AddMove(axis, is_clockwise: is_clockwise, is_short_name: is_short_name_in_journal);
            return true;
        }

        public override void Update()
        {
            base.Update();
            if (is_solved_from_start)
            {
                projection.AddVectorRotate(new Vector3(0.24f * 2f * Engine.DeltaTime, 2f * Engine.DeltaTime, 0));
            }
            else if (event_rotate != null && !event_rotate.isFinished)
            {
                // State: rotate
                event_rotate.Update();
                if (event_rotate.isFinished)
                {
                    if (state != RCMEnum_CubeState.Scramble && list_faces.Count <= list_faces.Count(t => t.isSolved())) // && !MeliHelperModule.Instance.Session.SolvedCubes.Contains(this.type))
                    {
                        state = RCMEnum_CubeState.Solved;
                        MeliHelperModule.Instance.Session.Points += this.count_points_on_solve;
                        CubeSessionInfo info = MeliHelperModule.Instance.Session.CubeInfo[id];
                        info.isCubeSolved = true;
                        foreach (JournalMove move in this.journal.GetListMoves)
                            info.GetJournal.AddMove(move);
                        if (MeliHelperModule.Instance.Session.RerollSeedOnSolve)
                        {
                            SeedController.GenerateNewSeed();
                            Level level = SceneAs<Level>();
                            Player player = level.Tracker.GetEntity<Player>();
                            if (player != null && !player.Dead)
                                level.Add(new CustomRubikWaveText(player, Dialog.Get("EVIDENCE02_RCM_MESSAGE_SEEDREROLLED")));
                        }
                        Add(new Coroutine(CutsceneSolve()));
                    }
                    event_rotate = null;
                }
                return;
            }


            switch (state)
            {
                case RCMEnum_CubeState.Scramble:
                    if (delay_scramble > 0)
                        delay_scramble -= Engine.DeltaTime;
                    else
                    {
                        int val = scramble_rotates[id_scramble];
                        int axis_id = Math.Abs(val);
                        bool is_clockwise = (val > 0);
                        TryRotateAxis(list_axis[axis_id], is_clockwise, is_manual: false);
                        if (++id_scramble >= scramble_rotates.Length)
                            state = RCMEnum_CubeState.Normal;
                        if (id_scramble % 3 == 0)
                            Audio.Play(SFX.char_bad_dreamblock_enter);
                    }
                    break;

                case RCMEnum_CubeState.Descramble:
                    Axis axis_m = journal.GetLastAxis();
                         if (axis_m != null) TryRotateAxis(axis_m, true, true );
                    else state = RCMEnum_CubeState.Normal;
                    break;

                case RCMEnum_CubeState.Normal: break;
                case RCMEnum_CubeState.Solved: break;
            }
        }

        public override void Render()
        {
            base.Render();
            projection.Render();
        }

        public void RenderDebugFaces(Vector2 pos)
        {
            foreach (var item in list_faces)
            {
                item.RenderDebug(pos);
                pos.Y += 8;
            }
        }

        public void RenderDebugFaceDetail(Vector2 pos, int id_face)
        {
            list_faces[id_face].RenderDebugDetail(pos);
        }

        IEnumerator CutsceneSolve()
        {
            Level level = SceneAs<Level>();
            foreach (var item in level.Entities.FindAll<RubikMoveTrigger>())
                item.RemoveSelf();
            foreach (var item in level.Entities.FindAll<RubikCubeProjectionEntity>())
                item.RemoveSelf();

            // 5 seconds to rotate
            Color[] mass_colors = { COLOR_UP, COLOR_LEFT, COLOR_FRONT, COLOR_DOWN, COLOR_RIGHT, COLOR_BACK };
            float angle_particles = 0;
            float delx = 0;
            int id_color = 0;
            while (delx < 12f)
            {
                projection.AddVectorRotate(new Vector3(0.24f * delx * Engine.DeltaTime, delx * Engine.DeltaTime, 0));
                delx += 2.4f * Engine.DeltaTime;
                angle_particles += 0.3f * delx * Engine.DeltaTime;
                level.ParticlesFG.Emit(ParticleTypes.SparkyDust, this.Center + new Vector2(
                    0.8f * cube_size * (float)Math.Cos(angle_particles),
                    0.8f * cube_size * (float)Math.Sin(angle_particles)
                    ),
                    mass_colors[id_color]);
                level.ParticlesFG.Emit(ParticleTypes.SparkyDust, this.Center - new Vector2(
                    0.8f * cube_size * (float)Math.Cos(angle_particles),
                    0.8f * cube_size * (float)Math.Sin(angle_particles)
                    ),
                    mass_colors[id_color]);
                if (++id_color >= mass_colors.Length)
                    id_color = 0;
                yield return null;
            }

            // 1.2 seconds to stop
            while (delx > 0)
            {
                projection.AddVectorRotate(new Vector3(0.2f * delx * Engine.DeltaTime, delx * Engine.DeltaTime, 0));
                delx -= 10f * Engine.DeltaTime;
                yield return null;
            }

            yield return 0.3f;

            // Final spark
            for (int i = 0; i < 40; i++)
            {
                float angle = i * 2 * (float)Math.PI / 40;
                level.ParticlesFG.Emit(ParticleTypes.SparkyDust, this.Center + new Vector2(
                    0.8f * cube_size * (float)Math.Cos(angle),
                    0.8f * cube_size * (float)Math.Sin(angle)
                    ), 
                    Color.Yellow
                    );
            }
        }

        public string GetCubeID
        {
            get
            {
                return id;
            }
        }

        public Journal GetJournalManual
        {
            get
            {
                return journal;
            }
        }

        public CubeProjection GetProjection
        {
            get
            {
                return projection;
            }
        }

        public List<Sticker> GetListStickers
        {
            get
            {
                return list_stickers;
            }
        }

        public RCMEnum_CubeState GetState
        {
            get
            {
                return state;
            }
        }

        public bool isRotateClockwise
        {
            get
            {
                return is_rotate_clockwise;
            }
            set
            {
                is_rotate_clockwise = value;
            }
        }

        public int GetStatePercentage
        {
            get
            {
                if (state == RCMEnum_CubeState.Scramble)
                    return (100 * id_scramble / scramble_rotates.Length);

                return 0;
            }
        }

    }
}
