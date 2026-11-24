
using отп1.Data;
using отп1.Models;

namespace отп1.Forms;

public partial class DriverListForm : Form
{
    private readonly IDriverRepository _driverRepository;
    private readonly ICarRepository _carRepository;

    private readonly DataGridView _dataGridView = new();

    private readonly Button _addButton = new();
    private readonly Button _editButton = new();
    private readonly Button _deleteButton = new();
    private readonly Button _refreshButton = new();

    public DriverListForm(
        IDriverRepository driverRepository,
        ICarRepository carRepository)
    {
        _driverRepository = driverRepository;
        _carRepository = carRepository;

        Text = "Водители";
        Width = 900;
        Height = 500;
        StartPosition = FormStartPosition.CenterScreen;

        BuildForm();
        LoadDrivers();
    }

    private void BuildForm()
    {
        _dataGridView.Dock = DockStyle.Fill;
        _dataGridView.ReadOnly = true;
        _dataGridView.AutoGenerateColumns = true;
        _dataGridView.SelectionMode =
            DataGridViewSelectionMode.FullRowSelect;
        _dataGridView.MultiSelect = false;
        _dataGridView.AllowUserToAddRows = false;
        _dataGridView.AutoSizeColumnsMode =
            DataGridViewAutoSizeColumnsMode.Fill;

        _addButton.Text = "Добавить";
        _editButton.Text = "Редактировать";
        _deleteButton.Text = "Удалить";
        _refreshButton.Text = "Обновить";

        _addButton.Click += AddButton_Click;
        _editButton.Click += EditButton_Click;
        _deleteButton.Click += DeleteButton_Click;
        _refreshButton.Click += RefreshButton_Click;

        var buttonsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 50,
            Padding = new Padding(5)
        };

        buttonsPanel.Controls.Add(_addButton);
        buttonsPanel.Controls.Add(_editButton);
        buttonsPanel.Controls.Add(_deleteButton);
        buttonsPanel.Controls.Add(_refreshButton);

        Controls.Add(_dataGridView);
        Controls.Add(buttonsPanel);
    }

    private void LoadDrivers()
    {
        var drivers = _driverRepository.GetAll()
            .Select(d => new
            {
                d.Id,
                d.FullName,
                d.LicenseNumber,
                Car = GetCarName(d.CarId)
            })
            .ToList();

        _dataGridView.DataSource = null;
        _dataGridView.DataSource = drivers;

        if (_dataGridView.Columns["Id"] != null)
            _dataGridView.Columns["Id"].HeaderText = "ID";

        if (_dataGridView.Columns["FullName"] != null)
            _dataGridView.Columns["FullName"].HeaderText = "ФИО";

        if (_dataGridView.Columns["LicenseNumber"] != null)
            _dataGridView.Columns["LicenseNumber"].HeaderText =
                "Водительское удостоверение";

        if (_dataGridView.Columns["Car"] != null)
            _dataGridView.Columns["Car"].HeaderText = "Автомобиль";
    }

    private string GetCarName(int carId)
    {
        var car = _carRepository.GetById(carId);

        if (car == null)
            return "Автомобиль не найден";

        return $"{car.Brand} {car.Model} ({car.LicensePlate})";
    }

    private Driver? GetSelectedDriver()
    {
        if (_dataGridView.CurrentRow == null)
            return null;

        if (_dataGridView.CurrentRow.Cells["Id"].Value == null)
            return null;

        int id = Convert.ToInt32(
            _dataGridView.CurrentRow.Cells["Id"].Value);

        return _driverRepository.GetById(id);
    }

    private void AddButton_Click(object? sender, EventArgs e)
    {
        using var form = new DriverEditForm(
            _driverRepository,
            _carRepository);

        if (form.ShowDialog(this) == DialogResult.OK)
            LoadDrivers();
    }

    private void EditButton_Click(object? sender, EventArgs e)
    {
        Driver? driver = GetSelectedDriver();

        if (driver == null)
        {
            MessageBox.Show("Выберите водителя из таблицы.");
            return;
        }

        using var form = new DriverEditForm(
            _driverRepository,
            _carRepository,
            driver);

        if (form.ShowDialog(this) == DialogResult.OK)
            LoadDrivers();
    }

    private void DeleteButton_Click(object? sender, EventArgs e)
    {
        Driver? driver = GetSelectedDriver();

        if (driver == null)
        {
            MessageBox.Show("Выберите водителя из таблицы.");
            return;
        }

        DialogResult result = MessageBox.Show(
            $"Удалить водителя {driver.FullName}?",
            "Подтверждение удаления",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            _driverRepository.Delete(driver.Id);
            LoadDrivers();
        }
    }

    private void RefreshButton_Click(object? sender, EventArgs e)
    {
        LoadDrivers();
    }
}
