
using отп1.Data;
using отп1.Models;

namespace отп1.Forms;

public partial class FuelListForm : Form
{
    private readonly IFuelRepository _repository;

    private readonly DataGridView _dataGridView = new();

    private readonly Button _addButton = new();
    private readonly Button _editButton = new();
    private readonly Button _deleteButton = new();
    private readonly Button _refreshButton = new();

    public FuelListForm(IFuelRepository repository)
    {
        _repository = repository;

        Text = "Марки топлива";
        Width = 750;
        Height = 450;
        StartPosition = FormStartPosition.CenterScreen;

        BuildForm();
        LoadFuels();
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

    private void LoadFuels()
    {
        _dataGridView.DataSource = null;
        _dataGridView.DataSource = _repository.GetAll();

        if (_dataGridView.Columns["Id"] != null)
            _dataGridView.Columns["Id"].HeaderText = "ID";

        if (_dataGridView.Columns["Name"] != null)
            _dataGridView.Columns["Name"].HeaderText = "Название топлива";

        if (_dataGridView.Columns["Description"] != null)
            _dataGridView.Columns["Description"].HeaderText = "Описание";
    }

    private Fuel? GetSelectedFuel()
    {
        if (_dataGridView.CurrentRow == null)
            return null;

        if (_dataGridView.CurrentRow.Cells["Id"].Value == null)
            return null;

        int id = Convert.ToInt32(
            _dataGridView.CurrentRow.Cells["Id"].Value);

        return _repository.GetById(id);
    }

    private void AddButton_Click(object? sender, EventArgs e)
    {
        using var form = new FuelEditForm(_repository);

        if (form.ShowDialog(this) == DialogResult.OK)
            LoadFuels();
    }

    private void EditButton_Click(object? sender, EventArgs e)
    {
        Fuel? fuel = GetSelectedFuel();

        if (fuel == null)
        {
            MessageBox.Show("Выберите топливо из таблицы.");
            return;
        }

        using var form = new FuelEditForm(_repository, fuel);

        if (form.ShowDialog(this) == DialogResult.OK)
            LoadFuels();
    }

    private void DeleteButton_Click(object? sender, EventArgs e)
    {
        Fuel? fuel = GetSelectedFuel();

        if (fuel == null)
        {
            MessageBox.Show("Выберите топливо из таблицы.");
            return;
        }

        DialogResult result = MessageBox.Show(
            $"Удалить марку топлива «{fuel.Name}»?",
            "Подтверждение удаления",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            _repository.Delete(fuel.Id);
            LoadFuels();
        }
    }

    private void RefreshButton_Click(object? sender, EventArgs e)
    {
        LoadFuels();
    }
}
