namespace HotelBookingManager;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        // Wire the ListView selection event to load inputs
        lvBookings.SelectedIndexChanged += (s, e) => LoadSelectionIntoInputs();
    }

    private void label1_Click(object sender, EventArgs e)
    {

    }

    private void dtIn_ValueChanged(object sender, EventArgs e)
    {

    }

    private void BookRoom(object sender, EventArgs e)
    {

    }

    private void txtGuest_Enter(object? sender, EventArgs e)
    {
        if (txtGuest.ForeColor == SystemColors.GrayText)
        {
            txtGuest.Text = string.Empty;
            txtGuest.ForeColor = SystemColors.WindowText;
        }
    }

    private void txtGuest_Leave(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtGuest.Text))
        {
            txtGuest.Text = "Guest Name";
            txtGuest.ForeColor = SystemColors.GrayText;
        }
    }

    private void txtRoom_Enter(object? sender, EventArgs e)
    {
        if (txtRoom.ForeColor == SystemColors.GrayText)
        {
            txtRoom.Text = string.Empty;
            txtRoom.ForeColor = SystemColors.WindowText;
        }
    }

    private void txtRoom_Leave(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtRoom.Text))
        {
            txtRoom.Text = "Room (e.g. 101)";
            txtRoom.ForeColor = SystemColors.GrayText;
        }
    }
}
