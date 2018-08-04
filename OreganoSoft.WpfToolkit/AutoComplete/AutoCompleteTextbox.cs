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

namespace OreganoSoft.WpfToolkit.AutoComplete
{
    [TemplatePart(Name = AutoCompleteTextbox.PartEditor, Type = typeof(TextBox))]
    [TemplatePart(Name = AutoCompleteTextbox.PartPopup, Type = typeof(Popup))]
    [TemplatePart(Name = AutoCompleteTextbox.PartSelector, Type = typeof(Selector))]
    public class AutoCompleteTextbox : Control, IDisposable
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
        ~AutoCompleteTextbox()
        {
            this.Dispose(false);
        }
        #endregion
        public const string PartEditor = "PART_Editor";
        public const string PartPopup = "PART_Popup";
        public const string PartSelector = "PART_Selector";
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark", typeof(string), typeof(AutoCompleteTextbox), new FrameworkPropertyMetadata(string.Empty));
        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register("SearchText", typeof(string), typeof(AutoCompleteTextbox), new FrameworkPropertyMetadata(string.Empty));
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(AutoCompleteTextbox), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty SelectedItemTemplateProperty = DependencyProperty.Register("SelectedItemTemplate", typeof(DataTemplate), typeof(AutoCompleteTextbox), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(AutoCompleteTextbox));
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(AutoCompleteTextbox), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable<object>), typeof(AutoCompleteTextbox), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(AutoCompleteTextbox), new FrameworkPropertyMetadata(false));

        private TextBox _editor;

        private Selector _itemsSelector;

        private Popup _popup;

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
            get { return ((DataTemplateSelector)(GetValue(AutoCompleteTextbox.ItemTemplateSelectorProperty))); }
            set { SetValue(AutoCompleteTextbox.ItemTemplateSelectorProperty, value); }
        }

        public Popup Popup
        {
            get { return _popup; }
            set { _popup = value; }
        }
        public AutoCompleteTextbox()
        {
            EventManager.RegisterClassHandler(typeof(AutoCompleteTextbox), Mouse.MouseDownEvent, new MouseButtonEventHandler(this.OnMouseButtonDown), true);
            EventManager.RegisterClassHandler(typeof(Window), MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.OnMouseButtonDownOnWindow), false);

        }


        private void OnMouseButtonDownOnWindow(object sender, MouseButtonEventArgs e)
        {
            TextBoxMouseDown(sender, e);
        }

        private void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBoxMouseDown(sender, e);
            e.Handled = true;
        }

        private void TextBoxMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is AutoCompleteTextbox txtBox)
            {
                if (this.SearchText != null)
                {

                }
                if (!txtBox.IsKeyboardFocusWithin)
                {
                    txtBox.Focus();
                }
                // focus on texbox showing popup.
                //if (txtBox.ItemsSelector.SelectedItem == null && this.SearchText == null)
                //{
                //    // this.IsDropDownOpen = true;
                //}
                if (e.LeftButton == MouseButtonState.Pressed &&
                    txtBox.ItemsSelector.IsMouseOver &&
                    txtBox.ItemsSelector.SelectedItem != null)
                {
                    txtBox.SetSelectedItem();
                }
            }
            else
            {
                this.IsDropDownOpen = false;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Editor = Template.FindName(PartEditor, this) as TextBox;
            Popup = Template.FindName(PartPopup, this) as Popup;
            ItemsSelector = Template.FindName(PartSelector, this) as Selector;
            WaterMarkTextBlock = Template.FindName("PART_Watermark", this) as TextBlock;
            if (this.Editor != null)
            {
                this.Editor.TextChanged += Editor_TextChanged;
                this.Editor.LostFocus += Editor_LostFocus;
                this.Editor.PreviewKeyDown += Editor_PreviewKeyDown;
                this.ItemsSelector.PreviewKeyDown += Editor_PreviewKeyDown;
            }
            if (SearchText != null)
                this.WaterMarkTextBlock.Visibility = Visibility.Collapsed;
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
                    this.SetSelectedItem();
                    break;
                case Key.Escape:
                    this.IsDropDownOpen = false;
                    break;
            }
        }

        private void SetSelectedItem()
        {
            this.SelectedItem = ItemsSelector.SelectedItem;
            this.IsDropDownOpen = false;
            if (ItemsSelector.SelectedItem != null)
                this.SearchText = ItemsSelector.SelectedItem.ToString();
            if (SearchText == null)
            {
                this.WaterMarkTextBlock.Visibility = Visibility.Visible;
            }
            else this.WaterMarkTextBlock.Visibility = Visibility.Collapsed;
            this.IsDropDownOpen = false;
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
        private void ScrollToSelectedItem()
        {
            if (this.ItemsSelector is ListBox listBox && listBox.SelectedItem != null)
                listBox.ScrollIntoView(listBox.SelectedItem);
        }

        private void Editor_LostFocus(object sender, RoutedEventArgs e)
        {
            this.IsDropDownOpen = false;
        }

        private void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.IsDropDownOpen = true;
            this.WaterMarkTextBlock.Visibility = (string.IsNullOrEmpty(this.SearchText)) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
