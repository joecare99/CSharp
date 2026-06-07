namespace Avln_ControlsAndLayout.Models;

/// <summary>
/// Centralizes AXAML snippets that are displayed next to their rendered previews.
/// </summary>
public static class SampleCode
{
    public const string Border = """
<Grid xmlns="https://github.com/avaloniaui">
    <Border Background="LightGray" CornerRadius="10" Padding="10"
            HorizontalAlignment="Center" VerticalAlignment="Center"
            BorderBrush="Black" BorderThickness="4">
        <TextBlock>Content inside of a Border</TextBlock>
    </Border>
</Grid>
""";

    public const string Canvas = """
<Grid xmlns="https://github.com/avaloniaui">
    <Canvas Margin="10">
        <Rectangle Canvas.Top="0" Canvas.Left="0" Width="100" Height="100" Fill="Red" />
        <Rectangle Canvas.Top="100" Canvas.Left="100" Width="100" Height="100" Fill="Green" />
        <Rectangle Canvas.Top="50" Canvas.Left="50" Width="100" Height="100" Fill="Blue" />
    </Canvas>
</Grid>
""";

    public const string Grid = """
<Grid xmlns="https://github.com/avaloniaui">
    <Grid Background="White" HorizontalAlignment="Center" VerticalAlignment="Center">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="30" /><ColumnDefinition Width="30" /><ColumnDefinition Width="30" />
            <ColumnDefinition Width="30" /><ColumnDefinition Width="30" /><ColumnDefinition Width="30" />
            <ColumnDefinition Width="30" />
        </Grid.ColumnDefinitions>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" /><RowDefinition Height="Auto" /><RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" /><RowDefinition Height="Auto" /><RowDefinition Height="Auto" />
        </Grid.RowDefinitions>
        <TextBlock Grid.ColumnSpan="7" HorizontalAlignment="Center" Margin="0,5">January 2004</TextBlock>
        <Rectangle Grid.Row="1" Grid.ColumnSpan="7" Fill="Black" Height="2" />
        <TextBlock Grid.Row="2" Grid.Column="0" Margin="5">Sun</TextBlock>
        <TextBlock Grid.Row="2" Grid.Column="1" Margin="5">Mon</TextBlock>
        <TextBlock Grid.Row="2" Grid.Column="2" Margin="5">Tue</TextBlock>
        <TextBlock Grid.Row="2" Grid.Column="3" Margin="5">Wed</TextBlock>
        <TextBlock Grid.Row="2" Grid.Column="4" Margin="5">Thu</TextBlock>
        <TextBlock Grid.Row="2" Grid.Column="5" Margin="5">Fri</TextBlock>
        <TextBlock Grid.Row="2" Grid.Column="6" Margin="5">Sat</TextBlock>
    </Grid>
</Grid>
""";

    public const string DockPanel = """
<Grid xmlns="https://github.com/avaloniaui">
    <DockPanel>
        <Border Height="25" Background="SkyBlue" BorderBrush="Black" BorderThickness="1" DockPanel.Dock="Top">
            <TextBlock>Dock = Top</TextBlock>
        </Border>
        <Border Height="25" Background="#ffff99" BorderBrush="Black" BorderThickness="1" DockPanel.Dock="Bottom">
            <TextBlock>Dock = Bottom</TextBlock>
        </Border>
        <Border Width="200" Background="PaleGreen" BorderBrush="Black" BorderThickness="1" DockPanel.Dock="Left">
            <TextBlock>Dock = Left</TextBlock>
        </Border>
        <Border Background="White" BorderBrush="Black" BorderThickness="1">
            <TextBlock>This content fills the remaining space.</TextBlock>
        </Border>
    </DockPanel>
</Grid>
""";

    public const string ViewBox = """
<Grid xmlns="https://github.com/avaloniaui">
    <Viewbox MaxWidth="500" MaxHeight="500" StretchDirection="Both" Stretch="Fill">
        <Grid Width="220" Height="120">
            <Ellipse Fill="#99ccff" Stroke="RoyalBlue" StrokeDashArray="3" />
            <TextBlock Text="Viewbox" HorizontalAlignment="Center" VerticalAlignment="Center" />
        </Grid>
    </Viewbox>
</Grid>
""";

