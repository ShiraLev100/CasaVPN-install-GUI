using CasaVPN_install_GUI.Properties;
using System;
using System.DirectoryServices;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Windows.Forms;
using System.Windows.Forms.PropertyGridInternal;

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


    public class RoundedButton : Button
    {
        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            GraphicsPath graphicsPath = new GraphicsPath();
            int radius = 20; // Adjust this value to get the desired curve radius
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


    public class RoundedTextBox : TextBox
    {
        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nheightRect, int nweightRect);

        private Color defaultBackColor = Color.White;
        private Color activeBackColor = Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(253)))), ((int)(((byte)(247))))); // Change this to your preferred active background color
        private Color defaultBorderColor = Color.Gray;
        private Color activeBorderColor = Color.SteelBlue; // Change this to your preferred active border color

        public RoundedTextBox()
        {
            // Adjust the size of the TextBox to be bigger than the text size
            //this.Size = new Size(300, 50); // Example size, adjust as needed
            //this.Multiline = false; // Single line for password
            //this.ScrollBars = ScrollBars.None; // No scrollbars

            // Set up rounded corners
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(2, 2, this.Width, this.Height, 15, 15));

            // Set other properties as needed
            this.BackColor = defaultBackColor;

            // Subscribe to events
            this.GotFocus += RoundedTextBox_GotFocus;
            this.LostFocus += RoundedTextBox_LostFocus;
        }

        private void RoundedTextBox_GotFocus(object sender, EventArgs e)
        {
            this.BackColor = activeBackColor;
            this.BorderStyle = BorderStyle.FixedSingle;
        }

        private void RoundedTextBox_LostFocus(object sender, EventArgs e)
        {
            this.BackColor = defaultBackColor;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(2, 2, this.Width, this.Height, 15, 15)); // Adjust the rounded corners
        }
    }

    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox pictureBoxBackground; // Define PictureBox here

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.pictureBoxBackground = new System.Windows.Forms.PictureBox();
            this.usernameTextBox = new UserCredentialsApp.RoundedTextBox();
            this.passwordTextBox = new UserCredentialsApp.RoundedTextBox();
            this.errorLabel = new System.Windows.Forms.Label();
            this.submitButton = new UserCredentialsApp.RoundedButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackground)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxBackground
            // 
            this.pictureBoxBackground.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxBackground.Image = global::CasaVPN_install_GUI.Properties.Resources.user_credentrials;
            this.pictureBoxBackground.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxBackground.Name = "pictureBoxBackground";
            this.pictureBoxBackground.Size = new System.Drawing.Size(646, 472);
            this.pictureBoxBackground.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxBackground.TabIndex = 0;
            this.pictureBoxBackground.TabStop = false;
            this.pictureBoxBackground.Click += new System.EventHandler(this.pictureBoxBackground_Click);
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.BackColor = System.Drawing.Color.White;
            this.usernameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usernameTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(133)))), ((int)(((byte)(153)))), ((int)(((byte)(145)))));
            this.usernameTextBox.Location = new System.Drawing.Point(259, 199);
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(194, 38);
            this.usernameTextBox.TabIndex = 3;
            this.usernameTextBox.TextChanged += new System.EventHandler(this.usernameTextBox_TextChanged);
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.BackColor = System.Drawing.SystemColors.Window;        
            this.passwordTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passwordTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(133)))), ((int)(((byte)(153)))), ((int)(((byte)(145)))));
            this.passwordTextBox.Location = new System.Drawing.Point(259, 249);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Size = new System.Drawing.Size(194, 38);
            this.passwordTextBox.TabIndex = 4;
            this.passwordTextBox.UseSystemPasswordChar = true;
            this.passwordTextBox.TextChanged += new System.EventHandler(this.passwordTextBox_TextChanged);
            // 
            // errorLabel
            // 
            this.errorLabel.AutoSize = true;
            this.errorLabel.BackColor = System.Drawing.Color.Transparent;
            this.errorLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.errorLabel.Font = new System.Drawing.Font("MS Reference Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.errorLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(45)))), ((int)(((byte)(42)))));
            this.errorLabel.Location = new System.Drawing.Point(190, 400);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(0, 15);
            this.errorLabel.TabIndex = 5;
            this.errorLabel.Click += new System.EventHandler(this.errorLabel_Click);
            // 
            // submitButton
            // 
            this.submitButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(133)))), ((int)(((byte)(153)))), ((int)(((byte)(145)))));
            this.submitButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.submitButton.ForeColor = System.Drawing.Color.Transparent;
            this.submitButton.Location = new System.Drawing.Point(166, 310);
            this.submitButton.Name = "submitButton";
            this.submitButton.Size = new System.Drawing.Size(162, 31);
            this.submitButton.TabIndex = 5;
            this.submitButton.Text = "NEXT";
            this.submitButton.UseVisualStyleBackColor = false;
            this.submitButton.Click += new System.EventHandler(this.submitButton_Click);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(646, 472);
            this.Controls.Add(this.errorLabel);
            this.Controls.Add(this.submitButton);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.usernameTextBox);
            this.Controls.Add(this.pictureBoxBackground);
            this.Name = "Form1";
            this.Text = "User Credentials";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackground)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

#pragma warning disable CS0649
        private RoundedButton submitButton;
        private Label errorLabel;
        private RoundedTextBox usernameTextBox;
        private RoundedTextBox passwordTextBox;

#pragma warning restore CS0649

    }
}

