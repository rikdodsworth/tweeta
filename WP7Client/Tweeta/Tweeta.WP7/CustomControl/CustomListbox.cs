using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Diagnostics;

namespace Tweeta.CustomControl
{
    public class CustomListbox : ListBox
    {
        public CustomListbox()
        {
            
        }

        ScrollViewer scrollViewer;
        protected override Size ArrangeOverride(Size finalSize)
        {
            scrollViewer = base.GetTemplateChild("ScrollViewer") as ScrollViewer;
            scrollViewer.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(scrollViewer_ManipulationDelta);
            var bind = new Binding();
            bind.Source = scrollViewer;
            bind.Path = new PropertyPath("VerticalOffset");
            bind.Mode = BindingMode.OneWay;
            this.SetBinding(ListVerticalOffsetProperty, bind);
            return base.ArrangeOverride(finalSize);
        }

        void scrollViewer_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            Debug.WriteLine("Y = " + e.DeltaManipulation.Translation.X);            
        }
        public readonly DependencyProperty ListVerticalOffsetProperty = DependencyProperty.Register("ListVerticalOffset",
        typeof(double), typeof(CustomListbox), new PropertyMetadata(new PropertyChangedCallback(OnListVerticalOffsetChanged)));
        private static void OnListVerticalOffsetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            CustomListbox page = obj as CustomListbox;
            ScrollViewer viewer = page.scrollViewer;

            Debug.WriteLine(viewer.VerticalOffset);
        }
        public double ListVerticalOffset
        {
            get { return (double)this.GetValue(ListVerticalOffsetProperty); }
            set { this.SetValue(ListVerticalOffsetProperty, value); }
        }

        public double VerticalOffset
        {
            get { return scrollViewer.VerticalOffset; }
        }

        bool isDraggingDown;
        double lastY;
        
    }
    
}
