using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multimedia;
using System.Windows.Forms;

namespace GameOfLife
{
    public class Player
    {
        int Width { get; set; }
        int Height { get; set; }
        int FPS { get; set; }
        int SPF { get; set; } = 1; // сколько состояний системы меняется за один фрейм
        Multimedia.Timer timer;
        public RenderWindow Window { get; set; }
        const int FPS_MIN = 1;
        const int FPS_MAX = 60;
        const int STAGE_BY_FRAME_MIN = 1;
        const int STAGE_BY_FRAME_MAX = 10;
        public Player(int w, int h, int fps, RenderMode mode)
        {
            Width = w;
            Height = h;
            FPS = fps;
            timer = new Multimedia.Timer() { Period = 1000 / fps };
            switch(mode)
            {
                case RenderMode.GDI:
                    Window = new GDIRenderer(w, h);
                    break;
                case RenderMode.OpenGL:
                    Window = new GLRenderer(w, h);
                    break;
            }
        }
        /// <summary>
        /// Изменить режим окна на полноэкранный/стандартный
        /// </summary>
        public void ChangeWindowMode()
        {
            Window.IsFullScreen = !Window.IsFullScreen;
        }
        private void SetBounds(int dx, int dy, int dw, int dh)
        {
            Window.SetBoundsDelta(dx, dy, dw, dh);
        }
        public void PlayStop()
        {
            if (timer.IsRunning) timer.Stop();
            else timer.Start();
        }
        public void Exit()
        {
            timer.Dispose();
            Window.Dispose();
        }
        public Control Controller { get => Window.Controller; } 
        IAnimated _system;
        GraphicsParam _param;
        public void InitPlay(IAnimated system, GraphicsParam param)
        {
            _system = system;
            param.FPS = FPS; param.SPF = SPF;
            _param = param;
            timer.Tick += (s, e) => Window.BeginInvoke(new System.Windows.Forms.MethodInvoker(() =>
            {
                for (int i = 0; i < SPF; i++) system.NextStage();
                Refresh();
            }));
            timer.Start();
            Application.Run(Window);
            Window.Shown += (s,e)=>Refresh(); // первичное отображение
        }
        /// <summary>
        /// Обновить отображение
        /// </summary>
        public void Refresh()
        {
            Window.DrawSystem(_system, _param);
        }
        const int FPS_DELTA = 1;
        const int STAGE_BY_FRAME_DELTA = 1;
        /// <summary>
        /// Замедлить воспроизведение
        /// </summary>
        public void FpsUp() { FpsWorker(FPS_DELTA); }
        /// <summary>
        /// Ускорить воспроизведение
        /// </summary>
        public void FpsDown() { FpsWorker(-FPS_DELTA); }
        private void FpsWorker(int delta)
        {
            FPS += delta;
            if (FPS < FPS_MIN || FPS > FPS_MAX) FPS -= delta;
            else
            {
                timer.Period = 1000 / FPS;
                FpsChanged?.Invoke(FPS);
            }
        }
        /// <summary>
        /// Увеличить количество состояний системы, сменяемых за один фрейм
        /// </summary>
        public void SPFUp() { SPFWorker(STAGE_BY_FRAME_DELTA); }
        /// <summary>
        /// Уменьшить количество состояний системы, сменяемых за один фрейм
        /// </summary>
        public void SPFDown() { SPFWorker(-STAGE_BY_FRAME_DELTA); }
        private void SPFWorker(int delta)
        {
            SPF += delta;
            if (SPF < STAGE_BY_FRAME_MIN || SPF > STAGE_BY_FRAME_MAX) SPF -= delta;
            else SPFChanged?.Invoke(SPF);
        }
        /// <summary>
        /// Обработчик изменения параметра
        /// </summary>
        /// <param name="param">Значение параметра</param>
        public delegate void ParameterChangeHandler(int param);
        public event ParameterChangeHandler FpsChanged;
        public event ParameterChangeHandler SPFChanged;
    }
    public enum RenderMode:int
    {
        GDI,
        OpenGL
    }
}
