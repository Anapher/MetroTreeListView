using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TreeListView.Extensions;

namespace TreeListView
{
    //Based on https://blogs.msdn.microsoft.com/atc_avalon_team/2006/03/01/treelistview-show-hierarchy-data-with-details-in-columns/
    public class TreeListView : TreeView
    {
        public static readonly DependencyProperty ViewProperty = DependencyProperty.Register(
            "View", typeof (GridView), typeof (TreeListView), new PropertyMetadata(default(GridView), OnViewChanged));

        public static readonly DependencyProperty SelectItemOnRightClickProperty = DependencyProperty.Register(
            "SelectItemOnRightClick", typeof (bool), typeof (TreeListView), new PropertyMetadata(true));

        public static readonly DependencyProperty SelectedItemExProperty = DependencyProperty.Register(
            "SelectedItemEx", typeof (object), typeof (TreeListView), new PropertyMetadata(default(object)));

        public static readonly DependencyProperty ChildrenPropertyNameProperty = DependencyProperty.Register(
            "ChildrenPropertyName", typeof (string), typeof (TreeListView),
            new PropertyMetadata(default(string), OnChildrenPropertyNameChanged));

        //Important: Disable property ItemTemplate
        public static readonly DependencyPropertyKey ReadOnlyItemTemplateProperty = DependencyProperty.RegisterReadOnly(
            "ItemTemplate", typeof (DataTemplate), typeof (TreeListView), new PropertyMetadata(default(DataTemplate)));

        public new static readonly DependencyProperty ItemTemplateProperty
            = ReadOnlyItemTemplateProperty.DependencyProperty;

        private GridViewColumn _currentGridViewColumn;
        private DataTemplate _oldDataTemplate;
        private BindingBase _oldDisplayMemberBindingBase;
        private GridViewColumnCollection _oldGridViewColumnCollection;
	    private DataTemplateSelector _oldDataTemplateSelector;

        static TreeListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListView),
                new FrameworkPropertyMetadata(typeof(TreeListView)));
        }

        public TreeListView()
        {
            SelectedItemChanged += OnSelectedItemChanged;
        }

        public new DataTemplate ItemTemplate
        {
            get => (DataTemplate) GetValue(ItemTemplateProperty);
	        protected set => SetValue(ItemTemplateProperty, value);
        }

        public string ChildrenPropertyName
        {
            get => (string) GetValue(ChildrenPropertyNameProperty);
	        set => SetValue(ChildrenPropertyNameProperty, value);
        }

        public GridView View
        {
            get => (GridView) GetValue(ViewProperty);
	        set => SetValue(ViewProperty, value);
        }

        public object SelectedItemEx
        {
            get => GetValue(SelectedItemExProperty);
	        set => SetValue(SelectedItemExProperty, value);
        }

        public bool SelectItemOnRightClick
        {
            get => (bool) GetValue(SelectItemOnRightClickProperty);
	        set => SetValue(SelectItemOnRightClickProperty, value);
        }

        private static void OnViewChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var treeListView = (TreeListView) dependencyObject;
            var gridView = (GridView) dependencyPropertyChangedEventArgs.NewValue;
            treeListView.OnUpdateGridView(gridView?.Columns);
        }

        private void OnUpdateGridView(GridViewColumnCollection gridViewColumnCollection)
        {
            var isOldGridView = gridViewColumnCollection == _oldGridViewColumnCollection;

            if (!isOldGridView && _oldGridViewColumnCollection != null)
            {
                //unsubscribe old GridView
                _oldGridViewColumnCollection.CollectionChanged -= ColumnsOnCollectionChanged;
                ResetCurrentGridViewColumn();
                _oldGridViewColumnCollection = null;
            }

            if (gridViewColumnCollection == null)
                return;

            if (!isOldGridView)
                gridViewColumnCollection.CollectionChanged += ColumnsOnCollectionChanged;

            if (gridViewColumnCollection.Count == 0)
                return;

            var firstColumn = gridViewColumnCollection[0];
            ResetCurrentGridViewColumn();
            _currentGridViewColumn = firstColumn;

	        _oldDataTemplate = firstColumn.CellTemplate;
	        _oldDataTemplateSelector = firstColumn.CellTemplateSelector;
	        _oldDisplayMemberBindingBase = firstColumn.DisplayMemberBinding;

	        var spFactory = new FrameworkElementFactory(typeof(ContentPresenter));
	        spFactory.SetBinding(ContentPresenter.ContentProperty, firstColumn.DisplayMemberBinding ?? new Binding("."));
	        if (firstColumn.CellTemplate != null)
		        spFactory.SetValue(ContentPresenter.ContentTemplateProperty, firstColumn.CellTemplate);
			else if (firstColumn.CellTemplateSelector != null)
		        spFactory.SetValue(ContentPresenter.ContentTemplateSelectorProperty, firstColumn.CellTemplateSelector);

	        spFactory.SetBinding(MarginProperty,
		        new Binding
		        {
			        RelativeSource =
				        new RelativeSource(RelativeSourceMode.FindAncestor, typeof(TreeListViewItem), 1),
			        Converter = (IValueConverter)Application.Current.Resources["LengthConverter"]
		        });

	        var dataTemplate = new DataTemplate {VisualTree = spFactory};
	        firstColumn.DisplayMemberBinding = null;
	        firstColumn.CellTemplateSelector = null;
	        firstColumn.CellTemplate = dataTemplate;

            _oldGridViewColumnCollection = gridViewColumnCollection;
        }

        private void ResetCurrentGridViewColumn()
        {
            if (_currentGridViewColumn == null)
                return;

	        _currentGridViewColumn.CellTemplate = _oldDataTemplate;
	        _currentGridViewColumn.DisplayMemberBinding = _oldDisplayMemberBindingBase;
	        _currentGridViewColumn.CellTemplateSelector = _oldDataTemplateSelector;

            _oldDataTemplate = null;
            _oldDisplayMemberBindingBase = null;
	        _oldDataTemplateSelector = null;
        }

        private void ColumnsOnCollectionChanged(object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            OnUpdateGridView((GridViewColumnCollection) sender);
        }

        private void OnSelectedItemChanged(object sender,
            RoutedPropertyChangedEventArgs<object> routedPropertyChangedEventArgs)
        {
            SelectedItemEx = routedPropertyChangedEventArgs.NewValue;
        }

        private static void OnChildrenPropertyNameChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var treeListView = (TreeListView) dependencyObject;
            var newValue = (string) dependencyPropertyChangedEventArgs.NewValue;
            treeListView.UpdateItemTemplate(new HierarchicalDataTemplate {ItemsSource = new Binding(newValue)});
        }

        private void UpdateItemTemplate(DataTemplate dataTemplate)
        {
            base.ItemTemplate = dataTemplate;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonDown(e);
            if (!SelectItemOnRightClick)
                return;

            var treeListViewItem =
                WpfExtensions.VisualUpwardSearch<TreeListViewItem>(e.OriginalSource as DependencyObject);
            if (treeListViewItem != null)
            {
                treeListViewItem.Focus();
                treeListViewItem.IsSelected = true;
                e.Handled = true;
            }
        }
    }
}
