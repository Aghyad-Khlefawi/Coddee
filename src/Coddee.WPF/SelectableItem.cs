using System;
using System.Windows.Input;
using Coddee.WPF.Commands;

namespace Coddee.WPF
{
    /// <summary>
    /// A wrapper object that can be selected (using CheckBox-list or radio buttons)
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class SelectableItem<TItem> : BindableBase
    {
        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static SelectableItem<TItem> Create(TItem item)
        {
            return new SelectableItem<TItem>(item);
        }

        public SelectableItem(TItem item, bool isSelected = false)
        {
            Item = item;
            IsSelected = isSelected;
        }

        /// <summary>
        /// Triggered when an item selections status is changed
        /// </summary>
        public event EventHandler<TItem> SelectChanged;

        /// <summary>
        /// Triggered when an item selection is set to true
        /// </summary>
        public event EventHandler<TItem> Selected;


        /// <summary>
        /// Triggered when an item selection is set to false
        /// </summary>
        public event EventHandler<TItem> UnSelected;

        /// <summary>
        /// Wraped object
        /// </summary>
        public TItem Item { get; set; }


        private bool _isSelected;

        /// <summary>
        /// The object selection status
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                SetProperty(ref this._isSelected, value);

                SelectChanged?.Invoke(this, Item);
                if (value)
                    Selected?.Invoke(this, Item);
                else
                    UnSelected?.Invoke(this, Item);
            }
        }

        /// <summary>
        /// A command for toggling the selections status
        /// </summary>
        public ICommand ToggleSelectCommand => new RelayCommand(ToggleSelect);

        /// <summary>
        /// A command for changing the selection value to true.
        /// </summary>
        public ICommand SelectCommand => new RelayCommand(Select);
        
        /// <summary>
        /// Toggle the selections status
        /// </summary>
        private void ToggleSelect()
        {
            IsSelected = !IsSelected;
        }

        public void Select()
        {
            IsSelected = true;
        }
    }

}
