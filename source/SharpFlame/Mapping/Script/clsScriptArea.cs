using System;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Collections;
using SharpFlame.Colors;
using SharpFlame.Core.Domain;
using SharpFlame.FileIO;
using SharpFlame.FileIO.Ini;
using SharpFlame.Mapping.Drawing;
using SharpFlame.Maths;
using SharpFlame.Util;

namespace SharpFlame.Mapping.Script
{
    public class clsScriptArea
    {
        public clsScriptArea()
        {
            _ParentMapLink = new ConnectedListLink<clsScriptArea, clsMap>(this);
        }

        private ConnectedListLink<clsScriptArea, clsMap> _ParentMapLink;

        public ConnectedListLink<clsScriptArea, clsMap> ParentMap
        {
            get { return _ParentMapLink; }
        }

        private string _Label;

        public string Label
        {
            get { return _Label; }
        }

        private XYInt _PosA;
        private XYInt _PosB;

        public XYInt PosA
        {
            set
            {
                clsMap Map = _ParentMapLink.Source;
                _PosA.X = MathUtil.Clamp_int(value.X, 0, Map.Terrain.TileSize.X * App.TerrainGridSpacing - 1);
                _PosA.Y = MathUtil.Clamp_int(value.Y, 0, Map.Terrain.TileSize.Y * App.TerrainGridSpacing - 1);
                MathUtil.ReorderXY(_PosA, _PosB, ref _PosA, ref _PosB);
            }
        }

        public XYInt PosB
        {
            set
            {
                clsMap Map = _ParentMapLink.Source;
                _PosB.X = MathUtil.Clamp_int(value.X, 0, Map.Terrain.TileSize.X * App.TerrainGridSpacing - 1);
                _PosB.Y = MathUtil.Clamp_int(value.Y, 0, Map.Terrain.TileSize.Y * App.TerrainGridSpacing - 1);
                MathUtil.ReorderXY(_PosA, _PosB, ref _PosA, ref _PosB);
            }
        }

        public int PosAX
        {
            get { return _PosA.X; }
            set
            {
                _PosA.X = MathUtil.Clamp_int(value, 0,
                    Convert.ToInt32(Convert.ToInt32(_ParentMapLink.Source.Terrain.TileSize.X * App.TerrainGridSpacing) - 1));
                MathUtil.ReorderXY(_PosA, _PosB, ref _PosA, ref _PosB);
            }
        }

        public int PosAY
        {
            get { return _PosA.Y; }
            set
            {
                _PosA.Y = MathUtil.Clamp_int(value, 0,
                    Convert.ToInt32(Convert.ToInt32(_ParentMapLink.Source.Terrain.TileSize.Y * App.TerrainGridSpacing) - 1));
                MathUtil.ReorderXY(_PosA, _PosB, ref _PosA, ref _PosB);
            }
        }

        public int PosBX
        {
            get { return _PosB.X; }
            set
            {
                _PosB.X = MathUtil.Clamp_int(value, 0,
                    Convert.ToInt32(Convert.ToInt32(_ParentMapLink.Source.Terrain.TileSize.X * App.TerrainGridSpacing) - 1));
                MathUtil.ReorderXY(_PosA, _PosB, ref _PosA, ref _PosB);
            }
        }

        public int PosBY
        {
            get { return _PosB.Y; }
            set
            {
                _PosB.Y = MathUtil.Clamp_int(value, 0,
                    Convert.ToInt32(Convert.ToInt32(_ParentMapLink.Source.Terrain.TileSize.Y * App.TerrainGridSpacing) - 1));
                MathUtil.ReorderXY(_PosA, _PosB, ref _PosA, ref _PosB);
            }
        }

        public static clsScriptArea Create(clsMap Map)
        {
            clsScriptArea Result = new clsScriptArea();

            Result._Label = Map.GetDefaultScriptLabel("Area");

            Result._ParentMapLink.Connect(Map.ScriptAreas);

            return Result;
        }

        public void SetPositions(XYInt PosA, XYInt PosB)
        {
            clsMap Map = _ParentMapLink.Source;

            PosA.X = MathUtil.Clamp_int(PosA.X, 0, Map.Terrain.TileSize.X * App.TerrainGridSpacing - 1);
            PosA.Y = MathUtil.Clamp_int(PosA.Y, 0, Map.Terrain.TileSize.Y * App.TerrainGridSpacing - 1);
            PosB.X = MathUtil.Clamp_int(PosB.X, 0, Map.Terrain.TileSize.X * App.TerrainGridSpacing - 1);
            PosB.Y = MathUtil.Clamp_int(PosB.Y, 0, Map.Terrain.TileSize.Y * App.TerrainGridSpacing - 1);

            MathUtil.ReorderXY(PosA, PosB, ref _PosA, ref _PosB);
        }

        public void GLDraw()
        {
            clsDrawTerrainLine Drawer = new clsDrawTerrainLine();
            Drawer.Map = _ParentMapLink.Source;
            if ( Program.frmMainInstance.SelectedScriptMarker == this )
            {
                GL.LineWidth(4.5F);
                Drawer.Colour = new sRGBA_sng(1.0F, 1.0F, 0.5F, 0.75F);
            }
            else
            {
                GL.LineWidth(3.0F);
                Drawer.Colour = new sRGBA_sng(1.0F, 1.0F, 0.0F, 0.5F);
            }

            Drawer.StartXY = _PosA;
            Drawer.FinishXY.X = _PosB.X;
            Drawer.FinishXY.Y = _PosA.Y;
            Drawer.ActionPerform();

            Drawer.StartXY = _PosA;
            Drawer.FinishXY.X = _PosA.X;
            Drawer.FinishXY.Y = _PosB.Y;
            Drawer.ActionPerform();

            Drawer.StartXY.X = _PosB.X;
            Drawer.StartXY.Y = _PosA.Y;
            Drawer.FinishXY = _PosB;
            Drawer.ActionPerform();

            Drawer.StartXY.X = _PosA.X;
            Drawer.StartXY.Y = _PosB.Y;
            Drawer.FinishXY = _PosB;
            Drawer.ActionPerform();
        }

        public void MapResizing(XYInt PosOffset)
        {
            SetPositions(new XYInt(_PosA.X - PosOffset.X, _PosA.Y - PosOffset.Y), new XYInt(_PosB.X - PosOffset.X, _PosB.Y - PosOffset.Y));
        }

        public void WriteWZ(IniWriter File)
        {
            File.AppendSectionName("area_" + _ParentMapLink.ArrayPosition.ToStringInvariant());
            File.AppendProperty("pos1", _PosA.X.ToStringInvariant() + ", " + _PosA.Y.ToStringInvariant());
            File.AppendProperty("pos2", _PosB.X.ToStringInvariant() + ", " + _PosB.Y.ToStringInvariant());
            File.AppendProperty("label", _Label);
            File.Gap_Append();
        }

        public sResult SetLabel(string Text)
        {
            sResult Result = new sResult();

            Result = _ParentMapLink.Source.ScriptLabelIsValid(Text);
            if ( Result.Success )
            {
                _Label = Text;
            }
            return Result;
        }

        public void Deallocate()
        {
            _ParentMapLink.Deallocate();
        }
    }
}