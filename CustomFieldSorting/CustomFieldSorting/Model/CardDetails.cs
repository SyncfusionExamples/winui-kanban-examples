namespace CustomFieldSorting
{
    using System.ComponentModel;

    /// <summary>
    /// Represents a Kanban card with sorting related information such as title, category, priority, and index.
    /// </summary>
    public class CardDetails : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// The title of the card.
        /// </summary>
        private string? _title;

        /// <summary>
        /// The description of the card.
        /// </summary>
        private string? _description;

        /// <summary>
        /// The category of the card. This property is used for column mapping in the Kanban board.
        /// </summary>
        private string? _category;

        /// <summary>
        /// The priority of the card. This property can be used for additional sorting or filtering.
        /// </summary>
        private Priority? _priority;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the title of the card.
        /// </summary>
        public string? Title
        {
            get { return this._title; }
            set
            {
                this._title = value;
                this.OnPropertyChanged(nameof(Title));
            }
        }

        /// <summary>
        /// Gets or sets the description of the card.
        /// </summary>
        public string? Description
        {
            get { return this._description; }
            set
            {
                this._description = value;
                this.OnPropertyChanged(nameof(Description));
            }
        }

        /// <summary>
        /// Gets or sets the category of the card. This property is used for column mapping in the Kanban board.
        /// </summary>
        public string? Category
        {
            get { return this._category; }
            set
            {
                this._category = value;
                this.OnPropertyChanged(nameof(Category));
            }
        }

        /// <summary>
        /// Gets or sets the priority of the card. This property can be used for additional sorting or filtering.
        /// </summary>
        public Priority? Priority
        {
            get { return this._priority; }
            set
            {
                this._priority = value;
                this.OnPropertyChanged(nameof(Priority));
            }
        }

        #endregion

        #region Event

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Methods

        /// <summary>
        /// Invokes the event when the value of a property has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}