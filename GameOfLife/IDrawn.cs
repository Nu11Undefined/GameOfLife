using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GameOfLife
{
    /// <summary>
    /// Определяет возможность отрисовки
    /// </summary>
    public interface IDrawn
    {
        /// <summary>
        /// Отобразить
        /// </summary>
        /// <param name="g">Поверхность рисования</param>
        /// <param name="param">Параметры отображения</param>
        void Draw(Graphics g, GraphicsParam param);

        /// <summary>
        /// Отобразить
        /// </summary>
        /// <param name="gl">Контекст OpenGL</param>
        /// <param name="param">Параметры отображения</param>
        void Draw(SharpGL.OpenGL gl, GraphicsParam param);
    }
}
