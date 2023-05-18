using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace game_life
{
    public class GameLifeUi : FrameworkElement
    {
        private readonly VisualCollection _children;
        private DrawingVisual canvas;
        private GameLifeArea area;
        private Brush aliveBrush;
        private Brush deadBrush;
        private Pen penBorder = new Pen(Brushes.Gray, 2);
        private int cellSize = -1;
        private int currX = 0;
        private int currY = 0;
        private int lastMouseX = -1;
        private int lastMouseY = -1;
        public Action<MouseEventArgs, int, int> CellUpdate;

        public GameLifeUi(GameLifeArea area, Brush aliveBrush, Brush deadBrush)
        {   
            canvas = new DrawingVisual();
            _children = new VisualCollection(this){canvas};
            this.area = area;
            this.aliveBrush = aliveBrush;
            this.deadBrush = deadBrush;
           
            MouseMove += GameLifeUi_MouseMove;
            MouseDown += GameLifeUi_MouseDown;
            PreviewMouseWheel += GameLifeUi_PreviewMouseWheel;
        }

        

        public void Draw()
        {
            var rect = new Rect();
            if (cellSize < 0)
                cellSize = Math.Max((int)Math.Min(ActualWidth, ActualHeight) / Math.Max(area.Width, area.Height), 1);
            rect.Width = cellSize;
            rect.Height = cellSize;
            var areaStartX = Math.Min(Math.Max((currX > 0 ? currX : 0) / cellSize - 1, 0), area.Width);
            var areaStartY = Math.Min(Math.Max((currY > 0 ? currY: 0) / cellSize - 1, 0), area.Height);
            var marginX = currX - areaStartX * cellSize;
            var marginY = currY - areaStartY * cellSize;
            var areaEndX = Math.Min(areaStartX + (int)ActualWidth / cellSize + 1, area.Width);
            var areaEndY = Math.Min(areaStartY + (int)ActualHeight / cellSize + 1, area.Height);
            
            var ctx = canvas.RenderOpen();
            
            var deadRect = new Rect(-marginX, -marginY, (areaEndX - areaStartX) * cellSize, (areaEndY - areaStartY) * cellSize);
            if (deadRect.X + deadRect.Width > ActualWidth)
            {
                deadRect.Width = Math.Max(0, ActualWidth - deadRect.X);
            }
            ctx.DrawRectangle(deadBrush, null, deadRect);

            for (int x = areaStartX; x < areaEndX; x++)
            {
                rect.X = (x - areaStartX) * cellSize - marginX;
                if (rect.X > ActualWidth)
                {
                    continue;
                }
                for (int y = areaStartY; y < areaEndY; y++)
                {
                    if (area[y][x] == CellType.Alive)
                    {
                        rect.Y = (y - areaStartY) * cellSize - marginY;
                        if (rect.Y > ActualHeight)
                        {
                            break;
                        }
                        ctx.DrawRectangle(aliveBrush , null, rect);
                    }
                }
            }
            ctx.DrawRectangle(null, penBorder, new Rect(0, 0, ActualWidth, ActualHeight));
            ctx.Close();
        }

        private void GameLifeUi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(this);
            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                case MouseButton.Right:
                    CellUpdate(e, (currX + (int)p.X) / cellSize, (currY + (int)p.Y) / cellSize);
                    break;
                case MouseButton.Middle:
                    lastMouseX = (int)p.X;
                    lastMouseY = (int)p.Y;
                    break;
            }
        }


        private void GameLifeUi_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                var p = e.GetPosition(this);
                currX = currX + (int)(lastMouseX - p.X);
                currY = currY + (int)(lastMouseY - p.Y);
                lastMouseX =(int)p.X;
                lastMouseY =(int)p.Y;
                Draw();
            } else if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
            {
                var p = e.GetPosition(this);
                CellUpdate(e, (currX + (int)p.X) / cellSize, (currY + (int)p.Y) / cellSize);
            }
        }

        private void GameLifeUi_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            cellSize += e.Delta / 120;
            if (cellSize < 1)
            {
                cellSize = 1;
            } else if (cellSize > (int)Math.Min(ActualWidth, ActualHeight)) {
                cellSize = (int)Math.Max(ActualWidth, ActualHeight);
            }
            Draw();
        }


        protected override int VisualChildrenCount => _children.Count;

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }
    }
}
