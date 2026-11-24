
using отп1.Data;
using отп1.Models;
using отп1.Enums;

namespace отп1.Forms;

public partial class CarEditForm : Form
{
    private readonly ICarRepository _repository;
    private readonly Car? _car;

    private readonly TextBox _licensePlateTextBox = new();
    private readonly TextBox _brandTextBox = new();
    private readonly TextBox _modelTextBox = new();
    private readonly NumericUpDown _capacityNumeric = new();

    private readonly ComboBox _statusComboBox = new();
    private readonly CheckedListBox _featuresListBox = new();

    public CarEditForm(ICarRepository repository, Car? car = null)
    {
        _repository = repository;
        _car = car;

        Text = car == null ? "Добавление автомобиля" : "Редактирование автомобиля";
        Width = 450;
        Height = 530;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        Controls.Clear();

        BuildForm();
        LoadStatuses();
        LoadFeatures();

        if (_car != null)
            LoadCar();
    }

    private void BuildForm()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(15),
            ColumnCount = 2,
            RowCount = 7
        };

        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));

        AddRow(panel, "Государственный номер:", _licensePlateTextBox, 0);
        AddRow(panel, "Марка:", _brandTextBox, 1);
        AddRow(panel, "Модель:", _modelTextBox, 2);

        _capacityNumeric.Minimum = 1;
        _capacityNumeric.Maximum = 100000;
        _capacityNumeric.DecimalPlaces = 2;

        AddRow(panel, "Вместимость (л):", _capacityNumeric, 3);

        _statusComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        AddRow(panel, "Состояние:", _statusComboBox, 4);

        _featuresListBox.CheckOnClick = true;
        _featuresListBox.Height = 110;
        AddRow(panel, "Оборудование:", _featuresListBox, 5, 120);

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
            FlowDirection = FlowDirection.RightToLeft,
            Dock = DockStyle.Fill
        };

        buttons.Controls.Add(cancelButton);
        buttons.Controls.Add(saveButton);

        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        panel.Controls.Add(buttons, 1, 6);

        Controls.Add(panel);

        AcceptButton = saveButton;
        CancelButton = cancelButton;
    }

    private void AddRow(
        TableLayoutPanel panel,
        string label,
        Control control,
        int row,
        int height = 45)
    {
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, height));

        panel.Controls.Add(new Label
        {
            Text = label,
            AutoSize = true,
            Anchor = AnchorStyles.Left
        }, 0, row);

        control.Width = 200;
        control.Anchor = AnchorStyles.Left | AnchorStyles.Right;

        panel.Controls.Add(control, 1, row);
    }

    private void LoadStatuses()
    {
        _statusComboBox.DataSource = new List<StatusOption>
        {
            new(VehicleStatus.Available, "Доступен"),
            new(VehicleStatus.OnRoute, "В рейсе"),
            new(VehicleStatus.InRepair, "На ремонте"),
            new(VehicleStatus.Decommissioned, "Списан")
        };

        _statusComboBox.DisplayMember = nameof(StatusOption.Name);
    }

    private void LoadFeatures()
    {
        _featuresListBox.Items.Add(
            new FeatureOption(VehicleFeatures.FuelTanker, "Топливная цистерна"));

        _featuresListBox.Items.Add(
            new FeatureOption(VehicleFeatures.GPS, "GPS"));

        _featuresListBox.Items.Add(
            new FeatureOption(VehicleFeatures.FireExtinguisher, "Огнетушитель"));

        _featuresListBox.Items.Add(
            new FeatureOption(VehicleFeatures.SafetySystem, "Система безопасности"));

        _featuresListBox.Items.Add(
            new FeatureOption(VehicleFeatures.HeatingSystem, "Система подогрева"));

        _featuresListBox.DisplayMember = nameof(FeatureOption.Name);
    }

    private void LoadCar()
    {
        if (_car == null)
            return;

        _licensePlateTextBox.Text = _car.LicensePlate;
        _brandTextBox.Text = _car.Brand;
        _modelTextBox.Text = _car.Model;
        _capacityNumeric.Value = _car.Capacity;

        for (int i = 0; i < _statusComboBox.Items.Count; i++)
        {
            if (_statusComboBox.Items[i] is StatusOption option &&
                option.Value == _car.Status)
            {
                _statusComboBox.SelectedIndex = i;
                break;
            }
        }

        for (int i = 0; i < _featuresListBox.Items.Count; i++)
        {
            if (_featuresListBox.Items[i] is FeatureOption option)
            {
                bool selected =
                    (_car.Features & option.Value) == option.Value;

                _featuresListBox.SetItemChecked(i, selected);
            }
        }
    }

    private VehicleFeatures GetSelectedFeatures()
    {
        VehicleFeatures features = VehicleFeatures.None;

        foreach (var item in _featuresListBox.CheckedItems)
        {
            if (item is FeatureOption option)
                features |= option.Value;
        }

        return features;
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        try
        {
            if (_statusComboBox.SelectedItem is not StatusOption status)
                throw new InvalidOperationException(
                    "Выберите состояние автомобиля.");

            VehicleFeatures features = GetSelectedFeatures();

            if (_car == null)
            {
                int id = _repository.GetAll()
                    .Select(c => c.Id)
                    .DefaultIfEmpty(0)
                    .Max() + 1;

                Car car = Car.Create(
                    id,
                    _licensePlateTextBox.Text,
                    _brandTextBox.Text,
                    _modelTextBox.Text,
                    _capacityNumeric.Value,
                    status.Value,
                    features
                );

                _repository.Add(car);
            }
            else
            {
                _car.Update(
                    _licensePlateTextBox.Text,
                    _brandTextBox.Text,
                    _modelTextBox.Text,
                    _capacityNumeric.Value,
                    status.Value,
                    features
                );

                _repository.Update(_car);
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Ошибка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }
    }

    private class StatusOption
    {
        public VehicleStatus Value { get; }
        public string Name { get; }

        public StatusOption(VehicleStatus value, string name)
        {
            Value = value;
            Name = name;
        }
    }

    private class FeatureOption
    {
        public VehicleFeatures Value { get; }
        public string Name { get; }

        public FeatureOption(VehicleFeatures value, string name)
        {
            Value = value;
            Name = name;
        }
    }
}
