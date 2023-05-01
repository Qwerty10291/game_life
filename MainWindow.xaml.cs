using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Diagnostics;

namespace game_life
{
    public partial class MainWindow : Window
    {
        private GameLife game;
        private GameLifeUi gameWidget;
        private bool isRunning = false;
        private DispatcherTimer timer;
        public MainWindow()
        {
            InitializeComponent();
            game = new GameLife(6, 6);
            gameWidget = new GameLifeUi(game.Area, Brushes.White, Brushes.Black);
            mainGrid.Children.Add(gameWidget);
            Grid.SetColumn(gameWidget, 0);
            Grid.SetRow(gameWidget, 0);
            gameWidget.CellUpdate = GameCellClicked;

            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(1000000);
        }

        private void GameCellClicked(MouseEventArgs e, int x, int y)
        {
            if (isRunning)
                return;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                game.Area.SetCell(CellType.Alive, x, y);
                gameWidget.Draw();
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                game.Area.SetCell(CellType.Dead, x, y);
                gameWidget.Draw();
            }
        }

        private void setSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            speedStatus.Content = $"Скорость: {e.NewValue}";
            if (timer != null) { 
                timer.Interval = new TimeSpan(100000 * (int)e.NewValue);
            }
        }

        private void updateSize_Click(object sender, RoutedEventArgs e)
        {
            try { 
                var width = Int32.Parse(areaWidth.Text);
                var height = Int32.Parse(areaHeight.Text);
                if (width > 0)
                {
                    game.Area.Width = width;
                }
                if (height > 0)
                {
                game.Area.Height = height;
                }
                gameWidget.Draw();
            }
            catch(Exception err){
                Trace.WriteLine(err);
                return;
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!isRunning)
            {
                btnStart.Content = "Стоп";
                timer.Start();
                isRunning = true;
            } else
            {
                btnStart.Content = "Старт";
                timer.Stop();
                isRunning = false;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            gameWidget.Draw();
        }

        private void timer_Tick(object sender, EventArgs e)

        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            game.Update();
            watch.Stop();
            Trace.WriteLine(watch.ElapsedMilliseconds);
            gameWidget.Draw();
            if (game.IsStable)
            {
                btnStart.Content = "Старт";
                timer.Stop();
                isRunning = false;
            }
        }
    }
}
