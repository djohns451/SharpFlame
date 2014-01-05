using SharpFlame.Collections;
using SharpFlame.MathExtra;

namespace SharpFlame
{
    public partial class clsMap
    {
        public class clsPointChanges
        {
            public bool[,] PointIsChanged;
            public SimpleList<clsXY_int> ChangedPoints = new SimpleList<clsXY_int>();

            public clsPointChanges(sXY_int PointSize)
            {
                PointIsChanged = new bool[PointSize.X, PointSize.Y];
                ChangedPoints.MinSize = PointSize.X * PointSize.Y;
                ChangedPoints.Clear();
            }

            public void Changed(sXY_int Num)
            {
                if ( !PointIsChanged[Num.X, Num.Y] )
                {
                    PointIsChanged[Num.X, Num.Y] = true;
                    ChangedPoints.Add(new clsXY_int(Num));
                }
            }

            public void SetAllChanged()
            {
                int X = 0;
                int Y = 0;
                sXY_int Num = new sXY_int();

                for ( Y = 0; Y <= PointIsChanged.GetUpperBound(1); Y++ )
                {
                    Num.Y = Y;
                    for ( X = 0; X <= PointIsChanged.GetUpperBound(0); X++ )
                    {
                        Num.X = X;
                        Changed(Num);
                    }
                }
            }

            public void Clear()
            {
                clsXY_int Point = default(clsXY_int);

                foreach ( clsXY_int tempLoopVar_Point in ChangedPoints )
                {
                    Point = tempLoopVar_Point;
                    PointIsChanged[Point.X, Point.Y] = false;
                }
                ChangedPoints.Clear();
            }

            public void PerformTool(clsAction Tool)
            {
                clsXY_int Point = default(clsXY_int);

                foreach ( clsXY_int tempLoopVar_Point in ChangedPoints )
                {
                    Point = tempLoopVar_Point;
                    Tool.PosNum = Point.XY;
                    Tool.ActionPerform();
                }
            }
        }

        public abstract class clsMapTileChanges : clsPointChanges
        {
            public clsMap Map;
            public clsTerrain Terrain;

            public clsMapTileChanges(clsMap Map, sXY_int PointSize) : base(PointSize)
            {
                this.Map = Map;
                Terrain = Map.Terrain;
            }

            public void Deallocate()
            {
                Map = null;
            }

            public abstract void TileChanged(sXY_int Num);

            public void VertexChanged(sXY_int Num)
            {
                if ( Num.X > 0 )
                {
                    if ( Num.Y > 0 )
                    {
                        TileChanged(new sXY_int(Num.X - 1, Num.Y - 1));
                    }
                    if ( Num.Y < Terrain.TileSize.Y )
                    {
                        TileChanged(new sXY_int(Num.X - 1, Num.Y));
                    }
                }
                if ( Num.X < Terrain.TileSize.X )
                {
                    if ( Num.Y > 0 )
                    {
                        TileChanged(new sXY_int(Num.X, Num.Y - 1));
                    }
                    if ( Num.Y < Terrain.TileSize.Y )
                    {
                        TileChanged(Num);
                    }
                }
            }

