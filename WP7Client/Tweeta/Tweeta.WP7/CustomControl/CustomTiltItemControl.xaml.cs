using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Tweeta.CustomControl
{
    public partial class CustomTiltItemControl : ContentControl
    {
        private static readonly PropertyPath RotationXProperty = new PropertyPath(PlaneProjection.RotationXProperty);
        private static readonly PropertyPath RotationYProperty = new PropertyPath(PlaneProjection.RotationYProperty);
        private static readonly PropertyPath GlobalOffsetZProperty = new PropertyPath(PlaneProjection.GlobalOffsetZProperty);

        private const double MaxAngle = 0.3;
        private const double MaxDepression = 50;
        private ContentPresenter contentPresenter;
        private double width;
        private double height;
        //private bool isTilting;

        public event RoutedEventHandler Click;

        public CustomTiltItemControl()
        {
            InitializeComponent();
        }

        protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
        {
            base.OnManipulationStarted(e);

            //isTilting = true;

            width = this.ActualWidth;
            height = this.ActualHeight;

            this.TiltBackStoryboard.Stop();
            DepressAndTilt(e.ManipulationOrigin, e.ManipulationContainer);
        }

        private void DepressAndTilt(Point origin, UIElement container)
        {
            try
            {
                GeneralTransform transform = container.TransformToVisual(this);

                Point transformedOrigin = transform.Transform(origin);

                Point normalizedPoint = new Point(
                    Math.Min(Math.Max(transformedOrigin.X / width, 0), 1),
                    Math.Min(Math.Max(transformedOrigin.Y / height, 0), 1));

                double xMagnitude = Math.Abs(normalizedPoint.X - 0.5);
                double yMagnitude = Math.Abs(normalizedPoint.Y - 0.5);

                double xDirection = -Math.Sign(normalizedPoint.X - 0.5);
                double yDirection = Math.Sign(normalizedPoint.Y - 0.5);

                double angleMagnitude = xMagnitude + yMagnitude;

                double xAngleContribution = xMagnitude + yMagnitude > 0 ? xMagnitude / (xMagnitude + yMagnitude) : 0;

                double angle = angleMagnitude * MaxAngle * 180 / Math.PI;

                double depression = (1 - angleMagnitude) * MaxDepression;

                ContentProjection.RotationY = angle * xAngleContribution * xDirection;
                ContentProjection.RotationX = angle * (1 - xAngleContribution) * yDirection;
                ContentProjection.GlobalOffsetZ = -depression;
            }
            catch (ArgumentException)
            {
                //Happens when you change a page on mouse down / up
                // Not sure why yet
            }
        }

        bool isPressed;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            isPressed = true;
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            isPressed = false;
            base.OnMouseLeave(e);
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (isPressed)
            {
                Dispatcher.BeginInvoke(delegate
                {
                    if (Click != null)
                        Click(this, new RoutedEventArgs());
                });
            }
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            base.OnManipulationCompleted(e);
            TiltBackStoryboard.Begin();
        }

        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            base.OnManipulationDelta(e);
            DepressAndTilt(e.ManipulationOrigin, e.ManipulationContainer);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            contentPresenter = GetImplementationRoot(this) as ContentPresenter;
            Storyboard.SetTarget(XRotationAnimation, this.ContentProjection);
            Storyboard.SetTargetProperty(XRotationAnimation, RotationXProperty);

            Storyboard.SetTarget(YRotationAnimation, this.ContentProjection);
            Storyboard.SetTargetProperty(YRotationAnimation, RotationYProperty);

            Storyboard.SetTarget(ZRotationAnimation, this.ContentProjection);
            Storyboard.SetTargetProperty(ZRotationAnimation, GlobalOffsetZProperty);

            TiltBackStoryboard.Completed += TiltBackCompleted;
        }

        private void TiltBackCompleted(object sender, EventArgs e)
        {
            TiltBackStoryboard.Stop();
            this.ContentProjection.RotationX =
                this.ContentProjection.RotationY =
                this.ContentProjection.GlobalOffsetZ = 0;
            //isTilting = false;
        }

        public static FrameworkElement GetImplementationRoot(DependencyObject dependencyObject)
        {
            return (1 == VisualTreeHelper.GetChildrenCount(dependencyObject)) ?
                VisualTreeHelper.GetChild(dependencyObject, 0) as FrameworkElement :
                null;
        }
    }

    public class LogarithmicEase : EasingFunctionBase
    {
        private const double NaturalLog2 = 0.693147181;

        protected override double EaseInCore(double normalizedTime)
        {
            return Math.Log(normalizedTime + 1) / NaturalLog2;
        }
    }
}
