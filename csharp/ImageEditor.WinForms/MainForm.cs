using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ImageEditor
{
    public class MainForm : Form
    {
        private PictureBox _picture = new() { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.Black };
        private Panel _top = new() { Dock = DockStyle.Top, Height = 56 };
        private Button _btnOpen = new() { Text = "Open", Width = 80, Left = 8, Top = 12 };
        private Button _btnSave = new() { Text = "Save As", Width = 80, Left = 96, Top = 12 };
        private Button _btnGray = new() { Text = "Grayscale", Width = 100, Left = 200, Top = 12 };
        private Button _btnInvert = new() { Text = "Invert", Width = 80, Left = 308, Top = 12 };
        private Label _lblBrightness = new() { Text = "Brightness", Left = 404, Top = 18, ForeColor = Color.White };
        private TrackBar _brightness = new() { Minimum = -100, Maximum = 100, TickFrequency = 20, Width = 180, Left = 480, Top = 12 };
        private Button _btnResize = new() { Text = "Resize 50%", Width = 100, Left = 670, Top = 12 };

        private Bitmap? _current;
        private Bitmap? _original;

        public MainForm()
        {
            Text = "Image Editor (WinForms)";
            Width = 980; Height = 640; StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(32, 32, 32);
            _top.BackColor = Color.FromArgb(24,24,24);
            Controls.Add(_picture);
            Controls.Add(_top);
            _top.Controls.AddRange(new Control[] { _btnOpen, _btnSave, _btnGray, _btnInvert, _lblBrightness, _brightness, _btnResize });

            _btnOpen.Click += OnOpen;
            _btnSave.Click += OnSave;
            _btnGray.Click += (s,e) => ApplyFilter(Grayscale);
            _btnInvert.Click += (s,e) => ApplyFilter(Invert);
            _brightness.ValueChanged += (s,e) => ApplyFilter((bmp) => AdjustBrightness(bmp, _brightness.Value));
            _btnResize.Click += (s,e) => ApplyFilter((bmp) => Resize(bmp, 0.5f));
        }

        private void OnOpen(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog { Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp" };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _original?.Dispose();
                _current?.Dispose();
                _original = new Bitmap(dlg.FileName);
                _current = new Bitmap(_original);
                _picture.Image = _current;
                _brightness.Value = 0;
            }
        }

        private void OnSave(object? sender, EventArgs e)
        {
            if (_current == null) return;
            using var dlg = new SaveFileDialog { Filter = "PNG|*.png|JPEG|*.jpg|Bitmap|*.bmp", FileName = "edited.png" };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var ext = System.IO.Path.GetExtension(dlg.FileName).ToLowerInvariant();
                var fmt = ImageFormat.Png;
                if (ext == ".jpg" || ext == ".jpeg") fmt = ImageFormat.Jpeg;
                else if (ext == ".bmp") fmt = ImageFormat.Bmp;
                _current.Save(dlg.FileName, fmt);
            }
        }

        private void ApplyFilter(Func<Bitmap, Bitmap> filter)
        {
            if (_original == null) return;
            _current?.Dispose();
            _current = filter(new Bitmap(_original));
            _picture.Image = _current;
        }

        // Filters
        private static Bitmap Grayscale(Bitmap src)
        {
            for (int y = 0; y < src.Height; y++)
            {
                for (int x = 0; x < src.Width; x++)
                {
                    var c = src.GetPixel(x, y);
                    int gray = (int)(0.299 * c.R + 0.587 * c.G + 0.114 * c.B);
                    src.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
            }
            return src;
        }

        private static Bitmap Invert(Bitmap src)
        {
            for (int y = 0; y < src.Height; y++)
            {
                for (int x = 0; x < src.Width; x++)
                {
                    var c = src.GetPixel(x, y);
                    src.SetPixel(x, y, Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                }
            }
            return src;
        }

        private static Bitmap AdjustBrightness(Bitmap src, int delta)
        {
            // delta in [-100, 100]; map to [-255, 255]
            int shift = (int)(delta * 2.55);
            for (int y = 0; y < src.Height; y++)
            {
                for (int x = 0; x < src.Width; x++)
                {
                    var c = src.GetPixel(x, y);
                    int r = Math.Clamp(c.R + shift, 0, 255);
                    int g = Math.Clamp(c.G + shift, 0, 255);
                    int b = Math.Clamp(c.B + shift, 0, 255);
                    src.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            return src;
        }

        private static Bitmap Resize(Bitmap src, float scale)
        {
            int nw = (int)(src.Width * scale);
            int nh = (int)(src.Height * scale);
            var dest = new Bitmap(nw, nh);
            using (var g = Graphics.FromImage(dest))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(src, 0, 0, nw, nh);
            }
            return dest;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _current?.Dispose();
            _original?.Dispose();
            base.OnFormClosed(e);
        }
    }
}
