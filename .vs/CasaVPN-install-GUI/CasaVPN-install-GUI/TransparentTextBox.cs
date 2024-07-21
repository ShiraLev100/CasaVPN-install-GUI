using System;
using System.Drawing;
using System.Windows.Forms;

namespace UserCredentialsApp
{
    public class TransparentTextBox : TextBox
    {
        private const int WM_ERASEBKGND = 0x14;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20;  // WS_EX_TRANSPARENT
                return cp;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Do not paint background
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_ERASEBKGND)
            {
                using (var brush = new SolidBrush(Color.FromArgb(0, Color.Transparent)))
                {
                    Graphics g = Graphics.FromHdc(m.WParam);
                    g.FillRectangle(brush, ClientRectangle);
                    g.Dispose();
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }
    }
}
