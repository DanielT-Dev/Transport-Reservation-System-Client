using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MPP_Client.Models;
using MPP_Client.Repository;
using MPP_Client.Service;

namespace MPP_Client
{
    public partial class ManageReservationsForm : Form
    {

        private static RaceDAO _raceDAO = new RaceDAO();
        private static ReservationDAO _reservationDAO = new ReservationDAO();
        private static UserDAO _userDAO = new UserDAO();
        private readonly RaceService        _raceService        = new RaceService(_raceDAO);
        private readonly ReservationService _reservationService = new ReservationService(_reservationDAO, _raceDAO);
        private readonly UserService        _userService        = new UserService(_userDAO);

        private Race         _selectedRace;
        private DataGridView _reservationsGrid;
        private Label        _lblReservationCount;
        private Label        _lblRaceInfo;
        private TextBox      _txtSearch;
        private ListBox      _searchResults;
        private Button       _btnDelete;

        public ManageReservationsForm()
        {
            InitializeComponent();
            BuildUI();
        }

        private void BuildUI()
        {
            UIStyles.ApplyForm(this, width: 960, height: 720);
            this.Text            = "MPP Client — Manage Reservations";
            this.MaximizeBox     = true;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.BackColor       = UIStyles.Background;

            // ── Top bar ────────────────────────────────────────────────
            var topBar = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 56,
                BackColor = UIStyles.Surface
            };
            topBar.Paint += (s, e) =>
            {
                using var pen = new Pen(UIStyles.Border, 1f);
                e.Graphics.DrawLine(pen, 0, topBar.Height - 1, topBar.Width, topBar.Height - 1);
            };

            var lblTitle = new Label
            {
                Text      = "Manage Reservations",
                Font      = new Font("Segoe UI", 13f),
                ForeColor = UIStyles.TextPrimary,
                AutoSize  = true,
                Location  = new Point(24, 16)
            };

