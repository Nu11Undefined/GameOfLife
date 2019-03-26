using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using SharpGL.Enumerations;
using SharpGL;

namespace GameOfLife
{
    public class GLRenderer:RenderWindow
    {
        OpenGLControl glControl;
        OpenGL gl;
        public override Control Controller { get => glControl; }
        public GLRenderer(int w,int h):base(w,h)
        {
            glControl = new OpenGLControl();
            ((ISupportInitialize)glControl).BeginInit();
            glControl.Dock = DockStyle.Fill;
            Controls.Add(glControl);
            ((ISupportInitialize)glControl).EndInit();
            gl = glControl.OpenGL;

            Shown += (s,e)=>UpdateGraphics();
        }
        /// <summary>
        /// Обновить отображение
        /// </summary>
        protected override void UpdateGraphics()
        {
            base.UpdateGraphics();
        }
        public override void DrawSystem(IDrawn system, GraphicsParam param)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            system.Draw(gl, param);
            gl.Flush();
        }
    }
}
