using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;

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
        private int cellWidth => (int)(ActualWidth / area.Width);
        private int cellHeight => (int)(ActualHeight / area.Height);
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
        }

        public void Draw()
        {
            var timeStart = DateTime.Now;
            int width = cellWidth;
            int height = cellHeight;
            int areaWidth = area.Width;
            int areaHeight = area.Height;
            var rect = new Rect();
            rect.Width = width;
            rect.Height = height;

            var ctx = canvas.RenderOpen();
            ctx.DrawRectangle(deadBrush,null, new Rect(0, 0, areaWidth * width, areaHeight * height));
            for (int x = 0; x < areaWidth; x++)
            {
                for (int y = 0; y < areaHeight; y++)
                {
                    if (area.IsAlive(x, y))
                    { 
                        rect.X = x * width;
                        rect.Y = y * height;
                        ctx.DrawRectangle(aliveBrush, null, rect);
                    }
                }
            }
            ctx.DrawRectangle(null, penBorder, new Rect(0, 0, ActualWidth, ActualHeight));
            ctx.Close();
        }

        private void GameLifeUi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CellUpdate != null)
            {
                var p = e.GetPosition(this);
                var x = (int)p.X / cellWidth;
                var y = (int)p.Y / cellHeight;
                if (x >= 0 && x < area.Width && y >= 0 && y < area.Height)
                {
                    CellUpdate(e, (int)p.X / cellWidth, (int)p.Y / cellHeight);
                }
            }
        }

        private void GameLifeUi_MouseMove(object sender, MouseEventArgs e)
        {
            if (CellUpdate != null)
            {
                var p = e.GetPosition(this);
                var x = (int)p.X / cellWidth;
                var y = (int)p.Y / cellHeight;
                if (x >= 0 && x < area.Width && y >= 0 && y < area.Height)
                { 
                    CellUpdate(e, (int)p.X / cellWidth, (int)p.Y / cellHeight);
                }
            }
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
