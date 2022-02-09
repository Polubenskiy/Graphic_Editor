using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace EightHourLater
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        enum GraphMode
        {
            Карандаш,
            Линия,
            Рамка,
            Окружность,
            Треугольник,
            Ломаная,
            Безье,
            Сплайн,
            Прямоугольник,
            Круг,
            Стерка,
            Пипетка,
            Картинка
        }

        enum TypeBrush
        {
            Сплошная,
            Градиентная,
            Текстурная,
            Узорная
        }

        int countMode, widthPen, height, width, x0, y0, x, y, heightRect, widthRect, startRectX, startRectY, countBezie = 1, tempWidth, countSpline = 1, sizeSpline = 0, sizeEraser;
        GraphMode mode;
        Bitmap bitModePicture, bitTexture, bitMain, bitGlass, tempSize;
        PictureBox picObject;
        DashStyle dashStylePen;
        Pen pen;
        Color mainColor, secondColor, fontColor, colorGlass = Color.FromArgb(0, 0, 0, 0), colorOld, colorNew;
        Font font;
        String s;
        Graphics graphicsMain, graphicsGlass;
        Boolean drag;
        Rectangle rect, eraserRect, rectEffect;
        Point[] points = new Point[3];
        Point[] pointsBezie = new Point[4];
        Point[] pointsSpline;
        Random rand = new Random();

        Brush brush, eraserBrush, brushEffect;

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.FileName != "")
                bitMain.Save(saveFileDialog1.FileName);
        }

        private void влевоНа90ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tempMenu = (ToolStripMenuItem)sender;
            bitMain.RotateFlip((RotateFlipType)Convert.ToInt32(tempMenu.Tag));
            pictureBoxEdit.Invalidate();
        }

        private void чернобелыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int r, g, b;
            //bitMain = new Bitmap(tempSize, bitMain.Width, bitMain.Height);
            //graphicsMain = Graphics.FromImage(bitMain);
            //graphicsGlass = Graphics.FromImage(bitGlass);
            //graphicsGlass.Clear(colorGlass);
            //pictureBoxEdit.Image = bitMain;
            ToolStripMenuItem tempMenu = (ToolStripMenuItem)sender;
            if (Convert.ToInt32(tempMenu.Tag) != 3 && Convert.ToInt32(tempMenu.Tag) != 4 && Convert.ToInt32(tempMenu.Tag) != 7)
            {
                for (int i = 0; i < bitMain.Width; i++)
                {
                    for (int j = 0; j < bitMain.Height; j++)
                    {
                        colorOld = bitMain.GetPixel(i, j);
                        switch (Convert.ToInt32(tempMenu.Tag))
                        {
                            case 1:
                                var gray = (colorOld.R + colorOld.G + colorOld.B) / 3;
                                colorNew = Color.FromArgb(255, gray, gray, gray);
                                break;
                            case 2:
                                colorNew = Color.FromArgb(255, 0xFF - colorOld.R, 0xFF - colorOld.G, 0xFF - colorOld.B);
                                break;
                            case 5:
                                r = (byte)(rand.Next(0, 2) == 1 ? colorOld.R : 255);
                                g = (byte)(rand.Next(0, 2) == 1 ? colorOld.G : 255);
                                b = (byte)(rand.Next(0, 2) == 1 ? colorOld.B : 255);
                                colorNew = Color.FromArgb(255, r, g, b);
                                break;
                            case 6:
                                r = (byte)Math.Min((colorOld.R * .393) + (colorOld.G * .769) + (colorOld.B * .189), 255.0);
                                g = (byte)Math.Min((colorOld.R * .349) + (colorOld.G * .686) + (colorOld.B * .168), 255.0);
                                b = (byte)Math.Min((colorOld.R * .272) + (colorOld.G * .534) + (colorOld.B * .131), 255.0);
                                colorNew = Color.FromArgb(255, r, g, b);
                                break;
                        }
                        bitMain.SetPixel(i, j, colorNew);
                    }
                }
            }
            else if (Convert.ToInt32(tempMenu.Tag) == 7)
            {
                bitMain = (Bitmap)ConvertImage(bitMain);
            }
            else 
            {
                rectEffect = new Rectangle(0, 0, bitMain.Width, bitMain.Height);
                if (Convert.ToInt32(tempMenu.Tag) == 4)
                    brushEffect = new SolidBrush(Color.FromArgb(128, Color.Black));
                else brushEffect = new SolidBrush(Color.FromArgb(128, Color.White));
                graphicsGlass.FillRectangle(brushEffect, rectEffect);
                graphicsMain.DrawImage(bitGlass, 0, 0);
            }
            pictureBoxEdit.Invalidate();
        }

        private Image ConvertImage(Image source)
        {
            int W_CELL = 12, H_CELL = 12;
            Image result = (Image)source.Clone();
            Bitmap bitmap = new Bitmap(source);

            using (Graphics g = Graphics.FromImage(result))
            {
                for (int y = 0; y < bitmap.Height; y += H_CELL)
                {
                    for (int x = 0; x < bitmap.Width; x += W_CELL)
                    {
                        Brush brush = new SolidBrush(bitmap.GetPixel(x, y));
                        g.FillRectangle(brush, x, y, W_CELL, H_CELL);
                    }
                }

            }

            return result;

        }
        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                bitMain.Save(saveFileDialog1.FileName);
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            countMode = Enum.GetNames(typeof(GraphMode)).Length;
            RadioButton[] rbMas = new RadioButton[countMode];
            ToolTip[] toolTip = new ToolTip[countMode];
            for (int i = 0; i < countMode; i++)
            {
                rbMas[i] = new RadioButton();
                rbMas[i].Width = 140;
                toolTip[i] = new ToolTip();
                toolTip[i].SetToolTip(rbMas[i], ((GraphMode)i).ToString());
                rbMas[i].Text = ((GraphMode)i).ToString();
                rbMas[i].Appearance = Appearance.Button;
                rbMas[i].Checked = false;
                rbMas[i].BackColor = Color.Linen;
                rbMas[i].AutoCheck = true;
                rbMas[i].Tag = i;
                rbMas[i].Click += radioButton_Click;
            }
            this.flowLayoutPanelGraphMode.FlowDirection = FlowDirection.TopDown;    //сверху
            this.flowLayoutPanelGraphMode.AutoScroll = true;        //Полосы прокрутки
            this.flowLayoutPanelGraphMode.WrapContents = false; //Нет перехода на новую строку
            this.flowLayoutPanelGraphMode.Font = new Font(FontFamily.GenericMonospace, 10);
            this.flowLayoutPanelGraphMode.Controls.Clear();

            flowLayoutPanelGraphMode.Controls.AddRange(rbMas);

            picObject = new PictureBox();                      
            picObject.Width = 140;
            picObject.Height = 100;
            picObject.BackColor = Color.Transparent;
            picObject.BorderStyle = BorderStyle.Fixed3D;
            picObject.BackColor = Color.AliceBlue;
            picObject.Visible = false;
            bitModePicture = new Bitmap(140, 100);
            this.flowLayoutPanelGraphMode.Controls.Add(picObject);

            openFileDialog1.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG";

            this.toolStripComboBoxStylePen.Items.Clear();
            //Заполнить свойство Items всеми значениями из стандартного перечисления DashStyle
            this.toolStripComboBoxStylePen.Items.AddRange(Enum.GetNames(typeof(DashStyle)));
            //Вывести название первого из перечисления
            this.toolStripComboBoxStylePen.SelectedItem = this.toolStripComboBoxStylePen.Items[0].ToString();

            toolStripComboBoxTypeBrush.Items.Clear();
            toolStripComboBoxTypeBrush.Items.AddRange(Enum.GetNames(typeof(TypeBrush)));
            toolStripComboBoxTypeBrush.SelectedItem = toolStripComboBoxTypeBrush.Items[0].ToString();

            pictureBoxEdit.Image = null;
            bitMain = null;
            pictureBoxEdit.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBoxEdit.Size = panel1.Size;
            pictureBoxEdit.BackColor = Color.White;

            toolStripButtonMainColor.BackColor = toolStripButtonFontColor.BackColor = toolStripButtonSecondColor.BackColor = Color.Black;

            mode = GraphMode.Карандаш;
            pen = new Pen(toolStripButtonMainColor.BackColor, Convert.ToInt32(toolStripComboBoxWidthPen.SelectedItem));
            width = pictureBoxEdit.Width;
            height = pictureBoxEdit.Height;

            bitMain = new Bitmap(width, height);
            graphicsMain = Graphics.FromImage(bitMain);
            graphicsMain.Clear(Color.White);


            bitGlass = new Bitmap(width, height);
            graphicsGlass = Graphics.FromImage(bitGlass);
            graphicsGlass.Clear(colorGlass);

            drag = false;

            mainColor = Color.Black;
            secondColor = Color.Black;
            fontColor = Color.Black;

            brush = new SolidBrush(Color.Black);
            pictureBoxEdit.Invalidate();

            toolStripStatusLabelDate.Text = DateTime.Now.ToLongDateString();
            pictureBoxEdit.Width = 913;
            pictureBoxEdit.Height = 504;
            toolStripStatusLabelSize.Text = $"Размер: {pictureBoxEdit.Width}x{pictureBoxEdit.Height}";
            toolStripComboBoxWidthPen.SelectedItem = toolStripComboBoxWidthPen.Items[0];

            saveFileDialog1.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
        }

        private void pictureBoxEdit_DoubleClick(object sender, EventArgs e)
        {
            drag = false;
            graphicsMain.DrawImage(bitMain, 0, 0);
            graphicsMain.DrawImage(bitGlass, 0, 0);
            pictureBoxEdit.Invalidate();
        }

        private void timerGraphEdit_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabelTime.Text = DateTime.Now.ToLongTimeString();
        }

        private void radioButton_Click(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;     //Ссылка на кнопку
            mode = (GraphMode)rb.Tag;               //Ее номер, чтобы получить название
            this.toolStripStatusLabelMode.Text = mode.ToString();   //Название режима
            picObject.Visible = false;
            switch (mode)
            {
                case GraphMode.Картинка:				//Отдельно режим «Картинка»
                    if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        Bitmap temp = new Bitmap(openFileDialog1.FileName); //Картинка изображения
                        bitModePicture = new Bitmap(temp, 100, 100);            //Подогнать размер по компонент
                        picObject.Image = bitModePicture;           //Перенести на компонент
                        picObject.Visible = true;
                    }
                    break;
                case GraphMode.Сплайн:
                    do
                    {
                        s = Interaction.InputBox("Введите количество точек сплайна\nМинимум: 4\nМаксимум: 10\nКликов необходимо на 2 меньше", "Сплайн");
                    }
                    while (!int.TryParse(s, out sizeSpline) || sizeSpline < 4 || sizeSpline > 10);
                    pointsSpline = new Point[sizeSpline];
                    countSpline = 1;
                    break;
                case GraphMode.Стерка:
                    do
                    {
                        s = Interaction.InputBox("Введите размер ластика (пиксели * 10)\nМинимум: 1\nМаксимум: 10", "Ластик");
                    }
                    while (!int.TryParse(s, out sizeEraser) || sizeEraser < 1 || sizeEraser > 10);
                    sizeEraser *= 10;
                    eraserBrush = new SolidBrush(pictureBoxEdit.BackColor);
                    break;
            }

        }
        private void pictureBoxEdit_MouseDown(object sender, MouseEventArgs e)
        {
            switch (mode)
            {
                case GraphMode.Ломаная:
                    graphicsMain.DrawImage(bitMain, 0, 0);
                    graphicsMain.DrawImage(bitGlass, 0, 0);
                    pictureBoxEdit.Invalidate();
                    drag = true;
                    x0 = e.X;
                    y0 = e.Y;
                    break;
                case GraphMode.Безье:
                    switch (countBezie)
                    {
                        case 1:
                            pointsBezie[0].X = e.X; pointsBezie[0].Y = e.Y; drag = true; tempWidth = widthPen;
                            break;
                        case 3:
                            pointsBezie[2].X = e.X; pointsBezie[2].Y = e.Y;
                            break;
                        case 4:
                            pointsBezie[3].X = e.X; pointsBezie[3].Y = e.Y;
                            pen.Color = pictureBoxEdit.BackColor;
                            pen.Width = tempWidth;
                            graphicsGlass.DrawLine(pen, pointsBezie[0], pointsBezie[1]);
                            pen.Color = mainColor;
                            pen.Width = widthPen;
                            graphicsGlass.DrawBezier(pen, pointsBezie[0], pointsBezie[2], pointsBezie[3], pointsBezie[1]);
                            countBezie = 0;
                            break;
                    }
                    countBezie++;
                    break;
                case GraphMode.Пипетка:
                    if (e.Button == MouseButtons.Left)
                    {
                        toolStripButtonMainColor.BackColor = bitMain.GetPixel(e.X, e.Y);
                        mainColor = toolStripButtonMainColor.BackColor;
                        pen.Color = mainColor;
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        toolStripButtonSecondColor.BackColor = bitMain.GetPixel(e.X, e.Y);
                        secondColor = toolStripButtonSecondColor.BackColor;
                    }
                    toolStripComboBoxStyleBrush_TextChanged(sender, e);
                    break;
                case GraphMode.Сплайн:
                    if (countSpline == 1)
                    {
                        pointsSpline[0].X = e.X; 
                        pointsSpline[0].Y = e.Y; 
                        drag = true; 
                        tempWidth = widthPen;
                    }
                    else if (countSpline == sizeSpline)
                    {
                        pointsSpline[countSpline-2].X = e.X; pointsSpline[countSpline - 2].Y = e.Y;
                        pen.Color = pictureBoxEdit.BackColor;
                        pen.Width = tempWidth;
                        graphicsGlass.DrawLine(pen, pointsSpline[0], pointsSpline[sizeSpline - 1]);
                        pen.Color = mainColor;
                        pen.Width = widthPen;
                        graphicsGlass.DrawCurve(pen, pointsSpline);
                        countSpline = 0;
                    }
                    else if (countSpline > 1 || countSpline < sizeSpline)
                    {
                        pointsSpline[countSpline-2].X = e.X; pointsSpline[countSpline - 2].Y = e.Y;
                    }
                    countSpline++;
                    break;
                default:
                    drag = true;
                    x0 = e.X;
                    y0 = e.Y;
                    startRectX = e.X;
                    startRectY = e.Y;
                    break;
            }
        }
        private void pictureBoxEdit_MouseMove(object sender, MouseEventArgs e)
        {
            toolStripStatusLabelCursor.Text = $"Курсор: {e.X}, {e.Y}";
            if (drag) 
            {
                x = e.X;
                y = e.Y;
                graphicsGlass.Clear(colorGlass);
                switch (mode)
                {
                    case GraphMode.Карандаш:
                        graphicsMain.DrawLine(pen, x0, y0, x, y);
                        x0 = x;
                        y0 = y;
                        break;
                    case GraphMode.Линия:
                        graphicsGlass.DrawLine(pen, x0, y0, x, y);
                        break;
                    case GraphMode.Рамка:
                        widthRect = 0;
                        heightRect = 0;

                        if (x > startRectX && y > startRectY) // top left to bottom right
                        {
                            widthRect = Math.Abs(x - startRectX);
                            heightRect = Math.Abs(y - startRectY);
                        }
                        else if (x < startRectX && y < startRectY) // bottom right to top left
                        {
                            widthRect = Math.Abs(startRectX - x);
                            heightRect = Math.Abs(startRectY - y);

                            x0 = x;
                            y0 = y;
                        }
                        else if (x < startRectX && y > startRectY) // top right to bottom left
                        {
                            widthRect = Math.Abs(startRectX - x);
                            heightRect = Math.Abs(startRectY - y);

                            x0 = x;
                        }
                        else if (x > startRectX && y < startRectY) // bottom left to top right
                        {
                            widthRect = Math.Abs(startRectX - x);
                            heightRect = Math.Abs(startRectY - y);

                            y0 = y;
                        }
                        rect = new Rectangle(x0, y0, widthRect, heightRect);
                        graphicsGlass.DrawRectangle(pen, rect);
                        break;
                    case GraphMode.Окружность:
                        graphicsGlass.FillEllipse(brush, x0, y0, (x - x0), (y - y0));
                        break;
                    case GraphMode.Треугольник:
                        points[0].X = x0; points[0].Y = y0;
                        points[1].X = x; points[1].Y = y0;
                        points[2].X = x; points[2].Y = y;
                        graphicsGlass.FillPolygon(brush, points);
                        break;
                    case GraphMode.Ломаная:
                        graphicsGlass.DrawLine(pen, x0, y0, x, y);
                        break;
                    case GraphMode.Безье:
                        if (countBezie == 2)
                        graphicsGlass.DrawLine(pen, pointsBezie[0].X, pointsBezie[0].Y, x, y);
                        break;
                    case GraphMode.Сплайн:
                        if (countSpline == 2)
                            graphicsGlass.DrawLine(pen, pointsSpline[0].X, pointsSpline[0].Y, x, y);
                        break;
                    case GraphMode.Прямоугольник:
                        widthRect = 0;
                        heightRect = 0;

                        if (x > startRectX && y > startRectY) // top left to bottom right
                        {
                            widthRect = Math.Abs(x - startRectX);
                            heightRect = Math.Abs(y - startRectY);
                        }
                        else if (x < startRectX && y < startRectY) // bottom right to top left
                        {
                            widthRect = Math.Abs(startRectX - x);
                            heightRect = Math.Abs(startRectY - y);

                            x0 = x;
                            y0 = y;
                        }
                        else if (x < startRectX && y > startRectY) // top right to bottom left
                        {
                            widthRect = Math.Abs(startRectX - x);
                            heightRect = Math.Abs(startRectY - y);

                            x0 = x;
                        }
                        else if (x > startRectX && y < startRectY) // bottom left to top right
                        {
                            widthRect = Math.Abs(startRectX - x);
                            heightRect = Math.Abs(startRectY - y);

                            y0 = y;
                        }
                        rect = new Rectangle(x0, y0, widthRect, heightRect);
                        graphicsGlass.FillRectangle(brush, rect);
                        break;
                    case GraphMode.Круг:
                        graphicsGlass.DrawEllipse(pen, x0, y0, (x - x0), (y - y0));
                        break;
                    case GraphMode.Стерка:
                        eraserRect = new Rectangle(x - sizeEraser/2, y - sizeEraser/2, sizeEraser, sizeEraser);
                        graphicsGlass.FillRectangle(eraserBrush, eraserRect);
                        graphicsMain.DrawImage(bitGlass, 0, 0);
                        break;
                }
                pictureBoxEdit.Invalidate();
                
            }
        }

        private void pictureBoxEdit_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(bitMain, 0, 0);
            if (drag) 
                e.Graphics.DrawImage(bitGlass, 0, 0);
        }

        private void pictureBoxEdit_MouseUp(object sender, MouseEventArgs e)
        {
            if (mode != GraphMode.Ломаная)
            {
                drag = false;
                graphicsMain.DrawImage(bitMain, 0, 0);
                graphicsMain.DrawImage(bitGlass, 0, 0);
                pictureBoxEdit.Invalidate();
                if (countBezie == 2)
                {
                    pointsBezie[1].X = e.X; pointsBezie[1].Y = e.Y;
                    countBezie++;
                }
                if (countSpline == 2)
                {
                    pointsSpline[sizeSpline-1].X = e.X; pointsSpline[sizeSpline - 1].Y = e.Y;
                    countSpline++;
                }
            }
        }

        private void toolStripComboBoxStylePen_TextChanged(object sender, EventArgs e)
        {
            pen = new Pen(mainColor, widthPen);
            dashStylePen = (DashStyle)toolStripComboBoxStylePen.SelectedIndex;
            pen.DashStyle = dashStylePen;
        }

        private void toolStripComboBoxTypeBrush_TextChanged(object sender, EventArgs e)
        {
            this.toolStripComboBoxStyleBrush.Items.Clear();
            switch (toolStripComboBoxTypeBrush.SelectedIndex)
            {
                case 0:
                    brush = new SolidBrush(mainColor);
                    break;
                case 1:
                    this.toolStripComboBoxStyleBrush.Items.AddRange(Enum.GetNames(typeof(LinearGradientMode)));
                    this.toolStripComboBoxStyleBrush.SelectedItem = this.toolStripComboBoxStyleBrush.Items[0];
                    break;
                case 2:
                    if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        Bitmap temp = new Bitmap(openFileDialog1.FileName); //Картинка изображения
                        bitTexture = new Bitmap(temp, pictureBoxEdit.Size);
                    }
                    else return;
                    this.toolStripComboBoxStyleBrush.Items.AddRange(Enum.GetNames(typeof(WrapMode)));
                    this.toolStripComboBoxStyleBrush.SelectedItem = this.toolStripComboBoxStyleBrush.Items[0];
                    break;
                case 3:
                    this.toolStripComboBoxStyleBrush.Items.AddRange(Enum.GetNames(typeof(HatchStyle)));
                    this.toolStripComboBoxStyleBrush.SelectedItem = this.toolStripComboBoxStyleBrush.Items[0];
                    break;
            }
        }

        private void toolStripButtonFontType_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                font = fontDialog1.Font;
            }
        }

        private void исходныйРазмерToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tempSize != null)
            {
                bitMain = new Bitmap(tempSize);
                bitGlass = new Bitmap(pictureBoxEdit.Width, pictureBoxEdit.Height);
                graphicsMain = Graphics.FromImage(bitMain);
                graphicsGlass = Graphics.FromImage(bitGlass);
                graphicsGlass.Clear(colorGlass);
                pictureBoxEdit.Image = bitMain;
            }
        }

        private void подогнатьРазмерToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tempSize = new Bitmap(bitMain);
            bitMain = new Bitmap(pictureBoxEdit.Image, panel1.Size);
            bitGlass = new Bitmap(pictureBoxEdit.Width, pictureBoxEdit.Height);
            graphicsMain = Graphics.FromImage(bitMain);
            graphicsGlass = Graphics.FromImage(bitGlass);
            graphicsGlass.Clear(colorGlass);
            pictureBoxEdit.Image = bitMain;
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void цветФонаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBoxEdit.BackColor = colorDialog.Color;
                graphicsGlass.Clear(colorDialog.Color);
                graphicsMain.DrawImage(bitGlass, 0, 0);
                eraserBrush = new SolidBrush(pictureBoxEdit.BackColor);
            }
        }

        private void задатьРазмерToolStripMenuItem_Click(object sender, EventArgs e)
        {
            do
            {
                s = Interaction.InputBox("Введите ширину (width)", "Ширина");
            }
            while (!int.TryParse(s, out width));
            do
            {
                s = Interaction.InputBox("Введите высоту (height)", "Высота");
            }
            while (!int.TryParse(s, out height));
            bitMain = new Bitmap(width, height);
            pictureBoxEdit.Image = bitMain;
            bitGlass = new Bitmap(pictureBoxEdit.Width, pictureBoxEdit.Height);
            graphicsMain = Graphics.FromImage(bitMain);
            graphicsGlass = Graphics.FromImage(bitGlass);
            graphicsGlass.Clear(colorGlass);
        }

        private void очиститьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1_Load(sender, e);
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap temp = new Bitmap(openFileDialog1.FileName); //Картинка изображения
                bitMain = new Bitmap(temp);
                tempSize = new Bitmap(bitMain);
                pictureBoxEdit.Image = bitMain;
                bitGlass = new Bitmap(pictureBoxEdit.Width, pictureBoxEdit.Height);
                graphicsMain = Graphics.FromImage(bitMain);
                graphicsGlass = Graphics.FromImage(bitGlass);
                graphicsGlass.Clear(colorGlass);
                toolStripStatusLabelFile.Text = $"Файл: {openFileDialog1.FileName}";
            }
        }

        private void toolStripComboBoxStyleBrush_TextChanged(object sender, EventArgs e)
        {
            rect = new Rectangle(0, 0, pictureBoxEdit.Width, pictureBoxEdit.Height);
            switch (toolStripComboBoxTypeBrush.SelectedIndex)
            {
                case 1:
                    brush = new LinearGradientBrush(rect, mainColor, secondColor, (LinearGradientMode)toolStripComboBoxStyleBrush.SelectedIndex);
                    break;
                case 2:
                    brush = new TextureBrush(bitTexture, (WrapMode)toolStripComboBoxStyleBrush.SelectedIndex);
                    break;
                case 3:
                    brush = new HatchBrush((HatchStyle)toolStripComboBoxStyleBrush.SelectedIndex, secondColor, mainColor);
                    break;
            }
        }

        public void toolStripComboBoxWidthPen_TextChanged(object sender, EventArgs e)
        {
            widthPen = Convert.ToInt32(toolStripComboBoxWidthPen.SelectedItem);
            pen = new Pen(mainColor, widthPen);
        }

        private void toolStripButtonMainColor_Click(object sender, EventArgs e)
        {
            ToolStripButton bColor = (ToolStripButton)sender;
            if (colorDialog.ShowDialog() == DialogResult.OK)
                bColor.BackColor = colorDialog.Color;
            mainColor = toolStripButtonMainColor.BackColor;
            secondColor = toolStripButtonSecondColor.BackColor;
            fontColor = toolStripButtonFontColor.BackColor;
            pen = new Pen(mainColor, widthPen);
            switch (toolStripComboBoxTypeBrush.SelectedIndex)
            {
                case 0:
                    brush = new SolidBrush(mainColor);
                    break;
                case 1:
                    brush = new LinearGradientBrush(rect, mainColor, secondColor, (LinearGradientMode)toolStripComboBoxStyleBrush.SelectedIndex);
                    break;
                case 3:
                    brush = new HatchBrush((HatchStyle)toolStripComboBoxStyleBrush.SelectedIndex, secondColor, mainColor);
                    break;
            }
        }
    }
}
