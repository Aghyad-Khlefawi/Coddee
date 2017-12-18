﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Coddee.WPF.Commands;

namespace Coddee.WPF.Controls
{
    [DefaultProperty("Steps")]
    [ContentProperty("Steps")]
    public class Wizard : Control
    {
        static Wizard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Wizard), new FrameworkPropertyMetadata(typeof(Wizard)));

        }



        public static readonly DependencyProperty StepsProperty = DependencyProperty.Register(
                                                                                              "Steps",
                                                                                              typeof(WizardStepsCollection),
                                                                                              typeof(Wizard),
                                                                                              new FrameworkPropertyMetadata(default(WizardStepsCollection), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, StepsValueChanged));

        public static readonly DependencyPropertyKey FirstStepPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                                                                "FirstStep",
                                                                                                                typeof(WizardStep),
                                                                                                                typeof(WizardStep),
                                                                                                                new PropertyMetadata(default(WizardStep)));



        public static readonly DependencyProperty FirstStepProperty = FirstStepPropertyKey.DependencyProperty;

        public static readonly DependencyPropertyKey LastStepPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                                                               "LastStep",
                                                                                                               typeof(WizardStep),
                                                                                                               typeof(WizardStep),
                                                                                                               new PropertyMetadata(default(WizardStep)));

        public static readonly DependencyProperty LastStepProperty = LastStepPropertyKey.DependencyProperty;

        public static readonly DependencyProperty CurrentStepProperty = DependencyProperty.Register(
                                                        "CurrentStep",
                                                        typeof(WizardStep),
                                                        typeof(Wizard),
                                                        new PropertyMetadata(default(WizardStep)));


        public static readonly DependencyProperty SaveCommandProperty = DependencyProperty.Register(
                                                        "SaveCommand",
                                                        typeof(ICommand),
                                                        typeof(Wizard),
                                                        new PropertyMetadata(default(ICommand)));

        public static readonly DependencyProperty CancelCommandProperty = DependencyProperty.Register(
                                                        "CancelCommand",
                                                        typeof(ICommand),
                                                        typeof(Wizard),
                                                        new PropertyMetadata(default(ICommand)));

        public static readonly DependencyPropertyKey ContentPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "Content",
                                                                      typeof(object),
                                                                      typeof(Wizard),
                                                                      new PropertyMetadata(default(object)));

        public static readonly DependencyProperty ContentProperty = ContentPropertyKey.DependencyProperty;

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            protected set { SetValue(ContentPropertyKey, value); }
        }

        public ICommand CancelCommand
        {
            get { return (ICommand)GetValue(CancelCommandProperty); }
            set { SetValue(CancelCommandProperty, value); }
        }

        public ICommand SaveCommand
        {
            get { return (ICommand)GetValue(SaveCommandProperty); }
            set { SetValue(SaveCommandProperty, value); }
        }

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

        public WizardStep LastStep
        {
            get { return (WizardStep)GetValue(LastStepProperty); }
            protected set { SetValue(LastStepPropertyKey, value); }
        }


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

        public IReactiveCommand NextCommand { get; }
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

        public void Clear()
        {
            if (Steps != null)
                foreach (var step in Steps)
                {
                    step.Clear();
                }
            FirstStep?.Select();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Key == Key.Right && Keyboard.IsKeyDown(Key.LeftCtrl))
                Next();
            else if (e.Key == Key.Left && Keyboard.IsKeyDown(Key.LeftCtrl))
                Back();
        }

        public WizardStepsCollection Steps
        {
            get { return (WizardStepsCollection)GetValue(StepsProperty); }
            set { SetValue(StepsProperty, value); }
        }

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

    public class WizardStep : Control
    {

        static WizardStep()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WizardStep), new FrameworkPropertyMetadata(typeof(WizardStep)));
            VisibilityProperty.OverrideMetadata(typeof(WizardStep), new FrameworkPropertyMetadata(VisiblityChanged));
        }


        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
                                                        "ViewModel",
                                                        typeof(IPresentableViewModel),
                                                        typeof(WizardStep),
                                                        new PropertyMetadata(default(IPresentableViewModel), ViewModelSet));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
                                                        "Title",
                                                        typeof(string),
                                                        typeof(WizardStep),
                                                        new PropertyMetadata(default(string)));

        public static readonly DependencyPropertyKey IndexPropertyKey = DependencyProperty.RegisterReadOnly(
                                                        "Index",
                                                        typeof(int),
                                                        typeof(WizardStep),
                                                        new PropertyMetadata(default(int)));

        public static readonly DependencyProperty IndexProperty = IndexPropertyKey.DependencyProperty;


        public static readonly DependencyPropertyKey IsFirstStepPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "IsFirstStep",
                                                                      typeof(bool),
                                                                      typeof(WizardStep),
                                                                      new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsFirstStepProperty = IsFirstStepPropertyKey.DependencyProperty;
        public static readonly DependencyPropertyKey IsLastStepPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "IsLastStep",
                                                                      typeof(bool),
                                                                      typeof(WizardStep),
                                                                      new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsLastStepProperty = IsLastStepPropertyKey.DependencyProperty;


        public static readonly DependencyPropertyKey IsCompletedPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "IsCompleted",
                                                                      typeof(bool),
                                                                      typeof(WizardStep),
                                                                      new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsCompletedProperty = IsCompletedPropertyKey.DependencyProperty;

        public static readonly DependencyPropertyKey IsCurrentPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "IsCurrent",
                                                                      typeof(bool),
                                                                      typeof(WizardStep),
                                                                      new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsCurrentProperty = IsCurrentPropertyKey.DependencyProperty;
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
                                                        "Content",
                                                        typeof(object),
                                                        typeof(WizardStep),
                                                        new PropertyMetadata(default(object)));

        public static readonly DependencyPropertyKey IsValidatedPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "IsValidated",
                                                                      typeof(bool),
                                                                      typeof(WizardStep),
                                                                      new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsValidatedProperty = IsValidatedPropertyKey.DependencyProperty;

        public static readonly DependencyPropertyKey IsValidPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "IsValid",
                                                                      typeof(bool),
                                                                      typeof(WizardStep),
                                                                      new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsValidProperty = IsValidPropertyKey.DependencyProperty;

        public static readonly DependencyPropertyKey IsOnlyStepPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "IsOnlyStep",
                                                                      typeof(bool),
                                                                      typeof(WizardStep),
                                                                      new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsOnlyStepProperty = IsOnlyStepPropertyKey.DependencyProperty;

        public static readonly DependencyProperty ErrorProperty = DependencyProperty.Register(
                                                        "Error",
                                                        typeof(string),
                                                        typeof(WizardStep),
                                                        new PropertyMetadata(default(string)));

        public string Error
        {
            get { return (string)GetValue(ErrorProperty); }
            set { SetValue(ErrorProperty, value); }
        }

        public bool IsOnlyStep
        {
            get { return (bool)GetValue(IsOnlyStepProperty); }
            protected set { SetValue(IsOnlyStepPropertyKey, value); }
        }

        public EventHandler<WizardStep> Selected;

        public bool ValidateOnNavigation { get; set; }

        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            protected set { SetValue(IsValidPropertyKey, value); }
        }

        public bool IsValidated
        {
            get { return (bool)GetValue(IsValidatedProperty); }
            protected set { SetValue(IsValidatedPropertyKey, value); }
        }

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        public bool IsCurrent
        {
            get { return (bool)GetValue(IsCurrentProperty); }
            protected set { SetValue(IsCurrentPropertyKey, value); }
        }

        public bool IsCompleted
        {
            get { return (bool)GetValue(IsCompletedProperty); }
            protected set { SetValue(IsCompletedPropertyKey, value); }
        }

        public bool IsLastStep
        {
            get { return (bool)GetValue(IsLastStepProperty); }
            protected set { SetValue(IsLastStepPropertyKey, value); }
        }
        public bool IsFirstStep
        {
            get { return (bool)GetValue(IsFirstStepProperty); }
            protected set { SetValue(IsFirstStepPropertyKey, value); }
        }

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            protected set { SetValue(IndexPropertyKey, value); }
        }


        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public IPresentableViewModel ViewModel
        {
            get { return (IPresentableViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        private void ViewModelValidated(object sender, System.Collections.Generic.IEnumerable<string> res)
        {
            IsValidated = true;
            Error = res?.Where(e => !string.IsNullOrEmpty(e)).GroupBy(b => b).Select(b => b.First()).Combine("\n");
            IsValid = string.IsNullOrWhiteSpace(Error);
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

        public void ClearFisrtAndLast()
        {
            IsFirstStep = false;
            IsLastStep = false;
        }

        public void SetFirst()
        {
            IsFirstStep = true;
            IsOnlyStep = false;
        }

        public void SetLast()
        {
            IsLastStep = true;
            IsOnlyStep = false;
        }

        public void SetCompleted(bool newValue)
        {
            IsCompleted = newValue;
        }
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

        public void OnVisibilityChanged()
        {
            _wizard?.RefreshSteps();
        }
        public void Validate()
        {
            if (ViewModel == null)
            {
                IsValidated = false;
                return;
            }
            ViewModel.Validate();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var button = GetTemplateChild("PART_BUTTON") as Button;
            if (button != null)
            {
                button.Click += (sender, args) => Select();
            }
        }

        public void Select()
        {
            Selected?.Invoke(this, this);
        }

        public void Clear()
        {
            IsValidated = false;
            IsCompleted = false;
        }

        public void SetOnlyStep()
        {
            IsOnlyStep = true;
        }
    }

    public class WizardStepTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FirstStepTemplate { get; set; }
        public DataTemplate LastStepTemplate { get; set; }
        public DataTemplate StepTemplate { get; set; }
        public DataTemplate OnlyStepTemplate { get; set; }

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

    public class WizardStepsCollection : ObservableCollection<WizardStep>
    {
        public IEnumerable<IPresentableViewModel> GetViewModels()
        {
            return this.Select(e => e.ViewModel).ToList();
        }
    }

    public interface IWizardStepViewModel : IPresentableViewModel
    {
        void WizardStepEntered();
        void WizardStepExited();
    }
}
