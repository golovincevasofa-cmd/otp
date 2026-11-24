
using отп1.Data;
using отп1.Models;
using отп1.Enums;

namespace отп1.Forms;

public partial class CarListForm : Form
{
    private readonly ICarRepository _repository;

    private readonly DataGridView _dataGridView = new();

    private readonly Button _addButton = new();
    private readonly Button _editButton = new();
    private readonly Button _deleteButton = new();
    private readonly Button _refreshButton = new();

    public CarListForm(ICarRepository repository)
    {
        _repository = repository;

        Text = "Автомобили";
        Width = 1100;
        Height = 550;
        StartPosition = FormStartPosition.CenterScreen;

        BuildForm();
        LoadCars();
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

    private void LoadCars()
    {
        var cars = _repository.GetAll()
            .Select(c => new
            {
                c.Id,
                LicensePlate = c.LicensePlate,
                Brand = c.Brand,
                Model = c.Model,
                Capacity = c.Capacity,
                Status = GetStatusName(c.Status),
                Features = GetFeaturesNames(c.Features)
            })
            .ToList();

        _dataGridView.DataSource = null;
        _dataGridView.DataSource = cars;

        _dataGridView.Columns["Id"].HeaderText = "ID";
        _dataGridView.Columns["LicensePlate"].HeaderText =
            "Государственный номер";
        _dataGridView.Columns["Brand"].HeaderText = "Марка";
        _dataGridView.Columns["Model"].HeaderText = "Модель";
        _dataGridView.Columns["Capacity"].HeaderText =
            "Вместимость (л)";
        _dataGridView.Columns["Status"].HeaderText = "Состояние";
        _dataGridView.Columns["Features"].HeaderText = "Оборудование";
    }

    private string GetStatusName(VehicleStatus status)
    {
        return status switch
        {
            VehicleStatus.Available => "Доступен",
            VehicleStatus.OnRoute => "В рейсе",
            VehicleStatus.InRepair => "На ремонте",
            VehicleStatus.Decommissioned => "Списан",
            _ => "Неизвестно"
        };
    }

    private string GetFeaturesNames(VehicleFeatures features)
    {
        var names = new List<string>();

        if (features.HasFlag(VehicleFeatures.FuelTanker))
            names.Add("Топливная цистерна");

        if (features.HasFlag(VehicleFeatures.GPS))
            names.Add("GPS");

        if (features.HasFlag(VehicleFeatures.FireExtinguisher))
            names.Add("Огнетушитель");

        if (features.HasFlag(VehicleFeatures.SafetySystem))
            names.Add("Система безопасности");

        if (features.HasFlag(VehicleFeatures.HeatingSystem))
            names.Add("Система подогрева");

        return names.Count == 0
            ? "Нет"
            : string.Join(", ", names);
    }

    private Car? GetSelectedCar()
    {
        if (_dataGridView.CurrentRow == null)
            return null;

        int id = Convert.ToInt32(
            _dataGridView.CurrentRow.Cells["Id"].Value);

        return _repository.GetById(id);
    }

    private void AddButton_Click(object? sender, EventArgs e)
    {
        using var form = new CarEditForm(_repository);

        if (form.ShowDialog(this) == DialogResult.OK)
            LoadCars();
    }

    private void EditButton_Click(object? sender, EventArgs e)
    {
        Car? car = GetSelectedCar();

        if (car == null)
        {
            MessageBox.Show("Выберите автомобиль.");
            return;
        }

        using var form = new CarEditForm(_repository, car);

        if (form.ShowDialog(this) == DialogResult.OK)
            LoadCars();
    }

    private void DeleteButton_Click(object? sender, EventArgs e)
    {
        Car? car = GetSelectedCar();

        if (car == null)
        {
            MessageBox.Show("Выберите автомобиль.");
            return;
        }

        var result = MessageBox.Show(
            $"Удалить автомобиль {car.Brand} {car.Model}?",
            "Подтверждение удаления",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            _repository.Delete(car.Id);
            LoadCars();
        }
    }

    private void RefreshButton_Click(object? sender, EventArgs e)
    {
        LoadCars();
    }
}
