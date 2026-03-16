using System.Text.RegularExpressions;

namespace HotelBookingManager;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    // Declare a BookingManager Object
    private readonly BookingManager manager = new();

    // Event Sender Method
    private void btnBook_Click(object sender, EventArgs e) => BookRoom();
    private void btnCancel_Click(object sender, EventArgs e) => CancelBooking();
    private void btnView_Click(object sender, EventArgs e) => RefreshList();
    private void btnExit_Click(object sender, EventArgs e) => Close();

    //void to book a hotel room
    private void BookRoom()
    {
        try
        {
            var room = txtRoom.Text.Trim();
            var guest = txtGuest.Text.Trim();
            var c_in = dtIn.Value;
            var c_out = dtOut.Value;

            //Validate that guest and room are not empty
            // Treat placeholder values as empty
            if (string.Equals(room, "Room (e.g. 101)", StringComparison.OrdinalIgnoreCase)) room = string.Empty;
            if (string.Equals(guest, "Guest Name", StringComparison.OrdinalIgnoreCase)) guest = string.Empty;

            if (string.IsNullOrEmpty(room) || string.IsNullOrEmpty(guest))
                throw new ArgumentException("Guest and Room are required.");

            // Validate guest name (letters, spaces, common punctuation)
            if (!Regex.IsMatch(guest, "^[A-Za-z .'-]{2,50}$"))
                throw new ArgumentException("Guest name must be 2-50 characters and contain only letters, spaces, apostrophes or hyphens.");

            // Validate room number (e.g. 101 or 101A)
            if (!Regex.IsMatch(room, "^\\d{1,4}[A-Za-z]?$"))
                throw new ArgumentException("Room number must be 1-4 digits optionally followed by a letter (e.g., 101 or 101A).");
            // Validate that check-out is after check-in
            if (c_out <= c_in)
                throw new ArgumentException("Check-out must be after Check-in.");

            //Create a booking and add it
            var b = new Booking(room, guest, c_in, c_out);
            // Persist the booking in the manager so it will appear in the list
            manager.Add(b);

            // Call helpers
            RefreshList();
            ClearInputs();
            SetStatus($"Room {room} booked for {guest}.", success: true);
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            SetStatus(ex.Message, success: false);
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(ex.Message, "Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            SetStatus(ex.Message, success: false);
        }
        catch (Exception ex)
        {
            // Show full exception details to aid debugging; keep a friendly caption
            MessageBox.Show($"Unexpected error. See details:\n{ex}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            SetStatus("Unexpected error occurred.", success: false);
        }
    }

    //void to Cancel a Booking
    private void CancelBooking()
    {
        // Get information from the text boxes
        var room = txtRoom.Text.Trim();
        var guest = txtGuest.Text.Trim();

        // Reject empty textboxes
        if (string.IsNullOrWhiteSpace(room) || string.IsNullOrWhiteSpace(guest))
        {
            MessageBox.Show("Enter both Room and Guest to cancel.", "Cancel Booking",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // Attempt to cancel the reservation
        var ok = manager.Cancel(room, guest);
        if (ok)
        {
            RefreshList();
            ClearInputs();
            SetStatus($"Canceled booking for {guest} in room {room}.", success: true);
        }
        else 
        {
            MessageBox.Show("No Matching booking found.", "Cancel Booking",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            SetStatus("No matching booking found to cancel.", success: false);
        }
    }

    // method to refresh the Bookings List View
    private void RefreshList()
    {
        // Clear and reload the ListView with current bookings
        lvBookings.BeginUpdate();
        lvBookings.Items.Clear();

        foreach (var b in manager.All())
        {
            var item = new ListViewItem(new[]
                {
                    b.RoomNumber,
                    b.CheckIn.ToString("yyyy-MM-dd HH:mm"),
                    b.CheckOut.ToString("yyyy-MM-dd HH:mm"),
                    b.GuestName
                });
            lvBookings.Items.Add(item);
        }

        lvBookings.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        lvBookings.EndUpdate();
        SetStatus($"Loaded {lvBookings.Items.Count} booking(s).", success: true);
    }

    // void to load selected booking into inputs
    private void LoadSelectionIntoInputs()
    {
        if (lvBookings.SelectedItems.Count == 0) return;
        var sel = lvBookings.SelectedItems[0];
        txtRoom.Text = sel.SubItems[0].Text;
        dtIn.Value = DateTime.Parse(sel.SubItems[1].Text);
        dtOut.Value = DateTime.Parse(sel.SubItems[2].Text);
        txtGuest.Text = sel.SubItems[3].Text;
        SetStatus("Loaded selection into inputs.", success: true);
    }

    //void to update the Status Label
    private void SetStatus(string message, bool success)
    {
        lblStatus.Text = message;
        lblStatus.ForeColor = success ? Color.FromArgb(24, 128, 26) : Color.FromArgb(176, 32, 32);
    }

    // void to clear all inputs
    private void ClearInputs()
    {
        txtGuest.Clear();
        txtRoom.Clear();
        txtGuest.Focus();
    }

    /// <summary>
    ///  Clean up any resources being used.
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

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        lblWelcomeLabel = new Label();
        lblGuestName = new Label();
        lblRoomNumber = new Label();
        lblCheckIn = new Label();
        lblCheckOut = new Label();
        txtGuest = new TextBox();
        txtRoom = new TextBox();
        dtIn = new DateTimePicker();
        dtOut = new DateTimePicker();
        btnBook = new Button();
        btnCancel = new Button();
        btnView = new Button();
        btnExit = new Button();
        lblStatus = new Label();
        lvBookings = new ListView();
        SuspendLayout();
        // 
        // lblWelcomeLabel
        // 
        lblWelcomeLabel.AutoSize = true;
        lblWelcomeLabel.Font = new Font("Perpetua Titling MT", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        lblWelcomeLabel.ForeColor = SystemColors.Control;
        lblWelcomeLabel.Location = new Point(30, 9);
        lblWelcomeLabel.Name = "lblWelcomeLabel";
        lblWelcomeLabel.Size = new Size(675, 42);
        lblWelcomeLabel.TabIndex = 0;
        lblWelcomeLabel.Text = "Welcome to the Wrenfield Hotel";
        lblWelcomeLabel.TextAlign = ContentAlignment.MiddleCenter;
        lblWelcomeLabel.Click += label1_Click;
        // 
        // lblGuestName
        // 
        lblGuestName.AutoSize = true;
        lblGuestName.ForeColor = SystemColors.ControlLight;
        lblGuestName.ImageAlign = ContentAlignment.TopRight;
        lblGuestName.Location = new Point(30, 74);
        lblGuestName.Name = "lblGuestName";
        lblGuestName.Size = new Size(75, 15);
        lblGuestName.TabIndex = 1;
        lblGuestName.Text = "Guest Name;";
        // 
        // lblRoomNumber
        // 
        lblRoomNumber.AutoSize = true;
        lblRoomNumber.ForeColor = SystemColors.ButtonFace;
        lblRoomNumber.Location = new Point(413, 74);
        lblRoomNumber.Name = "lblRoomNumber";
        lblRoomNumber.Size = new Size(89, 15);
        lblRoomNumber.TabIndex = 2;
        lblRoomNumber.Text = "Room Number;";
        lblRoomNumber.TextAlign = ContentAlignment.TopRight;
        // 
        // lblCheckIn
        // 
        lblCheckIn.AutoSize = true;
        lblCheckIn.ForeColor = SystemColors.ButtonFace;
        lblCheckIn.Location = new Point(63, 147);
        lblCheckIn.Name = "lblCheckIn";
        lblCheckIn.Size = new Size(58, 15);
        lblCheckIn.TabIndex = 3;
        lblCheckIn.Text = "Check-In;";
        lblCheckIn.TextAlign = ContentAlignment.TopRight;
        // 
        // lblCheckOut
        // 
        lblCheckOut.AutoSize = true;
        lblCheckOut.ForeColor = SystemColors.ButtonFace;
        lblCheckOut.Location = new Point(460, 147);
        lblCheckOut.Name = "lblCheckOut";
        lblCheckOut.Size = new Size(68, 15);
        lblCheckOut.TabIndex = 4;
        lblCheckOut.Text = "Check-Out;";
        lblCheckOut.TextAlign = ContentAlignment.TopRight;
        // 
        // txtGuest
        // 
        txtGuest.Location = new Point(111, 74);
        txtGuest.Name = "txtGuest";
        txtGuest.Size = new Size(276, 23);
        txtGuest.TabIndex = 5;
        txtGuest.Text = "Guest Name";
        txtGuest.ForeColor = SystemColors.GrayText;
        txtGuest.Enter += txtGuest_Enter;
        txtGuest.Leave += txtGuest_Leave;
        // 
        // txtRoom
        // 
        txtRoom.Location = new Point(508, 74);
        txtRoom.Name = "txtRoom";
        txtRoom.Size = new Size(98, 23);
        txtRoom.TabIndex = 6;
        txtRoom.Text = "Room (e.g. 101)";
        txtRoom.ForeColor = SystemColors.GrayText;
        txtRoom.Enter += txtRoom_Enter;
        txtRoom.Leave += txtRoom_Leave;
        // 
        // dtIn
        // 
        dtIn.Location = new Point(127, 141);
        dtIn.Name = "dtIn";
        dtIn.Size = new Size(260, 23);
        dtIn.TabIndex = 7;
        dtIn.Value = new DateTime(2026, 3, 2, 0, 0, 0, 0);
        dtIn.ValueChanged += dtIn_ValueChanged;
        // 
        // dtOut
        // 
        dtOut.Location = new Point(534, 139);
        dtOut.Name = "dtOut";
        dtOut.Size = new Size(240, 23);
        dtOut.TabIndex = 8;
        dtOut.Value = new DateTime(2026, 3, 2, 0, 0, 0, 0);
        // 
        // btnBook
        // 
        btnBook.Location = new Point(127, 201);
        btnBook.Name = "btnBook";
        btnBook.Size = new Size(79, 23);
        btnBook.TabIndex = 9;
        btnBook.Text = "Book Room";
        btnBook.UseVisualStyleBackColor = true;
        btnBook.Click += btnBook_Click;
        // 
        // btnCancel
        // 
        btnCancel.Location = new Point(221, 201);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(102, 23);
        btnCancel.TabIndex = 10;
        btnCancel.Text = "Cancel Booking";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += btnCancel_Click;
        // 
        // btnView
        // 
        btnView.Location = new Point(383, 201);
        btnView.Name = "btnView";
        btnView.Size = new Size(130, 23);
        btnView.TabIndex = 11;
        btnView.Text = "View All Bookings";
        btnView.UseVisualStyleBackColor = true;
        btnView.Click += btnView_Click;
        // 
        // btnExit
        // 
        btnExit.Location = new Point(596, 201);
        btnExit.Name = "btnExit";
        btnExit.Size = new Size(75, 23);
        btnExit.TabIndex = 12;
        btnExit.Text = "Exit";
        btnExit.UseVisualStyleBackColor = true;
        btnExit.Click += btnExit_Click;
        // 
        // lblStatus
        // 
        lblStatus.AutoSize = true;
        lblStatus.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        lblStatus.ForeColor = SystemColors.ButtonHighlight;
        lblStatus.Location = new Point(12, 420);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(56, 21);
        lblStatus.TabIndex = 13;
        lblStatus.Text = "Ready.";
        // 
        // lvBookings
        // 
        lvBookings.Location = new Point(63, 241);
        lvBookings.Name = "lvBookings";
        lvBookings.Size = new Size(711, 168);
        lvBookings.TabIndex = 14;
        lvBookings.UseCompatibleStateImageBehavior = false;
        // Show items in columns so subitems are visible
        lvBookings.View = View.Details;
        lvBookings.FullRowSelect = true;
        lvBookings.GridLines = true;
        lvBookings.Columns.Add("Room", 100, HorizontalAlignment.Left);
        lvBookings.Columns.Add("Check-In", 200, HorizontalAlignment.Left);
        lvBookings.Columns.Add("Check-Out", 200, HorizontalAlignment.Left);
        lvBookings.Columns.Add("Guest", 200, HorizontalAlignment.Left);
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.Black;
        ClientSize = new Size(800, 450);
        Controls.Add(lvBookings);
        Controls.Add(lblStatus);
        Controls.Add(btnExit);
        Controls.Add(btnView);
        Controls.Add(btnCancel);
        Controls.Add(btnBook);
        Controls.Add(dtOut);
        Controls.Add(dtIn);
        Controls.Add(txtRoom);
        Controls.Add(txtGuest);
        Controls.Add(lblCheckOut);
        Controls.Add(lblCheckIn);
        Controls.Add(lblRoomNumber);
        Controls.Add(lblGuestName);
        Controls.Add(lblWelcomeLabel);
        Name = "Form1";
        Text = "Hotel Booking Manager";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblWelcomeLabel;
    private Label lblGuestName;
    private Label lblRoomNumber;
    private Label lblCheckIn;
    private Label lblCheckOut;
    private TextBox txtGuest;
    private TextBox txtRoom;
    private DateTimePicker dtIn;
    private DateTimePicker dtOut;
    private Button btnBook;
    private Button btnCancel;
    private Button btnView;
    private Button btnExit;
    private Label lblStatus;
    private ListView lvBookings;
}
