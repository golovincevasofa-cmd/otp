
using отп1.Data;
using отп1.Models;

namespace отп1.Forms;

public partial class DriverEditForm : Form
{
    private readonly IDriverRepository _driverRepository;
    private readonly ICarRepository _carRepository;
    private readonly Driver? _driver;

    private readonly TextBox _fullNameTextBox = new();
    private readonly TextBox _licenseNumberTextBox = new();
    private readonly ComboBox _carComboBox = new();

    public DriverEditForm(
        IDriverRepository driverRepository,
        ICarRepository carRepository,
        Driver? driver = null)
    {
        _driverRepository = driverRepository;
        _carRepository = carRepository;
        _driver = driver;

        Text = driver == null ? "Добавление водителя" : "Редактирование водителя";
        Width = 450;
        Height = 300;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        BuildForm();
        LoadCars();

        if (_driver != null)
            LoadDriver();
    }

    private void BuildForm()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(15),
            ColumnCount = 2,
            RowCount = 4
        };

        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));

        AddRow(panel, "ФИО:", _fullNameTextBox, 0);
        AddRow(panel, "Водительское удостоверение:",
            _licenseNumberTextBox, 1);

        _carComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        AddRow(panel, "Автомобиль:", _carComboBox, 2);

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
        panel.Controls.Add(buttons, 1, 3);

        Controls.Add(panel);

        AcceptButton = saveButton;
        CancelButton = cancelButton;
    }

    private void AddRow(
        TableLayoutPanel panel,
        string label,
        Control control,
        int row)
    {
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

        panel.Controls.Add(new Label
        {
            Text = label,
            AutoSize = true,
            Anchor = AnchorStyles.Left
        }, 0, row);

        control.Width = 220;
        control.Anchor = AnchorStyles.Left | AnchorStyles.Right;

        panel.Controls.Add(control, 1, row);
    }

    private void LoadCars()
    {
        var cars = _carRepository.GetAll()
            .Select(c => new CarOption
            {
                Id = c.Id,
                Display = $"{c.Brand} {c.Model} ({c.LicensePlate})"
            })
            .ToList();

        _carComboBox.DataSource = cars;
        _carComboBox.DisplayMember = nameof(CarOption.Display);
        _carComboBox.ValueMember = nameof(CarOption.Id);
    }

    private void LoadDriver()
    {
        if (_driver == null)
            return;

        _fullNameTextBox.Text = _driver.FullName;
        _licenseNumberTextBox.Text = _driver.LicenseNumber;
        _carComboBox.SelectedValue = _driver.CarId;
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        try
        {
            if (_carComboBox.SelectedValue == null)
                throw new InvalidOperationException(
                    "Сначала добавьте автомобиль и выберите его.");

            int carId = (int)_carComboBox.SelectedValue;

            if (_driver == null)
            {
                int id = _driverRepository.GetAll()
                    .Select(d => d.Id)
                    .DefaultIfEmpty(0)
                    .Max() + 1;

                Driver driver = Driver.Create(
                    id,
                    _fullNameTextBox.Text,
                    _licenseNumberTextBox.Text,
                    carId
                );

                _driverRepository.Add(driver);
            }
            else
            {
                _driver.Update(
                    _fullNameTextBox.Text,
                    _licenseNumberTextBox.Text,
                    carId
                );

                _driverRepository.Update(_driver);
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

    private class CarOption
    {
        public int Id { get; set; }
        public string Display { get; set; } = string.Empty;
    }
}