            public void VertexAndNormalsChanged(sXY_int Num)
            {
                if ( Num.X > 1 )
                {
                    if ( Num.Y > 0 )
                    {
                        TileChanged(new sXY_int(Num.X - 2, Num.Y - 1));
                    }
                    if ( Num.Y < Terrain.TileSize.Y )
                    {
                        TileChanged(new sXY_int(Num.X - 2, Num.Y));
                    }
                }
                if ( Num.X > 0 )
                {
                    if ( Num.Y > 1 )
                    {
                        TileChanged(new sXY_int(Num.X - 1, Num.Y - 2));
                    }
                    if ( Num.Y > 0 )
                    {
                        TileChanged(new sXY_int(Num.X - 1, Num.Y - 1));
                    }
                    if ( Num.Y < Terrain.TileSize.Y )
                    {
                        TileChanged(new sXY_int(Num.X - 1, Num.Y));
                    }
                    if ( Num.Y < Terrain.TileSize.Y - 1 )
                    {
                        TileChanged(new sXY_int(Num.X - 1, Num.Y + 1));
                    }
                }
                if ( Num.X < Terrain.TileSize.X )
                {
                    if ( Num.Y > 1 )
                    {
                        TileChanged(new sXY_int(Num.X, Num.Y - 2));
                    }
                    if ( Num.Y > 0 )
                    {
                        TileChanged(new sXY_int(Num.X, Num.Y - 1));
                    }
                    if ( Num.Y < Terrain.TileSize.Y )
                    {
                        TileChanged(Num);
                    }
                    if ( Num.Y < Terrain.TileSize.Y - 1 )
                    {
                        TileChanged(new sXY_int(Num.X, Num.Y + 1));
                    }
                }
                if ( Num.X < Terrain.TileSize.X - 1 )
                {
                    if ( Num.Y > 0 )
                    {
                        TileChanged(new sXY_int(Num.X + 1, Num.Y - 1));
                    }
                    if ( Num.Y < Terrain.TileSize.Y )
                    {
                        TileChanged(new sXY_int(Num.X + 1, Num.Y));
                    }
                }
            }

            public void SideHChanged(sXY_int Num)
            {
                if ( Num.Y > 0 )
                {
                    TileChanged(new sXY_int(Num.X, Num.Y - 1));
                }
                if ( Num.Y < Map.Terrain.TileSize.Y )
                {
                    TileChanged(Num);
                }
            }

            public void SideVChanged(sXY_int Num)
            {
                if ( Num.X > 0 )
                {
                    TileChanged(new sXY_int(Num.X - 1, Num.Y));
                }
                if ( Num.X < Map.Terrain.TileSize.X )
                {
                    TileChanged(Num);
                }
            }
        }

        public class clsSectorChanges : clsMapTileChanges
        {
            public clsSectorChanges(clsMap Map) : base(Map, Map.SectorCount)
            {
            }

            public override void TileChanged(sXY_int Num)
            {
                sXY_int SectorNum = new sXY_int();

                SectorNum = Map.GetTileSectorNum(Num);
                Changed(SectorNum);
            }
        }

        public class clsAutoTextureChanges : clsMapTileChanges
        {
            public clsAutoTextureChanges(clsMap Map) : base(Map, Map.Terrain.TileSize)
            {
            }

            public override void TileChanged(sXY_int Num)
            {
                Changed(Num);
            }
        }

        public clsSectorChanges SectorGraphicsChanges;
        public clsSectorChanges SectorUnitHeightsChanges;
        public clsSectorChanges SectorTerrainUndoChanges;
        public clsAutoTextureChanges AutoTextureChanges;

        public class clsTerrainUpdate
        {
            public clsPointChanges Vertices;
            public clsPointChanges Tiles;
            public clsPointChanges SidesH;
            public clsPointChanges SidesV;

            public void Deallocate()
            {
            }

            public clsTerrainUpdate(sXY_int TileSize)
            {
                Vertices = new clsPointChanges(new sXY_int(TileSize.X + 1, TileSize.Y + 1));
                Tiles = new clsPointChanges(new sXY_int(TileSize.X, TileSize.Y));
                SidesH = new clsPointChanges(new sXY_int(TileSize.X, TileSize.Y + 1));
                SidesV = new clsPointChanges(new sXY_int(TileSize.X + 1, TileSize.Y));
            }

            public void SetAllChanged()
            {
                Vertices.SetAllChanged();
                Tiles.SetAllChanged();
                SidesH.SetAllChanged();
                SidesV.SetAllChanged();
            }

            public void ClearAll()
            {
                Vertices.Clear();
                Tiles.Clear();
                SidesH.Clear();
                SidesV.Clear();
            }
        }

        public clsTerrainUpdate TerrainInterpretChanges;
    }
}