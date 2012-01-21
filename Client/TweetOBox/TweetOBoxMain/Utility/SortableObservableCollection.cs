using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Reflection;
using System.ComponentModel;
using TOB.Entities;
using System.Windows.Forms;

namespace TweetOBoxMain.Utility
{
    public class SortableObservableCollection<K> : ObservableCollection<K>
    {
        public SortableObservableCollection(List<K> list)
            : base(list)
        {
        }

        public SortableObservableCollection(IEnumerable<K> collection)
            : base(collection)
        {
        }

        //public new event PropertyChangedEventHandler PropertyChanged;
        //public override event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged;

        //protected void NotifyPropertyChanged(String info)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(info));
        //    }
        //}

        //protected void NotifyCollectionChanged(CollectionChangeAction action, object element)
        //{
        //    if(CollectionChanged != null)
        //    {
        //        //CollectionChanged(this, new CollectionChangeEventArgs(action , element));
        //    }
        //}

        public void Sort<TKey>(Func<K, TKey> keySelector, System.ComponentModel.ListSortDirection direction)
        {
            switch (direction)
            {
                case System.ComponentModel.ListSortDirection.Ascending:
                    {
                        ApplySort(Items.OrderBy(keySelector));
                        break;
                    }
                case System.ComponentModel.ListSortDirection.Descending:
                    {
                        ApplySort(Items.OrderByDescending(keySelector));
                        break;
                    }
            }
        }

        public void Sort<TKey>(Func<K, TKey> keySelector, IComparer<TKey> comparer)
        {
            ApplySort(Items.OrderBy(keySelector, comparer));
        }

        public void LocalSort(System.ComponentModel.ListSortDirection direction)
        {
            try
            {
                if (Items.Count < 1)
                {
                    return;
                }

                Func<K, DateTime?> keySelector = (s=>(s as TOBEntityBase).SortableColumn);

                switch (direction)
                {
                    case System.ComponentModel.ListSortDirection.Ascending:
                        {
                            ApplySort(Items.OrderBy(keySelector));
                            break;
                        }
                    case System.ComponentModel.ListSortDirection.Descending:
                        {
                            ApplySort(Items.OrderByDescending(keySelector));
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //public void Take(int count)
        //{
        //    while (Items.Count > count)
        //    {
        //        Items.RemoveAt(Items.Count - 1);
        //    }

        //    //OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Remove));
        //}

        private void ApplySort(IEnumerable<K> sortedItems)
        {
            //Temp hack...
            if (sortedItems == null)
                return;

            List<K> sortedItemsList;
            try { sortedItemsList = sortedItems.ToList(); }
            catch (Exception ex) { return; }

            foreach (var item in sortedItemsList)
                Move(IndexOf(item), sortedItemsList.IndexOf(item));
        }

        public void ForEach(Action<K> loopAction)
        {            
            foreach (K item in this)
            {
                loopAction(item);
            }
        }
    }

   
}
