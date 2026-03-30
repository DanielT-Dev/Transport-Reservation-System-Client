using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MPP_Client.Models;
using MPP_Client.Service;
using MPP_Client.Repository;

namespace MPP_Client
{
    public partial class DashboardForm : Form
    {
        private readonly string _userEmail;

        private static RaceDAO _raceDAO = new RaceDAO();
        private static ReservationDAO _reservationDAO = new ReservationDAO();
        private static UserDAO _userDAO = new UserDAO();
        private readonly RaceService        _raceService        = new RaceService(_raceDAO);
        private readonly ReservationService _reservationService = new ReservationService(_reservationDAO, _raceDAO);
        private readonly UserService        _userService        = new UserService(_userDAO);


        private DataGridView _usersGrid;
        private DataGridView _racesGrid;
        private DataGridView _reservationsGrid;

        public DashboardForm(string userEmail)
        {
            _userEmail = userEmail;
            InitializeComponent();
            BuildUI();
            LoadAllData();
        }

        private void BuildUI()
        {
            UIStyles.ApplyForm(this, width: 980, height: 820);
            this.Text            = "MPP Client — Dashboard";
            this.MaximizeBox     = true;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.FormClosed     += (s, e) => Application.Exit();

            // ── Top bar ────────────────────────────────────────────────
            var topBar = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 56,
                BackColor = UIStyles.Surface,
                Padding   = new Padding(24, 0, 24, 0)
            };
            topBar.Paint += (s, e) =>
            {
                using var pen = new Pen(UIStyles.Border, 1f);
                e.Graphics.DrawLine(pen, 0, topBar.Height - 1, topBar.Width, topBar.Height - 1);
            };

            var lblAppName = new Label
            {
                Text      = "MPP Client",
                Font      = new Font("Segoe UI", 13f, FontStyle.Regular),
                ForeColor = UIStyles.TextPrimary,
                AutoSize  = true,
                Location  = new Point(24, 16)
            };

            var lblUser = new Label
            {
                Text      = _userEmail,
                Font      = UIStyles.FontSubtitle,
                ForeColor = UIStyles.TextSecondary,
                AutoSize  = true
            };
            lblUser.Location = new Point(topBar.Width - lblUser.PreferredWidth - 24, 20);
            lblUser.Anchor   = AnchorStyles.Top | AnchorStyles.Right;

            var btnRefresh = new Button
            {
                Text     = "↻ Refresh",
                Location = new Point(topBar.Width - 400, 12),
                Width    = 90,
                Height   = 32,
                Anchor   = AnchorStyles.Top | AnchorStyles.Right
            };
            UIStyles.ApplyPrimaryButton(btnRefresh);
            btnRefresh.Click += (s, e) => LoadAllData();


            var btnManageReservations = new Button
            {
                Text     = "Manage-Reservations",
                Location = new Point(topBar.Width - 620, 12),
                Width    = 170,
                Height   = 32,
                Anchor   = AnchorStyles.Top | AnchorStyles.Right
            };
            UIStyles.ApplyPrimaryButton(btnManageReservations);
            btnManageReservations.BackColor = Color.FromArgb(14, 116, 144); // teal
            btnManageReservations.FlatAppearance.MouseOverBackColor = Color.FromArgb(12, 90, 110);
            btnManageReservations.Click += (s, e) =>
            {
                var form = new ManageReservationsForm();
                form.Show();
            };

            topBar.Controls.AddRange(new Control[] { lblAppName, lblUser, btnManageReservations, btnRefresh });

            // ── Scrollable content area ────────────────────────────────
            var scroll = new Panel
            {
                Dock       = DockStyle.Fill,
                AutoScroll = true,
                Padding    = new Padding(24),
                BackColor  = UIStyles.Background
            };

            // Build 3 table cards
            var usersCard = MakeTableCard(
                "Users",
                new[] { "ID", "Name", "Email" },
                out _usersGrid
            );

            var racesCard = MakeTableCard(
                "Races",
                new[] { "ID", "Destination", "Date", "Time", "Seats" },
                out _racesGrid
            );

            var reservationsCard = MakeTableCard(
                "Reservations",
                new[] { "ID", "Passenger", "User ID", "Race ID", "Seats" },
                out _reservationsGrid
            );

            var cards      = new[] { usersCard, racesCard, reservationsCard };
            int cardHeight = 230;
            int gap        = 20;

