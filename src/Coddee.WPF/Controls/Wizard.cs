// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Coddee.Mvvm;
using Coddee.WPF.Commands;

namespace Coddee.WPF.Controls
{
    /// <summary>
    /// Wizard control.
    /// </summary>
    [DefaultProperty("Steps")]
    [ContentProperty("Steps")]
    public class Wizard : Control
    {
        static Wizard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Wizard), new FrameworkPropertyMetadata(typeof(Wizard)));

        }


        /// <summary>
        /// The wizard steps collection.
        /// </summary>
        public static readonly DependencyProperty StepsProperty = DependencyProperty.Register(
                                                                                              "Steps",
                                                                                              typeof(WizardStepsCollection),
                                                                                              typeof(Wizard),
                                                                                              new FrameworkPropertyMetadata(default(WizardStepsCollection), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, StepsValueChanged));

        /// <summary>
        /// References the first step in the wizard.
        /// </summary>
        public static readonly DependencyPropertyKey FirstStepPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                                                                "FirstStep",
                                                                                                                typeof(WizardStep),
                                                                                                                typeof(WizardStep),
                                                                                                                new PropertyMetadata(default(WizardStep)));


        /// <inheritdoc cref="FirstStepProperty"/>
        public static readonly DependencyProperty FirstStepProperty = FirstStepPropertyKey.DependencyProperty;


        /// <summary>
        /// References the last step in the wizard.
        /// </summary>
        public static readonly DependencyPropertyKey LastStepPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                                                               "LastStep",
                                                                                                               typeof(WizardStep),
                                                                                                               typeof(WizardStep),
                                                                                                               new PropertyMetadata(default(WizardStep)));

        /// <inheritdoc cref="LastStepPropertyKey"/>
        public static readonly DependencyProperty LastStepProperty = LastStepPropertyKey.DependencyProperty;


        
            /// <summary>
            /// References the current step in the wizard.
            /// </summary>
            public static readonly DependencyProperty CurrentStepProperty = DependencyProperty.Register(
                                                        "CurrentStep",
                                                        typeof(WizardStep),
                                                        typeof(Wizard),
                                                        new PropertyMetadata(default(WizardStep)));

        /// <summary>
        /// Command that will be executed on the save button.
        /// </summary>
        public static readonly DependencyProperty SaveCommandProperty = DependencyProperty.Register(
                                                        "SaveCommand",
                                                        typeof(ICommand),
                                                        typeof(Wizard),
                                                        new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// Command that will be executed on the cancel button.
        /// </summary>
        public static readonly DependencyProperty CancelCommandProperty = DependencyProperty.Register(
                                                        "CancelCommand",
                                                        typeof(ICommand),
                                                        typeof(Wizard),
                                                        new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// The current content of the wizard.
        /// </summary>
        public static readonly DependencyPropertyKey ContentPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "Content",
                                                                      typeof(object),
                                                                      typeof(Wizard),
                                                                      new PropertyMetadata(default(object)));

        /// <inheritdoc cref="ContentPropertyKey"/>
        public static readonly DependencyProperty ContentProperty = ContentPropertyKey.DependencyProperty;

        /// <inheritdoc cref="ContentProperty"/>
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            protected set { SetValue(ContentPropertyKey, value); }
        }

        /// <inheritdoc cref="CancelCommandProperty"/>
        public ICommand CancelCommand
        {
            get { return (ICommand)GetValue(CancelCommandProperty); }
            set { SetValue(CancelCommandProperty, value); }
        }

        /// <inheritdoc cref="SaveCommandProperty"/>
        public ICommand SaveCommand
        {
            get { return (ICommand)GetValue(SaveCommandProperty); }
            set { SetValue(SaveCommandProperty, value); }
        }

        /// <inheritdoc cref="CurrentStepProperty"/>
        public WizardStep CurrentStep
        {
            get { return (WizardStep)GetValue(CurrentStepProperty); }
            set
            {
                if (CurrentStep != null)
                {
                    if (CurrentStep.ValidateOnNavigation)
                        CurrentStep.Validate();
                    CurrentStep.SetCurrent(false);
                }
                SetValue(CurrentStepProperty, value);
                RefreshCommands();
                Content = value?.Content;
                value?.SetCompleted(true);
                value?.SetCurrent(true);
            }
        }

        /// <inheritdoc cref="LastStepProperty"/>
        public WizardStep LastStep
        {
            get { return (WizardStep)GetValue(LastStepProperty); }
            protected set { SetValue(LastStepPropertyKey, value); }
        }


        /// <inheritdoc cref="FirstStepProperty"/>
        public WizardStep FirstStep
        {
            get { return (WizardStep)GetValue(FirstStepProperty); }
            protected set { SetValue(FirstStepPropertyKey, value); }
        }

        private static void StepsValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is Wizard wizard)
            {
                if (dependencyPropertyChangedEventArgs.NewValue is WizardStepsCollection Steps)
                {
                    Steps.CollectionChanged += (sender, args) =>
                    {
                        wizard.RefreshSteps();
                    };
                }
                wizard.RefreshSteps();
            }
        }

        /// <inheritdoc />
        public Wizard()
        {

            if (Steps == null)
                Steps = new WizardStepsCollection();
            Steps.CollectionChanged += (sender, args) =>
            {
                RefreshSteps();
            };

            NextCommand = new ReactiveCommand<Wizard>(this, Next)
                .ObserveProperty(e => e.CurrentStep, ValidateCanGoNext);

            BackCommand = new ReactiveCommand<Wizard>(this, Back)
                .ObserveProperty(e => e.CurrentStep, ValidateCanGoBack);

        }

        /// <summary>
        /// Navigate to the next step.
        /// </summary>
        public IReactiveCommand NextCommand { get; }

        /// <summary>
        /// Navigate to the previous step.
        /// </summary>
        public IReactiveCommand BackCommand { get; }


        private void RefreshCommands()
        {
            NextCommand?.UpdateCanExecute();
            BackCommand?.UpdateCanExecute();
        }
        private bool ValidateCanGoNext(object obj)
        {
            if (obj is WizardStep step)
                return !step.IsLastStep;
            return false;
        }
        private bool ValidateCanGoBack(object obj)
        {
            if (obj is WizardStep step)
                return !step.IsFirstStep;
            return false;
        }

        private void Back()
        {
            if (!CurrentStep.IsFirstStep)
            {
                CurrentStep.SetCompleted(false);
                CurrentStep = Steps.OrderByDescending(e => e.Index).First(e => e.Index < CurrentStep.Index && e.Visibility == Visibility.Visible);
            }
        }

        private void Next()
        {
            if (!CurrentStep.IsLastStep)
            {
                CurrentStep = Steps.OrderBy(e => e.Index).First(e => e.Index > CurrentStep.Index && e.Visibility == Visibility.Visible);
            }
        }

        /// <summary>
        /// Clears the steps and goes to the first step of the wizard.
        /// </summary>
        public void Clear()
        {
            if (Steps != null)
                foreach (var step in Steps)
                {
                    step.Clear();
                }
            FirstStep?.Select();
        }

        /// <inheritdoc />
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Key == Key.Right && Keyboard.IsKeyDown(Key.LeftCtrl))
                Next();
            else if (e.Key == Key.Left && Keyboard.IsKeyDown(Key.LeftCtrl))
                Back();
        }

        /// <inheritdoc cref="StepsProperty"/>
        public WizardStepsCollection Steps
        {
            get { return (WizardStepsCollection)GetValue(StepsProperty); }
            set { SetValue(StepsProperty, value); }
        }

        /// <summary>
        /// Refresh the wizard steps.
        /// </summary>
        public void RefreshSteps()
        {
            if (Steps != null)
            {

                for (int i = 0; i < Steps.Count; i++)
                {
                    var step = Steps[i];
                    if (step != null)
                    {
                        step.SetWizard(this);
                        if (step.Visibility == Visibility.Visible)
                        {
                            step.SetIndex(i);
                            step.Selected = OnStepSelected;
                        }
                    }
                }

                FirstStep?.ClearFisrtAndLast();
                LastStep?.ClearFisrtAndLast();

                FirstStep = Steps.FirstOrDefault(e => e.Visibility == Visibility.Visible);
                LastStep = Steps.LastOrDefault(e => e.Visibility == Visibility.Visible);

                if (Steps.Count(e => e.Visibility == Visibility.Visible) > 1)
                {
                    FirstStep?.SetFirst();
                    LastStep?.SetLast();
                }
                else
                {
                    FirstStep?.SetOnlyStep();
                }

                CurrentStep = FirstStep;
            }
        }

        private void OnStepSelected(object sender, WizardStep step)
        {
            CurrentStep = step;
        }
    }

    /// <summary>
    /// A step of <see cref="Wizard"/> control.
    /// </summary>
    public class WizardStep : Control
    {

        static WizardStep()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WizardStep), new FrameworkPropertyMetadata(typeof(WizardStep)));
            VisibilityProperty.OverrideMetadata(typeof(WizardStep), new FrameworkPropertyMetadata(VisiblityChanged));
        }

        /// <summary>
        /// The ViewModel presented by the step.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
                                                        "ViewModel",
                                                        typeof(IPresentableViewModel),
                                                        typeof(WizardStep),
                                                        new PropertyMetadata(default(IPresentableViewModel), ViewModelSet));

        /// <summary>
        /// The title of the step.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
                                                        "Title",
                                                        typeof(string),
                                                        typeof(WizardStep),
                                                        new PropertyMetadata(default(string)));
        /// <summary>
        /// The order of the step in the wizard.
        /// </summary>
        public static readonly DependencyPropertyKey IndexPropertyKey = DependencyProperty.RegisterReadOnly(
                                                        "Index",
                                                        typeof(int),
                                                        typeof(WizardStep),
                                                        new PropertyMetadata(default(int)));

        /// <inheritdoc cref="IndexPropertyKey"/>
        public static readonly DependencyProperty IndexProperty = IndexPropertyKey.DependencyProperty;

        /// <summary>
        /// Indicates whether this step is the first step in the wizard.
        /// </summary>
        public static readonly DependencyPropertyKey IsFirstStepPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "IsFirstStep",
                                                                      typeof(bool),
                                                                      typeof(WizardStep),
                                                                      new PropertyMetadata(default(bool)));
        /// <inheritdoc cref="IsFirstStepPropertyKey"/>
        public static readonly DependencyProperty IsFirstStepProperty = IsFirstStepPropertyKey.DependencyProperty;

        /// <summary>
        /// Indicates whether this step is the last step in the wizard.
        /// </summary>
        public static readonly DependencyPropertyKey IsLastStepPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "IsLastStep",
                                                                      typeof(bool),
                                                                      typeof(WizardStep),
                                                                      new PropertyMetadata(default(bool)));

        /// <inheritdoc cref="IsLastStepPropertyKey"/>
        public static readonly DependencyProperty IsLastStepProperty = IsLastStepPropertyKey.DependencyProperty;


        /// <summary>
        /// Indicates whether this step was completed.
        /// </summary>
        public static readonly DependencyPropertyKey IsCompletedPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "IsCompleted",
                                                                      typeof(bool),
                                                                      typeof(WizardStep),
                                                                      new PropertyMetadata(default(bool)));

        /// <inheritdoc cref="IsCompletedPropertyKey"/>
        public static readonly DependencyProperty IsCompletedProperty = IsCompletedPropertyKey.DependencyProperty;

        /// <summary>
        /// Indicates whether this step is the current step.
        /// </summary>
        public static readonly DependencyPropertyKey IsCurrentPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "IsCurrent",
                                                                      typeof(bool),
                                                                      typeof(WizardStep),
                                                                      new PropertyMetadata(default(bool)));
        /// <inheritdoc cref="IsCurrentPropertyKey"/>
        public static readonly DependencyProperty IsCurrentProperty = IsCurrentPropertyKey.DependencyProperty;

        /// <summary>
        /// The content of the step.
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
                                                        "Content",
                                                        typeof(object),
                                                        typeof(WizardStep),
                                                        new PropertyMetadata(default(object)));

        /// <summary>
        /// Indicates whether the step was validated.
        /// </summary>
        public static readonly DependencyPropertyKey IsValidatedPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "IsValidated",
                                                                      typeof(bool),
                                                                      typeof(WizardStep),
                                                                      new PropertyMetadata(default(bool)));

        /// <inheritdoc cref="IsValidatedPropertyKey"/>
        public static readonly DependencyProperty IsValidatedProperty = IsValidatedPropertyKey.DependencyProperty;

        /// <summary>
        /// Indicates whether the step is valid.
        /// </summary>
        public static readonly DependencyPropertyKey IsValidPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "IsValid",
                                                                      typeof(bool),
                                                                      typeof(WizardStep),
                                                                      new PropertyMetadata(default(bool)));

        /// <inheritdoc cref="IsValidPropertyKey"/>
        public static readonly DependencyProperty IsValidProperty = IsValidPropertyKey.DependencyProperty;

        /// <summary>
        /// Indicates whether the step is the only step in the wizard.
        /// </summary>
        public static readonly DependencyPropertyKey IsOnlyStepPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "IsOnlyStep",
                                                                      typeof(bool),
                                                                      typeof(WizardStep),
                                                                      new PropertyMetadata(default(bool)));

        /// <inheritdoc cref="IsOnlyStepPropertyKey"/>
        public static readonly DependencyProperty IsOnlyStepProperty = IsOnlyStepPropertyKey.DependencyProperty;

        /// <summary>
        /// Tool-tip message of the wizard.
        /// </summary>
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
                                                        "Message",
                                                        typeof(string),
                                                        typeof(WizardStep),
                                                        new PropertyMetadata(default(string)));
        /// <summary>
        /// The <see cref="IViewModel.ValidationResult"/> of the wizard ViewModel
        /// </summary>
        public static readonly DependencyPropertyKey ValidationResultPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "ValidationResult",
                                                                      typeof(Validation.ValidationResult),
                                                                      typeof(WizardStep),
                                                                      new PropertyMetadata(default(Validation.ValidationResult)));

        /// <inheritdoc cref="ValidationResultProperty"/>
        public static readonly DependencyProperty ValidationResultProperty = ValidationResultPropertyKey.DependencyProperty;

        /// <inheritdoc cref="ValidationResultProperty"/>
        public Validation.ValidationResult ValidationResult
        {
            get { return (Validation.ValidationResult)GetValue(ValidationResultProperty); }
            protected set { SetValue(ValidationResultPropertyKey, value); }
        }

        /// <inheritdoc cref="MessageProperty"/>
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        /// <inheritdoc cref="IsOnlyStepProperty"/>
        public bool IsOnlyStep
        {
            get { return (bool)GetValue(IsOnlyStepProperty); }
            protected set { SetValue(IsOnlyStepPropertyKey, value); }
        }

        /// <summary>
        /// Triggered when the step is selected.
        /// </summary>
        public EventHandler<WizardStep> Selected;

        /// <summary>
        /// If true the ViewModel will be validates on navigation to this step.
        /// </summary>
        public bool ValidateOnNavigation { get; set; }

        /// <inheritdoc cref="IsValidProperty"/>
        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            protected set { SetValue(IsValidPropertyKey, value); }
        }

        /// <inheritdoc cref="IsValidatedProperty"/>
        public bool IsValidated
        {
            get { return (bool)GetValue(IsValidatedProperty); }
            protected set { SetValue(IsValidatedPropertyKey, value); }
        }

        /// <inheritdoc cref="ContentProperty"/>
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        /// <inheritdoc cref="IsCurrentProperty"/>
        public bool IsCurrent
        {
            get { return (bool)GetValue(IsCurrentProperty); }
            protected set { SetValue(IsCurrentPropertyKey, value); }
        }

        /// <inheritdoc cref="IsCompletedProperty"/>
        public bool IsCompleted
        {
            get { return (bool)GetValue(IsCompletedProperty); }
            protected set { SetValue(IsCompletedPropertyKey, value); }
        }

        /// <inheritdoc cref="IsLastStepProperty"/>
        public bool IsLastStep
        {
            get { return (bool)GetValue(IsLastStepProperty); }
            protected set { SetValue(IsLastStepPropertyKey, value); }
        }
        /// <inheritdoc cref="IsFirstStepPropertyKey"/>
        public bool IsFirstStep
        {
            get { return (bool)GetValue(IsFirstStepProperty); }
            protected set { SetValue(IsFirstStepPropertyKey, value); }
        }

        /// <inheritdoc cref="IndexProperty"/>
        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            protected set { SetValue(IndexPropertyKey, value); }
        }


        /// <inheritdoc cref="TitleProperty"/>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <inheritdoc cref="ViewModelProperty"/>
        public IPresentableViewModel ViewModel
        {
            get { return (IPresentableViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        private void ViewModelValidated(object sender, Validation.ValidationResult res)
        {
            IsValidated = true;
            var builder = new StringBuilder();
            if (res != null)
            {
                foreach (var error in res.Errors.GroupBy(b => b).Select(b => b.First()))
                {
                    builder.Append($"(Error) {error}");
                    builder.Append("\n");
                }
                foreach (var warning in res.Warnings.GroupBy(b => b).Select(b => b.First()))
                {
                    builder.Append($"(Warning) {warning}");
                    builder.Append("\n");
                }
                Message = builder.Length > 0 ? builder.ToString(0, builder.Length - "\n".Length) : null;
                IsValid = res.IsValid;
            }
            ValidationResult = res;
        }

        private static void ViewModelSet(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WizardStep wizardStep)
            {
                wizardStep.UpdateContent();
            }
        }

        private void UpdateContent()
        {
            ViewModel.Validated += ViewModelValidated;
            void UpdateView()
            {
                Content = ViewModel?.GetView();
            }

            ViewModel.ViewIndexChanged += delegate
            {
                UpdateView();
            };
            UpdateView();
        }

        internal void SetIndex(int index)
        {
            Index = index;
        }

        /// <summary>
        /// Sets the <see cref="IsFirstStep"/> and <see cref="IsLastStep"/> to false
        /// </summary>
        public void ClearFisrtAndLast()
        {
            IsFirstStep = false;
            IsLastStep = false;
        }

        /// <summary>
        /// Sets the <see cref="IsFirstStep"/> to true
        /// </summary>
        public void SetFirst()
        {
            IsFirstStep = true;
            IsOnlyStep = false;
        }

        /// <summary>
        /// Sets the <see cref="IsLastStep"/> to true
        /// </summary>
        public void SetLast()
        {
            IsLastStep = true;
            IsOnlyStep = false;
        }

        /// <summary>
        /// Sets the <see cref="IsCompleted"/> property
        /// </summary>
        public void SetCompleted(bool newValue)
        {
            IsCompleted = newValue;
        }

        /// <summary>
        /// Sets the <see cref="IsCurrent"/> property
        /// </summary>
        public void SetCurrent(bool newValue)
        {
            IsCurrent = newValue;
            if (ViewModel is IWizardStepViewModel wizardStepViewModel)
            {
                if (newValue)
                    wizardStepViewModel.WizardStepEntered();
                else
                    wizardStepViewModel.WizardStepExited();
            }
        }

        private Wizard _wizard;

        /// <summary>
        /// Sets the wizard this step belongs to.
        /// </summary>
        public void SetWizard(Wizard wizard)
        {
            _wizard = wizard;
        }

        private static void VisiblityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WizardStep step)
            {
                step.OnVisibilityChanged();
            }
        }

        /// <summary>
        /// called when the step visibility is changed,
        /// </summary>
        public void OnVisibilityChanged()
        {
            _wizard?.RefreshSteps();
        }

        /// <summary>
        /// Validate the ViewModel.
        /// </summary>
        public void Validate()
        {
            if (ViewModel == null)
            {
                IsValidated = false;
                return;
            }
            ViewModel.Validate();
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (GetTemplateChild("PART_BUTTON") is Button button)
            {
                button.Click += (sender, args) => Select();
            }
        }

        /// <summary>
        /// Raise <see cref="Selected"/> event.
        /// </summary>
        public void Select()
        {
            Selected?.Invoke(this, this);
        }

        /// <summary>
        /// Clear the step.
        /// </summary>
        public void Clear()
        {
            IsValidated = false;
            IsCompleted = false;
        }

        /// <summary>
        /// Sets the <see cref="IsOnlyStep"/> to true;
        /// </summary>
        public void SetOnlyStep()
        {
            IsOnlyStep = true;
        }
    }

    /// <summary>
    /// Template selector for the wizard steps.
    /// </summary>
    public class WizardStepTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Template of the first step.
        /// </summary>
        public DataTemplate FirstStepTemplate { get; set; }

        /// <summary>
        /// Template of the last step.
        /// </summary>
        public DataTemplate LastStepTemplate { get; set; }

        /// <summary>
        /// Template of the step.
        /// </summary>
        public DataTemplate StepTemplate { get; set; }

        /// <summary>
        /// Template of the only step.
        /// </summary>
        public DataTemplate OnlyStepTemplate { get; set; }

        /// <inheritdoc />
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is WizardStep step)
            {
                if (step.IsOnlyStep)
                    return OnlyStepTemplate;
                if (step.IsFirstStep)
                    return FirstStepTemplate;
                if (step.IsLastStep)
                    return LastStepTemplate;

                return StepTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }

    /// <summary>
    /// A collection of the wizard steps.
    /// </summary>
    public class WizardStepsCollection : ObservableCollection<WizardStep>
    {
        /// <summary>
        /// Returns the ViewModels of the steps.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IPresentableViewModel> GetViewModels()
        {
            return this.Select(e => e.ViewModel).ToList();
        }
    }

    /// <summary>
    /// A wizard step ViewModel.
    /// </summary>
    public interface IWizardStepViewModel : IPresentableViewModel
    {
        /// <summary>
        /// Called when a wizard step is entered.
        /// </summary>
        void WizardStepEntered();

        /// <summary>
        /// Called when a wizard step is exited.
        /// </summary>
        void WizardStepExited();
    }
}
