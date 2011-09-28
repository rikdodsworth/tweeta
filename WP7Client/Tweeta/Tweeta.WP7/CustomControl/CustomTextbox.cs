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

namespace Tweeta.CustomControl
{
    public class CustomTextbox : TextBox
    {
        TextBlock txtTip;

        public CustomTextbox()
        {
            this.TextChanged += new TextChangedEventHandler(CustomTextbox_TextChanged);
        }

        public override void OnApplyTemplate()
        {
            try
            {
                this.txtTip = this.GetTemplateChild("txtTip") as TextBlock;

                this.txtTip.Text = TipText;

                this.UpdateTextTipVisibility();
            }
            catch
            {

            }
            base.OnApplyTemplate();
        }

        public void UpdateTextTipVisibility()
        {
            if (txtTip == null)
                return;

            if (string.IsNullOrEmpty(this.Text))
                txtTip.Visibility = System.Windows.Visibility.Visible;
            else
                txtTip.Visibility = System.Windows.Visibility.Collapsed;
        }

        void CustomTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.MaxLength > 0 && this.Text.Length > this.MaxLength)
            {
                int ind = this.SelectionStart;
                this.Text = this.Text.Substring(0, this.MaxLength);
                this.SelectionStart = ind;
            }

            UpdateTextTipVisibility();
        }

        public static readonly DependencyProperty TipTextProperty =
                     DependencyProperty.Register("TipText",
                            typeof(string), typeof(CustomTextbox),
                            new PropertyMetadata("",
                          new PropertyChangedCallback(OnTipTextChanged)));

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UpdateBinding();
            }
            base.OnKeyDown(e);
        }
        private void UpdateBinding()
        {
            var binding = this.GetBindingExpression(TextBox.TextProperty);
            if (binding != null)
            {
                binding.UpdateSource();
            }

        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            UpdateBinding();
            base.OnKeyUp(e);
        }

        private static void OnTipTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {

        }

        public string TipText
        {
            get
            {
                return (string)GetValue(TipTextProperty);
            }
            set
            {
                SetValue(TipTextProperty, value);
                if (txtTip != null)
                {
                    this.txtTip.Text = TipText;
                }
            }
        }
    }


}
