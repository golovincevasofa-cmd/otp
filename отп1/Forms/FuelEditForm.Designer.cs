
using отп1.Data;
using отп1.Models;

namespace отп1.Forms;

public partial class FuelEditForm : Form
{
    private readonly IFuelRepository _repository;
    private readonly Fuel? _fuel;

    private readonly TextBox _nameTextBox = new();
    private readonly TextBox _descriptionTextBox = new();

    public FuelEditForm(IFuelRepository repository, Fuel? fuel = null)
    {
        _repository = repository;
        _fuel = fuel;

        Text = fuel == null ? "Добавление топлива" : "Редактирование топлива";
        Width = 450;
        Height = 250;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        BuildForm();

        if (_fuel != null)
            LoadFuel();
    }

    private void BuildForm()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(15),
            ColumnCount = 2,
            RowCount = 3
        };

        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));

        AddRow(panel, "Название топлива:", _nameTextBox, 0);
        AddRow(panel, "Описание:", _descriptionTextBox, 1);

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

        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        panel.Controls.Add(buttons, 1, 2);

        Controls.Add(panel);

        AcceptButton = saveButton;
        CancelButton = cancelButton;
    }

    private void AddRow(TableLayoutPanel panel, string label,
        Control control, int row)
    {
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));

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

    private void LoadFuel()
    {
        if (_fuel == null)
            return;

        _nameTextBox.Text = _fuel.Name;
        _descriptionTextBox.Text = _fuel.Description ?? string.Empty;
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        try
        {
            if (_fuel == null)
            {
                int id = _repository.GetAll()
                    .Select(f => f.Id)
                    .DefaultIfEmpty(0)
                    .Max() + 1;

                Fuel fuel = Fuel.Create(
                    id,
                    _nameTextBox.Text,
                    _descriptionTextBox.Text
                );

                _repository.Add(fuel);
            }
            else
            {
                _fuel.Update(
                    _nameTextBox.Text,
                    _descriptionTextBox.Text
                );

                _repository.Update(_fuel);
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
}
