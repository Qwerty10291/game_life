using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Win32;
using System.IO;
using System.Linq;

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
            game = new GameLife(100, 100);
            gameWidget = new GameLifeUi(game.Area, Brushes.White, Brushes.Black);
            mainGrid.Children.Add(gameWidget);
            Grid.SetColumn(gameWidget, 0);
            Grid.SetRow(gameWidget, 0);
            gameWidget.CellUpdate = GameCellClicked;

            timer = new DispatcherTimer(DispatcherPriority.Render);
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
            startStop();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            gameWidget.Draw();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            game.Update();
            gameWidget.Draw();
            if (game.IsStable)
            {
                btnStart.Content = "Старт";
                timer.Stop();
                isRunning = false;
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            if (!isRunning)
            {
                game.Area.Clear();
                gameWidget.Draw();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    startStop();
                    break;
                case Key.Delete:
                    game.Area.Clear();
                    gameWidget.Draw();
                    break;
            }
        }

        private void startStop()
        {
            if (!isRunning)
            {
                btnStart.Content = "Стоп";
                timer.Start();
                isRunning = true;
            }
            else
            {
                btnStart.Content = "Старт";
                timer.Stop();
                isRunning = false;
            }
        }

        private void btnFillRandom_Click(object sender, RoutedEventArgs e)
        {
            if (!isRunning)
            {
                game.Area.FillRandom();
                gameWidget.Draw();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
                return;
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Text Files (*.txt)|*.txt";
            if (dialog.ShowDialog() == true)
            {
                using (StreamWriter writer = new StreamWriter(dialog.FileName))
                {
                    for (int y = 0; y < game.Area.Height; y++)
                    {
                        for (int x = 0; x < game.Area.Width; x++)
                        {
                            writer.Write(game.Area.IsAlive(x, y) ? 1 : 0);
                        }
                        writer.Write('\n');
                    }
                }
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
            {
                return;
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text Files (*.txt)|*.txt";
            if (dialog.ShowDialog() == true)
            {
                var lines = File.ReadAllLines(dialog.FileName);
                HashSet<int> nums = new HashSet<int>(lines.Select(x => x.Length));
                if (lines.Length == 0 || nums.Count != 1)
                {
                    MessageBox.Show("Неверный формат файла");
                    return;
                }
                game.Area.SetHeight(lines.Length);
                game.Area.SetWidth(lines[0].Length);
                for(int y = 0; y < lines.Length; y++)
                {
                    for (int x = 0; x < lines[0].Length; x++)
                    {
                        game.Area.SetCell(lines[y][x] - '0' == 0? CellType.Dead : CellType.Alive, x, y);
                    }
                }
                gameWidget.Draw();
            }
        }
    }
}
