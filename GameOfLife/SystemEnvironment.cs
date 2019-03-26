using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GameOfLife
{
    class SystemEnvironment
    {
        Life Model { get; set; }
        Controller Con { get; set; }
        Player Visualizator { get; set; }

        GraphicsParam GraphParams { get; set; }
        static int width = 550, height = 800, wCount = 80, hCount = 45, w0 = 50, h0 = 50, ageCount = 16, fps = 2, margin = 10;
        public SystemEnvironment()
        {
            GraphParams = new GraphicsParam(
                w0, h0, new RectangleF(margin, margin, width - 2 * margin, height - 2 * margin),
                ageCount, Color.Purple, Color.DarkGray, Color.White, Color.Red, Color.Gray, Color.FromArgb(100, 100, 255));
            Model = new Life(wCount, hCount, (byte)ageCount);
            //Model.FillRandom(0.5);
            Visualizator = new Player(width, height, fps, RenderMode.
                GDI
                //OpenGL
                );
            Con = new Controller(Visualizator.Controller);
            InitObjectConnection();

            Visualizator.InitPlay(Model, GraphParams);
        }
        private void InitObjectConnection()
        {
            // из контроллера
            Con.ShiftByCell += (x, y) =>
            {
                GraphParams.ShiftByCell(x, y);
                Visualizator.Refresh();
            };
            Con.ShiftByPixel += (x, y) =>
            {
                GraphParams.ShiftByPixel(x, y);
                Visualizator.Refresh();
            };
            Con.ZoomIn += (x, y) =>
            {
                GraphParams.ZoomIn(x, y);
                Visualizator.Refresh();
            };
            Con.ZoomOut += (x, y) =>
            {
                GraphParams.ZoomOut(x, y);
                Visualizator.Refresh();
            };
            Con.InvertCellAtLocation += (x, y) =>
            {
                Model.InvertCell(GraphParams.ScreenToCellLocation(x, y, Model));
                Visualizator.Refresh();
            };
            Con.ReviveCellAtLocation += (x, y) =>
            {
                Model.ReviveCell(GraphParams.ScreenToCellLocation(x, y, Model));
                // отрисовка в Model.CellsChanged - когда будет изменение
                //Visualizator.Refresh(); 
            };
            Con.CursorPositionChanged += (x, y) =>
            {
                Console.WriteLine($"Point: {GraphParams.ScreenToCellLocation(x, y, Model)}");
            };
            Con.SetRandom += (k) =>
            {
                Model.FillRandom(k);
                Visualizator.Refresh();
            };
            Con.StatisticModeChange += () =>
            {
                GraphParams.IsStatisticShow = !GraphParams.IsStatisticShow;
                Visualizator.Refresh();
            };
            Con.StopPlay += () => Visualizator.PlayStop();
            Con.FpsUp += () => Visualizator.FpsUp();
            Con.FpsDown += () => Visualizator.FpsDown();
            Con.StagesByFrameUp += () => Visualizator.SPFUp();
            Con.StagesByFrameDown += () => Visualizator.SPFDown();
            Con.WindowModeChanged += () => Visualizator.ChangeWindowMode();
            Con.Exit += () => Visualizator.Exit();

            // из плеера
            Visualizator.Window.GraphicsSizeChanged += (w, h) =>
            {
                GraphParams.SetRect(w,h, margin);
                Visualizator.Refresh();
            };
            Visualizator.FpsChanged += (fps) =>
            {
                GraphParams.FPS = fps;
                Visualizator.Refresh();
            };
            Visualizator.SPFChanged += (spf) =>
            {
                GraphParams.SPF = spf;
                Visualizator.Refresh();
            };
            // из модели
            Model.CellsChanged += () => Visualizator.Refresh();
        }
    }
}
