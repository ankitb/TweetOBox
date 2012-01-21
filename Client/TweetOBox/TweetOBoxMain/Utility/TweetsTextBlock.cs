using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Controls;

namespace TweetOBoxMain.Utility
{
    public class TweetsTextBlock : TextBlock
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
          typeof(TweetsTextBlock),
              new UIPropertyMetadata((PropertyChangedCallback)((sender, args) =>
              {
                  TweetsTextBlock textBlock = sender as TweetsTextBlock;

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
