using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DrawingApplication
{
    // Интерфейс Рисунок
    public interface IDrawing
    {
        void DrawLine(Color color, int thickness, Point startPoint, Point endPoint);
        void DrawCircle(Color color, int thickness, Point center, int radius);
        void DrawRectangle(Color color, int thickness, Point topLeft, int width, int height);
        void DrawEllipse(Color color, int thickness, Point topLeft, int width, int height);
        void ClearCanvas();
        void SetFillColor(Color color);
        void SetFillMode(bool fill);
        Bitmap GetCanvasImage();
    }

    // Класс для работы с холстом
    public class CanvasDrawing : IDrawing
    {
        private Bitmap canvas;
        private Graphics graphics;
        private Color fillColor;
        private bool fillShapes;
        private List<Shape> shapes;

        public CanvasDrawing(int width, int height)
        {
            canvas = new Bitmap(width, height);
            graphics = Graphics.FromImage(canvas);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.Clear(Color.White);
            fillColor = Color.Transparent;
            fillShapes = false;
            shapes = new List<Shape>();
        }

        public void DrawLine(Color color, int thickness, Point startPoint, Point endPoint)
        {
            using (Pen pen = new Pen(color, thickness))
            {
                graphics.DrawLine(pen, startPoint, endPoint);
            }
            shapes.Add(new LineShape(color, thickness, startPoint, endPoint));
        }

        public void DrawCircle(Color color, int thickness, Point center, int radius)
        {
            Rectangle rect = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);

            if (fillShapes && fillColor != Color.Transparent)
            {
                using (Brush brush = new SolidBrush(fillColor))
                {
                    graphics.FillEllipse(brush, rect);
                }
            }

            using (Pen pen = new Pen(color, thickness))
            {
                graphics.DrawEllipse(pen, rect);
            }
            shapes.Add(new CircleShape(color, thickness, center, radius, fillColor, fillShapes));
        }

        public void DrawRectangle(Color color, int thickness, Point topLeft, int width, int height)
        {
            Rectangle rect = new Rectangle(topLeft.X, topLeft.Y, width, height);

            if (fillShapes && fillColor != Color.Transparent)
            {
                using (Brush brush = new SolidBrush(fillColor))
                {
                    graphics.FillRectangle(brush, rect);
                }
            }

            using (Pen pen = new Pen(color, thickness))
            {
                graphics.DrawRectangle(pen, rect);
            }
            shapes.Add(new RectangleShape(color, thickness, topLeft, width, height, fillColor, fillShapes));
        }

        public void DrawEllipse(Color color, int thickness, Point topLeft, int width, int height)
        {
            Rectangle rect = new Rectangle(topLeft.X, topLeft.Y, width, height);

            if (fillShapes && fillColor != Color.Transparent)
            {
                using (Brush brush = new SolidBrush(fillColor))
                {
                    graphics.FillEllipse(brush, rect);
                }
            }

            using (Pen pen = new Pen(color, thickness))
            {
                graphics.DrawEllipse(pen, rect);
            }
            shapes.Add(new EllipseShape(color, thickness, topLeft, width, height, fillColor, fillShapes));
        }

        public void ClearCanvas()
        {
            graphics.Clear(Color.White);
            shapes.Clear();
        }

        public void SetFillColor(Color color)
        {
            fillColor = color;
        }

        public void SetFillMode(bool fill)
        {
            fillShapes = fill;
        }

        public Bitmap GetCanvasImage()
        {
            return canvas;
        }

        public void Redraw()
        {
            graphics.Clear(Color.White);
            foreach (var shape in shapes)
            {
                shape.Draw(graphics);
            }
        }
    }

    // Базовый класс для фигур
    public abstract class Shape
    {
        public Color Color { get; set; }
        public int Thickness { get; set; }
        public Color FillColor { get; set; }
        public bool Filled { get; set; }

        public abstract void Draw(Graphics graphics);
    }

    // Класс для линии
    public class LineShape : Shape
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }

        public LineShape(Color color, int thickness, Point startPoint, Point endPoint)
        {
            Color = color;
            Thickness = thickness;
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public override void Draw(Graphics graphics)
        {
            using (Pen pen = new Pen(Color, Thickness))
            {
                graphics.DrawLine(pen, StartPoint, EndPoint);
            }
        }
    }

    // Класс для круга
    public class CircleShape : Shape
    {
        public Point Center { get; set; }
        public int Radius { get; set; }

        public CircleShape(Color color, int thickness, Point center, int radius, Color fillColor, bool filled)
        {
            Color = color;
            Thickness = thickness;
            Center = center;
            Radius = radius;
            FillColor = fillColor;
            Filled = filled;
        }

        public override void Draw(Graphics graphics)
        {
            Rectangle rect = new Rectangle(Center.X - Radius, Center.Y - Radius, Radius * 2, Radius * 2);

            if (Filled && FillColor != Color.Transparent)
            {
                using (Brush brush = new SolidBrush(FillColor))
                {
                    graphics.FillEllipse(brush, rect);
                }
            }

            using (Pen pen = new Pen(Color, Thickness))
            {
                graphics.DrawEllipse(pen, rect);
            }
        }
    }

    // Класс для прямоугольника
    public class RectangleShape : Shape
    {
        public Point TopLeft { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public RectangleShape(Color color, int thickness, Point topLeft, int width, int height, Color fillColor, bool filled)
        {
            Color = color;
            Thickness = thickness;
            TopLeft = topLeft;
            Width = width;
            Height = height;
            FillColor = fillColor;
            Filled = filled;
        }

        public override void Draw(Graphics graphics)
        {
            Rectangle rect = new Rectangle(TopLeft.X, TopLeft.Y, Width, Height);

            if (Filled && FillColor != Color.Transparent)
            {
                using (Brush brush = new SolidBrush(FillColor))
                {
                    graphics.FillRectangle(brush, rect);
                }
            }

            using (Pen pen = new Pen(Color, Thickness))
            {
                graphics.DrawRectangle(pen, rect);
            }
        }
    }

    // Класс для эллипса
    public class EllipseShape : Shape
    {
        public Point TopLeft { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public EllipseShape(Color color, int thickness, Point topLeft, int width, int height, Color fillColor, bool filled)
        {
            Color = color;
            Thickness = thickness;
            TopLeft = topLeft;
            Width = width;
            Height = height;
            FillColor = fillColor;
            Filled = filled;
        }

        public override void Draw(Graphics graphics)
        {
            Rectangle rect = new Rectangle(TopLeft.X, TopLeft.Y, Width, Height);

            if (Filled && FillColor != Color.Transparent)
            {
                using (Brush brush = new SolidBrush(FillColor))
                {
                    graphics.FillEllipse(brush, rect);
                }
            }

            using (Pen pen = new Pen(Color, Thickness))
            {
                graphics.DrawEllipse(pen, rect);
            }
        }
    }

    // Основная форма приложения
    public partial class Form1 : Form
    {
        private IDrawing drawing;
        private Bitmap canvasBitmap;
        private Graphics canvasGraphics;
        private Point startPoint;
        private Point endPoint;
        private bool isDrawing;
        private string currentTool;
        private Color currentColor;
        private int currentThickness;
        private Color fillColor;
        private bool fillShapes;

        public Form1()
        {
            InitializeComponent();
            InitializeDrawing();
            InitializeTools();
            InitializeColors();
        }

        private void InitializeDrawing()
        {
            int canvasWidth = picCanvas.Width;
            int canvasHeight = picCanvas.Height;

            drawing = new CanvasDrawing(canvasWidth, canvasHeight);
            canvasBitmap = new Bitmap(canvasWidth, canvasHeight);
            canvasGraphics = Graphics.FromImage(canvasBitmap);
            canvasGraphics.Clear(Color.White);

            currentColor = Color.Black;
            currentThickness = 2;
            fillColor = Color.Blue;
            fillShapes = false;

            UpdateCanvas();
        }

        private void InitializeTools()
        {
            cmbTool.Items.Add("Линия");
            cmbTool.Items.Add("Прямоугольник");
            cmbTool.Items.Add("Круг");
            cmbTool.Items.Add("Эллипс");
            cmbTool.Items.Add("Ластик");
            cmbTool.SelectedIndex = 0;
            currentTool = "Линия";

            numThickness.Value = currentThickness;
        }

        private void InitializeColors()
        {
            // Создаем палитру цветов
            Color[] colors = {
                Color.Black, Color.White, Color.Red,Color.Green, Color.Blue,
                Color.Yellow, Color.Magenta, Color.Cyan, Color.Orange, Color.Purple,
                Color.Brown, Color.Pink, Color.Gray, Color.LightBlue, Color.LightGreen
            };

            int buttonSize = 25;
            int margin = 5;
            int x = margin;
            int y = btnColor.Location.Y + btnColor.Height + margin;

            foreach (Color color in colors)
            {
                Button colorBtn = new Button();
                colorBtn.Size = new Size(buttonSize, buttonSize);
                colorBtn.Location = new Point(x, y);
                colorBtn.BackColor = color;
                colorBtn.FlatStyle = FlatStyle.Flat;
                colorBtn.FlatAppearance.BorderSize = 1;
                colorBtn.Click += (s, e) =>
                {
                    currentColor = color;
                    UpdateColorPreview();
                };
                this.Controls.Add(colorBtn);

                x += buttonSize + margin;
                if (x + buttonSize > this.Width - margin)
                {
                    x = margin;
                    y += buttonSize + margin;
                }
            }

            UpdateColorPreview();
        }

        private void UpdateColorPreview()
        {
            btnColor.BackColor = currentColor;
            btnFillColor.BackColor = fillColor;
        }

        private void UpdateCanvas()
        {
            picCanvas.Image = drawing.GetCanvasImage();
            picCanvas.Refresh();
        }

        private void cmbTool_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentTool = cmbTool.SelectedItem.ToString();
            btnFill.Enabled = currentTool != "Линия" && currentTool != "Ластик";
        }

        private void numThickness_ValueChanged(object sender, EventArgs e)
        {
            currentThickness = (int)numThickness.Value;
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = currentColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                currentColor = colorDialog.Color;
                UpdateColorPreview();
            }
        }

        private void btnFillColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = fillColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                fillColor = colorDialog.Color;
                drawing.SetFillColor(fillColor);
                UpdateColorPreview();
            }
        }

        private void btnFill_Click(object sender, EventArgs e)
        {
            fillShapes = !fillShapes;
            drawing.SetFillMode(fillShapes);
            btnFill.BackColor = fillShapes ? SystemColors.ControlDark : SystemColors.Control;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            drawing.ClearCanvas();
            UpdateCanvas();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|BMP Image|*.bmp";
            saveDialog.Title = "Сохранить рисунок";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    drawing.GetCanvasImage().Save(saveDialog.FileName);
                    MessageBox.Show("Рисунок успешно сохранен!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void picCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = true;
                startPoint = e.Location;
                endPoint = e.Location;
            }
        }

        private void picCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                endPoint = e.Location;

                // Предварительный просмотр рисования
                if (currentTool != "Ластик")
                {
                    Bitmap tempBitmap = new Bitmap(drawing.GetCanvasImage());
                    using (Graphics tempGraphics = Graphics.FromImage(tempBitmap))
                    {
                        tempGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                        DrawPreview(tempGraphics);
                    }
                    picCanvas.Image = tempBitmap;
                }
            }
        }

        private void picCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrawing && e.Button == MouseButtons.Left)
            {
                isDrawing = false;
                endPoint = e.Location;

                if (currentTool == "Ластик")
                {
                    // Ластик - рисуем белым цветом
                    drawing.DrawRectangle(Color.White, currentThickness * 5,
                        new Point(e.X - currentThickness * 2, e.Y - currentThickness * 2),
                        currentThickness * 4, currentThickness * 4);
                }
                else
                {
                    DrawShape();
                }

                UpdateCanvas();
            }
        }

        private void DrawPreview(Graphics graphics)
        {
            using (Pen pen = new Pen(currentColor, currentThickness))
            {
                pen.DashStyle = DashStyle.Dash;

                switch (currentTool)
                {
                    case "Линия":
                        graphics.DrawLine(pen, startPoint, endPoint);
                        break;
                    case "Прямоугольник":
                        int width = endPoint.X - startPoint.X;
                        int height = endPoint.Y - startPoint.Y;
                        graphics.DrawRectangle(pen, startPoint.X, startPoint.Y, width, height);
                        break;
                    case "Круг":
                        int radius = (int)Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) + Math.Pow(endPoint.Y - startPoint.Y, 2));
                        graphics.DrawEllipse(pen, startPoint.X - radius, startPoint.Y - radius, radius * 2, radius * 2);
                        break;
                    case "Эллипс":
                        int ellWidth = endPoint.X - startPoint.X;
                        int ellHeight = endPoint.Y - startPoint.Y;
                        graphics.DrawEllipse(pen, startPoint.X, startPoint.Y, ellWidth, ellHeight);
                        break;
                }
            }
        }

        private void DrawShape()
        {
            switch (currentTool)
            {
                case "Линия":
                    drawing.DrawLine(currentColor, currentThickness, startPoint, endPoint);
                    break;
                case "Прямоугольник":
                    int width = endPoint.X - startPoint.X;
                    int height = endPoint.Y - startPoint.Y;
                    drawing.DrawRectangle(currentColor, currentThickness, startPoint, width, height);
                    break;
                case "Круг":
                    int radius = (int)Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) + Math.Pow(endPoint.Y - startPoint.Y, 2));
                    drawing.DrawCircle(currentColor, currentThickness, startPoint, radius);
                    break;
                case "Эллипс":
                    int ellWidth = endPoint.X - startPoint.X;
                    int ellHeight = endPoint.Y - startPoint.Y;
                    drawing.DrawEllipse(currentColor, currentThickness, startPoint, ellWidth, ellHeight);
                    break;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (picCanvas != null)
            {
                picCanvas.Width = this.ClientSize.Width - picCanvas.Left - 20;
                picCanvas.Height = this.ClientSize.Height - picCanvas.Top - 20;

                if (drawing is CanvasDrawing canvasDrawing)
                {
                    // Пересоздаем холст при изменении размера
                    InitializeDrawing();
                }
            }
        }
    }
}