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
    public class GDIRenderer: RenderWindow
    {
        BufferedGraphicsContext ctx;
        BufferedGraphics buf;
        Graphics g;
        public GDIRenderer(int w, int h):base(w,h)
        {
            ctx = BufferedGraphicsManager.Current;
            g = CreateGraphics();
            UpdateGraphics();
        }
        public override Control Controller { get => this; }
        /// <summary>
        /// Обновить отображение
        /// </summary>
        protected override void UpdateGraphics()
        {
            if (buf != null) buf.Dispose();
            buf = ctx.Allocate(g, ClientRectangle);
            buf.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            base.UpdateGraphics();
        }
        public override void DrawSystem(IDrawn system, GraphicsParam param)
        {
            system.Draw(buf.Graphics, param);
            buf.Render();
        }
    }
}
