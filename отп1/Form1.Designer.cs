
using отп1.Data;
using отп1.Forms;

namespace отп1;

public partial class Form1 : Form
{
    private readonly ICarRepository _carRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IFuelRepository _fuelRepository;
    private readonly ITransportationRepository _transportationRepository;

    public Form1(
        ICarRepository carRepository,
        IDriverRepository driverRepository,
        IFuelRepository fuelRepository,
        ITransportationRepository transportationRepository)
    {
        _carRepository = carRepository;
        _driverRepository = driverRepository;
        _fuelRepository = fuelRepository;
        _transportationRepository = transportationRepository;

       
        Controls.Clear();

        BuildForm();
    }

    private void BuildForm()
    {
        Text = "Автотранспортное предприятие — перевозка топлива";
        Width = 800;
        Height = 550;
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(650, 450);

        var title = new Label
        {
            Text = "Система управления автотранспортным предприятием",
            Dock = DockStyle.Top,
            Height = 90,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 17, FontStyle.Bold)
        };

        var subtitle = new Label
        {
            Text = "Управление автомобилями, водителями, топливом и перевозками",
            Dock = DockStyle.Top,
            Height = 45,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 10)
        };

        var buttonsPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Padding = new Padding(40),
        };

        buttonsPanel.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 50));
        buttonsPanel.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 50));

        buttonsPanel.RowStyles.Add(
            new RowStyle(SizeType.Percent, 50));
        buttonsPanel.RowStyles.Add(
            new RowStyle(SizeType.Percent, 50));

        var carsButton = CreateButton("Автомобили");
        var driversButton = CreateButton("Водители");
        var fuelsButton = CreateButton("Марки топлива");
        var transportationsButton = CreateButton("Перевозки");

        carsButton.Click += (s, e) =>
        {
            using var form = new CarListForm(_carRepository);
            form.ShowDialog(this);
        };

        driversButton.Click += (s, e) =>
        {
            using var form = new DriverListForm(
                _driverRepository,
                _carRepository);

            form.ShowDialog(this);
        };

        fuelsButton.Click += (s, e) =>
        {
            using var form = new FuelListForm(_fuelRepository);
            form.ShowDialog(this);
        };

        transportationsButton.Click += (s, e) =>
        {
            using var form = new TransportationListForm(
                _transportationRepository,
                _carRepository,
                _driverRepository,
                _fuelRepository);

            form.ShowDialog(this);
        };

        buttonsPanel.Controls.Add(carsButton, 0, 0);
        buttonsPanel.Controls.Add(driversButton, 1, 0);
        buttonsPanel.Controls.Add(fuelsButton, 0, 1);
        buttonsPanel.Controls.Add(transportationsButton, 1, 1);

        Controls.Add(buttonsPanel);
        Controls.Add(subtitle);
        Controls.Add(title);
    }

    private Button CreateButton(string text)
    {
        return new Button
        {
            Text = text,
            Dock = DockStyle.Fill,
            Margin = new Padding(12),
            Font = new Font("Segoe UI", 13, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
    }
}
