using System;
using System.Collections.Generic;
using System.Linq;

namespace game_life
{
    public enum CellType
    {
        Dead,
        Alive,
        ToAlive,
        ToDead,
    }
    public class GameLifeArea
    {
        private List<List<CellType>> area;
        private int aliveCount = 0; 
        public int AliveCount { get => aliveCount; }
        public List<CellType> this[int key]
        {
            get => area[key];
        }
        public int Width
        {
            get => area[0].Count;
            set { SetWidth(value); }
        }

        public int Height
        {
            get => area.Count;
            set { SetHeight(value); }
        }

        public GameLifeArea(int width, int height)
        {
            area = new List<List<CellType>>(height);
            for (int i = 0; i < height; i++)
            {
                area.Add(new List<CellType>(Enumerable.Range(0, width).Select(x => CellType.Dead)));
            }
        }

        public CellType CellAt(int x, int y)
        {
            if (x < 0)
            {
                x = Width + x;
            }
            else if (x >= Width)
            {
                x -= Width;
            }
            if (y < 0)
            {
                y = Height + y;
            }
            else if (y >= Height)
            {
                y -= Height;
            }

            return area[y][x];
        }

        public void SetCell(CellType cell, int x, int y)
        {
            if (cell == CellType.Alive)
                aliveCount += 1;
            else if (cell == CellType.Dead)
                aliveCount -= 1;
            area[y][x] = cell;
        }

        public bool IsAlive(int x, int y)
        {
            return CellAt(x, y) == CellType.Alive || CellAt(x, y) == CellType.ToDead;
        }

        public int NeighborsCount(int x, int y)
        {
            return boolToInt(IsAlive(x - 1, y - 1)) +
                boolToInt(IsAlive(x, y - 1)) +
                boolToInt(IsAlive(x + 1, y - 1)) +
                boolToInt(IsAlive(x - 1, y)) +
                boolToInt(IsAlive(x + 1, y)) +
                boolToInt(IsAlive(x - 1, y + 1)) +
                boolToInt(IsAlive(x, y + 1)) +
                boolToInt(IsAlive(x + 1, y + 1));
        }

        public void SetWidth(int width)
        {
            if (width < 0)
            {
                throw new ArgumentOutOfRangeException("width");
            }
            if (width > area[0].Count)
            {
                foreach (var row in area)
                {
                    row.AddRange(Enumerable.Range(0, width - row.Count).Select(x => CellType.Dead));
                }
            }
            else if (width < area[0].Count)
            {
                foreach (var row in area)
                {
                    row.RemoveRange(width, Width - width);
                }
            }
        }

        public void SetHeight(int height)
        {
            if (height < 0)
            {
                throw new ArgumentOutOfRangeException("height");
            }
            if (height > area.Count)
            {
                for (int i = 0; i < height - area.Count; i++)
                {
                    area.Add(new List<CellType>(Enumerable.Range(0, Width).Select(x => CellType.Dead)));
                }
            }
            else if (height < area.Count)
            {
                area.RemoveRange(area.Count - height - 1, height);
            }
        }

        public void UpdateArea()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    switch (area[y][x])
                    {
                        case CellType.ToAlive:
                            {
                                SetCell(CellType.Alive, x, y);
                                break;
                            }
                        case CellType.ToDead:
                            { 
                                SetCell(CellType.Dead, x, y); 
                                break; 
                            }
                    }
                }
            }
        }

        private int boolToInt(bool value)
        {
            return value ? 1 : 0;
        }
    }
    class GameLife
    {
        private GameLifeArea area;
        public GameLifeArea Area { get => area; }
        private bool isStable = true;
        public bool IsStable { get => isStable; }
        public GameLife(int areaWidth, int areaHeight)
        {
            area = new GameLifeArea(areaWidth, areaHeight);
        }

        public void Update()
        {
            int updated = 0;
            for (int x = 0; x < area.Width; x ++)
            {
                for (int y = 0; y < area.Height; y ++)
                {
                    var neighbors = area.NeighborsCount(x, y);
                    if (area.IsAlive(x, y)) {
                        if (neighbors < 2 || neighbors > 3)
                        { 
                            updated++;
                            area.SetCell(CellType.ToDead, x, y);
                        }
                    }
                    if (!area.IsAlive(x, y) && neighbors == 3)
                    {
                        updated++;
                        area.SetCell(CellType.ToAlive, x, y);
                    }
                }
            }
            area.UpdateArea();
            isStable = area.AliveCount == 0 || updated == 0;
        }


    }
}
