using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SharpGL;
using SharpGL.Enumerations;

namespace GameOfLife
{
    /// <summary>
    /// Модель замкнутого мира Conway's Game of Life
    /// </summary>
    public class Life: IAnimated
    {
        static Random rand = new Random(1);
        /// <summary>
        /// Количество ячеек по горизонтали
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Количество ячеек по вертикали
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Количество возрастных групп
        /// </summary>
        public byte AgeCount { get; set; }
        const byte ZERO = 0;
        int _l = 0, _l1 = 1;
        byte[,,] _cells; // 2 буфера, чтобы не перезаписывать
        /// <summary>
        /// Время системы
        /// </summary>
        public int Time { get; set; }
        /// <summary>
        /// Публичный доступ к ячейкам
        /// </summary>
        /// <param name="l">Номер слоя</param>
        public byte this[int x, int y]
        {
            get => _cells[_l, y + 1, x + 1];
            set => _cells[_l, y + 1, x + 1] = value;
        }
        /// <summary>
        /// Доступ к ячейкам
        /// </summary>
        /// <param name="l">Номер слоя</param>
        byte this[int l, int x, int y]
        {
            get => _cells[l, y+1, x+1];
            set => _cells[l, y+1, x+1] = value;
        }
        // маска обхода соседей
        static int[,] mask = new int[2, 8]
        {
            {-1, 0, 1, -1, 1, -1, 0, 1 }, // x
            {-1, -1, -1, 0, 0, 1, 1 ,1 } // y
        };
        Statistic _stat; // для хранения количества ячеек каждого возраста
        /// <summary>
        /// Возвращает новое значение ячейки
        /// </summary>
        LifeWorker _calcWorker;
        public Life(int w = 192, int h = 108, byte ageCount = 16)
        {
            int buf1;
            _stat = new Statistic(ageCount);
            _calcWorker = (x, y) =>
            {
                buf1 = AliveNeighborCount(x, y);
                if (buf1 == 3)
                {
                    //SaveToBitmap();
                }
                // присвоение значений другому слою
                this[_l1, x,y] = 
                    buf1 == 3 || (buf1 == 2 && this[_l, x, y] == 0)? ZERO: // новорожденная ячейка
                    (this[_l, x, y] == AgeCount)? AgeCount: // если пусто, оставить в покое
                    (byte)(this[_l, x, y]+1); // увеличить возраст

                _stat.Add(this[_l1, x, y]);
            };
            Width = w;
            Height = h;
            AgeCount = ageCount;
            InitCells();
        }
        public void SaveToBitmap()
        {
            int l = 50;
            Brush sb;
            using (var bmp = new Bitmap((Width+2) * l, (Height+2) * l))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    for (int i = 0; i < Height + 2; i++)
                    {
                        for (int j = 0; j < Width + 2; j++)
                        {
                            sb = _cells[_l, i, j] == ZERO ? Brushes.Black : Brushes.White;
                            g.FillRectangle(sb, j * l, i * l, l, l);
                        }
                    }
                }
                bmp.Save("map.png");
            }
        }
        /// <summary>
        /// Возвращает количество живых клеток в окрестности данной
        /// </summary>
        private int AliveNeighborCount(int x, int y)
        {
            int count = 0;
            // обход по всем соседям
            for (int i = 0; i < mask.GetLength(1); i++)
            {
                if (this[_l, x + mask[0, i], y + mask[1, i]] == ZERO) count++;
            }
            return count;
        }
        /// <summary>
        /// Инициация массива клеток
        /// </summary>
        private void InitCells()
        {
            _cells = new byte[2, Height+2, Width+2];
            Clear();
        }
        // Рассчитать следующее состояние модели
        public void NextStage()
        {
            _stat.Clear();
            CopyBorderCells();
            PerformValue(_calcWorker);
            // инверсия слоев
            _l = ++_l & 1; // 0->1->0->...
            _l1 = ++_l1 & 1;
            Time++;
        }
        /// <summary>
        /// Имитация замкнутости - копирование крайних ячеек на противоположные дополнительные стороны
        /// </summary>
        private void CopyBorderCells()
        {
            // вертикальные
            for(int i = 0;i<Height;i++)
            {
                // слева направо
                this[_l, Width, i] = this[_l, 0, i];
                // справа налево
                this[_l, -1, i] = this[_l, Width - 1, i];
            }
            // горизонтальные
            for (int i = 0; i < Width; i++)
            {
                // сверху вниз
                this[_l, i, Height] = this[_l, i, 0];
                // снизу вверх
                this[_l, i, -1] = this[_l, i, Height - 1];
            }
            // угловые
            // SW -> NE
            this[_l, Width, -1] = this[_l, 0, Height-1];
            // NE -> SW
            this[_l, -1, Height] = this[_l, Width - 1, 0];
            // SE -> NW
            this[_l, -1, -1] = this[_l, Width-1, Height-1];
            // NW -> SE
            this[_l, Width, Height] = this[_l, 0, 0];
        }
        /// <summary>
        /// Заполнить случайно
        /// </summary>
        /// <param name="k">Коэффициент заполненности</param>
        public void FillRandom(double k)
        {
            PerformValue((x,y) => this[_l, x, y] = rand.NextDouble() < k ? ZERO : AgeCount);
        }
        /// <summary>
        /// Очистить все клетки
        /// </summary>
        public void Clear()
        {
            PerformValue((x, y) => this[_l, x,y] = AgeCount);
        }
        /// <summary>
        /// Добавить фигуру
        /// </summary>
        public void AddFigure(int x, int y, bool [,] f)
        {
            int x1 = x, y1 = y;
            for(int i = 0;i<f.GetLength(0);i++, y1++)
            {
                for (int j = 0; j < f.GetLength(1); j++, x1++)
                {
                    // цикличность
                    while (x1 > Width - 1) x1 -= Width;
                    while (y1 > Height - 1) y1 -= Height;
                    this[_l, x, y] = f[j, i] ? ZERO : AgeCount;
                }
            }
        }
        /// <summary>
        /// Инвертировать ячейку
        /// </summary>
        public void InvertCell(Point p)
        {
            InvertCell(p.X, p.Y);
        }
        private void InvertCell(int x, int y)
        {
             if (this[x,y]==ZERO)
            {
                this[x, y] = AgeCount;
                _stat.Delete(0);
            }
            else
            {
                _stat.Invert(this[x,y], 0);
                this[x, y] = ZERO;
            }
        }
        /// <summary>
        /// Оживить ячейку
        /// </summary>
        /// <param name="p">Позиция ячейку</param>
        public void ReviveCell(Point p)
        {
            if (this[p.X, p.Y] != ZERO)
            {
                _stat.Invert(this[p.X, p.Y], 0);
                this[p.X, p.Y] = ZERO;
                CellsChanged?.Invoke();
            }
        }
        /// <summary>
        /// Операция с клеткой
        /// </summary>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        /// <returns></returns>
        delegate void LifeWorker(int x, int y);
        /// <summary>
        /// Выполнить операцию с каждой ячейкой
        /// </summary>
        /// <param name="worker"></param>
        private void PerformValue(LifeWorker worker)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    worker(j, i);
                }
            }
        }
        /// <summary>
        /// Обработчик событий внешнего изменения ячеек
        /// </summary>
        public delegate void CellEventHandler();
        /// <summary>
        /// Происходит, когда пользователь меняет состояние ячейки
        /// </summary>
        public event CellEventHandler CellsChanged;
        public void Draw(Graphics g, GraphicsParam p)
        {
            p.Validate(this);
            float x = p.DrawRect.Left, y = p.DrawRect.Top, w = p.FirstWidth, h = p.FirstHeight;
            int cX = p.FirstCellX, cY = p.FirstCellY;
            //Console.WriteLine($"First = {p.FirstCellX},{p.FirstCellY}");
            for (int i = 0; i < p.CellCountH + 2; y += h, i++)
            {
                h = i == 0 ? p.FirstHeight : i == p.CellCountH + 1 ? p.LastHeight : p.CellHeight;
                x = p.DrawRect.Left;
                w = p.FirstWidth;
                cX = p.FirstCellX;
                // ячейка слева 
                g.FillRectangle(p.CellBrushes[this[cX++ % Width, cY % Height]], x, y, w, h);
                x += w;
                w = p.CellWidth;
                for (int j = 0; j < p.CellCountW; j++, x += w)
                {
                    g.FillRectangle(p.CellBrushes[this[cX++ % Width, cY % Height]], x, y, w, h);
                }
                // ячейка справа
                g.FillRectangle(p.CellBrushes[this[cX % Width, cY++ % Height]], x, y, p.LastWidth, h);
            }
            // отрисовка границ между ячейками
            x = p.DrawRect.X + p.FirstWidth;
            y = p.DrawRect.Y + p.FirstHeight;
            // вертикальные линии
            while (x < p.DrawRect.Right)
            {
                g.DrawLine(p.CellBorderPen, x, p.DrawRect.Top, x, p.DrawRect.Bottom);
                x += p.CellWidth;
            }
            // горизонтальные линии
            while (y < p.DrawRect.Bottom)
            {
                g.DrawLine(p.CellBorderPen, p.DrawRect.Left, y, p.DrawRect.Right, y);
                y += p.CellHeight;
            }
            // отрисовка границ клонов систем
            x = p.DrawRect.X + p.FirstWidth + p.CellWidth * (Width - p.FirstCellX-1);
            y = p.DrawRect.Y + p.FirstHeight + p.CellHeight * (Height - p.FirstCellY-1);
            // приведение к области отображения
            while (x < p.DrawRect.X) x += Width * p.CellWidth;
            while (y < p.DrawRect.Y) y += Height * p.CellHeight;
            // вертикальные линии
            while (x < p.DrawRect.Right)
            {
                g.DrawLine(p.BorderPen, x, p.DrawRect.Top, x, p.DrawRect.Bottom);
                x += Width * p.CellWidth;
            }
            // горизонтальные линии
            while(y<p.DrawRect.Bottom)
            {
                g.DrawLine(p.BorderPen, p.DrawRect.Left, y, p.DrawRect.Right, y);
                y += Height * p.CellHeight;
            }
            // статистика
            if (p.IsStatisticShow)
            {
                g.DrawStatistic(_stat, p.CellBrushes, p.FontBrush, p.StatRect);
            }
            float fH = p.TimingHeight / 4;
            using (var whiteTr = new SolidBrush(Color.FromArgb(192, Color.White))) g.FillRectangle(whiteTr, p.DrawRect.X, p.DrawRect.Y, 100, fH * 4);
            using (var f = new Font(Extension.Font, fH, GraphicsUnit.Pixel))
            {
                g.DrawString($"T = {Time}\nFPS = {p.FPS}\nSPF = {p.SPF}", f, p.FontBrush, p.DrawRect.X, p.DrawRect.Y);
            }
        }
        
        public void Draw(OpenGL gl, GraphicsParam p)
        {
            p.Validate(this);
            gl.ProjectToScreen(p.ClientSize.Width, p.ClientSize.Height);
            
            float x = p.DrawRect.Left, y = p.DrawRect.Top, w = p.FirstWidth, h = p.FirstHeight;
            int cX = p.FirstCellX, cY = p.FirstCellY;
            //Console.WriteLine($"First = {p.FirstCellX},{p.FirstCellY}");
            for (int i = 0; i < p.CellCountH + 2; y += h, i++)
            {
                h = i == 0 ? p.FirstHeight : i == p.CellCountH + 1 ? p.LastHeight : p.CellHeight;
                x = p.DrawRect.Left;
                w = p.FirstWidth;
                cX = p.FirstCellX;
                // ячейка слева 
                gl.FillRectangle((p.CellBrushes[this[cX++ % Width, cY % Height]] as SolidBrush).Color, x, y, w, h);
                x += w;
                w = p.CellWidth;
                for (int j = 0; j < p.CellCountW; j++, x += w)
                {
                    gl.FillRectangle(p.CellBrushes[this[cX++ % Width, cY % Height]].Color, x, y, w, h);
                }
                // ячейка справа
                gl.FillRectangle(p.CellBrushes[this[cX % Width, cY++ % Height]].Color, x, y, p.LastWidth, h);
            }
            // отрисовка границ между ячейками
            x = p.DrawRect.X + p.FirstWidth;
            y = p.DrawRect.Y + p.FirstHeight;
            // вертикальные линии
            while (x < p.DrawRect.Right)
            {
                gl.DrawLine(p.CellBorderPen.Color, p.CellBorderPen.Width, x, p.DrawRect.Top, x, p.DrawRect.Bottom);
                x += p.CellWidth;
            }
            // горизонтальные линии
            while (y < p.DrawRect.Bottom)
            {
                gl.DrawLine(p.CellBorderPen.Color, p.CellBorderPen.Width, p.DrawRect.Left, y, p.DrawRect.Right, y);
                y += p.CellHeight;
            }
            // отрисовка границ клонов систем
            x = p.DrawRect.X + p.FirstWidth + p.CellWidth * (Width - p.FirstCellX - 1);
            y = p.DrawRect.Y + p.FirstHeight + p.CellHeight * (Height - p.FirstCellY - 1);
            // приведение к области отображения
            while (x < p.DrawRect.X) x += Width * p.CellWidth;
            while (y < p.DrawRect.Y) y += Height * p.CellHeight;
            // вертикальные линии
            while (x < p.DrawRect.Right)
            {
                gl.DrawLine(p.BorderPen.Color, p.BorderPen.Width, x, p.DrawRect.Top, x, p.DrawRect.Bottom);
                x += Width * p.CellWidth;
            }
            // горизонтальные линии
            while (y < p.DrawRect.Bottom)
            {
                gl.DrawLine(p.BorderPen.Color, p.BorderPen.Width, p.DrawRect.Left, y, p.DrawRect.Right, y);
                y += Height * p.CellHeight;
            }
            // статистика
            if (p.IsStatisticShow)
            {
                //gl.DrawStatistic(_stat, p.CellBrushes, p.FontBrush, p.StatRect);
            }
            float fH = p.TimingHeight / 4;
            using (var f = new Font(Extension.Font, fH, GraphicsUnit.Pixel))
            {
                //gl.DrawString($"T = {Time}\nFPS = {p.FPS}\nSPF = {p.SPF}", f, p.FontBrush, p.DrawRect.X, p.DrawRect.Y);
            }
        }
    }
}
