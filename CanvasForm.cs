using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace doodle
{
    public partial class CanvasForm : Form
    {
        private static Point _lastPoint;
        private static bool _penDown;
        private static Pen _currentPen = Pens.Black;

        public CanvasForm()
        {
            InitializeComponent();
            CreateWidthMenu();
            CreateCanvas();
            pictureBox.MouseDown += (sender, e) => _penDown = true;
            pictureBox.MouseMove += (sender1, e1) =>
            {
                var g = Graphics.FromImage(pictureBox.Image);
                var newPoint = new Point(e1.X, e1.Y);
                if (_penDown)
                    g.DrawLine(_currentPen, _lastPoint, newPoint);
                _lastPoint = newPoint;
                pictureBox.Image = pictureBox.Image;
            };
            pictureBox.MouseUp += (sender2, e2) => _penDown = false;
        }

        private void CreateWidthMenu()
        {
            for (var w = 1; w <= 8; w++)
            {
                var subMenu = new ToolStripMenuItem { Tag = w, Text = w.ToString() };
                var width = w;
                subMenu.Click += (o, e) => _currentPen = new Pen(_currentPen.Color, width);
                mnuWidth.DropDownItems.Add(subMenu);
            }
        }

        private void CreateCanvas()
        {
            var width = pictureBox.Width;
            var height = pictureBox.Height;
            pictureBox.Image = new Bitmap(width, height);
            var g = Graphics.FromImage(pictureBox.Image);
            g.FillRectangle(Brushes.White, new Rectangle(0, 0, width, height));
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
                _currentPen = new Pen(colorDialog.Color, _currentPen.Width);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
            var myBitmap = pictureBox.Image;
            var encoderInfo = GetEncoderInfo("image/jpeg");
            var @params = new EncoderParameters(1) { Param = { [0] = new EncoderParameter(Encoder.Quality, 75L) } };
            myBitmap.Save(saveFileDialog.FileName, encoderInfo, @params);
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            return ImageCodecInfo.GetImageEncoders().FirstOrDefault(t => t.MimeType == mimeType);
        }

        private void mnuWidth_Click(object sender, EventArgs e)
        {
            mnuWidth.ShowDropDown();
        }
    }
}