using System;
using Coddee.Mvvm;

namespace Coddee.Xamarin.Services.Navigation
{
    /// <summary>
    /// A navigation item for navigating application sections.
    /// </summary>
    public interface INavigationItem : IInitializable
    {
        /// <summary>
        /// The type of the destination targeted by the item.
        /// </summary>
        Type DestinationType { get; }

        /// <summary>
        /// Is the destination instance created.
        /// </summary>
        bool DestinationResolved { get; }

        /// <summary>
        /// Indicates if the navigation item is currently active.
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Show the navigation item title.
        /// </summary>
        bool ShowTitle { get; set; }

        /// <summary>
        /// Is the content initialized.
        /// </summary>
        bool IsContentInitialized { get; }

        /// <summary>
        /// Indicates if the navigation item is visible in the navigation bar.
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// Navigate to this item.
        /// </summary>
        void Navigate();

        /// <summary>
        /// Initialize the content of the item.
        /// </summary>
        void InitializeContent();

        /// <summary>
        /// Set the destination object.
        /// </summary>
        void SetDestination(IPresentable destination);

        /// <summary>
        /// Triggered when the navigation to this item is requested.
        /// </summary>
        event EventHandler<IPresentable> NavigationRequested;

        /// <summary>
        /// Triggered when the content of the item is initialized.
        /// </summary>
        event EventHandler ContentInitialized;
    }
}