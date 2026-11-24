
using отп1.Data;
using отп1.Models;

namespace отп1.Forms;

public partial class TransportationListForm : Form
{
    private readonly ITransportationRepository _transportationRepository;
    private readonly ICarRepository _carRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IFuelRepository _fuelRepository;

    private readonly DataGridView _dataGridView = new();

    private readonly Button _addButton = new();
    private readonly Button _refreshButton = new();

    public TransportationListForm(
        ITransportationRepository transportationRepository,
        ICarRepository carRepository,
        IDriverRepository driverRepository,
        IFuelRepository fuelRepository)
    {
        _transportationRepository = transportationRepository;
        _carRepository = carRepository;
        _driverRepository = driverRepository;
        _fuelRepository = fuelRepository;

        Text = "Перевозки топлива";
        Width = 1000;
        Height = 500;
        StartPosition = FormStartPosition.CenterScreen;

        BuildForm();
        LoadTransportations();
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

        _addButton.Text = "Добавить перевозку";
        _refreshButton.Text = "Обновить";

        _addButton.Click += AddButton_Click;
        _refreshButton.Click += RefreshButton_Click;

        var buttonsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 50,
            Padding = new Padding(5)
        };

        buttonsPanel.Controls.Add(_addButton);
        buttonsPanel.Controls.Add(_refreshButton);

        Controls.Add(_dataGridView);
        Controls.Add(buttonsPanel);
    }

    private void LoadTransportations()
    {
        var transportations = _transportationRepository.GetAll()
            .Select(t => new
            {
                t.Id,
                Car = GetCarName(t.CarId),
                Driver = GetDriverName(t.DriverId),
                Fuel = GetFuelName(t.FuelId),
                t.Quantity,
                t.TransportationDate
            })
            .ToList();

        _dataGridView.DataSource = null;
        _dataGridView.DataSource = transportations;

        if (_dataGridView.Columns["Id"] != null)
            _dataGridView.Columns["Id"].HeaderText = "ID";

        if (_dataGridView.Columns["Car"] != null)
            _dataGridView.Columns["Car"].HeaderText = "Автомобиль";

        if (_dataGridView.Columns["Driver"] != null)
            _dataGridView.Columns["Driver"].HeaderText = "Водитель";

        if (_dataGridView.Columns["Fuel"] != null)
            _dataGridView.Columns["Fuel"].HeaderText = "Топливо";

        if (_dataGridView.Columns["Quantity"] != null)
            _dataGridView.Columns["Quantity"].HeaderText =
                "Количество (л)";

        if (_dataGridView.Columns["TransportationDate"] != null)
            _dataGridView.Columns["TransportationDate"].HeaderText =
                "Дата перевозки";
    }

    private string GetCarName(int carId)
    {
        var car = _carRepository.GetById(carId);

        if (car == null)
            return "Автомобиль не найден";

        return $"{car.Brand} {car.Model} ({car.LicensePlate})";
    }

    private string GetDriverName(int driverId)
    {
        var driver = _driverRepository.GetById(driverId);

        return driver?.FullName ?? "Водитель не найден";
    }

    private string GetFuelName(int fuelId)
    {
        var fuel = _fuelRepository.GetById(fuelId);

        return fuel?.Name ?? "Топливо не найдено";
    }

    private void AddButton_Click(object? sender, EventArgs e)
    {
        using var form = new TransportationAddForm(
            _transportationRepository,
            _carRepository,
            _driverRepository,
            _fuelRepository);

        if (form.ShowDialog(this) == DialogResult.OK)
            LoadTransportations();
    }

    private void RefreshButton_Click(object? sender, EventArgs e)
    {
        LoadTransportations();
    }
}
