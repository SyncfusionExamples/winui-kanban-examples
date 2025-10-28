namespace IndexBasedSorting
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Data;
    using Syncfusion.UI.Xaml.Kanban;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

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
        private KanbanColumn? targetColumn;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.selectedCard = new KanbanCardItem();
            this.sortOrderComboBox.ItemsSource = new ObservableCollection<string>() { "Ascending", "Descending" };
            this.sortOrderComboBox.SelectedIndex = 0;
            this.sortOrderComboBox.SelectionChanged += OnSortOrderComboBoxSelectionChanged;
            this.kanban.CardDragStarting += OnKanbanCardDragStarting;
            this.kanban.CardDrop += OnKanbanCardDrop;
        }

        #endregion

        #region Property changed

        /// <summary>
        /// Occurs when the sorting order value is changed.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="e">The event args.</param>
        private void OnSortOrderComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
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

        /// <summary>
        /// Occurs when a card drag event is started.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="e">The event args.</param>
        private void OnKanbanCardDragStarting(object? sender, KanbanCardDragStartingEventArgs e)
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

            this.ApplySortingWithoutPositionChange(e);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Applies sorting without changing the position of the cards.
        /// </summary>
        /// <param name="eventArgs">The event args.</param>
        private void ApplySortingWithoutPositionChange(KanbanCardDropEventArgs e)
        {
            var cardItems = this.targetColumn?.Items.Cast<KanbanCardItem>().ToList();
            if (this.kanban == null || this.selectedCard == null || this.selectedCard.Content == null || this.targetColumn == null || this.targetColumn.AllowedTransitionCategory == null || cardItems == null || !cardItems.Any()
                || e.SourceColumn == null || (e.SourceColumn == e.TargetColumn && e.PreviousCardIndex == e.TargetCardIndex))
            {
                return;
            }

            // Retrieve sorting configuration
            var sortMappingPath = this.kanban.SortingMappingPath;
            var sortingOrder = this.kanban.SortingOrder;
            CardDetails? cardDetails = this.selectedCard.Content as CardDetails;

            // Proceed only if sorting path is defined
            if (cardDetails == null || string.IsNullOrEmpty(sortMappingPath))
            {
                return;
            }

            // Extract items from the target column
            var targetColumnItems = cardItems.Where(card => card != null && card.Content != null).ToList();

            // Sort items based on the sorting order
            if (targetColumnItems.Count > 0)
            {
                Func<KanbanCardItem, object> keySelector = item => this.GetPropertyInfo(item.GetType(), sortMappingPath);
                targetColumnItems = sortingOrder == KanbanSortingOrder.Ascending
                    ? targetColumnItems.OrderBy(item => keySelector(item) ?? 0).ToList()
                    : targetColumnItems.OrderByDescending(item => keySelector(item) ?? 0).ToList();
            }

            // Determine the index to insert the dragged card.
            int currentCardIndex = e.TargetCardIndex;

            if (e.SourceColumn != e.TargetColumn)
            {
                cardDetails.Category = this.targetColumn.AllowedTransitionCategory.ToString();
            }

            if (currentCardIndex >= 0 && currentCardIndex <= targetColumnItems.Count)
            {
                targetColumnItems.Insert(currentCardIndex, this.selectedCard);
            }
            else
            {
                targetColumnItems.Add(this.selectedCard);
                currentCardIndex = targetColumnItems.Count - 1;
            }

            // Update index property of all items using smart positioning logic
            this.ApplySmartIndexUpdate(targetColumnItems, e.TargetCardIndex);
        }

        /// <summary>
        /// Updates index property with smart positioning based on sorting order.
        /// </summary>
        /// <param name="items">The list of Kanban card items to be reordered.</param>
        /// <param name="droppedIndex">The index position where the card was dropped.</param>
        private void ApplySmartIndexUpdate(List<KanbanCardItem> items, int droppedIndex)
        {
            if (this.kanban == null || items == null || items.Count == 0)
            {
                return;
            }

            if (this.kanban.SortingOrder == KanbanSortingOrder.Ascending)
            {
                this.HandleAscendingIndexSorting(items, droppedIndex);
                return;
            }

            this.HandleDescendingIndexSorting(items, droppedIndex);
        }

        /// <summary>
        /// Method to handle ascending index sorting with smart positioning.
        /// </summary>
        /// <param name="items">The list of items to update.</param>
        /// <param name="currentCardIndex">The current card index.</param>
        private void HandleAscendingIndexSorting(List<KanbanCardItem> items, int currentCardIndex)
        {
            int afterCardIndex = -1;
            int lastItemIndex = -1;

            // Get the index of the card after the insertion point
            if (currentCardIndex < items.Count - 1)
            {
                var afterCard = items[currentCardIndex + 1];
                afterCardIndex = this.GetCardIndex(afterCard?.Content) ?? -1;
            }

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i].Content;
                if (item == null)
                {
                    continue;
                }

                PropertyInfo? propertyInfo = this.GetPropertyInfo(item.GetType(), "Index");
                if (propertyInfo == null)
                {
                    continue;
                }

                int itemIndex = Convert.ToInt32(propertyInfo.GetValue(item) ?? 0);
                bool isFirstItem = i == 0;
                if (isFirstItem)
                {
                    // If the inserted card is at the beginning, assign a smart index
                    if (currentCardIndex == 0)
                    {
                        lastItemIndex = afterCardIndex > 1 ? afterCardIndex - 1 : 1;
                        propertyInfo.SetValue(item, lastItemIndex);
                    }
                    else
                    {
                        lastItemIndex = itemIndex;
                    }
                }
                else
                {
                    // Increment index for subsequent items
                    lastItemIndex++;
                    propertyInfo.SetValue(item, lastItemIndex);
                }
            }
        }

        /// <summary>
        /// Method to handle descending index sorting with smart positioning.
        /// </summary>
        /// <param name="items">The list of items to update.</param>
        /// <param name="currentCardIndex">The current card index.</param>
        private void HandleDescendingIndexSorting(List<KanbanCardItem> items, int currentCardIndex)
        {
            int beforeCardIndex = -1;
            int lastItemIndex = -1;

            // Get the index of the card before the insertion point
            if (currentCardIndex > 0 && currentCardIndex < items.Count)
            {
                var cardBefore = items[currentCardIndex - 1];
                beforeCardIndex = this.GetCardIndex(cardBefore?.Content) ?? -1;
            }

            for (int i = items.Count - 1; i >= 0; i--)
            {
                var item = items[i].Content;
                if (item == null)
                {
                    continue;
                }

                PropertyInfo? propertyInfo = this.GetPropertyInfo(item.GetType(), "Index");
                if (propertyInfo == null)
                {
                    continue;
                }

                int itemIndex = Convert.ToInt32(propertyInfo.GetValue(item) ?? 0);
                bool isLastItem = i == items.Count - 1;
                if (isLastItem)
                {
                    // If the inserted card is at the end, assign a smart index
                    if (currentCardIndex == items.Count - 1)
                    {
                        lastItemIndex = beforeCardIndex > 1 ? beforeCardIndex - 1 : 1;
                        propertyInfo.SetValue(item, lastItemIndex);
                    }
                    else
                    {
                        lastItemIndex = itemIndex;
                    }
                }
                else
                {
                    lastItemIndex++;
                    propertyInfo.SetValue(item, lastItemIndex);
                }
            }
        }

        /// <summary>
        /// Method to get the index property value from a card object.
        /// </summary>
        /// <param name="cardDetails">The card object.</param>
        /// <returns>The index value or null if not found.</returns>
        private int? GetCardIndex(object? cardDetails)
        {
            if (cardDetails == null)
            {
                return null;
            }

            PropertyInfo? propertyInfo = this.GetPropertyInfo(cardDetails.GetType(), "Index");
            if (propertyInfo == null)
            {
                return null;
            }

            var indexValue = propertyInfo.GetValue(cardDetails);
            if (indexValue != null)
            {
                return Convert.ToInt32(indexValue);
            }

            return null;
        }

        /// <summary>
        /// Method to get the property info for the specified property.
        /// </summary>
        /// <param name="type">The property type.</param>
        /// <param name="key">The property name.</param>
        /// <returns>The property info of the specified property.</returns>
        private PropertyInfo? GetPropertyInfo(Type type, string key)
        {
            return this.GetPropertyInfoCustomType(type, key);
        }

        /// <summary>
        /// Method to get the property info for the specified property from the given type.
        /// </summary>
        /// <param name="type">The property type.</param>
        /// <param name="key">The property name.</param>
        /// <returns>The property info of the specified property.</returns>
        private PropertyInfo? GetPropertyInfoCustomType(Type type, string key)
        {
            return type.GetProperty(key);
        }

        #endregion
    }


    /// <summary>
    /// A value converter that formats an index value into a rank string representation.
    /// </summary>
    public sealed class RankFormatConverter : IValueConverter
    {
        /// <summary>
        /// Converts an index value to a formatted rank string (e.g., "Rank #1").
        /// </summary>
        /// <param name="value">The value to convert, typically an index or rank number.</param>
        /// <param name="targetType">The target type of the binding (not used).</param>
        /// <param name="parameter">An optional parameter for conversion (not used).</param>
        /// <param name="language">The language of the conversion (not used).</param>
        /// <returns>A formatted string representing the rank, or an empty string if the value is null.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "";
            }

            return $"Rank #{value}";
        }

        /// <summary>
        /// Not implemented. Conversion back from string to value is not supported for one-way binding.
        /// </summary>
        /// <param name="value">The value to convert back (not used).</param>
        /// <param name="targetType">The target type of the binding (not used).</param>
        /// <param name="parameter">An optional parameter for conversion (not used).</param>
        /// <param name="language">The language of the conversion (not used).</param>
        /// <returns>Returns <see cref="Microsoft.UI.Xaml.DependencyProperty.UnsetValue"/> to indicate that conversion is not supported.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Microsoft.UI.Xaml.DependencyProperty.UnsetValue;
        }
    }
}
