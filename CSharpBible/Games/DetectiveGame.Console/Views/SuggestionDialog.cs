using System.Linq;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using DetectiveGame.Engine.Cards;
using DetectiveGame.Engine.Game;

namespace DetectiveGame.Console.Views;

/// <summary>
/// Ein modales Panel zum Auswählen einer Verdachts-Kombination
/// </summary>
public class SuggestionDialog : Panel, IPopup
{
    private readonly ListBox _persons;
    private readonly ListBox _weapons;
    private readonly ListBox _rooms;
    private readonly Label _selPerson;
    private readonly Label _selWeapon;
    private readonly Label _selRoom;
    private readonly Button _ok;
    private readonly Button _cancel;

    public Card? SelectedPerson { get; private set; }
    public Card? SelectedWeapon { get; private set; }
    public Card? SelectedRoom { get; private set; }
    public bool Accepted { get; private set; }

    public event Action? Closed;

    public SuggestionDialog()
    {
        size = new System.Drawing.Size(60, 12);
        BackColor = ConsoleColor.DarkGray;
        ForeColor = ConsoleColor.Black;
        Text = "Verdacht";
        Shadow = true;

        // Listen
        _persons = new ListBox { Position = new System.Drawing.Point(1, 2), size = new System.Drawing.Size(16, 6), Text = "Personen" };
        _weapons = new ListBox { Position = new System.Drawing.Point(20, 2), size = new System.Drawing.Size(16, 6), Text = "Waffen" };
        _rooms = new ListBox { Position = new System.Drawing.Point(39, 2), size = new System.Drawing.Size(16, 6), Text = "Räume" };

        _persons.ItemsSource = GameData.Persons.Select(c => c.Name).ToList();
        _weapons.ItemsSource = GameData.Weapons.Select(c => c.Name).ToList();
        _rooms.ItemsSource = GameData.Rooms.Select(c => c.Name).ToList();

        // Auswahl Labels
        _selPerson = new Label { Position = new System.Drawing.Point(1, 9), size = new System.Drawing.Size(16, 1), Text = "-" };
        _selWeapon = new Label { Position = new System.Drawing.Point(20, 9), size = new System.Drawing.Size(16, 1), Text = "-" };
        _selRoom = new Label { Position = new System.Drawing.Point(39, 9), size = new System.Drawing.Size(16, 1), Text = "-" };

        // Buttons
        _ok = new Button();
        _ok.Set(1, 10, "OK", ConsoleColor.DarkGreen);
        _cancel = new Button();
        _cancel.Set(8, 10, "Abbruch", ConsoleColor.DarkRed);

        _persons.OnClick += (_, _) => UpdateSelection();
        _weapons.OnClick += (_, _) => UpdateSelection();
        _rooms.OnClick += (_, _) => UpdateSelection();
        _ok.OnClick += (_, _) => { if (TryFinalize()) Close(true); };
        _cancel.OnClick += (_, _) => Close(false);

        Add(_persons).Add(_weapons).Add(_rooms)
            .Add(_selPerson).Add(_selWeapon).Add(_selRoom)
            .Add(_ok).Add(_cancel);
    }

    private void UpdateSelection()
    {
        if (_persons.SelectedIndex >= 0)
        {
            SelectedPerson = GameData.Persons[_persons.SelectedIndex];
            _selPerson.Text = SelectedPerson.Name;
        }
        if (_weapons.SelectedIndex >= 0)
        {
            SelectedWeapon = GameData.Weapons[_weapons.SelectedIndex];
            _selWeapon.Text = SelectedWeapon.Name;
        }
        if (_rooms.SelectedIndex >= 0)
        {
            SelectedRoom = GameData.Rooms[_rooms.SelectedIndex];
            _selRoom.Text = SelectedRoom.Name;
        }
        Valid = SelectedPerson != null && SelectedWeapon != null && SelectedRoom != null;
    }

    private bool TryFinalize() => Valid;

    private void Close(bool accept)
    {
        Accepted = accept;
        Closed?.Invoke();
        Hide();
        Parent?.Remove(this);
    }

    public void Hide()
    {
        Visible = false;
    }

    public void Show()
    {
        Visible = true;
        (Parent as IGroupControl).BringToFront(this);
    }
}
