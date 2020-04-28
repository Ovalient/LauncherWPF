using LauncherWPF.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LauncherWPF.Views
{
    /// <summary>
    /// AppListView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AppListView : UserControl
    {
        public AppListView()
        {
            InitializeComponent();

            //this.listView.PreviewMouseLeftButtonDown += listView_PreviewMouseLeftButtonDown;
            //this.listView.PreviewMouseMove += listView_PreviewMouseMove;
            //this.listView.Drop += listView_Drop;
            //this.listView.DragOver += listView_DragOver;
        }

    //    private const string DATA_FORMAT = "AppModelsDataFormat";
    //    private bool isDragging = false;
    //    private int dragIndex = -1;

    //    private void listView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    //    {
    //        ListView listView = sender as ListView;
    //        Point mousePoint = e.GetPosition(listView);
    //        IInputElement inputElement = listView.InputHitTest(mousePoint);
    //        ListViewItem item = FindAncestor<ListViewItem>(inputElement as DependencyObject);

    //        if (item != null)
    //            this.isDragging = true;
    //    }

    //    private void listView_PreviewMouseMove(object sender, MouseEventArgs e)
    //    {
    //        if(this.isDragging && e.LeftButton == MouseButtonState.Pressed)
    //        {
    //            ListView listView = sender as ListView;
    //            ListViewItem listViewItem = FindAncestor<ListViewItem>(e.OriginalSource as DependencyObject);

    //            if(listViewItem == null)
    //            {
    //                return;
    //            }
                
    //            this.dragIndex = listView.Items.IndexOf(listViewItem.Content);

    //            DataObject dataObject = new DataObject(DATA_FORMAT, listViewItem.Content);
    //            DragDrop.DoDragDrop(listViewItem, dataObject, DragDropEffects.Move);
    //        }
    //    }

    //    private void listView_Drop(object sender, DragEventArgs e)
    //    {
    //        if(e.Data.GetDataPresent(DATA_FORMAT))
    //        {
    //            ListView listView = sender as ListView;
    //            Point mousePoint = e.GetPosition(listView);
    //            IInputElement inputElement = listView.InputHitTest(mousePoint);
    //            ListViewItem item = FindAncestor<ListViewItem>(inputElement as DependencyObject);

    //            int itemIndex = -1;

    //            if(item == null)
    //            {
    //                itemIndex = listView.Items.Count - 1;
    //            }
    //            else
    //            {
    //                itemIndex = listView.Items.IndexOf(item.Content);
    //            }

    //            object content = e.Data.GetData(DATA_FORMAT);
    //            listView.Items.RemoveAt(dragIndex);

    //            if((dragIndex < itemIndex) && (item != null))
    //            {
    //                itemIndex--;
    //            } 

    //            listView.Items.Insert(itemIndex, content);
    //            listView.Items.Refresh();
    //            this.dragIndex = -1;
    //            this.isDragging = false;
    //        }
    //    }

    //    void listView_DragOver(object sender, DragEventArgs e)
    //    {
    //        ListView listView = sender as ListView;
    //        ScrollViewer scrollViewer = FindChild<ScrollViewer>(listView);

    //        double tolerance = 10;
    //        double mouseY    = e.GetPosition(listView).Y;
    //        double offset    = 1; 

    //        if(mouseY < tolerance)
    //        {
    //            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - offset);
    //        }
    //        else if(mouseY > listView.ActualHeight - tolerance)
    //        {
    //            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + offset);
    //        }
    //    }

    //    private static TAncestor FindAncestor<TAncestor>(DependencyObject dependencyObject) where TAncestor : DependencyObject
    //    {
    //        DependencyObject current = dependencyObject; 

    //        do
    //        {
    //            if(current is TAncestor)
    //            {
    //                return (TAncestor)current;
    //            }
    //            current = VisualTreeHelper.GetParent(current);
    //        }
    //        while(current != null);

    //        return null;
    //    }

    //    private static TChild FindChild<TChild>(DependencyObject dependencyObject) where TChild : DependencyObject
    //    {
    //        int childCount = VisualTreeHelper.GetChildrenCount(dependencyObject); 

    //        for(int i = 0; i < childCount; i++)
    //        {
    //            DependencyObject child = VisualTreeHelper.GetChild(dependencyObject, i); 

    //            if(child != null && child is TChild)
    //            {
    //                return (TChild)child;
    //            }
    //            else
    //            {
    //                TChild grandChild = FindChild<TChild>(child); 

    //                if(grandChild != null)
    //                {
    //                    return grandChild;
    //                }
    //            }
    //        }
    //        return null;
    //    }
    }
}
