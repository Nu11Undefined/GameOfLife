using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GameOfLife
{
    /// <summary>
    /// Параметры отображения
    /// </summary>
    public class GraphicsParam
    {
        /// <summary>
        /// Координата X начальной точки
        /// </summary>
        float X { get; set; }
        /// <summary>
        /// Координата Y начальной точки
        /// </summary>
        float Y { get; set; }
        /// <summary>
        /// Отображаемая ширина ячейки
        /// </summary>
        public float CellWidth { get; set; }
        /// <summary>
        /// Отображаемая высота ячейки
        /// </summary>
        public float CellHeight { get; set; }
        /// <summary>
        /// Ширина сетки
        /// </summary>
        public float GridWidth { get; set; } = 1;
        /// <summary>
        /// Прямоугольник отображения статистики
        /// </summary>
        static RectangleF _statK = new RectangleF(0.00F, 0.08F, 0.5F, 0.3F);
        /// <summary>
        /// Прямоугольник окна
        /// </summary>
        public SizeF ClientSize { get; set; }
        /// <summary>
        /// Прямоугольник отображения
        /// </summary>
        public RectangleF DrawRect { get; set; }
        public RectangleF StatRect => Extension.Mult(DrawRect, _statK);
        /// <summary>
        /// Параметр отображения времени
        /// </summary>
        public float TimingHeight => _statK.Y * DrawRect.Height;
        /// <summary>
        /// Обновить линейные размеры
        /// </summary>
        public void SetRect(float w, float h, float margin)
        {
            DrawRect = new RectangleF(margin, margin, w - 2 * margin, h - 2 * margin);
            ClientSize = new SizeF(w, h);
        }
        /// <summary>
        /// Показывать ли статистику
        /// </summary>
        public bool IsStatisticShow { get; set; }
        /// <summary>
        /// Зум
        /// </summary>
        public float Zoom { get; set; } = 1;
        float _zoomK { get; set; } = 1.1F; // коэффициент зумирования
        /// <summary>
        /// Текущее количество кадров в секунду
        /// </summary>
        public int FPS { get; set; }
        /// <summary>
        /// Текущее количество этапов на фрейм
        /// </summary>
        public int SPF { get; set; }
        /// <summary>
        /// Кисти для ячеек
        /// </summary>
        public SolidBrush[] CellBrushes { get; set; }
        /// <summary>
        /// Заливка для сетки
        /// </summary>
        public Brush GridBrush { get; set; }
        public Brush FontBrush { get; set; }
        /// <summary>
        /// Кисть между клонами миров
        /// </summary>
        public Pen BorderPen { get; set; }
        /// <summary>
        /// Кисть между ячейками
        /// </summary>
        public Pen CellBorderPen { get; set; }
        /// <summary>
        /// Количество возрастных групп
        /// </summary>
        public int AgeCount
        {
            get=>CellBrushes.Length-1;
            set
            {
                if (value != AgeCount)
                {
                    InitBrushes(value, (CellBrushes[0] as SolidBrush).Color, (CellBrushes[1] as SolidBrush).Color, (CellBrushes[AgeCount] as SolidBrush).Color);
                }
            }
        }
        /// <param name="w">Ширина ячейки</param>
        /// <param name="h">Высота ячейки</param>
        /// <param name="drawRect">Прямоугольник отображения модели</param>
        /// <param name="ageCount">Количество возрастных групп</param>
        /// <param name="newCell">Цвет новых ячеек</param>
        /// <param name="oldCell">Начальный цвет постаревших ячеек</param>
        /// <param name="emptyCell">Цвет мертвых ячеек - цвет пустого пространства</param>
        /// <param name="borderColor">Цвет границы</param>
        public GraphicsParam(float w, float h, RectangleF drawRect, int ageCount, Color newCell, Color oldCell, Color emptyCell, Color borderColor, Color cellBorderColor, Color fontColor)
        {
            CellWidth = w;
            CellHeight = h;
            DrawRect = drawRect;
            BorderPen = new Pen(borderColor, 2);
            CellBorderPen = new Pen(cellBorderColor);
            FontBrush = new SolidBrush(fontColor);
            InitBrushes(ageCount, newCell, oldCell, emptyCell);
        }
        /// <summary>
        /// Создать заливки
        /// </summary>
        private void InitBrushes(int ageCount, Color newCell, Color oldCell, Color emptyCell)
        {
            CellBrushes = new SolidBrush[ageCount+1];
            CellBrushes[0] = new SolidBrush(newCell);
            for (int i  = 0;i<ageCount;i++)
            {
                CellBrushes[i + 1] = new SolidBrush(Extension.Interpolate(oldCell, emptyCell, (double)i / (ageCount-1)));
            }
        }
        /// <summary>
        /// Начальная ячейка для отрисовки по горизонтали
        /// </summary>
        public int FirstCellX { get; set; }
        /// <summary>
        /// Начальная ячейка для отрисовки по вертикали
        /// </summary>
        public int FirstCellY { get; set; }
        /// <summary>
        /// Количество полностью отрисовываемых ячеек по ширине
        /// </summary>
        public int CellCountW { get; set; }
        /// <summary>
        /// Количество полностью отрисовываемых ячеек по высоте
        /// </summary>
        public int CellCountH { get; set; }
        /// <summary>
        /// Ширина первой отрисовываемой ячейки
        /// </summary>
        public float FirstWidth { get; set; }
        /// <summary>
        /// Высота первой отрисовываемой ячейки
        /// </summary>
        public float FirstHeight { get; set; }
        /// <summary>
        /// Ширина конечной отрисовываемой ячейки
        /// </summary>
        public float LastWidth { get; set; }
        /// <summary>
        /// Высота конечной отрисовываемой ячейки
        /// </summary>
        public float LastHeight { get; set; }
        /// <summary>
        /// Приблизить отображение в данной точке
        /// </summary>
        public void ZoomIn(float x, float y)
        {
            ZoomWorker(x,y,_zoomK);
        }
        /// <summary>
        /// Отдалить отображение в данной точке
        /// </summary>
        public void ZoomOut(float x, float y)
        {
            ZoomWorker(x,y,1 / _zoomK);
        }
        private void ZoomWorker(float x, float y, float k)
        {
            // текущие расстояния до краев отображения
            float distToLeft = x - X, distToTop = y - Y;
            // обновить крайние точки
            X = x - k * distToLeft;
            Y = y - k * distToTop;
            // обновить размеры ячейки
            CellWidth *= k;
            CellHeight *= k;
        }
        /// <summary>
        /// Сместить обзор попиксельно
        /// </summary>
        public void ShiftByPixel(int dx, int dy)
        {
            ShiftWorker(dx, dy);
        }
        /// <summary>
        /// Сместить обзор по-клеточно
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public void ShiftByCell(int dx, int dy)
        {
            ShiftWorker(dx * CellWidth, dy * CellHeight);
        }
        /// <summary>
        /// Сместить начальную точку
        /// </summary>
        /// <param name="x">Смещение по X</param>
        /// <param name="y">Смещенеи по Y</param>
        private void ShiftWorker(float x, float y)
        {
            X += x;
            Y += y;
        }
        /// <summary>
        /// Малая величина для нивелировки погрешности вычислений в float
        /// </summary>
        const float EPS = 0.00001F;
        /// <summary>
        /// Скорректировать параметры перед отрисовкой
        /// </summary>
        /// <param name="model">Модель для отрисовки</param>
        public void Validate(Life model)
        {
            float 
                W = CellWidth * model.Width, // ширина отображения всей системы
                H = CellHeight * model.Height; // высота отображения всей системы
            // приведение начальных точек отображения системы в интервал (rect.left-size.w;rect.left]
            // смещение вправо-вниз
            while (X < DrawRect.Left - W - EPS) X += W;
            while (Y < DrawRect.Top - H - EPS) Y += H;
            // смещение влево-вверх
            while (X > DrawRect.Left) X -= W;
            while (Y > DrawRect.Top) Y -= H;
            FirstCellX = (int)Math.Floor((DrawRect.Left - X) / CellWidth);
            FirstCellY = (int)Math.Floor((DrawRect.Top - Y) / CellHeight);
            //Console.WriteLine($"First = ({FirstCellX}, {FirstCellY})");
            FirstWidth = X + CellWidth * (FirstCellX + 1) - DrawRect.Left;
            FirstHeight = Y + CellHeight * (FirstCellY + 1) - DrawRect.Top;
            CellCountW = (int)Math.Floor((DrawRect.Width - FirstWidth) / CellWidth);
            CellCountH = (int)Math.Floor((DrawRect.Height - FirstHeight) / CellHeight);
            LastWidth = DrawRect.Width - FirstWidth - CellCountW * CellWidth;
            LastHeight = DrawRect.Height - FirstHeight - CellCountH * CellHeight;
        }
        public Point ScreenToCellLocation(int x, int y, Life model)
        {
            return new Point(
                (FirstCellX + 1 + (int)Math.Floor((x - DrawRect.X - FirstWidth) / CellWidth))%model.Width,
                (FirstCellY + 1 + (int)Math.Floor((y - DrawRect.Y - FirstHeight) / CellHeight))%model.Height);
        }
    }
}
