using System;
using System.Collections.Generic;
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
using System.Text.RegularExpressions;
using System.Globalization;
using System.ComponentModel;
using System.Diagnostics;

namespace Hypocycloid
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HypocycloidCurve curve;

        private Ellipse outerCircle;
        private Ellipse innerCircle;
        private Ellipse alpha;
        private List<Line> trail;

        private Point canvasMid;

        public MainWindow()
        {
            InitializeComponent();

            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            speedTextBox.Text = "1.0";
            StartButton.IsEnabled = false;

            curve = new HypocycloidCurve();

            trail = new List<Line>();

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += ComputeCurve;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(10);
            dispatcherTimer.Start();

            innerCircle = new Ellipse()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            outerCircle = new Ellipse()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            alpha = new Ellipse()
            {
                Width = 8,
                Height = 8,
                Fill = new SolidColorBrush(Color.FromArgb(255, 0, 150, 255))
            };

            MainCanvas.Children.Add(innerCircle);
            MainCanvas.Children.Add(outerCircle);
            MainCanvas.Children.Add(alpha);
        }

        private void ComputeCurve(object sender, EventArgs e)
        {
            curve.Compute();

            tTextBox.Text = curve.t.ToString();

            //outerCircle.SetValue(Canvas.LeftProperty, canvasMid.x + curve.O.x);
            //outerCircle.SetValue(Canvas.TopProperty, canvasMid.y - curve.O.y);

            //innerCircle.SetValue(Canvas.LeftProperty, canvasMid.x + curve.o.x);
            //innerCircle.SetValue(Canvas.TopProperty, canvasMid.y - curve.o.y);

            canvasMid = new Point(MainCanvas.ActualWidth / 2, MainCanvas.ActualHeight / 2);

            Canvas.SetLeft(outerCircle, canvasMid.x + curve.O.x - curve.R);
            Canvas.SetTop(outerCircle, canvasMid.y - curve.O.y - curve.R);

            Canvas.SetLeft(innerCircle, canvasMid.x + curve.o.x - curve.r);
            Canvas.SetTop(innerCircle, canvasMid.y - curve.o.y - curve.r);

            Canvas.SetLeft(alpha, canvasMid.x + curve.alpha.x - alpha.Width / 2);
            Canvas.SetTop(alpha, canvasMid.y - curve.alpha.y - alpha.Height / 2);

            outerCircle.Width = 2 * curve.R;
            outerCircle.Height = 2 * curve.R;

            innerCircle.Width = 2 * curve.r;
            innerCircle.Height = 2 * curve.r;

            int trainCount = curve.trail.Count;
            if (trainCount > 1)
            {
                Line newLine = new Line()
                {
                    X1 = canvasMid.x + curve.trail[trainCount - 2].x,
                    Y1 = canvasMid.y - curve.trail[trainCount - 2].y,
                    X2 = canvasMid.x + curve.trail[trainCount - 1].x,
                    Y2 = canvasMid.y - curve.trail[trainCount - 1].y,
                    Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 150, 255)),
                    StrokeThickness = 2
                };
                trail.Add(newLine);
                MainCanvas.Children.Add(newLine);
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void rTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (Double.TryParse(rTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double r))
            {
                if (Double.TryParse(RTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double R))
                {
                    RkTextBox.Text = (R / r).ToString();
                }
                else if (Double.TryParse(RkTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double Rk))
                {
                    RTextBox.Text = (r * Rk).ToString();
                    curve.R = r * Rk;
                }
                curve.r = r;
            }
            CheckIfStartPossible();
        }

        private void RTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (Double.TryParse(RTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double R))
            {
                if (Double.TryParse(rTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double r))
                {
                    RkTextBox.Text = (R / r).ToString();
                }
                else if (Double.TryParse(RkTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double Rk))
                {
                    rTextBox.Text = (R / Rk).ToString();
                    curve.r = R / Rk;
                }
                curve.R = R;
            }
            CheckIfStartPossible();
        }

        private void RkTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (Double.TryParse(RkTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double Rk))
            {
                if (Double.TryParse(RTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double R))
                {
                    rTextBox.Text = (R / Rk).ToString();
                    curve.r = R / Rk;
                }
                else if (Double.TryParse(rTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double r))
                {
                    RTextBox.Text = (r * Rk).ToString();
                    curve.R = r * Rk;
                }
            }
            CheckIfStartPossible();
        }

        private void SpeedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Double.TryParse(speedTextBox.Text, out double speed) && curve != null)
            {
                curve.speed = speed;
            }
            CheckIfStartPossible();
        }

        private void CheckIfStartPossible()
        {
            StartButton.IsEnabled =
                Double.TryParse(rTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double f) &&
                Double.TryParse(RTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out f) &&
                Double.TryParse(speedTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out f);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            curve.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            curve.Stop();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var line in trail)
            {
                if (MainCanvas.Children.Contains(line))
                {
                    MainCanvas.Children.Remove(line);
                }
            }
            trail.Clear();
            curve.Reset();
        }

        private void CirclesCheckBox_Click(object sender, RoutedEventArgs e)
        {
            innerCircle.Visibility = circlesCheckBox.IsChecked ?? false ? Visibility.Visible : Visibility.Hidden;
            outerCircle.Visibility = circlesCheckBox.IsChecked ?? false ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
