// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Mvvm
{
    /// <summary>
    /// Base class for ViewModel event that invokes the handlers in all the parent ViewModels.
    /// </summary>
    public class ViewModelBubbleEvent<TPayload> : ViewModelEvent<TPayload>
    {
        /// <inheritdoc />
        public ViewModelBubbleEvent() : base(EventRoutingStrategy.Bubble)
        {

        }

        /// <inheritdoc />
        public override void Raise(IViewModel sender, TPayload args)
        {
            InvokeParent(sender, sender, args);
        }

        private void InvokeParent(IViewModel sender, IViewModel target, TPayload args)
        {
            var parentViewModels = _viewModelsManager.GetParentViewModel(target);
            if (parentViewModels != null)
            {
                var handler = GetHandler(parentViewModels.ViewModel);
                handler?.Invoke(sender, args);
                InvokeParent(sender, parentViewModels.ViewModel, args);
            }
        }
    }
}