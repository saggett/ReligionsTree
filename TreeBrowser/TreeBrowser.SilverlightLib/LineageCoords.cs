using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using TreeBrowser.Entities;

namespace TreeBrowser.SilverlightLib
{
    public class LineageCoords : INotifyPropertyChanged
    {
        private readonly Collection<LineageCoords> _children = new Collection<LineageCoords>();
        private readonly Color _defaultLineColor = Colors.Brown;
        private const int DefaultLineThickness = 5;
        private const int DefaultFontSize = 12;

        private bool _isMouseOver;

        public LineageCoords(int xStart, int yStart, int xEnd, int yEnd, ReadOnlyLineage lin, double thicknessScaleFactor, double fontScaleFactor)
        {
            AbsoluteXStart = xStart;
            AbsoluteYStart = yStart;
            AbsoluteXEnd = xEnd;
            AbsoluteYEnd = yEnd;
            Lineage = lin;
            ThicknessScaleFactor = thicknessScaleFactor;
            FontScaleFactor = fontScaleFactor;
        }


        private double FontScaleFactor { get; set; }

        private double ThicknessScaleFactor { get; set; }

        public ReadOnlyLineage Lineage { get; private set; }

        public int AbsoluteOriginX
        {
            get { return Math.Min(AbsoluteXStart, AbsoluteXEnd); }
        }

        public int AbsoluteOriginY
        {
            get { return Math.Min(AbsoluteYStart, AbsoluteYEnd); }
        }

        public int AbsoluteXStart { get; private set; }
        public int AbsoluteXEnd { get; private set; }
        public int AbsoluteYStart { get; private set; }
        public int AbsoluteYEnd { get; private set; }

        public int XEnd
        {
            get { return AbsoluteXEnd - AbsoluteOriginX; }
        }

        public int YEnd
        {
            get { return AbsoluteYEnd - AbsoluteOriginY; }
        }

        public int XStart
        {
            get { return AbsoluteXStart - AbsoluteOriginX; }
        }

        public int YStart
        {
            get { return AbsoluteYStart - AbsoluteOriginY; }
        }

        public string LineageName
        {
            get { return Lineage.Name; }
        }

        public bool BranchesLeft
        {
            get { return XEnd < XStart; }
        }

        public int BranchAbsoluteMinX
        {
            get
            {
                int minX = Math.Min(AbsoluteXStart, AbsoluteXEnd);
                foreach (LineageCoords lineageCoord in Children)
                    minX = Math.Min(minX, lineageCoord.BranchAbsoluteMinX);
                return minX;
            }
        }

        public int BranchAbsoluteMaxX
        {
            get
            {
                int maxX = Math.Max(AbsoluteXStart, AbsoluteXEnd);
                foreach (LineageCoords lineageCoord in Children)
                    maxX = Math.Max(maxX, lineageCoord.BranchAbsoluteMaxX);
                return maxX;
            }
        }

        public int BranchAbsoluteMinY
        {
            get
            {
                int minY = Math.Min(AbsoluteYStart, AbsoluteYEnd);
                foreach (LineageCoords lineageCoord in Children)
                    minY = Math.Min(minY, lineageCoord.BranchAbsoluteMinY);
                return minY;
            }
        }

        public int BranchAbsoluteMaxY
        {
            get
            {
                int maxY = Math.Max(AbsoluteYStart, AbsoluteYEnd);
                foreach (LineageCoords lineageCoord in Children)
                    maxY = Math.Max(maxY, lineageCoord.BranchAbsoluteMaxY);
                return maxY;
            }
        }

        public int LabelX
        {
            get
            {

                //double lineToLabelDistance = (LineThickness * 4d) / AdjustedCanvasScaleFactor;
                //lineToLabelDistance = 18d / AdjustedCanvasScaleFactor;
                //if (lineToLabelDistance < 20)
                //    lineToLabelDistance = 20;
                //double asf = GetAdjustedScaleFactor(10d);
                double lineToLabelDistance = LineThickness + FontSize + 5d;

                return Convert.ToInt32(XEnd - lineToLabelDistance);
            }
        }

