using System;
using System.Drawing;
using System.Windows.Forms;

namespace MPP_Client
{
    public partial class Form1 : Form
    {
        private Panel   pnlCard;
        private Label   lblTitle;
        private Label   lblSubtitle;
        private Label   lblEmail;
        private Label   lblPassword;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private Button  btnLogin;
        private LinkLabel lnkForgot;

        public Form1()
        {
            InitializeComponent();
            BuildUI();
        }

        private void BuildUI()
        {
            UIStyles.ApplyForm(this, width: 460, height: 500);
            this.Text = "MPP Client — Login";

            // Card panel centered on form
            pnlCard = new Panel
            {
                Size     = new Size(360, 390),
                Location = new Point(50, 50)
            };
            UIStyles.ApplyCard(pnlCard);
            pnlCard.Paint += (s, e) => DrawCardBorder(e.Graphics, pnlCard);

            // Title
            lblTitle = new Label
            {
                Text      = "Sign in",
                Font      = UIStyles.FontTitle,
                ForeColor = UIStyles.TextPrimary,
                AutoSize  = true,
                Location  = new Point(0, 24),
                Width     = pnlCard.Width,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Subtitle
            lblSubtitle = new Label
            {
                Text      = "Welcome to MPP Client",
                Font      = UIStyles.FontSubtitle,
                ForeColor = UIStyles.TextSecondary,
                AutoSize  = true,
                Location  = new Point(0, 62),
                Width     = pnlCard.Width,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Separator line
            var sep = new Panel
            {
                BackColor = UIStyles.Border,
                Size      = new Size(300, 1),
                Location  = new Point(30, 96)
            };

            // Email
            lblEmail = MakeLabel("EMAIL", new Point(30, 116));
            txtEmail = new TextBox { PlaceholderText = "your@email.com", Location = new Point(30, 136), Width = 300 };
            UIStyles.ApplyTextBox(txtEmail);

            // Password
            lblPassword = MakeLabel("PASSWORD", new Point(30, 178));
            txtPassword = new TextBox { PlaceholderText = "Enter your password", PasswordChar = '●', Location = new Point(30, 198), Width = 300 };
            UIStyles.ApplyTextBox(txtPassword);

            // Login button
            btnLogin = new Button
            {
                Text     = "Sign in",
                Location = new Point(30, 250),
                Width    = 300
            };
            UIStyles.ApplyPrimaryButton(btnLogin);
            btnLogin.Click += BtnLogin_Click;

            // Forgot link
            lnkForgot = new LinkLabel
            {
                Text      = "Forgot password?",
                Font      = UIStyles.FontSubtitle,
                LinkColor = UIStyles.TextSecondary,
                Location  = new Point(0, 310),
                Width     = pnlCard.Width,
                TextAlign = ContentAlignment.MiddleCenter
            };

            pnlCard.Controls.AddRange(new Control[]
            {
                lblTitle, lblSubtitle, sep,
                lblEmail, txtEmail,
                lblPassword, txtPassword,
                btnLogin, lnkForgot
            });

            this.Controls.Add(pnlCard);
        }

        private Label MakeLabel(string text, Point location) => new Label
        {
            Text      = text,
            Font      = UIStyles.FontLabel,
            ForeColor = UIStyles.TextSecondary,
            AutoSize  = true,
            Location  = location
        };

        private void DrawCardBorder(Graphics g, Panel p)
        {
            using var pen = new Pen(UIStyles.Border, 1f);
            g.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string email    = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill in all fields.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dashboard = new DashboardForm(email);
            dashboard.Show();
            this.Hide();
        }
    }
}