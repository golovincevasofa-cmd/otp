
using отп1.Data;
using отп1.Models;

namespace отп1.Forms;

public partial class TransportationAddForm : Form
{
    private readonly ITransportationRepository _transportationRepository;
    private readonly ICarRepository _carRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IFuelRepository _fuelRepository;

    private readonly ComboBox _carComboBox = new();
    private readonly ComboBox _driverComboBox = new();
    private readonly ComboBox _fuelComboBox = new();

    private readonly NumericUpDown _quantityNumeric = new();
    private readonly DateTimePicker _datePicker = new();

    public TransportationAddForm(
        ITransportationRepository transportationRepository,
        ICarRepository carRepository,
        IDriverRepository driverRepository,
        IFuelRepository fuelRepository)
    {
        _transportationRepository = transportationRepository;
        _carRepository = carRepository;
        _driverRepository = driverRepository;
        _fuelRepository = fuelRepository;

        Text = "Добавление перевозки";
        Width = 500;
        Height = 400;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        BuildForm();
        LoadCars();
        LoadFuels();

        _carComboBox.SelectedIndexChanged += CarComboBox_SelectedIndexChanged;
    }

    private void BuildForm()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(15),
            ColumnCount = 2,
            RowCount = 6
        };

        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));

        _carComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _driverComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _fuelComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

        AddRow(panel, "Автомобиль:", _carComboBox, 0);
        AddRow(panel, "Водитель:", _driverComboBox, 1);
        AddRow(panel, "Топливо:", _fuelComboBox, 2);

        _quantityNumeric.Minimum = 0.01m;
        _quantityNumeric.Maximum = 1000000;
        _quantityNumeric.DecimalPlaces = 2;
        _quantityNumeric.Increment = 100;
        AddRow(panel, "Количество (л):", _quantityNumeric, 3);

        _datePicker.Format = DateTimePickerFormat.Short;
        _datePicker.MaxDate = DateTime.Today;
        AddRow(panel, "Дата перевозки:", _datePicker, 4);

        var saveButton = new Button
        {
            Text = "Сохранить",
            Width = 110
        };

        var cancelButton = new Button
        {
            Text = "Отмена",
            Width = 110,
            DialogResult = DialogResult.Cancel
        };

        saveButton.Click += SaveButton_Click;

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft
        };

        buttons.Controls.Add(cancelButton);
        buttons.Controls.Add(saveButton);

        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        panel.Controls.Add(buttons, 1, 5);

        Controls.Add(panel);

        AcceptButton = saveButton;
        CancelButton = cancelButton;
    }

    private void AddRow(TableLayoutPanel panel, string label,
        Control control, int row)
    {
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

        panel.Controls.Add(new Label
        {
            Text = label,
            AutoSize = true,
            Anchor = AnchorStyles.Left
        }, 0, row);

        control.Width = 230;
        control.Anchor = AnchorStyles.Left | AnchorStyles.Right;

        panel.Controls.Add(control, 1, row);
    }

    private void LoadCars()
    {
        var cars = _carRepository.GetAll()
            .Select(c => new OptionItem
            {
                Id = c.Id,
                Name = $"{c.Brand} {c.Model} ({c.LicensePlate})"
            })
            .ToList();

        _carComboBox.DataSource = cars;
        _carComboBox.DisplayMember = nameof(OptionItem.Name);
        _carComboBox.ValueMember = nameof(OptionItem.Id);
    }

    private void LoadDrivers(int carId)
    {
        var drivers = _driverRepository.GetAll()
            .Where(d => d.CarId == carId)
            .Select(d => new OptionItem
            {
                Id = d.Id,
                Name = d.FullName
            })
            .ToList();

        _driverComboBox.DataSource = null;
        _driverComboBox.DataSource = drivers;
        _driverComboBox.DisplayMember = nameof(OptionItem.Name);
        _driverComboBox.ValueMember = nameof(OptionItem.Id);
    }

    private void LoadFuels()
    {
        var fuels = _fuelRepository.GetAll()
            .Select(f => new OptionItem
            {
                Id = f.Id,
                Name = f.Name
            })
            .ToList();

        _fuelComboBox.DataSource = fuels;
        _fuelComboBox.DisplayMember = nameof(OptionItem.Name);
        _fuelComboBox.ValueMember = nameof(OptionItem.Id);
    }

    private void CarComboBox_SelectedIndexChanged(
        object? sender, EventArgs e)
    {
        if (_carComboBox.SelectedValue == null)
            return;

        if (_carComboBox.SelectedValue is int carId)
            LoadDrivers(carId);
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        try
        {
            if (_carComboBox.SelectedValue is not int carId)
                throw new InvalidOperationException(
                    "Выберите автомобиль.");

            if (_driverComboBox.SelectedValue is not int driverId)
                throw new InvalidOperationException(
                    "Для выбранного автомобиля нет водителей. Выберите другого автомобиль или добавьте водителя.");

            if (_fuelComboBox.SelectedValue is not int fuelId)
                throw new InvalidOperationException(
                    "Добавьте и выберите марку топлива.");

            var car = _carRepository.GetById(carId);
            var driver = _driverRepository.GetById(driverId);

            if (car == null || driver == null)
                throw new InvalidOperationException(
                    "Автомобиль или водитель не найдены.");

            if (_quantityNumeric.Value > car.Capacity)
                throw new InvalidOperationException(
                    "Количество топлива превышает вместимость автомобиля.");

            if (driver.CarId != carId)
                throw new InvalidOperationException(
                    "Выбранный водитель не закреплён за этим автомобилем.");

            int id = _transportationRepository.GetAll()
                .Select(t => t.Id)
                .DefaultIfEmpty(0)
                .Max() + 1;

            Transportation transportation = Transportation.Create(
                id,
                carId,
                driverId,
                fuelId,
                _quantityNumeric.Value,
                _datePicker.Value
            );

            _transportationRepository.Add(transportation);

            MessageBox.Show(
                "Перевозка успешно добавлена.",
                "Успешно",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Ошибка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private class OptionItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
