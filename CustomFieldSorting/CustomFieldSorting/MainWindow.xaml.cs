namespace CustomFieldSorting
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Syncfusion.UI.Xaml.Kanban;
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        #region Fields

        /// <summary>
        /// The kanban selected card. 
        /// </summary>
        private KanbanCardItem selectedCard;

        /// <summary>
        /// The Kanban column where the card is dropped.
        /// </summary>

        private KanbanColumn targetColumn;

        /// <summary>
        /// To store initial Kanban SortingMappingPath value
        /// </summary>
        private string? sortingMappingPathValue;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
            this.sortOrderCombobox.ItemsSource = new ObservableCollection<string>() { "Ascending", "Descending" };
            this.sortOrderCombobox.SelectedIndex = 0;
            this.mappingPathCombobox.ItemsSource = new ObservableCollection<string>() { "Title", "Priority" };
            this.mappingPathCombobox.SelectedIndex = 0;
            this.kanban.CardDragStarting += this.OnKanbanCardDragStarting;
            this.kanban.CardDrop += OnKanbanCardDrop;
        }

        #endregion

        #region Property changed

        /// <summary>
        /// Occurs when the sorting order value is changed.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="e">The event args.</param>
        private void OnSortOrderSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (sender as ComboBox)?.SelectedItem?.ToString();
            if (this.kanban == null || selectedItem == null)
            {
                return;
            }

            if (Enum.TryParse<KanbanSortingOrder>(selectedItem.ToString(), out KanbanSortingOrder sortOrder))
            {
                this.kanban.SortingOrder = sortOrder;
            }
        }

        private void OnMappingPathSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (sender as ComboBox)?.SelectedItem?.ToString();
            if (this.kanban == null || string.IsNullOrEmpty(selectedItem))
            {
                return;
            }

            this.kanban.SortingMappingPath = selectedItem;
            this.sortingMappingPathValue = selectedItem;
        }

        /// <summary>
        /// Occurs when a card drag event is started.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="e">The event args.</param>
        private void OnKanbanCardDragStarting(object sender, KanbanCardDragStartingEventArgs e)
        {
            this.selectedCard = e.Card;
        }

        /// <summary>
        /// Occurs when a card drag end event is completed.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="e">The event args.</param>
        private void OnKanbanCardDrop(object? sender, KanbanCardDropEventArgs e)
        {
            this.targetColumn = e.TargetColumn;
            if (this.kanban == null)
            {
                return;
            }

            this.kanban.RefreshKanbanColumn(this.targetColumn.AllowedTransitionCategory.ToString());
        }

        #endregion
    }
}
