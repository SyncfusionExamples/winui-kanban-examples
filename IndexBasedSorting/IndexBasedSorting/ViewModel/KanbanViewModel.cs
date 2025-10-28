namespace IndexBasedSorting
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents the ViewModel responsible for managing a collection of Kanban cards with sorting functionality.
    /// </summary>
    public class KanbanViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KanbanViewModel"/> class.
        /// </summary>
        public KanbanViewModel()
        {
            this.Cards = new ObservableCollection<CardDetails>()
            {
                new CardDetails() { Title = "Task - 1", Index = 5, Category = "Open", Description = "Fix the issue reported in the Edge browser." },
                new CardDetails() { Title = "Task - 3", Index = 9, Category = "In Progress", Description = "Analyze the new requirements gathered from the customer." },
                new CardDetails() { Title = "Task - 4", Index = 2, Category = "Open", Description = "Arrange a web meeting with the customer to get new requirements." },
                new CardDetails() { Title = "Task - 2", Index = 1, Category = "In Progress", Description = "Test the application in the Edge browser." },
                new CardDetails() { Title = "Task - 5", Index = 8, Category = "Done", Description = "Enhance editing functionality." },
                new CardDetails() { Title = "Task - 8", Index = 3, Category = "In Progress", Description = "Improve application performance." },
                new CardDetails() { Title = "Task - 9", Index = 6, Category = "Done", Description = "Improve the performance of the editing functionality." },
                new CardDetails() { Title = "Task - 10", Index = 4, Category = "Open", Description = "Analyze grid control." },
                new CardDetails() { Title = "Task - 12", Index = 7, Category = "Done", Description = "Analyze stored procedures." }
            };
        }

        /// <summary>
        /// Gets or sets the collection of <see cref="CardDetails"/> objects representing cards in various stages.
        /// </summary>
        public ObservableCollection<CardDetails> Cards { get; set; }
    }
}
