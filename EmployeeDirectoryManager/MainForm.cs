using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDirectoryManager
{
    internal class MainForm : Form //Makes our class extend the Form class
    {
        //Form Controls
        //Inputs
        private readonly TextBox txtId = new() { PlaceholderText = "E0001" };
        private readonly TextBox txtName = new() { PlaceholderText = "Full Name" };
        private readonly TextBox txtDept = new() { PlaceholderText = "Department" };
        private readonly TextBox txtRole = new() { PlaceholderText = "Role" };
        private readonly TextBox txtSalary = new() { PlaceholderText = "Salary (e.g 55000" };
        private readonly DateTimePicker dtHire = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd" };

        //Buttons
        private readonly Button btnAdd = new() { Text = "Add" };
        private readonly Button btnUpdate = new() { Text = "Update" };
        private readonly Button btnDelete = new() { Text = "Delete" };
        private readonly Button btnSave = new() { Text = "Save" };
        private readonly Button btnLoad = new() { Text = "Load" };
        private readonly Button btnExit = new() { Text = "Exit" };


        //Grid & Status
        private readonly DataGridView grid = new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = true,
            AutoGenerateColumns = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };
        private readonly Label lblStatus = new() { Text = "Status: Ready", AutoEllipsis = true };


        //Employee Manager
        private readonly EmployeeManager manager = new();

        //Constructor
        public MainForm()
        {
            //Form Settings
            Text = "Employee Directory Manager";
            MinimumSize = new Size(960, 560);
            StartPosition = FormStartPosition.CenterScreen;

            //Top Layout
            var top = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 120,
                ColumnCount = 6,
                RowCount = 2,
                Padding = new Padding(10)
            };
            for (int i = 0; i < 6; i++) top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.6f));
            top.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
            top.RowStyles.Add(new RowStyle(SizeType.Percent, 55));

            //Row Labels
            //Row 0 Labels
            top.Controls.Add(new Label() { Text = "Employee ID", AutoSize = true }, 0, 0);
            top.Controls.Add(new Label() { Text = "Full Name", AutoSize = true }, 1, 0);
            top.Controls.Add(new Label() { Text = "Department", AutoSize = true }, 2, 0);
            top.Controls.Add(new Label() { Text = "Role", AutoSize = true }, 3, 0);
            top.Controls.Add(new Label() { Text = "Salary", AutoSize = true }, 4, 0);
            top.Controls.Add(new Label() { Text = "Hire Date", AutoSize = true }, 5, 0);

            //Row 1 Labels
            txtId.Anchor = txtName.Anchor = txtDept.Anchor = txtRole.Anchor = txtSalary.Anchor = dtHire.Anchor =
                AnchorStyles.Left | AnchorStyles.Right;
            top.Controls.Add(txtId, 0, 1);
            top.Controls.Add(txtName, 1, 1);
            top.Controls.Add(txtDept, 2, 1);
            top.Controls.Add(txtRole, 3, 1);
            top.Controls.Add(txtSalary, 4, 1);
            top.Controls.Add(dtHire, 5, 1);

            //Button row
            var btnRow = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(10, 0, 10, 0) };
            btnRow.Controls.AddRange(new Control[] { btnAdd, btnUpdate, btnDelete, btnSave, btnLoad, btnExit });

            //Grid
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Employee.Id), HeaderText = "ID", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Employee.FullName),
                HeaderText = "Name",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Employee.Department), HeaderText = "Dept", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Employee.Role), HeaderText = "Role", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Employee.Salary),
                HeaderText = "Salary",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C0" }
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(Employee.HireDate),
                HeaderText = "Hire Date",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd" }
            });

            //Bind the grid
            grid.DataSource = manager.Employees;

            //Status Bar
            var status = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 28,
                Padding = new Padding(10, 6, 10, 6)
            };
            lblStatus.Dock = DockStyle.Fill;
            status.Controls.Add(lblStatus);

            //Compose the Form
            Controls.Add(grid);
            Controls.Add(btnRow);
            Controls.Add(top);
            Controls.Add(status);

            //Default Values
            dtHire.Value = DateTime.Today;

            //Events
            btnAdd.Click += (_, __) => AddEmployees();
            btnUpdate.Click += (_, __) => UpdateEmployee();
            btnDelete.Click += (_, __) => DeleteEmployee();
            btnSave.Click += (_, __) => SaveEmployees();
            btnLoad.Click += (_, __) => LoadEmployees();
            btnExit.Click += (_, __) => Close();
            grid.SelectionChanged += (_, __) => LoadSelectionToInputs();

            //DoubleClick Method to edit fields quickly
            grid.CellDoubleClick += (_, __) => LoadSelectionToInputs();

        }

        //Actions (Methods)
        private void AddEmployees()
        {
            try
            {
                var (id, name, dept, role, salary, hire) = ReadInputs();
                manager.AddEmployee(new Employee(id, name, dept, role, salary, hire));
                SetStatus($"Added {id} - {name}", true);
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Add Employee", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetStatus(ex.Message, false);
            }
        }

        private void UpdateEmployee()
        {
            try
            {
                var (id, name, dept, role, salary, hire) = ReadInputs();
                manager.UpdateEmployee (new Employee(id, name, dept, role, salary, hire));
                grid.Refresh(); //Update the Grid View
                SetStatus($"Updated '{id}'", true);
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Update Employees", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetStatus(ex.Message, false);
            }
        }

        private void DeleteEmployee()
        {
            var id = txtId.Text.Trim();
            if (string.IsNullOrWhiteSpace(id))
            {
                MessageBox.Show("Please enter an Employee ID to delete.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (manager.RemoveEmployee(id))
            {
                SetStatus($"Deleted '{id}'", true);
                ClearInputs();
            }
            else
            {
                MessageBox.Show($"Employee with ID '{id}' not found.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetStatus("Employee not found.", false);
            }
        }


        private void SaveEmployees()
        {
            using var sfd = new SaveFileDialog { Filter = "CSV Files|*.csv", FileName = "employees.csv" };
            if (sfd.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                manager.SaveToCsv(sfd.FileName);
                SetStatus($"Saved {manager.Employees.Count} employee(s) to {sfd.FileName}", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Save Failed.", false);
            }
        }

        private void LoadEmployees()
        {
            using var ofd = new OpenFileDialog { Filter = "CSV Files|*.csv", FileName = "employees.csv" };
            if (ofd.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                manager.LoadFromCsv(ofd.FileName);
                SetStatus($"Loaded {manager.Employees.Count} employee(s) from {ofd.FileName}", true);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Load Failed.", false);
            }
        }

        private void LoadSelectionToInputs()
        {
            if (grid.CurrentRow?.DataBoundItem is not Employee e) return;
            txtId.Text = e.Id;
            txtName.Text = e.FullName;
            txtDept.Text = e.Department;
            txtRole.Text = e.Role;
            txtSalary.Text = e.Salary.ToString();
            dtHire.Value = e.HireDate;
            SetStatus($"Selected {e.Id}", true);

        }

        //Helper Methods
        private (string id, string name, string dept, string role, double salary, DateTime hire) ReadInputs()
        {
            var id = txtId.Text.Trim();
            var name = txtName.Text.Trim();
            var dept = txtDept.Text.Trim();
            var role = txtRole.Text.Trim();
            if (!double.TryParse(txtSalary.Text.Trim(), System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out var salary))
                throw new FormatException("Invalid salary format. Please enter a valid number.");
            var hire = dtHire.Value.Date;

            //Basic checks for validity
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(dept))
                throw new ArgumentException("All text fields are required.");
            if (salary < 0) throw new ArgumentException("Salary must be a positive.");
            if (hire > DateTime.Today) throw new ArgumentException("Hire date cannot be in the future.");

            return (id, name, dept, role, salary, hire);
        }


        private void ClearInputs()
        {
            txtId.Clear();
            txtName.Clear();
            txtDept.Clear();
            txtRole.Clear();
            txtSalary.Clear();
            dtHire.Value = DateTime.Today;
            txtId.Focus();
        }

        private void SetStatus(string message, bool success)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = success ? Color.FromArgb(24,127, 56) : Color.FromArgb(176, 32, 32);

        }
    }
}
