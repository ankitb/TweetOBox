using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;

namespace TweetOBoxMain.CustomControls
{
    /// <summary>
    ///  Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:TweetOBoxMain.CustomControls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:TweetOBoxMain.CustomControls;assembly=TweetOBoxMain.CustomControls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:SearchTextBox/>
    ///
    /// </summary>
    public class SearchTextBox : TextBox
    {
        static SearchTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchTextBox), new FrameworkPropertyMetadata(typeof(SearchTextBox)));

            TextProperty.OverrideMetadata(
                typeof(SearchTextBox), 
                new FrameworkPropertyMetadata(new PropertyChangedCallback(TextPropertyChanged)));
        }

        public static readonly DependencyProperty TextBoxInfoProperty = DependencyProperty.Register(
            "TextBoxInfo",
            typeof(string),
            typeof(SearchTextBox),
            new PropertyMetadata(string.Empty));

        public string TextBoxInfo
        {
            get { return (string)GetValue(TextBoxInfoProperty); }
            set { SetValue(TextBoxInfoProperty, value); }
        }

        private static readonly DependencyPropertyKey HasTextPropertyKey = DependencyProperty.RegisterReadOnly(
            "HasText",
            typeof(bool),
            typeof(SearchTextBox),
            new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty HasTextProperty = HasTextPropertyKey.DependencyProperty;

        public bool HasText
        {
            get
            {
                return (bool)GetValue(HasTextProperty);
            }
        }

        static void TextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {            
            SearchTextBox itb = (SearchTextBox)sender;

            bool actuallyHasText = itb.Text.Length > 0;
            if (actuallyHasText != itb.HasText)
            {
                itb.SetValue(HasTextPropertyKey, actuallyHasText);
            }
        }
    }
}
