One of the easier ways "when reacting to a selected item in a collection view" is to bind the `SelectedItem` property. You mention changing an attribute of `HorizontalStackLayout` so let's say for example that you want its background color to turn blue when selected.  I kludged a CollectionView onto the MAUI default as a minimal example of doing this.

~~~          
<CollectionView 
    MinimumHeightRequest="400"
    BackgroundColor="Azure"
    SelectionMode="Single"
    ItemsSource="{Binding items}"
    SelectedItem="{Binding SelectedItem}">
    <CollectionView.ItemsLayout>
        <LinearItemsLayout Orientation="Vertical" ItemSpacing="1" />
    </CollectionView.ItemsLayout>
    <CollectionView.ItemTemplate>
        <DataTemplate>
            <ContentView>
                <Frame 
                    Margin="2"
                    Padding="10"
                    BackgroundColor="White"
                    CornerRadius="10"
                    HasShadow="True"
                    InputTransparent="true">
                    <HorizontalStackLayout
                        BackgroundColor="{Binding HorizontalStackLayoutColor}">
                        <Label 
                            Text="{Binding title}" 
                            TextColor="{Binding LabelTextColor}"
                            FontSize="16"
                            FontAttributes="Bold"
                            Padding="5"/>
                    </HorizontalStackLayout>
                </Frame>
                <VisualStateManager.VisualStateGroups>
                    <VisualStateGroup Name="CommonStates">
                        <VisualState Name="Normal">
                            <VisualState.Setters>
                                <Setter Property="BackgroundColor" Value="Transparent" />
                            </VisualState.Setters>
                        </VisualState>
                        <VisualState Name="Selected">
                            <VisualState.Setters>
                                <Setter Property="BackgroundColor" Value="LightBlue" />
                            </VisualState.Setters>
                        </VisualState>
                    </VisualStateGroup>
                </VisualStateManager.VisualStateGroups>
            </ContentView>
        </DataTemplate>                   
    </CollectionView.ItemTemplate>
</CollectionView>
~~~

___

**Binding context for MainPage**

~~~
class MainPageBindingContext : INotifyPropertyChanged
{
    public ObservableCollection<Item> items { get; } = new ObservableCollection<Item>
    {
        new Item{title = "Item A"},
        new Item{title = "Item B"},
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
~~~

___

**Binding context for Item**

~~~
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
~~~