    public const string StackPanel = """
<Grid xmlns="https://github.com/avaloniaui">
    <StackPanel>
        <Border Background="SkyBlue" BorderBrush="Black" BorderThickness="1"><TextBlock FontSize="12">Stacked Item #1</TextBlock></Border>
        <Border Width="400" Background="CadetBlue" BorderBrush="Black" BorderThickness="1"><TextBlock FontSize="14">Stacked Item #2</TextBlock></Border>
        <Border Background="#ffff99" BorderBrush="Black" BorderThickness="1"><TextBlock FontSize="16">Stacked Item #3</TextBlock></Border>
        <Border Width="200" Background="PaleGreen" BorderBrush="Black" BorderThickness="1"><TextBlock FontSize="18">Stacked Item #4</TextBlock></Border>
        <Border Background="White" BorderBrush="Black" BorderThickness="1"><TextBlock FontSize="20">Stacked Item #5</TextBlock></Border>
    </StackPanel>
</Grid>
""";

    public const string WrapPanel = """
<Grid xmlns="https://github.com/avaloniaui">
    <WrapPanel ItemWidth="90" ItemHeight="40" Margin="10">
        <Button Content="One" /><Button Content="Two" /><Button Content="Three" />
        <Button Content="Four" /><Button Content="Five" /><Button Content="Six" />
    </WrapPanel>
</Grid>
""";

    public const string UniformGrid = """
<Grid xmlns="https://github.com/avaloniaui">
    <Grid Margin="10">
        <Grid.RowDefinitions><RowDefinition Height="*" /><RowDefinition Height="*" /></Grid.RowDefinitions>
        <Grid.ColumnDefinitions><ColumnDefinition Width="*" /><ColumnDefinition Width="*" /><ColumnDefinition Width="*" /></Grid.ColumnDefinitions>
        <Button Content="1" /><Button Content="2" /><Button Content="3" />
        <Button Grid.Row="1" Content="4" /><Button Grid.Row="1" Grid.Column="1" Content="5" /><Button Grid.Row="1" Grid.Column="2" Content="6" />
    </Grid>
</Grid>
""";

    public const string Button = """
<Grid xmlns="https://github.com/avaloniaui">
    <Button HorizontalAlignment="Center" VerticalAlignment="Center" Content="Click Me" />
</Grid>
""";

    public const string CheckBox = """
<Grid xmlns="https://github.com/avaloniaui">
    <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center">
        <CheckBox Name="cb1" Content="Check Box" />
        <TextBlock Text="CheckBox supports true, false and null states." />
    </StackPanel>
</Grid>
""";

    public const string ComboBox = """
<Grid xmlns="https://github.com/avaloniaui">
    <ComboBox HorizontalAlignment="Center" VerticalAlignment="Center" SelectedIndex="0">
        <ComboBoxItem>Item 1</ComboBoxItem>
        <ComboBoxItem>Item 2</ComboBoxItem>
        <ComboBoxItem>Item 3</ComboBoxItem>
    </ComboBox>
</Grid>
""";

    public const string Expander = """
<Grid xmlns="https://github.com/avaloniaui">
    <Expander Width="180" HorizontalAlignment="Left" VerticalAlignment="Top" Header="Header" IsExpanded="False">
        <TextBlock Margin="10">Expanded content</TextBlock>
    </Expander>
</Grid>
""";

    public const string Image = """
<Grid xmlns="https://github.com/avaloniaui">
    <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center" Text="Image Source can be set to an app asset path." />
</Grid>
""";

    public const string ListBox = """
<Grid xmlns="https://github.com/avaloniaui">
    <ListBox VerticalAlignment="Center" HorizontalAlignment="Center" SelectedIndex="0">
        <ListBoxItem>Item 1</ListBoxItem>
        <ListBoxItem>Item 2</ListBoxItem>
        <ListBoxItem>Item 3</ListBoxItem>
    </ListBox>
</Grid>
""";

    public const string Menu = """
<Grid xmlns="https://github.com/avaloniaui">
    <Menu Background="Gray" HorizontalAlignment="Center" VerticalAlignment="Center">
        <MenuItem Header="File">
            <MenuItem Header="New" />
            <MenuItem Header="Open" />
            <Separator />
            <MenuItem Header="Submenu">
                <MenuItem Header="Submenu item 1" />
                <MenuItem Header="Submenu item 2" />
            </MenuItem>
        </MenuItem>
        <MenuItem Header="View"><MenuItem Header="Source" /></MenuItem>
    </Menu>
</Grid>
""";

