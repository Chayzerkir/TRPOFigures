using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GraphicEditorXP
{
    public class Form1 : Form
    {
        // Состояние
        private enum Tool { Pencil, Line, Rectangle, Circle, Clear }
        private Tool currentTool = Tool.Pencil;
        private Color currentColor = Color.Black;
        private Point startPoint;
        private Point lastPoint;
        private bool isDrawing = false;
        private List<Shape> shapes = new List<Shape>();
        private Shape currentShape = null;
        private PictureBox canvas;
        private Panel controlPanel;
        private Bitmap canvasBitmap;

        // Элементы управления
        private ComboBox colorCombo;
        private Button btnSave;
        private Button btnPencil;
        private Button btnLine;
        private Button btnRectangle;
        private Button btnCircle;
        private Button btnClear;

        public Form1()
        {
            // Настройка окна
            this.Text = "XP Graphic Editor v1.1";
            this.Size = new Size(850, 650);
            this.StartPosition = FormStartPosition.CenterScreen;

            SetupUI();
            SetupCanvas();
        }

        private void SetupUI()
        {
            // Панель инструментов
            controlPanel = new Panel();
            controlPanel.Height = 50;
            controlPanel.Dock = DockStyle.Top;
            controlPanel.BackColor = Color.LightGray;
            this.Controls.Add(controlPanel);

            // Создаем кнопки инструментов
            btnPencil = CreateToolButton("Карандаш", 5);
            btnPencil.Click += (s, e) => currentTool = Tool.Pencil;

            btnLine = CreateToolButton("Линия", 110);
            btnLine.Click += (s, e) => currentTool = Tool.Line;

            btnRectangle = CreateToolButton("Прямоугольник", 210);
            btnRectangle.Click += (s, e) => currentTool = Tool.Rectangle;

            btnCircle = CreateToolButton("Круг", 310);
            btnCircle.Click += (s, e) => currentTool = Tool.Circle;

            btnClear = CreateToolButton("Очистить", 400);
            btnClear.Click += (s, e) => ClearCanvas();

            // Кнопка Сохранить
            btnSave = CreateToolButton("Сохранить", 490);
            btnSave.Click += BtnSave_Click;

            // Надпись для цвета
            var colorLabel = new Label();
            colorLabel.Text = "Цвет:";
            colorLabel.Location = new Point(550, 17);
            colorLabel.Size = new Size(40, 20);
            controlPanel.Controls.Add(colorLabel);

            // Выпадающий список цветов
            colorCombo = new ComboBox();
            colorCombo.Location = new Point(600, 15);
            colorCombo.Width = 100;
            colorCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            colorCombo.Items.AddRange(new object[] { "Черный", "Красный", "Зеленый", "Синий", "Желтый" });
            colorCombo.SelectedIndex = 0;
            colorCombo.SelectedIndexChanged += (s, e) =>
            {
                switch (colorCombo.Text)
                {
                    case "Красный":
                        currentColor = Color.Red;
                        break;
                    case "Зеленый":
                        currentColor = Color.Green;
                        break;
                    case "Синий":
                        currentColor = Color.Blue;
                        break;
                    case "Желтый":
                        currentColor = Color.Yellow;
                        break;
                    default:
                        currentColor = Color.Black;
                        break;
                }
            };
            controlPanel.Controls.Add(colorCombo);
        }

        private Button CreateToolButton(string text, int x)
        {
            var btn = new Button();
            btn.Text = text;
            btn.Location = new Point(x, 12);
            btn.Size = new Size(100, 30);
            controlPanel.Controls.Add(btn);
            return btn;
        }

        private void SetupCanvas()
        {
            canvas = new PictureBox();
            canvas.Dock = DockStyle.Fill;
            canvas.BackColor = Color.White;
            canvas.Cursor = Cursors.Cross;
            canvas.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(canvas);

            // Создаем Bitmap для рисования
            canvasBitmap = new Bitmap(canvas.Width, canvas.Height);
            ClearBitmap();
            canvas.Image = canvasBitmap;

            // Обработчики событий мыши
            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseUp += Canvas_MouseUp;
            canvas.Paint += Canvas_Paint;
        }

        // Метод сохранения
        private void BtnSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Title = "Сохранить рисунок";
                saveDialog.Filter = "PNG файлы|*.png|JPEG файлы|*.jpg|BMP файлы|*.bmp";
                saveDialog.DefaultExt = "png";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        canvasBitmap.Save(saveDialog.FileName);
                        MessageBox.Show($"Рисунок сохранен!\n{saveDialog.FileName}",
                            "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ClearCanvas()
        {
            shapes.Clear();
            ClearBitmap();
            canvas.Invalidate();
        }

        private void ClearBitmap()
        {
            using (var g = Graphics.FromImage(canvasBitmap))
            {
                g.Clear(Color.White);
            }
        }

        //Обработчики событий мыши
        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            isDrawing = true;
            startPoint = e.Location;
            lastPoint = e.Location;

            if (currentTool == Tool.Clear)
            {
                ClearCanvas();
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return;

            lastPoint = e.Location;

            if (currentTool == Tool.Pencil)
            {
                DrawOnBitmap();
                canvas.Invalidate();
            }
            else if (currentTool == Tool.Line || currentTool == Tool.Rectangle || currentTool == Tool.Circle)
            {
                currentShape = new Shape
                {
                    Type = currentTool == Tool.Line ? ShapeType.Line :
                           currentTool == Tool.Rectangle ? ShapeType.Rectangle : ShapeType.Circle,
                    Points = new[] { startPoint, lastPoint },
                    Color = currentColor,
                    IsPreview = true
                };
                canvas.Invalidate();
            }
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return;

            isDrawing = false;

            if (currentTool == Tool.Line || currentTool == Tool.Rectangle || currentTool == Tool.Circle)
            {
                DrawFinalOnBitmap();
                shapes.Add(new Shape
                {
                    Type = currentTool == Tool.Line ? ShapeType.Line :
                           currentTool == Tool.Rectangle ? ShapeType.Rectangle : ShapeType.Circle,
                    Points = new[] { startPoint, lastPoint },
                    Color = currentColor
                });
                currentShape = null;
                canvas.Invalidate();
            }
        }

        private void DrawOnBitmap()
        {
            using (var g = Graphics.FromImage(canvasBitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var pen = new Pen(currentColor, 2))
                {
                    g.DrawLine(pen, startPoint, lastPoint);
                }
            }
            startPoint = lastPoint;
        }

        private void DrawFinalOnBitmap()
        {
            using (var g = Graphics.FromImage(canvasBitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var pen = new Pen(currentColor, 2))
                {
                    if (currentTool == Tool.Line)
                    {
                        g.DrawLine(pen, startPoint, lastPoint);
                    }
                    else if (currentTool == Tool.Rectangle)
                    {
                        var rect = GetRectangle(startPoint, lastPoint);
                        g.DrawRectangle(pen, rect);
                    }
                    else if (currentTool == Tool.Circle)
                    {
                        var rect = GetCircle(startPoint, lastPoint);
                        g.DrawEllipse(pen, rect);
                    }
                }
            }
        }

        // Отрисовка предпросмотра
        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.DrawImage(canvasBitmap, 0, 0);
            if (currentShape != null)
            {
                DrawShape(g, currentShape);
            }
        }

        private void DrawShape(Graphics g, Shape shape)
        {
            using (var pen = new Pen(shape.Color, 2))
            {
                if (shape.IsPreview)
                {
                    pen.DashPattern = new float[] { 3, 3 };
                }

                if (shape.Type == ShapeType.Line && shape.Points.Length == 2)
                {
                    g.DrawLine(pen, shape.Points[0], shape.Points[1]);
                }
                else if (shape.Type == ShapeType.Rectangle && shape.Points.Length == 2)
                {
                    var rect = GetRectangle(shape.Points[0], shape.Points[1]);
                    g.DrawRectangle(pen, rect);
                }
                else if (shape.Type == ShapeType.Circle && shape.Points.Length == 2)
                {
                    var rect = GetCircle(shape.Points[0], shape.Points[1]);
                    g.DrawEllipse(pen, rect);
                }
            }
        }

        // Вспомогательные методы
        private Rectangle GetRectangle(Point p1, Point p2)
        {
            int x = Math.Min(p1.X, p2.X);
            int y = Math.Min(p1.Y, p2.Y);
            int width = Math.Abs(p1.X - p2.X);
            int height = Math.Abs(p1.Y - p2.Y);
            return new Rectangle(x, y, width, height);
        }

        private Rectangle GetCircle(Point p1, Point p2)
        {
            // Круг от центра с радиусом = расстоянию между точками
            int radius = (int)Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
            int x = p1.X - radius;
            int y = p1.Y - radius;
            int diameter = radius * 2;
            return new Rectangle(x, y, diameter, diameter);
        }
    }

    //Модель 
    public enum ShapeType { Line, Rectangle, Circle }

    public class Shape
    {
        public ShapeType Type { get; set; }
        public Point[] Points { get; set; }
        public Color Color { get; set; }
        public bool IsPreview { get; set; }
    }
}