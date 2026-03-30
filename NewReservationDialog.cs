using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MPP_Client.Models;

namespace MPP_Client
{
    public class NewReservationDialog : Form
    {
        private readonly Race        _race;
        private readonly List<User>  _users;

        public Reservation Result { get; private set; }

        private TextBox  _txtPassenger;
        private ComboBox _cmbUser;
        private CheckedListBox _seatPicker;
        private Label    _lblAvailable;

        public NewReservationDialog(Race race, List<User> users)
        {
            _race  = race;
            _users = users;
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text            = "New Reservation";
            this.Size            = new Size(500, 580);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.StartPosition   = FormStartPosition.CenterParent;
            this.BackColor       = UIStyles.Background;

            var card = new Panel
            {
                Location = new Point(20, 20),
                Width    = 400,
                Height   = 400,
                Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            UIStyles.ApplyCard(card);
            card.Paint += (s, e) =>
            {
                using var pen = new Pen(UIStyles.Border, 1f);
                e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };

            // Race info banner
            var banner = new Panel
            {
                Location  = new Point(0, 0),
                Width     = card.Width,
                Height    = 52,
                BackColor = Color.FromArgb(238, 238, 255)
            };
            banner.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            var lblRace = new Label
            {
                Text      = $"{_race.Destination}  ·  {_race.Date}  {_race.Time}",
                Font      = UIStyles.FontSubtitle,
                ForeColor = UIStyles.Accent,
                AutoSize  = true,
                Location  = new Point(16, 10)
            };

            int freeCount = _race.AvailableSeats.Count(s => s);
            _lblAvailable = new Label
            {
                Text      = $"{freeCount} seats available",
                Font      = UIStyles.FontSubtitle,
                ForeColor = UIStyles.TextSecondary,
                AutoSize  = true,
                Location  = new Point(16, 30)
            };

            banner.Controls.AddRange(new Control[] { lblRace, _lblAvailable });

            // Passenger name
            var lblPassenger = MakeLabel("PASSENGER NAME", new Point(16, 68));
            _txtPassenger = new TextBox
            {
                PlaceholderText = "Full name on booking",
                Location        = new Point(16, 88),
                Width           = card.Width - 32
            };
            _txtPassenger.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            UIStyles.ApplyTextBox(_txtPassenger);

            // User selector
            var lblUser = MakeLabel("LINKED USER", new Point(16, 128));
            _cmbUser = new ComboBox
            {
                Location     = new Point(16, 148),
                Width        = card.Width - 32,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font         = UIStyles.FontInput,
                BackColor    = UIStyles.Surface,
                ForeColor    = UIStyles.TextPrimary
            };
            _cmbUser.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            foreach (var u in _users)
                _cmbUser.Items.Add(new UserComboItem(u));
            if (_cmbUser.Items.Count > 0) _cmbUser.SelectedIndex = 0;

            // Seat picker
            var lblSeats = MakeLabel("SELECT SEATS", new Point(16, 188));
            _seatPicker = new CheckedListBox
            {
                Location      = new Point(16, 208),
                Width         = card.Width - 32,
                Height        = 130,
                BorderStyle   = BorderStyle.FixedSingle,
                Font          = UIStyles.FontTableCell,
                BackColor     = UIStyles.Surface,
                ForeColor     = UIStyles.TextPrimary,
                CheckOnClick  = true,
                IntegralHeight = false
            };
            _seatPicker.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            for (int i = 0; i < _race.AvailableSeats.Count; i++)
            {
                bool free = _race.AvailableSeats[i];
                _seatPicker.Items.Add($"Seat {i + 1}", false);
                // Disable taken seats
                if (!free)
                {
                    _seatPicker.SetItemCheckState(i, CheckState.Indeterminate);
                }
            }

            card.Controls.AddRange(new Control[]
            {
                banner, lblPassenger, _txtPassenger,
                lblUser, _cmbUser,
                lblSeats, _seatPicker
            });

            // Buttons
            var btnConfirm = new Button
            {
                Text     = "Create Reservation",
                Location = new Point(20, 426),
                Width    = 210,
                Anchor   = AnchorStyles.Bottom | AnchorStyles.Left
            };
            UIStyles.ApplyPrimaryButton(btnConfirm);
            btnConfirm.Click += OnConfirm;

            var btnCancel = new Button
            {
                Text      = "Cancel",
                Location  = new Point(240, 426),
                Width     = 120,
                Height    = 42,
                Anchor    = AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor = UIStyles.Surface,
                ForeColor = UIStyles.TextSecondary,
                FlatStyle = FlatStyle.Flat,
                Font      = UIStyles.FontButton,
                Cursor    = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderColor = UIStyles.Border;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] { card, btnConfirm, btnCancel });
        }

        private void OnConfirm(object sender, EventArgs e)
        {
            string passenger = _txtPassenger.Text.Trim();
            if (string.IsNullOrEmpty(passenger))
            {
                MessageBox.Show("Please enter a passenger name.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedSeats = new List<int>();
            for (int i = 0; i < _seatPicker.Items.Count; i++)
                if (_seatPicker.GetItemCheckState(i) == CheckState.Checked)
                    selectedSeats.Add(i);   // 0-based index matches AvailableSeats list

            if (selectedSeats.Count == 0)
            {
                MessageBox.Show("Please select at least one seat.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int userId = (_cmbUser.SelectedItem is UserComboItem uci) ? uci.User.Id.Value : 0;

            Result = new Reservation(
                userId:  userId,
                raceId:  _race.Id.Value,
                name:    passenger,
                seats:   selectedSeats
            );

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private Label MakeLabel(string text, Point location) => new Label
        {
            Text      = text,
            Font      = UIStyles.FontLabel,
            ForeColor = UIStyles.TextSecondary,
            AutoSize  = true,
            Location  = location
        };

        private class UserComboItem
        {
            public User User { get; }
            public UserComboItem(User user) => User = user;
            public override string ToString() => $"{User.Name} ({User.Email})";
        }
    }
}