    public const string PasswordBox = """
<Grid xmlns="https://github.com/avaloniaui">
    <TextBox PasswordChar="*" HorizontalAlignment="Center" VerticalAlignment="Center" PlaceholderText="Password" />
</Grid>
""";

    public const string RadioButton = """
<Grid xmlns="https://github.com/avaloniaui">
    <RadioButton HorizontalAlignment="Center" VerticalAlignment="Center" FontSize="14" Content="Radio Button" />
</Grid>
""";

    public const string ScrollViewer = """
<Grid xmlns="https://github.com/avaloniaui">
    <ScrollViewer>
        <StackPanel VerticalAlignment="Top" HorizontalAlignment="Left">
            <TextBlock TextWrapping="Wrap" Margin="0,0,0,20">Scrolling is enabled when it is necessary.</TextBlock>
            <Rectangle Fill="Red" Width="400" Height="800" />
        </StackPanel>
    </ScrollViewer>
</Grid>
""";

    public const string Slider = """
<Grid xmlns="https://github.com/avaloniaui">
    <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center" Width="180">
        <Slider Minimum="0" Maximum="100" Value="45" />
        <TextBlock Text="Slider value can be data-bound." />
    </StackPanel>
</Grid>
""";

    public const string TabControl = """
<Grid xmlns="https://github.com/avaloniaui">
    <TabControl HorizontalAlignment="Center" VerticalAlignment="Center">
        <TabItem Header="Background"><Button Background="LightGray" Content="Background" /></TabItem>
        <TabItem Header="Foreground"><Button Foreground="Black" Content="Foreground" /></TabItem>
        <TabItem Header="FontSize"><Button FontSize="14" Content="FontSize" /></TabItem>
    </TabControl>
</Grid>
""";

    public const string TextBlock = """
<Grid xmlns="https://github.com/avaloniaui">
    <TextBlock Background="Orange" FontFamily="Verdana" FontSize="14" FontWeight="Bold"
            HorizontalAlignment="Center" VerticalAlignment="Center">Hello World!</TextBlock>
</Grid>
""";

    public const string TextBox = """
<Grid xmlns="https://github.com/avaloniaui">
    <TextBox VerticalAlignment="Center" HorizontalAlignment="Center" Text="Some text to select..." />
</Grid>
""";

    public const string ToolTip = """
<Grid xmlns="https://github.com/avaloniaui">
    <TextBox HorizontalAlignment="Center" VerticalAlignment="Center" Text="TextBox with ToolTip"
            ToolTip.Tip="Useful information goes here" />
</Grid>
""";

    public const string SplitView = """
<Grid xmlns="https://github.com/avaloniaui">
    <SplitView IsPaneOpen="True" OpenPaneLength="160">
        <SplitView.Pane><Border Background="#ddeeff" Padding="12"><TextBlock>Navigation pane</TextBlock></Border></SplitView.Pane>
        <Border Padding="12"><TextBlock>Main content area</TextBlock></Border>
    </SplitView>
</Grid>
""";

    public const string ToggleSwitch = """
<Grid xmlns="https://github.com/avaloniaui">
    <ToggleSwitch HorizontalAlignment="Center" VerticalAlignment="Center" Content="Avalonia ToggleSwitch" IsChecked="True" />
</Grid>
""";

    public const string CalendarDatePicker = """
<Grid xmlns="https://github.com/avaloniaui">
    <CalendarDatePicker HorizontalAlignment="Center" VerticalAlignment="Center" PlaceholderText="Choose a date" />
</Grid>
""";

    public const string TransitioningContentControl = """
<Grid xmlns="https://github.com/avaloniaui">
    <TransitioningContentControl HorizontalAlignment="Center" VerticalAlignment="Center" Content="Animated content transitions" />
</Grid>
""";

    public const string NativeMenu = """
<Grid xmlns="https://github.com/avaloniaui">
    <TextBlock TextWrapping="Wrap" Text="NativeMenu is attached to a Window. Its visual rendering depends on the desktop platform." />
</Grid>
""";
}
