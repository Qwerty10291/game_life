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
        private List<List<int>> neighbors;
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
            initArea(width, height);
        }

        public CellType CellAt(int x, int y)
        {
            
            (x, y) = checkCoords(x, y);
            return area[y][x];
        }

        public void SetCell(CellType cell, int x, int y)
        {
            var prevCell = area[y][x];
            area[y][x] = cell;
            if (cell == CellType.Alive)
            {
                aliveCount += 1;
                if (prevCell != cell)
                    updateNeighbors(x, y);
            }
            else if (cell == CellType.Dead)
            {
                aliveCount -= 1;
                if (prevCell != cell)
                    updateNeighbors(x, y);
            }
        }

        public bool IsAlive(int x, int y)
        {
            return CellAt(x, y) == CellType.Alive || CellAt(x, y) == CellType.ToDead;
        }

        public int NeighborsCount(int x, int y)
        {
            return neighbors[y][x];
        }

        public void SetWidth(int width)
        {
            if (width < 0)
            {
                throw new ArgumentOutOfRangeException("width");
            }
            initArea(width, Height);
        }

        public void SetHeight(int height)
        {
            if (height < 0)
            {
                throw new ArgumentOutOfRangeException("height");
            }
            initArea(Width, height);
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

        public void Clear()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    area[y][x] = CellType.Dead;
                    neighbors[y][x] = 0;
                }
            }
        }
        
        public void FillRandom()
        {
            var rand = new Random();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var num = rand.Next(0, 2);
                    SetCell(num == 0 ? CellType.Dead : CellType.Alive, x, y);
                }
            }
        }

        private void updateNeighbors(int x, int y)
        {
            var delta = IsAlive(x, y) ? 1 : -1;
            updateNeighbor(x - 1, y - 1, delta);
            updateNeighbor(x, y - 1, delta);
            updateNeighbor(x + 1, y - 1, delta);
            updateNeighbor(x - 1, y, delta);
            updateNeighbor(x + 1, y, delta);
            updateNeighbor(x - 1, y + 1, delta);
            updateNeighbor(x, y + 1, delta);
            updateNeighbor(x + 1, y + 1, delta);
        }

        private void updateNeighbor(int x, int y, int delta)
        {
            (x, y) = checkCoords(x, y);
            neighbors[y][x] += delta;
        }

        private (int, int) checkCoords(int x, int y)
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
            return (x, y);
        }

        private void initArea(int width, int height)
        {
            area = new List<List<CellType>>(height);
            neighbors = new List<List<int>>(height);
            for (int i = 0; i < height; i++)
            {
                area.Add(new List<CellType>(Enumerable.Range(0, width).Select(x => CellType.Dead)));
                neighbors.Add(new List<int>(Enumerable.Range(0, width).Select(x => 0)));
            }
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