            var btnBack = new Button
            {
                Text   = "← Back",
                Width  = 80,
                Height = 32,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnBack.Location = new Point(this.ClientSize.Width - 104, 12);
            UIStyles.ApplyPrimaryButton(btnBack);
            btnBack.BackColor = UIStyles.Primary;
            btnBack.Click    += (s, e) => this.Close();

            topBar.Controls.AddRange(new Control[] { lblTitle, btnBack });

            // ── Content area ───────────────────────────────────────────
            var content = new Panel
            {
                Dock    = DockStyle.Fill,
                Padding = new Padding(24),
                BackColor = UIStyles.Background
            };

            // ── LEFT: search panel ─────────────────────────────────────
            var searchPanel = new Panel
            {
                Width     = 260,
                Dock      = DockStyle.Left,
                BackColor = UIStyles.Background,
                Padding   = new Padding(0, 0, 16, 0)
            };

            var searchCard = new Panel { Dock = DockStyle.Fill };
            UIStyles.ApplyCard(searchCard);
            searchCard.Paint += (s, e) =>
            {
                using var pen = new Pen(UIStyles.Border, 1f);
                e.Graphics.DrawRectangle(pen, 0, 0, searchCard.Width - 1, searchCard.Height - 1);
            };

            var lblSearchHeading = new Label
            {
                Text      = "SEARCH RACE",
                Font      = UIStyles.FontLabel,
                ForeColor = UIStyles.TextSecondary,
                AutoSize  = true,
                Location  = new Point(16, 16)
            };

            _txtSearch = new TextBox
            {
                PlaceholderText = "Filter by destination...",
                Location        = new Point(16, 36),
                Width           = 200
            };
            _txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            UIStyles.ApplyTextBox(_txtSearch);
            _txtSearch.TextChanged += (s, e) => PopulateSearchResults(_txtSearch.Text.Trim());

            var lblResultsHeading = new Label
            {
                Text      = "RESULTS",
                Font      = UIStyles.FontLabel,
                ForeColor = UIStyles.TextSecondary,
                AutoSize  = true,
                Location  = new Point(16, 78)
            };

            _searchResults = new ListBox
            {
                Location       = new Point(16, 96),
                Width          = 200,
                Height         = 260,
                BorderStyle    = BorderStyle.FixedSingle,
                Font           = UIStyles.FontTableCell,
                BackColor      = UIStyles.Surface,
                ForeColor      = UIStyles.TextPrimary,
                IntegralHeight = false
            };
            _searchResults.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _searchResults.SelectedIndexChanged += OnRaceSelected;

            // Selected race info box
            var infoBox = new Panel
            {
                Location  = new Point(16, 376),
                Width     = 200,
                Height    = 120,
                BackColor = Color.FromArgb(238, 238, 255)
            };
            infoBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            infoBox.Paint += (s, e) =>
            {
                using var pen = new Pen(UIStyles.Accent, 1f);
                e.Graphics.DrawRectangle(pen, 0, 0, infoBox.Width - 1, infoBox.Height - 1);
            };

            _lblRaceInfo = new Label
            {
                Text      = "No race selected",
                Font      = UIStyles.FontSubtitle,
                ForeColor = UIStyles.TextSecondary,
                Location  = new Point(10, 8),
                Width     = 180,
                Height    = 120,
                AutoSize  = false
            };
            infoBox.Controls.Add(_lblRaceInfo);

            searchCard.Controls.AddRange(new Control[]
            {
                lblSearchHeading, _txtSearch,
                lblResultsHeading, _searchResults,
                infoBox
            });
            searchPanel.Controls.Add(searchCard);

            // ── RIGHT: reservations table ──────────────────────────────
            var rightPanel = new Panel
            {
                Dock    = DockStyle.Fill,
                Padding = new Padding(16, 0, 0, 0)
            };

            var tableCard = new Panel { Dock = DockStyle.Fill };
            UIStyles.ApplyCard(tableCard);
            tableCard.Paint += (s, e) =>
            {
                using var pen = new Pen(UIStyles.Border, 1f);
                e.Graphics.DrawRectangle(pen, 0, 0, tableCard.Width - 1, tableCard.Height - 1);
            };

            // Table header
            var tableHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 64,
                BackColor = UIStyles.Surface
            };
            tableHeader.Paint += (s, e) =>
            {
                using var pen = new Pen(UIStyles.Border, 1f);
                e.Graphics.DrawLine(pen, 0, tableHeader.Height - 1, tableHeader.Width, tableHeader.Height - 1);
            };

            var lblTableTitle = new Label
            {
                Text      = "Reservations",
                Font      = UIStyles.FontSectionHead,
                ForeColor = UIStyles.TextPrimary,
                AutoSize  = true,
                Location  = new Point(20, 10)
            };

            _lblReservationCount = new Label
            {
                Text      = "Select a race to view reservations",
                Font      = UIStyles.FontSubtitle,
                ForeColor = UIStyles.TextMuted,
                AutoSize  = true,
                Location  = new Point(20, 36)
            };

            var btnNew = new Button
            {
                Text   = "+ New Reservation",
                Width  = 150,
                Height = 32,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            UIStyles.ApplyPrimaryButton(btnNew);
            btnNew.Click += OnNewReservationClick;

            _btnDelete = new Button
            {
                Text     = "Delete",
                Width    = 80,
                Height   = 32,
                Anchor   = AnchorStyles.Top | AnchorStyles.Right,
                Enabled  = false
            };
            UIStyles.ApplyPrimaryButton(_btnDelete);
            _btnDelete.BackColor = UIStyles.Danger;
            _btnDelete.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 35, 51);
            _btnDelete.Click += OnDeleteReservationClick;

            // Position buttons on resize
            tableHeader.Resize += (s, e) =>
            {
                _btnDelete.Location = new Point(tableHeader.Width - 100, 16);
                btnNew.Location     = new Point(tableHeader.Width - 260, 16);
            };

            tableHeader.Controls.AddRange(new Control[]
            {
                lblTableTitle, _lblReservationCount, btnNew, _btnDelete
            });

