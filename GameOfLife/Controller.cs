using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace GameOfLife
{
    /// <summary>
    /// Система ввода информации
    /// </summary>
    public class Controller
    {
        bool _isLeftMouseDown = false; // для заселения модели
        bool _isRightMouseDown = false; // для смещения обзора
        bool _isMiddleMouseDown = false; // для перемещения окна отображения
        Point MouseDownPoint { get; set; }
        Point CurrentMousePosition { get; set; }
        /// <param name="win">Окно управления</param>
        public Controller(Control win)
        {
            Form main = win.FindForm();
            win.MouseDown += (s, e) =>
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        _isLeftMouseDown = true;
                        InvertCellAtLocation?.Invoke(e.X, e.Y);
                        break;
                    case MouseButtons.Right:
                        _isRightMouseDown = true;
                        CurrentMousePosition = new Point(e.X, e.Y);
                        break;
                    case MouseButtons.Middle:
                        _isMiddleMouseDown = true;
                        CurrentMousePosition = new Point(e.X, e.Y);
                        break;
                }
            };
            win.MouseUp += (s, e) =>
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        _isLeftMouseDown = false;
                        break;
                    case MouseButtons.Right:
                        _isRightMouseDown = false;
                        break;
                    case MouseButtons.Middle:
                        _isMiddleMouseDown = false;
                        break;
                }
            };
            win.MouseWheel += (s, e) =>
            {
                if (e.Delta > 0) ZoomIn?.Invoke(e.X, e.Y);
                else ZoomOut?.Invoke(e.X, e.Y);
            };
            win.MouseMove += (s, e) =>
            {
                CursorPositionChanged?.Invoke(e.X, e.Y);
                // определить обработку только одной кнопки мыши
                if (_isLeftMouseDown)
                {
                    ReviveCellAtLocation?.Invoke(e.X, e.Y);
                }
                else if (_isRightMouseDown)
                {
                    ShiftByPixel?.Invoke(e.X - CurrentMousePosition.X, e.Y - CurrentMousePosition.Y);
                    CurrentMousePosition = new Point(e.X, e.Y);
                }
                else if (_isMiddleMouseDown)
                {
                    main.Location = new Point(main.Location.X + e.X - CurrentMousePosition.X, main.Location.Y + e.Y - CurrentMousePosition.Y);
                }
            };
            win.KeyUp += (s, e) =>
            {
                int
                    dxC = 0, dyC = 0, // смещение по ячейкам
                    dxP = 0, dyP = 0, // смещение по пикселям
                    zoom = 0;
                Console.WriteLine(e.KeyCode);
                switch ((HotKeys)e.KeyCode)
                {
                    case HotKeys.Random:
                        SetRandom?.Invoke(0.5);
                        break;
                    case HotKeys.Clear:
                        Clear?.Invoke();
                        break;
                    case HotKeys.StopPlay:
                        StopPlay?.Invoke();
                        break;
                    case HotKeys.Statistic:
                        StatisticModeChange?.Invoke();
                        break;
                    case HotKeys.ChangeWindowMode:
                        WindowModeChanged?.Invoke();
                        break;
                    case HotKeys.FpsUp:
                    case HotKeys.FpsUp2:
                        FpsUp?.Invoke();
                        break;
                    case HotKeys.FpsDown:
                    case HotKeys.FpsDown2:
                        FpsDown?.Invoke();
                        break;
                    case HotKeys.SpfUp:
                        StagesByFrameUp?.Invoke();
                        break;
                    case HotKeys.SpfDown:
                        StagesByFrameDown?.Invoke();
                        break;
                    case HotKeys.Exit:
                        Exit?.Invoke();
                        break;

                        // перемещение по ячейкам
                    case HotKeys.CellDown:
                        dyC += 1;
                        break;
                    case HotKeys.CellUp:
                        dyC -= 1;
                        break;
                    case HotKeys.CellLeft:
                        dxC -= 1;
                        break;
                    case HotKeys.CellRight:
                        dxC += 1;
                        break;

                    // перемещение по пикселям
                    case HotKeys.PixelDown:
                        dyP += 1;
                        break;
                    case HotKeys.PixelUp:
                        dyP -= 1;
                        break;
                    case HotKeys.PixelLeft:
                        dxP -= 1;
                        break;
                    case HotKeys.PixelRight:
                        dxP += 1;
                        break;
                    case HotKeys.ZoomOut:
                        zoom -= 1;
                        break;
                    case HotKeys.ZoomIn:
                        zoom += 1;
                        break;
                }
                // если смещенеи ненулевое, вызвать его
                if (dxC != 0 || dyC != 0) ShiftByCell?.Invoke(dxC, dyC);
                if (dxP != 0 || dyP != 0) ShiftByPixel?.Invoke(dxP, dyP);
                // -1 - идентификатор зумирования по центру
                if (zoom == 1) ZoomIn?.Invoke(-1, -1);
                else if (zoom == -1) ZoomOut?.Invoke(-1, -1);
            };
            win.SizeChanged += (s, e) =>
            {
                SizeChanged?.Invoke(win.ClientRectangle.Width, win.ClientRectangle.Height);
            };
        }
        enum HotKeys: int
        {
            Random = Keys.R,
            Clear = Keys.Delete,
            CellUp = Keys.Up,
            PixelUp = Keys.W,
            CellLeft = Keys.Left,
            PixelLeft = Keys.A,
            CellDown = Keys.Down, 
            PixelDown = Keys.S,
            CellRight = Keys.Right,
            PixelRight = Keys.D,
            ZoomIn = Keys.NumPad8,
            ZoomOut = Keys.NumPad2,
            StopPlay = Keys.Space,
            Statistic = Keys.Tab,
            ChangeWindowMode = Keys.F11,
            Exit = Keys.Escape,
            FpsUp = Keys.NumPad7,
            FpsUp2 = Keys.Home,
            FpsDown = Keys.NumPad1,
            FpsDown2 = Keys.End,
            SpfUp = Keys.NumPad9,
            SpfDown = Keys.NumPad3,
        }

        #region ModelEvents
        /// <summary>
        /// Обработчик добавления фигуры
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="figure"></param>
        public delegate void AddEventHandler(int x, int y, bool[,] figure);
        /// <summary>
        /// Добавление фигуры
        /// </summary>
        public event AddEventHandler AddFigure;

        /// <summary>
        /// Общие события
        /// </summary>
        public delegate void LifeEventHandler();

        /// <summary>
        /// Очистка всех клеток
        /// </summary>
        public event LifeEventHandler Clear;
        /// <summary>
        /// Запустить/остановить воспроизведение
        /// </summary>
        public event LifeEventHandler StopPlay;
        /// <summary>
        /// Увеличить скорость отображения кадров
        /// </summary>
        public event LifeEventHandler FpsUp;
        /// <summary>
        /// Уменьшить скорость отображения кадров
        /// </summary>
        public event LifeEventHandler FpsDown;
        /// <summary>
        /// Увеличить количество этапов за один фрейм
        /// </summary>
        public event LifeEventHandler StagesByFrameUp;
        /// <summary>
        /// Уменьшить количество этапов за один фрейм
        /// </summary>
        public event LifeEventHandler StagesByFrameDown;
        /// <summary>
        /// Показать/скрыть статистику
        /// </summary>
        public event LifeEventHandler StatisticModeChange;
        /// <summary>
        /// Изменение оконного режима полноэкранный-стандартный
        /// </summary>
        public event LifeEventHandler WindowModeChanged;
        /// <summary>
        /// Выйти из программы
        /// </summary>
        public event LifeEventHandler Exit;

        /// <summary>
        /// Обработчик случайного заполнения
        /// </summary>
        /// <param name="k"></param>
        public delegate void SetRandomEventHandler(double k);

        /// <summary>
        /// Случайное заполнение
        /// </summary>
        public event SetRandomEventHandler SetRandom;
        #endregion

        #region VisualEvents

        /// <summary>
        /// Передача информации, связанной с координатами
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public delegate void LocationEventHandler(int x, int y);
        /// <summary>
        /// Приближение
        /// </summary>
        public event LocationEventHandler ZoomIn;
        /// <summary>
        /// Отдаление
        /// </summary>
        public event LocationEventHandler ZoomOut;
        /// <summary>
        /// Смещение по пикселям
        /// </summary>
        public event LocationEventHandler ShiftByPixel;
        /// <summary>
        /// Смещение по ячейкам
        /// </summary>
        public event LocationEventHandler ShiftByCell;
        /// <summary>
        /// Происходит при изменении положения прямоугольника отображения
        /// </summary>
        public event LocationEventHandler LocationChanged;
        /// <summary>
        /// Происходит при изменении размера прямоугольника отображения
        /// </summary>
        public event LocationEventHandler SizeChanged;
        /// <summary>
        /// Сообщает о необходимости инвертировать ячейку в текущей позиции курсора
        /// </summary>
        public event LocationEventHandler InvertCellAtLocation;
        /// <summary>
        /// Сообщает о необходимости оживить ячейку в текущей позиции курсора
        /// </summary>
        public event LocationEventHandler ReviveCellAtLocation;
        /// <summary>
        /// Сообщает о перемещении курсора
        /// </summary>
        public event LocationEventHandler CursorPositionChanged;
        #endregion

    }
}
