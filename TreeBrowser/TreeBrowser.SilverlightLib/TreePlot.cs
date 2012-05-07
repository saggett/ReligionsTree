using System;
using System.Collections.Generic;
using System.Linq;
using TreeBrowser.Entities;

namespace TreeBrowser.SilverlightLib
{
    public class TreePlot : List<LineageCoords>
    {

        private int _canvasWidth;
        private int _canvasHeight;
        private int _maxYear;
        private int _rootStartYear;
        private int _xSpaceBetweenLines;

        public static TreePlot CreateTreePlot(ReadOnlyLineage rootLineage, int maxYear, int canvasHeight, int canvasWidth)
        {
            var plot = new TreePlot();
            plot.Create(rootLineage, maxYear, canvasHeight, canvasWidth);
            return plot;
        }

        private double ThicknessScaleFactor
        {
            get { return 1 + (0.2 * ((Convert.ToDouble(_xSpaceBetweenLines) / 40) - 1)); }
        }

        private double FontScaleFactor
        {
            get { return 1 + (0.2 * ((Convert.ToDouble(_xSpaceBetweenLines) / 40) - 1)); }
        }

        private void Create(ReadOnlyLineage rootLineage, int maxYear, int canvasHeight, int canvasWidth)
        {
            _canvasWidth = canvasWidth;
            _canvasHeight = canvasHeight;
            _maxYear = maxYear;
            _xSpaceBetweenLines = CalcXSpaceBetweenLines(_canvasWidth, rootLineage.ChildCount + 1);
            _rootStartYear = rootLineage.StartYear;
            int rootXCoord = GetRootXCoord(rootLineage);
            LineageCoords rootCoords = AddRootLine(rootLineage, rootXCoord);
            bool drawLeft = true;
            foreach (ReadOnlyLineage lineage in rootLineage.DirectChildren.OrderByDescending(l => l.StartYear))
            {
                AddBranch(rootCoords, lineage, rootXCoord, drawLeft);
                drawLeft = !drawLeft;
            }
        }

        private LineageCoords AddRootLine(ReadOnlyLineage root, int rootX)
        {
            int effectiveEndYear = CalcEffectiveEndYear(root);
            int yStart = ConvertYearToYCoord(root.StartYear);
            int yEnd = ConvertYearToYCoord(effectiveEndYear);
            LineageCoords coords = new LineageCoords(rootX, yStart, rootX, yEnd, root, ThicknessScaleFactor, FontScaleFactor);
            Add(coords);
            return coords;
        }

        /// <summary>
        /// If the lineage has an end year set, returns it. Otherwise returns the earlier of the current year and the maximum year of the plot.
        /// </summary>
        /// <param name="lin"></param>
        /// <returns></returns>
        private int CalcEffectiveEndYear(ReadOnlyLineage lin)
        {
            return lin.EndYear.HasValue ? lin.EndYear.Value : Math.Min(_maxYear, DateTime.Today.Year);
        }

        public void AddBranch(LineageCoords parentCoords, ReadOnlyLineage lin, int parentX, bool drawLeft)
        {
            LineageCoords myCoords = AddLineage(parentCoords, lin, parentX, drawLeft);
            foreach (ReadOnlyLineage child in lin.DirectChildren.OrderByDescending(l => l.StartYear))
                AddBranch(myCoords, child, myCoords.AbsoluteXEnd, drawLeft);
        }

        private LineageCoords AddLineage(LineageCoords parentCoords, ReadOnlyLineage lin, int parentX, bool drawLeft)
        {
            //int lineYStart = ConvertYearToYCoord(lin.StartYear);
            int lineX = drawLeft ? CalcMinX() - _xSpaceBetweenLines : CalcMaxX() + _xSpaceBetweenLines;
            int effectiveEndYear = CalcEffectiveEndYear(lin);
            int yStart = ConvertYearToYCoord(lin.StartYear);
            int yEnd = ConvertYearToYCoord(effectiveEndYear);
            var newCoords = new LineageCoords(parentX, yStart, lineX, yEnd, lin, ThicknessScaleFactor, FontScaleFactor);
            Add(newCoords);
            if (parentCoords != null)
                parentCoords.Children.Add(newCoords);
            return newCoords;
        }

        private int ConvertYearToYCoord(int year)
        {
            return _canvasHeight - Convert.ToInt32(_canvasHeight * ((year - _rootStartYear) / (double)(_maxYear - _rootStartYear)));
        }

        private static int CalcXSpaceBetweenLines(int canvasWidth, int lineageCount)
        {
            int neededSpace = canvasWidth / lineageCount;
            return neededSpace > 10 ? neededSpace : 10;
    }

        private int CalcMinX()
        {
            return this.Min(lc => lc.AbsoluteXEnd);
        }

        private int CalcMaxX()
        {
            return this.Max(lc => lc.AbsoluteXEnd);
        }

        private int GetRootXCoord(ReadOnlyLineage root)
        {
            return (int)(CalcRootXPosition(root) * _canvasWidth);
        }

        private static double CalcRootXPosition(ReadOnlyLineage root)
        {
            bool drawLeft = true;
            int leftChildCount = 0;
            int rightChildCount = 0;
            foreach (ReadOnlyLineage lineage in root.DirectChildren.OrderByDescending(l => l.StartYear))
            {
                if (drawLeft)
                {
                    leftChildCount += 1;
                    leftChildCount += lineage.ChildCount;
                }
                else
                {
                    rightChildCount += 1;
                    rightChildCount += lineage.ChildCount;
                }
                drawLeft = !drawLeft;
            }
            if (rightChildCount == 0 & leftChildCount != 0)
                return 0.95;
            if ((rightChildCount + leftChildCount) <= 2)
                return 0.5;
            return leftChildCount / (double)(leftChildCount + rightChildCount);
        }

    }
}