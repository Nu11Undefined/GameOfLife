using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public abstract class RenderWindow : Form
    {
        protected static Size _fullSize;
        /// <summary>
        /// Находится ли окно в режиме полноэкранного отображения
        /// </summary>
        public bool IsFullScreen
        {
            get => Size == _fullSize;
            set
            {
                // если происходит смена режима
                if (value!=IsFullScreen)
                {
                    // полноэкранный режим
                    if (value)
                    {
                        SetBoundsDelta(-Location.X, -Location.Y, _fullSize.Width - Width, _fullSize.Height - Height);
                    }
                    // стандартный режим - половина экрана по середине
                    else
                    {
                        SetBoundsDelta(_fullSize.Width / 4, _fullSize.Height / 4, -_fullSize.Width / 2, -_fullSize.Height / 2);
                    }
                }
            }
        }
        public virtual Control Controller { get; set; }
        public RenderWindow(int w, int h)
        {
            _fullSize = Screen.PrimaryScreen.Bounds.Size;
            Width = w;
            Height = h;
            FormBorderStyle = FormBorderStyle.None;
        }
        public event Controller.LocationEventHandler GraphicsSizeChanged;
        /// <summary>
        /// Обновить отображение
        /// </summary>
        protected virtual void UpdateGraphics()
        {
            GraphicsSizeChanged?.Invoke(ClientRectangle.Width, ClientRectangle.Height);
        }
        /// <summary>
        /// Установить границы отображения по изменению параметров
        /// </summary>
        public void SetBoundsDelta(int dx, int dy, int dw, int dh)
        {
            SetBoundsCore(Location.X + dx, Location.Y + dy, Width + dw, Height + dh, BoundsSpecified.All);
            if (dw!=0 && dh!=0)
            {
                UpdateGraphics();
            }
        }
        public virtual void DrawSystem(IDrawn system, GraphicsParam param)
        {
        }
    }
}
