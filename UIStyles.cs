using System.Drawing;
using System.Windows.Forms;

namespace MPP_Client
{
    public static class UIStyles
    {
        // Colors
        public static readonly Color Background     = Color.FromArgb(245, 245, 247);
        public static readonly Color Surface        = Color.White;
        public static readonly Color Primary        = Color.FromArgb(30, 30, 30);
        public static readonly Color Accent         = Color.FromArgb(99, 102, 241);   // indigo
        public static readonly Color AccentHover    = Color.FromArgb(79, 82, 221);
        public static readonly Color TextPrimary    = Color.FromArgb(15, 15, 20);
        public static readonly Color TextSecondary  = Color.FromArgb(120, 120, 130);
        public static readonly Color TextMuted      = Color.FromArgb(170, 170, 180);
        public static readonly Color Border         = Color.FromArgb(220, 220, 228);
        public static readonly Color TableHeader    = Color.FromArgb(248, 248, 252);
        public static readonly Color TableRowAlt    = Color.FromArgb(252, 252, 255);
        public static readonly Color Danger         = Color.FromArgb(220, 53, 69);

        // Fonts
        public static readonly Font FontTitle       = new Font("Segoe UI", 20f, FontStyle.Regular);
        public static readonly Font FontSubtitle    = new Font("Segoe UI", 10f, FontStyle.Regular);
        public static readonly Font FontLabel       = new Font("Segoe UI", 8f,  FontStyle.Bold);
        public static readonly Font FontInput       = new Font("Segoe UI", 10f, FontStyle.Regular);
        public static readonly Font FontButton      = new Font("Segoe UI", 10f, FontStyle.Regular);
        public static readonly Font FontTableHeader = new Font("Segoe UI", 9f,  FontStyle.Bold);
        public static readonly Font FontTableCell   = new Font("Segoe UI", 9.5f,FontStyle.Regular);
        public static readonly Font FontSectionHead = new Font("Segoe UI", 11f, FontStyle.Regular);

        // Apply styles to common controls
        public static void ApplyForm(Form form, int width = 460, int height = 520)
        {
            form.BackColor        = Background;
            form.Font             = FontInput;
            form.Size             = new Size(width, height);
            form.FormBorderStyle  = FormBorderStyle.FixedSingle;
            form.MaximizeBox      = false;
            form.StartPosition    = FormStartPosition.CenterScreen;
        }

        public static void ApplyPrimaryButton(Button btn)
        {
            btn.BackColor                    = Accent;
            btn.ForeColor                    = Color.White;
            btn.FlatStyle                    = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize    = 0;
            btn.FlatAppearance.MouseOverBackColor = AccentHover;
            btn.Font                         = FontButton;
            btn.Cursor                       = Cursors.Hand;
            btn.Height                       = 42;
        }

        public static void ApplyTextBox(TextBox txt)
        {
            txt.BackColor    = Surface;
            txt.ForeColor    = TextPrimary;
            txt.BorderStyle  = BorderStyle.FixedSingle;
            txt.Font         = FontInput;
            txt.Height       = 32;
        }

        public static void ApplyCard(Panel panel)
        {
            panel.BackColor  = Surface;
            panel.Padding    = new Padding(20);
        }

        public static void ApplyDataGrid(DataGridView grid, string title = "")
        {
            grid.BackgroundColor              = Surface;
            grid.BorderStyle                  = BorderStyle.None;
            grid.CellBorderStyle              = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor                    = Border;
            grid.RowHeadersVisible            = false;
            grid.AllowUserToAddRows           = false;
            grid.AllowUserToDeleteRows        = false;
            grid.AllowUserToResizeRows        = false;
            grid.ReadOnly                     = true;
            grid.SelectionMode               = DataGridViewSelectionMode.FullRowSelect;
            grid.Font                         = FontTableCell;
            grid.AutoSizeColumnsMode          = DataGridViewAutoSizeColumnsMode.Fill;

            // Header style
            grid.ColumnHeadersDefaultCellStyle.BackColor   = TableHeader;
            grid.ColumnHeadersDefaultCellStyle.ForeColor   = TextPrimary;
            grid.ColumnHeadersDefaultCellStyle.Font        = FontTableHeader;
            grid.ColumnHeadersDefaultCellStyle.Padding     = new Padding(8, 0, 0, 0);
            grid.ColumnHeadersBorderStyle                  = DataGridViewHeaderBorderStyle.Single;
            grid.ColumnHeadersHeightSizeMode               = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight                       = 36;

            // Row style
            grid.DefaultCellStyle.BackColor                = Surface;
            grid.DefaultCellStyle.ForeColor                = TextPrimary;
            grid.DefaultCellStyle.SelectionBackColor       = Color.FromArgb(238, 238, 255);
            grid.DefaultCellStyle.SelectionForeColor       = TextPrimary;
            grid.DefaultCellStyle.Padding                  = new Padding(8, 0, 0, 0);
            grid.RowTemplate.Height                        = 34;

            // Alternating row color
            grid.AlternatingRowsDefaultCellStyle.BackColor = TableRowAlt;
        }
    }
}