        public int LabelY
        {
            get { return YStart - Convert.ToInt32(LineThickness * 3); }
        }

        public string PolylinePointsString
        {
            get { return String.Format("{0},{1} {2},{1} {2},{3}", XStart, YStart, XEnd, YEnd); }
        }

        public Brush Brush
        {
            get
            {
                var brush = new SolidColorBrush(Color);
                brush.Opacity = ShowHighlight ? 1 : 0.6;
                return brush;
            }
        }

        //private double _canvasScaleFactor = 1;

        //public double CanvasScaleFactor
        //{
        //    get { return _canvasScaleFactor; }
        //    set
        //    {
        //        if (_canvasScaleFactor != value)
        //        {
        //            _canvasScaleFactor = value;
        //            PropertyChanged(this, new PropertyChangedEventArgs("CanvasScaleFactor"));
        //            PropertyChanged(this, new PropertyChangedEventArgs("FontSize"));
        //            PropertyChanged(this, new PropertyChangedEventArgs("LabelX"));
        //            PropertyChanged(this, new PropertyChangedEventArgs("LineThickness"));
        //        }
        //    }
        //}

        public int FontSize
        {
            get
            {
                double fontSize = (DefaultFontSize * FontScaleFactor);
                //if (fontSize < 4)
                //    fontSize = 4;
                //if (fontSize > 22)
                //    fontSize = 22;
                return Convert.ToInt32(fontSize);
            }
        }

        private bool ShowHighlight
        {
            get { return IsSelected | IsMouseOver; }
        }

        public int LineThickness
        {
            get
            {
                double highlightFactor = ShowHighlight ? 1.2d : 1d;
                double thickness = DefaultLineThickness * ThicknessScaleFactor * highlightFactor;
                //thickness = thickness / AdjustedCanvasScaleFactor;
                if (thickness < 3)
                    thickness = 3;
                //else if (thickness > 25)
                //    thickness = 25;
                return Convert.ToInt32(thickness);
            }
        }

        public string TooltipText
        {
            get
            {
                return Lineage.Description;
                //return string.Format("Click to {0} {1}", IsSelected ? "zoom in on" : "select", LineageName); 
            }
        }

        public Point AbsoluteLineageOriginPoint
        {
            get { return new Point(AbsoluteXEnd, AbsoluteYStart); }
        }

        public int LineageId
        {
            get { return Lineage.Id; }
        }

        public bool IsMouseOver
        {
            get { return _isMouseOver; }
            set
            {
                bool oldValue = _isMouseOver;
                _isMouseOver = value;
                if (oldValue != value)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Brush"));
                    PropertyChanged(this, new PropertyChangedEventArgs("LineThickness"));
                }
            }
        }

        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                bool oldValue = _IsSelected;
                _IsSelected = value;
                if (oldValue != value)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Brush"));
                    PropertyChanged(this, new PropertyChangedEventArgs("LineThickness"));
                    PropertyChanged(this, new PropertyChangedEventArgs("TooltipText"));
                }
            }
        }

        private Color Color
        {
            get { return GetColorForLineageGroup(Lineage.LineageGroupId); }
        }

        public Collection<LineageCoords> Children
        {
            get { return _children; }
        }

        public List<LineageCoords> SelfAndAllChildren
        {
            get
            {
                var coll = new List<LineageCoords>();
                coll.Add(this);
                foreach (LineageCoords coord in Children)
                    coll.AddRange(coord.SelfAndAllChildren);
                return coll;
            }
        }


        private Color GetColorForLineageGroup(int? lineageGroupId)
        {
            if (!lineageGroupId.HasValue)
                return _defaultLineColor;
            switch (lineageGroupId.Value)
            {
                case 1:
                    return Colors.Green;
                case 2:
                    return Colors.Blue;
                case 3:
                    return Colors.Orange;
                case 4:
                    return Colors.Purple;
                case 5:
                    return Colors.Red;
                case 6:
                    return Colors.Magenta;
                default:
                    throw new NotSupportedException();
            }
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}