using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class RoundedButton : Button
{
    protected override void OnPaint(PaintEventArgs pevent)
    {
        base.OnPaint(pevent);

        GraphicsPath graphicsPath = new GraphicsPath();
        int radius = 40; // Adjust this value to get the desired curve radius
        Rectangle newRect = new Rectangle(0, 0, this.Width, this.Height);

        graphicsPath.AddArc(newRect.X, newRect.Y, radius, radius, 180, 90);
        graphicsPath.AddArc(newRect.X + newRect.Width - radius, newRect.Y, radius, radius, 270, 90);
        graphicsPath.AddArc(newRect.X + newRect.Width - radius, newRect.Y + newRect.Height - radius, radius, radius, 0, 90);
        graphicsPath.AddArc(newRect.X, newRect.Y + newRect.Height - radius, radius, radius, 90, 90);
        graphicsPath.CloseAllFigures();

        this.Region = new Region(graphicsPath);

        using (Pen pen = new Pen(Color.SteelBlue, 2))
        {
            pevent.Graphics.DrawPath(pen, graphicsPath);
        }

        TextRenderer.DrawText(pevent.Graphics, this.Text, this.Font, newRect, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }
}
