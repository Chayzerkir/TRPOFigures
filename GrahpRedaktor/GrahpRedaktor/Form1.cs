using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GraphicEditorXP
{
    public partial class Form1 : Form
    {
        // Состояние
        private enum Tool { Pencil, Line, Rectangle, Clear }
        private Tool currentTool = Tool.Pencil;
        private Color currentColor = Color.Black;
        private Point startPoint;
        private Point lastPoint;
        private bool isDrawing = false;
        private List<Shape> shapes = new List<Shape>();
        private Shape currentShape = null;
        private PictureBox canvas;
        private Panel controlPanel;

        public Form1()
        {
            // Настройка окна
            this.Text = "Graphic Editor 1.5";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            SetupUI();
            SetupCanvas();
        }

        private void SetupUI()
        {
            // Панель инструментов
            controlPanel = new Panel();
            controlPanel.Height = 40;
            controlPanel.Dock = DockStyle.Top;
            controlPanel.BackColor = Color.LightGray;

            // Кнопки инструментов
            AddToolButton("Карандаш", () => currentTool = Tool.Pencil);
            AddToolButton("Линия", () => currentTool = Tool.Line);
            AddToolButton("Прямоугольник", () => currentTool = Tool.Rectangle);
            AddToolButton("Очистить", ClearCanvas);

            // Выбор цвета
            var colorLabel = new Label();
            colorLabel.Text = "Цвет:";
            colorLabel.Location = new Point(350, 12);
            colorLabel.Size = new Size(40, 20);
            controlPanel.Controls.Add(colorLabel);

            var colorCombo = new ComboBox();
            colorCombo.Location = new Point(390, 10);
            colorCombo.Width = 100;
            colorCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            colorCombo.Items.AddRange(new object[] { "Черный", "Красный", "Зеленый", "Синий" });
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
                    default:
                        currentColor = Color.Black;
                        break;
                }
            };
            controlPanel.Controls.Add(colorCombo);

            this.Controls.Add(controlPanel);
        }

        private void AddToolButton(string text, Action action)
        {
            var btn = new Button();
            btn.Text = text;
            btn.Location = new Point(10 + controlPanel.Controls.Count * 100, 10);
            btn.Size = new Size(80, 25);
            btn.Click += (s, e) => action();
            controlPanel.Controls.Add(btn);
        }

        private void SetupCanvas()
        {
            canvas = new PictureBox();
            canvas.Dock = DockStyle.Fill;
            canvas.BackColor = Color.White;
            canvas.Cursor = Cursors.Cross;

            // Обработчики событий
            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseUp += Canvas_MouseUp;
            canvas.Paint += Canvas_Paint;

            this.Controls.Add(canvas);
        }

        private void ClearCanvas()
        {
            shapes.Clear();
            canvas.Invalidate();
        }

        // === Обработчики событий ===

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
                // Для карандаша рисуем сразу
                shapes.Add(new Shape
                {
                    Type = ShapeType.Line,
                    Points = new[] { startPoint, lastPoint },
                    Color = currentColor
                });
                startPoint = lastPoint; // Непрерывная линия
                canvas.Invalidate();
            }
            else if (currentTool == Tool.Line || currentTool == Tool.Rectangle)
            {
                // Создаем временную фигуру для предпросмотра
                currentShape = new Shape
                {
                    Type = currentTool == Tool.Line ? ShapeType.Line : ShapeType.Rectangle,
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

            // Финализируем фигуру (кроме карандаша, который уже нарисован)
            if (currentTool == Tool.Line || currentTool == Tool.Rectangle)
            {
                shapes.Add(new Shape
                {
                    Type = currentTool == Tool.Line ? ShapeType.Line : ShapeType.Rectangle,
                    Points = new[] { startPoint, lastPoint },
                    Color = currentColor
                });
                currentShape = null;
                canvas.Invalidate();
            }
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Рисуем все сохраненные фигуры
            foreach (var shape in shapes)
            {
                DrawShape(g, shape);
            }

            // Рисуем предпросмотр
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
                    // Пунктир для предпросмотра
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
            }
        }

        private Rectangle GetRectangle(Point p1, Point p2)
        {
            int x = Math.Min(p1.X, p2.X);
            int y = Math.Min(p1.Y, p2.Y);
            int width = Math.Abs(p1.X - p2.X);
            int height = Math.Abs(p1.Y - p2.Y);

            return new Rectangle(x, y, width, height);
        }
    }

    // Модель данных
    public enum ShapeType { Line, Rectangle }

    public class Shape
    {
        public ShapeType Type { get; set; }
        public Point[] Points { get; set; }
        public Color Color { get; set; }
        public bool IsPreview { get; set; }
    }
}