using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace change_element_in_collection_view
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        public MainPage() => InitializeComponent();
        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;
            var plural = count > 1 ? "s" : string.Empty;
            CounterBtn.Text = $"Clicked {count} time{plural}";
            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
    class MainPageBindingContext : INotifyPropertyChanged
    {
        public ObservableCollection<Item> items { get; } = new ObservableCollection<Item>
        {
            new Item{title = "Item A"},
            new Item{title = "Item B"},
            new Item{title = "Item C"},
        };

        // Because we're in "single" mode, we bind SelectedItem, not SelectedItems
        public Item? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (!Equals(_selectedItem, value))
                {
                    _selectedItem = value;
                    OnPropertyChanged();
                    // Quick and dirty. You will want to improve on this.
                    foreach (var item in items)
                    {
                        item.IsSelected = ReferenceEquals(_selectedItem, item);
                    }
                }
            }
        }
        Item? _selectedItem = default;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler? PropertyChanged;
    }
    class Item : INotifyPropertyChanged
    {
        public string title
        {
            get => IsSelected ? $"{_title} SELECTED"  : _title;
            set
            {
                if (!Equals(_title, value))
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }
        string _title = "{New Item}";

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (!Equals(_isSelected, value))
                {
                    _isSelected = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HorizontalStackLayoutColor));
                    OnPropertyChanged(nameof(LabelTextColor));
                    OnPropertyChanged(nameof(title));
                }
            }
        }
        bool _isSelected = default;

        public Color HorizontalStackLayoutColor => IsSelected ? Colors.CornflowerBlue : Colors.White;
        public Color LabelTextColor => IsSelected ? Colors.White : Colors.Black;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