            // Grid
            _reservationsGrid = new DataGridView { Dock = DockStyle.Fill };
            UIStyles.ApplyDataGrid(_reservationsGrid);
            _reservationsGrid.Columns.Add("id",        "ID");
            _reservationsGrid.Columns.Add("passenger", "Passenger");
            _reservationsGrid.Columns.Add("seats",     "Seats");
            _reservationsGrid.Columns.Add("user_id",   "User ID");
            _reservationsGrid.SelectionChanged += (s, e) =>
                _btnDelete.Enabled = _selectedRace != null && _reservationsGrid.SelectedRows.Count > 0;

            tableCard.Controls.Add(_reservationsGrid);
            tableCard.Controls.Add(tableHeader);

            rightPanel.Controls.Add(tableCard);

            content.Controls.Add(rightPanel);
            content.Controls.Add(searchPanel);

            this.Controls.Add(content);
            this.Controls.Add(topBar);

            PopulateSearchResults("");
        }

        // ── Search ─────────────────────────────────────────────────────

        private void PopulateSearchResults(string filter)
        {
            try
            {
                _searchResults.Items.Clear();
                var races = _raceService.GetAll()
                    .Where(r => string.IsNullOrEmpty(filter) ||
                                r.Destination.Contains(filter, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (var race in races)
                    _searchResults.Items.Add(new RaceListItem(race));

                if (_searchResults.Items.Count == 0)
                    _searchResults.Items.Add("No races found");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load races:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnRaceSelected(object sender, EventArgs e)
        {
            if (_searchResults.SelectedItem is not RaceListItem item) return;

            _selectedRace = item.Race;

            int available = _selectedRace.AvailableSeats.Count(s => s);
            int total     = _selectedRace.AvailableSeats.Count;

            _lblRaceInfo.Text =
                $"ID: {_selectedRace.Id}\n" +
                $"{_selectedRace.Destination}\n" +
                $"{_selectedRace.Date} {_selectedRace.Time}\n" +
                $"Seats free: {available}/{total}";

            LoadReservations();
        }

        // ── Load table ─────────────────────────────────────────────────

        private void LoadReservations()
        {
            if (_selectedRace == null) return;

            try
            {
                _reservationsGrid.Rows.Clear();
                var reservations = _reservationService.GetByRace(_selectedRace.Id.Value);

                foreach (var r in reservations)
                    _reservationsGrid.Rows.Add(r.Id, r.Name, string.Join(", ", r.Seats), r.UserId);

                int count = reservations.Count;
                _lblReservationCount.Text =
                    $"{count} {(count == 1 ? "reservation" : "reservations")} — {_selectedRace.Destination}";

                _btnDelete.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load reservations:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── New Reservation ────────────────────────────────────────────

        private void OnNewReservationClick(object sender, EventArgs e)
        {
            if (_selectedRace == null)
            {
                MessageBox.Show("Please select a race first.", "No race selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var users = _userService.GetAll();
            using var dialog = new NewReservationDialog(_selectedRace, users);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _reservationService.Add(dialog.Result);
                    _selectedRace = _raceService.GetById(_selectedRace.Id.Value);
                    LoadReservations();
                    PopulateSearchResults(_txtSearch.Text.Trim());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to create reservation:\n{ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ── Delete Reservation ─────────────────────────────────────────

        private void OnDeleteReservationClick(object sender, EventArgs e)
        {
            if (_reservationsGrid.SelectedRows.Count == 0) return;

            var row     = _reservationsGrid.SelectedRows[0];
            int id      = Convert.ToInt32(row.Cells["id"].Value);
            string name = row.Cells["passenger"].Value?.ToString();

            var confirm = MessageBox.Show(
                $"Delete reservation for \"{name}\"?",
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm != DialogResult.Yes) return;

            try
            {
                _reservationService.Delete(id);
                _selectedRace = _raceService.GetById(_selectedRace.Id.Value);
                LoadReservations();
                PopulateSearchResults(_txtSearch.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Helper wrapper for ListBox items ───────────────────────────
        private class RaceListItem
        {
            public Race Race { get; }
            public RaceListItem(Race race) => Race = race;
            public override string ToString() =>
                $"{Race.Destination}  {Race.Date} {Race.Time}";
        }
    }
}