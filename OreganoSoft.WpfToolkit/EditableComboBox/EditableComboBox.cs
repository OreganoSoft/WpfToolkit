using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace OreganoSoft.WpfToolkit.EditableComboBox
{

    [TemplatePart(Name = EditableComboBox.PartEditor, Type = typeof(TextBox))]
    [TemplatePart(Name = EditableComboBox.PartPopup, Type = typeof(Popup))]
    [TemplatePart(Name = EditableComboBox.PartSelector, Type = typeof(Selector))]
    [TemplatePart(Name = EditableComboBox.PartSelectedItemTemplate, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = EditableComboBox.PartToggleButton, Type = typeof(ToggleButton))]
    public class EditableComboBox : Control, IDisposable
    {
        #region Disposable Methods

        /// <summary>
        /// The disposed
        /// </summary>
        bool _disposed;

        // Instantiate a SafeHandle instance.
        /// <summary>
        /// The handle
        /// </summary>
        readonly SafeHandle _handle = new SafeFileHandle(IntPtr.Zero, true);
        // Public implementation of Dispose pattern callable by consumers.
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        // Protected implementation of Dispose pattern.
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                _handle.Dispose();
            }
            _disposed = true;
        }
        //Descructor to release memory
        ~EditableComboBox()
        {
            this.Dispose(false);
        }
        #endregion
        public const string PartEditor = "PART_Editor";
        public const string PartPopup = "PART_Popup";
        public const string PartSelector = "PART_Selector";
        public const string PartSelectedItemTemplate = "PART_SelectedItemTemplate";
        public const string PartToggleButton = "PART_ToggleButton";
        public const string PartSelectedItem = "PART_SelecctedItem";

        public static readonly DependencyProperty SelectedItemVisibilityProperty = DependencyProperty.Register("SelectedItemVisibility", typeof(Visibility), typeof(EditableComboBox), new FrameworkPropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark", typeof(string), typeof(EditableComboBox), new FrameworkPropertyMetadata(string.Empty));
        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register("SearchText", typeof(string), typeof(EditableComboBox), new FrameworkPropertyMetadata(string.Empty));
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(EditableComboBox), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty SelectedItemTemplateProperty = DependencyProperty.Register("SelectedItemTemplate", typeof(DataTemplate), typeof(EditableComboBox), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(EditableComboBox));
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(EditableComboBox), new FrameworkPropertyMetadata(null, SelectedItemChanged));

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable<object>), typeof(EditableComboBox), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty SearchTextLengthProperty = DependencyProperty.Register("SearchTextLength", typeof(int), typeof(EditableComboBox), new FrameworkPropertyMetadata(3));
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(EditableComboBox), new FrameworkPropertyMetadata(false));

        private TextBox _editor;

        private Selector _itemsSelector;

        private Popup _popup;

        public Visibility SelectedItemVisibility
        {
            get { return (Visibility)GetValue(SelectedItemVisibilityProperty); }
            set { SetValue(SelectedItemVisibilityProperty, value); }
        }

        public IEnumerable<object> ItemsSource
        {
            get { return (IEnumerable<object>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public TextBox Editor
        {
            get { return _editor; }
            set
            {
                _editor = value;
            }
        }

        private Grid selectedGrid;

        public Grid SelectedGrid
        {
            get { return selectedGrid; }
            set { selectedGrid = value; }
        }

        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        private static object SelectedItemChanged(DependencyObject d, object baseValue)
        {
            if (d is EditableComboBox comboBox && baseValue != null)
            {
                comboBox.TextBoxVisibilities();
            }
            return baseValue;
        }

        public Selector ItemsSelector
        {
            get { return _itemsSelector; }
            set { _itemsSelector = value; }
        }

        private ContentPresenter selectedItemPresenter;
        public ContentPresenter SelectedItemPresenter
        {
            get { return selectedItemPresenter; }
            set { selectedItemPresenter = value; }
        }

        private ToggleButton toggleButton;
        public ToggleButton ToggleSwitch
        {
            get { return toggleButton; }
            set { toggleButton = value; }
        }

        private TextBlock waterMarkTextBlock;
        public TextBlock WaterMarkTextBlock
        {
            get { return waterMarkTextBlock; }
            set { waterMarkTextBlock = value; }
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public DataTemplate SelectedItemTemplate
        {
            get { return (DataTemplate)GetValue(SelectedItemTemplateProperty); }
            set { SetValue(SelectedItemTemplateProperty, value); }
        }

        public DataTemplateSelector ItemTemplateSelector
        {
            get { return ((DataTemplateSelector)(GetValue(EditableComboBox.ItemTemplateSelectorProperty))); }
            set { SetValue(EditableComboBox.ItemTemplateSelectorProperty, value); }
        }

        public int SearchTextLength
        {
            get { return (int)GetValue(SearchTextLengthProperty); }
            set { SetValue(EditableComboBox.SearchTextLengthProperty, value); }
        }

        public Popup Popup
        {
            get { return _popup; }
            set { _popup = value; }
        }

        public bool IsToogleButtonClicked { get; set; }

        private void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is EditableComboBox comboBox)
            {
                if (!comboBox.IsKeyboardFocusWithin)
                {
                    comboBox.Focus();
                }
                if (comboBox.ItemsSelector.SelectedItem == null)
                    ComboBoxVisibilities(comboBox);
                if (IsToogleButtonClicked)
                    ComboBoxVisibilities(comboBox);
                if (e.LeftButton == MouseButtonState.Pressed &&
                    comboBox.ItemsSelector.IsMouseOver &&
                    comboBox.ItemsSelector.SelectedItem != null)
                {
                    comboBox.SetSelectedItem();
                }
            }
            else
            {
                IsDropDownOpen = false;
                if (SelectedItem != null)
                    TextBoxVisibilities();
            }
        }

        private void ComboBoxVisibilities(EditableComboBox comboBox)
        {
            comboBox.SelectedItemVisibility = Visibility.Collapsed;
            comboBox.Editor.Visibility = Visibility.Visible;
            comboBox.Editor.Focus();
            comboBox.IsDropDownOpen = true;
        }

        public EditableComboBox()
        {
            this.LostFocus += EditableComboBox_LostFocus;
            EventManager.RegisterClassHandler(typeof(EditableComboBox), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButtonDown), true);
            EventManager.RegisterClassHandler(typeof(Window), MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseButtonDown), false);
        }

        private void EditableComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!this.IsKeyboardFocusWithin)
                this.IsDropDownOpen = false;
            if (SelectedItem != null)
            {
                IsToogleButtonClicked = false;
                this.ItemsSelector.SelectedItem = this.SelectedItem;
                TextBoxVisibilities();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Editor = Template.FindName(PartEditor, this) as TextBox;
            Popup = Template.FindName(PartPopup, this) as Popup;
            ItemsSelector = Template.FindName(PartSelector, this) as Selector;
            SelectedItemPresenter = Template.FindName(PartSelectedItemTemplate, this) as ContentPresenter;
            ToggleSwitch = Template.FindName(PartToggleButton, this) as ToggleButton;
            WaterMarkTextBlock = Template.FindName("PART_Watermark", this) as TextBlock;
            if (ToggleSwitch != null) ToggleSwitch.Click += OpenPopup;
            if (this.Editor != null)
            {
                this.Editor.TextChanged += Editor_TextChanged;
                this.Editor.LostFocus += Editor_LostFocus;
                this.Editor.PreviewKeyDown += Editor_PreviewKeyDown;
                this.ItemsSelector.PreviewKeyDown += Editor_PreviewKeyDown;
            }
        }

        private void OpenPopup(object sender, RoutedEventArgs e)
        {
            IsToogleButtonClicked = true;
            WaterMarkTextBlock.Visibility = (string.IsNullOrEmpty(this.SearchText)) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.IsDropDownOpen = true;
            WaterMarkTextBlock.Visibility = (string.IsNullOrEmpty(this.SearchText)) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Popup_Closed(object sender, EventArgs e)
        {
            if (this.ItemsSelector.SelectedItem != null)
            {
                SetSelectedItem();
            }
        }

        private void Editor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HandleKeyboard(e);
        }

        private void HandleKeyboard(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                    MoveDown();
                    break;
                case Key.Up:
                    MoveUp();
                    break;
                case Key.Enter:
                    SetSelectedItem();
                    break;
                case Key.Escape:
                    IsDropDownOpen = false;
                    this.SelectedItem = this.SelectedItem;
                    if (SelectedItem != null)
                        TextBoxVisibilities();
                    break;
            }
        }

        private void TextBoxVisibilities()
        {
            this.SelectedItemVisibility = Visibility.Visible;
            this.Editor.Visibility = this.WaterMarkTextBlock.Visibility = Visibility.Collapsed;
        }

        private void ScrollToSelectedItem()
        {
            ListBox listBox = ItemsSelector as ListBox;
            if (listBox != null && listBox.SelectedItem != null)
                listBox.ScrollIntoView(listBox.SelectedItem);
        }

        private void SetSelectedItem()
        {
            this.SelectedItem = ItemsSelector.SelectedItem;
            this.IsDropDownOpen = false;
            IsToogleButtonClicked = false;
            if (this.SelectedItem != null)
                TextBoxVisibilities();
            else if (SearchText == null)
            {
                this.SelectedItemVisibility = Visibility.Collapsed;
                this.Editor.Visibility = this.WaterMarkTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void MoveDown()
        {
            if (ItemsSelector.SelectedIndex == ItemsSelector.Items.Count - 1)
                ItemsSelector.SelectedIndex = 0;
            else
                ItemsSelector.SelectedIndex += 1;

            ScrollToSelectedItem();
        }

        private void MoveUp()
        {
            if (ItemsSelector.SelectedIndex == 0 || ItemsSelector.SelectedIndex == -1)
                ItemsSelector.SelectedIndex = ItemsSelector.Items.Count - 1;
            else
                ItemsSelector.SelectedIndex -= 1;

            ScrollToSelectedItem();
        }

        private void Editor_LostFocus(object sender, RoutedEventArgs e)
        {
            this.IsDropDownOpen = false;
        }
    }
}
