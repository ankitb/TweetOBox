using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Controls;

namespace TweetOBoxMain.Utility
{
    public class TweetsTextBox : TextBlock
    {
        public InlineCollection InlineCollection
        {
            get
            {
                return (InlineCollection)GetValue(InlineCollectionProperty);
            }
            set
            {
                SetValue(InlineCollectionProperty, value);
            }
        }

        public static readonly DependencyProperty InlineCollectionProperty = DependencyProperty.Register(
          "InlineCollection",
          typeof(InlineCollection),
          typeof(TweetsTextBox),
              new UIPropertyMetadata((PropertyChangedCallback)((sender, args) =>
              {
                  TweetsTextBox textBlock = sender as TweetsTextBox;

                  if (textBlock != null)
                  {
                      textBlock.Inlines.Clear();

                      InlineCollection inlines = args.NewValue as InlineCollection;

                      if (inlines != null)
                          textBlock.Inlines.AddRange(inlines.ToList());
                  }
              })));
    }
}
