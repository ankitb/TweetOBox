using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TweetOBoxMain.Utility;
using TOB.Utility;

namespace TweetOBoxMain.CustomControls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
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
    ///     <MyNamespace:TweetOTextBox/>
    ///
    /// </summary>
    public class TweetOTextBox : TextBox
    {
        int _lastUpdate = 0;
        int _prevSize = 0;
        byte _changeCount = 0;

        public TweetOTextBox()
        {
            this.TextChanged += new TextChangedEventHandler(TweetOTextBox_TextChanged);
            HasText = false;
        }

        void TweetOTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            HasText = !String.IsNullOrEmpty(this.Text);
            _changeCount++;

            if ((this.Text.Length - _lastUpdate) > 10 || _changeCount > 10)
            {
                if (ShortenUrl)
                {
                    _prevSize = this.Text.Length;
                    this.Text = TinyUrl.ToTinyURLS(this.Text);

                    if (_prevSize != this.Text.Length)
                    {
                        this.CaretIndex = this.Text.Length + 1;
                    }
                }

                _changeCount = 0;
                _lastUpdate = this.Text.Length;
            }
        }

        public bool HasText
        {
            get { return (bool)GetValue(HasTextProperty); }
            set { SetValue(HasTextProperty, value); }
        }

        public static readonly DependencyProperty HasTextProperty =
           DependencyProperty.Register(
           "HasText",
           typeof(bool),
           typeof(TweetOTextBox));


        public bool ShortenUrl
        {
            get { return (bool)GetValue(ShortenUrlProperty); }
            set { SetValue(ShortenUrlProperty, value); }
        }

        public static readonly DependencyProperty ShortenUrlProperty =
           DependencyProperty.Register(
           "ShortenUrl",
           typeof(bool),
           typeof(TweetOTextBox));   
    }
}