            for (int i = 0; i < cards.Length; i++)
            {
                cards[i].Location = new Point(0, i * (cardHeight + gap));
                cards[i].Width    = scroll.ClientSize.Width - scroll.Padding.Horizontal - 4;
                cards[i].Height   = cardHeight;
                cards[i].Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                scroll.Controls.Add(cards[i]);
            }

            // Add in correct z-order (topBar last so it docks on top)
            this.Controls.Add(scroll);
            this.Controls.Add(topBar);
        }

        // ── Data loading ───────────────────────────────────────────────

        private void LoadAllData()
        {
            LoadUsers();
            LoadRaces();
            LoadReservations();
        }

        private void LoadUsers()
        {
            try
            {
                _usersGrid.Rows.Clear();
                var users = _userService.GetAll();

                foreach (var u in users)
                    _usersGrid.Rows.Add(u.Id, u.Name, u.Email);

                UpdateRowCount(_usersGrid, users.Count);
            }
            catch (Exception ex)
            {
                ShowError("Failed to load users", ex);
            }
        }

        private void LoadRaces()
        {
            try
            {
                _racesGrid.Rows.Clear();
                var races = _raceService.GetAll();

                foreach (var r in races)
                {
                    int available = r.AvailableSeats.FindAll(s => s).Count;
                    int total     = r.AvailableSeats.Count;
                    _racesGrid.Rows.Add(r.Id, r.Destination, r.Date, r.Time, $"{available}/{total}");
                }

                UpdateRowCount(_racesGrid, races.Count);
            }
            catch (Exception ex)
            {
                ShowError("Failed to load races", ex);
            }
        }

        private void LoadReservations()
        {
            try
            {
                _reservationsGrid.Rows.Clear();
                var reservations = _reservationService.GetAll();

                foreach (var r in reservations)
                    _reservationsGrid.Rows.Add(
                        r.Id,
                        r.Name,
                        r.UserId,
                        r.RaceId,
                        string.Join(", ", r.Seats)
                    );

                UpdateRowCount(_reservationsGrid, reservations.Count);
            }
            catch (Exception ex)
            {
                ShowError("Failed to load reservations", ex);
            }
        }

        // ── Helpers ────────────────────────────────────────────────────

        private Panel MakeTableCard(string title, string[] columns, out DataGridView gridOut)
        {
            var card = new Panel { Padding = new Padding(0) };
            UIStyles.ApplyCard(card);
            card.Paint += (s, e) =>
            {
                using var pen = new Pen(UIStyles.Border, 1f);
                e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };

            var lblTitle = new Label
            {
                Text      = title,
                Font      = UIStyles.FontSectionHead,
                ForeColor = UIStyles.TextPrimary,
                AutoSize  = true,
                Location  = new Point(20, 16)
            };

            var lblCount = new Label
            {
                Name      = $"lbl_{title}_count",
                Text      = "0 rows",
                Font      = UIStyles.FontSubtitle,
                ForeColor = UIStyles.TextMuted,
                AutoSize  = true,
                Location  = new Point(20, 38)
            };

            var sep = new Panel
            {
                BackColor = UIStyles.Border,
                Height    = 1,
                Location  = new Point(0, 60),
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            var grid = new DataGridView
            {
                Location = new Point(0, 61),
                Anchor   = AnchorStyles.Top | AnchorStyles.Bottom |
                           AnchorStyles.Left | AnchorStyles.Right
            };
            UIStyles.ApplyDataGrid(grid);

            foreach (var col in columns)
                grid.Columns.Add(col.ToLower().Replace(" ", "_"), col);

            card.Resize += (s, e) =>
            {
                sep.Width   = card.Width;
                grid.Width  = card.Width;
                grid.Height = card.Height - 62;
            };

            card.Controls.AddRange(new Control[] { lblTitle, lblCount, sep, grid });

            gridOut = grid;
            return card;
        }

        private void UpdateRowCount(DataGridView grid, int count)
        {
            var card = grid.Parent;
            foreach (Control c in card.Controls)
                if (c is Label lbl && lbl.Name.EndsWith("_count"))
                    lbl.Text = $"{count} {(count == 1 ? "row" : "rows")}";
        }

        private static void ShowError(string message, Exception ex) =>
            MessageBox.Show($"{message}:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}