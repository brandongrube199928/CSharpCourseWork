using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace InventoryManagerDb
{
    public sealed class MainForm : Form
    {
        // Inputs
        private readonly TextBox txtId   = new() { ReadOnly = true, PlaceholderText = "DB Id (auto)" };
        private readonly TextBox txtName = new() { PlaceholderText = "Product name" };
        private readonly TextBox txtCat  = new() { PlaceholderText = "Category" };
        private readonly NumericUpDown numQty = new() { Minimum = 0, Maximum = 1_000_000, ThousandsSeparator = true };
        private readonly NumericUpDown numPrice = new() { Minimum = 0, Maximum = 10_000_000, DecimalPlaces = 2, Increment = 0.50M, ThousandsSeparator = true };

        // Search
        private readonly TextBox txtSearchName = new() { PlaceholderText = "Search name..." };
        private readonly TextBox txtSearchCat  = new() { PlaceholderText = "Search category..." };

        // Buttons
        private readonly Button btnAdd     = new() { Text = "Add" };
        private readonly Button btnUpdate  = new() { Text = "Update" };
        private readonly Button btnDelete  = new() { Text = "Delete" };
        private readonly Button btnRefresh = new() { Text = "Refresh" };
        private readonly Button btnSearch  = new() { Text = "Search" };
        private readonly Button btnClear   = new() { Text = "Clear Filters" };
        private readonly Button btnExit    = new() { Text = "Exit" };

        // Grid + status
        private readonly DataGridView grid = new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells
        };

        private readonly Label lblStatus = new() { Text = "Ready.", AutoEllipsis = true };

        // Backing table
        private DataTable _table = new();

        public MainForm()
        {
            Text = "Inventory Manager (SQLite)";
            MinimumSize = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterScreen;

            // Top inputs layout
            var top = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 110,
                ColumnCount = 6,
                RowCount = 2,
                Padding = new Padding(10)
            };
            for (int i = 0; i < 6; i++) top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66f));
            top.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
            top.RowStyles.Add(new RowStyle(SizeType.Percent, 55));

            top.Controls.Add(new Label { Text = "Id", AutoSize = true }, 0, 0);
            top.Controls.Add(new Label { Text = "Name", AutoSize = true }, 1, 0);
            top.Controls.Add(new Label { Text = "Category", AutoSize = true }, 2, 0);
            top.Controls.Add(new Label { Text = "Quantity", AutoSize = true }, 3, 0);
            top.Controls.Add(new Label { Text = "Price", AutoSize = true }, 4, 0);

            txtId.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtName.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtCat.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numQty.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numPrice.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            top.Controls.Add(txtId,   0, 1);
            top.Controls.Add(txtName, 1, 1);
            top.Controls.Add(txtCat,  2, 1);
            top.Controls.Add(numQty,  3, 1);
            top.Controls.Add(numPrice,4, 1);

            // Buttons row
            var btnRow = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(10, 0, 10, 0) };
            btnRow.Controls.AddRange(new Control[] { btnAdd, btnUpdate, btnDelete, btnRefresh, btnExit });

            // Search row
            var searchRow = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(10, 0, 10, 0) };
            searchRow.Controls.AddRange(new Control[] { txtSearchName, txtSearchCat, btnSearch, btnClear });

            // Grid columns set at runtime after data bind (auto)

            // Status bar
            var status = new Panel { Dock = DockStyle.Bottom, Height = 28, Padding = new Padding(10, 6, 10, 6) };
            lblStatus.Dock = DockStyle.Fill;
            status.Controls.Add(lblStatus);

            // Compose
            Controls.Add(grid);
            Controls.Add(searchRow);
            Controls.Add(btnRow);
            Controls.Add(top);
            Controls.Add(status);

            // Events
            Load += (_, __) => InitializeAndRefresh();
            btnAdd.Click += (_, __) => AddItem();
            btnUpdate.Click += (_, __) => UpdateItem();
            btnDelete.Click += (_, __) => DeleteItem();
            btnRefresh.Click += (_, __) => RefreshGrid();
            btnExit.Click += (_, __) => Close();
            btnSearch.Click += (_, __) => Search();
            btnClear.Click += (_, __) => { txtSearchName.Clear(); txtSearchCat.Clear(); RefreshGrid(); };
            grid.SelectionChanged += (_, __) => GridSelectionToInputs();
            grid.CellDoubleClick += (_, __) => GridSelectionToInputs();
        }

        private void InitializeAndRefresh()
        {
            try
            {
                Db.EnsureCreated();
                RefreshGrid();
                SetStatus("Database ready.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Startup error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Startup error.");
            }
        }

        private void RefreshGrid()
        {
            try
            {
                _table = Db.GetAll();
                grid.DataSource = _table;
                FormatGrid();
                SetStatus($"Loaded {_table.Rows.Count} product(s).");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Refresh failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Refresh failed.");
            }
        }

        private void Search()
        {
            try
            {
                _table = Db.Search(txtSearchName.Text, txtSearchCat.Text);
                grid.DataSource = _table;
                FormatGrid();
                SetStatus($"Search: {_table.Rows.Count} result(s).");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Search failed.");
            }
        }

        private void FormatGrid()
        {
            if (grid.Columns["Price"] != null)
                grid.Columns["Price"].DefaultCellStyle.Format = "C2";
        }

        private void AddItem()
        {
            //Add this method
            try 
            {
                var (name, cat, qty, price) = ReadInputsForWrite();
                Db.Insert(new Product(name, cat, qty, price));
                RefreshGrid();
                ClearInputs();
                SetStatus("Product added.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Add Product", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetStatus("Add failed.");
            }
        }

        private void UpdateItem()
        {
            //Add this method

            try 
            {
                if (!long.TryParse(txtId.Text.Trim(), out var id) || id <=0)
                    throw new ArgumentException("Select a product row first");

                var (name, cat, qty, price) = ReadInputsForWrite();
                Db.Update(new Product(name, cat, qty, price));
                RefreshGrid();
                SetStatus("Product updated.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Update Product", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetStatus("Update failed.");
            }
        }

        private void DeleteItem()
        {
            //Add this method
            try
            {
                if (!long.TryParse(txtId.Text.Trim(), out var id) || id <= 0)
                {
                    MessageBox.Show("Select a product row first.", "Delete Product", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var answer = MessageBox.Show($"Delete Product ID{id},", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (answer != DialogResult.Yes) return;

                Db.Delete(id);
                RefreshGrid();
                ClearInputs();
                SetStatus("Product deleted.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Delete Product", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetStatus("Delete failed.");
            }
        }

        private void GridSelectionToInputs()
        {
            if (grid.CurrentRow?.DataBoundItem is DataRowView drv)
            {
                txtId.Text = drv.Row.Field<long>("Id").ToString();
                txtName.Text = drv.Row.Field<string>("Name");
                txtCat.Text = drv.Row.Field<string>("Category");
                numQty.Value = drv.Row.Field<long>("Quantity"); // SQLite int maps to Int64
                numPrice.Value = Convert.ToDecimal(drv.Row.Field<double>("Price"), CultureInfo.InvariantCulture);
            }
        }

        private (string name, string cat, int qty, double price) ReadInputsForWrite()
        {
            var name = txtName.Text.Trim();
            var cat = txtCat.Text.Trim();
            var qty = (int)numQty.Value;
            var price = (double)numPrice.Value;

            // Product constructor re-validates, but fast checks help UX
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
            if (string.IsNullOrWhiteSpace(cat)) throw new ArgumentException("Category is required.");
            if (qty < 0) throw new ArgumentException("Quantity must be >= 0.");
            if (price < 0) throw new ArgumentException("Price must be >= 0.");

            return (name, cat, qty, price);
        }

        private void ClearInputs()
        {
            txtId.Clear();
            txtName.Clear();
            txtCat.Clear();
            numQty.Value = 0;
            numPrice.Value = 0;
            txtName.Focus();
        }

        private void SetStatus(string msg) => lblStatus.Text = msg;
    }
}
