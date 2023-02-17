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
using Mandelbrot_generator.Presentation;
using System.Text.RegularExpressions;

namespace Mandelbrot_generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel vm)
        {
            var viewModel = DataContext as MainViewModel;
            DataContext = vm;
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MainViewModel;
            if (viewModel.DoWorkCommand.CanExecute(null)) viewModel.DoWorkCommand.Execute(null);
        }
        private void SizeChangedEvent(object sender, SizeChangedEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            vm.ChangeResolution(this.Bitmap.ActualHeight, this.Bitmap.ActualWidth);
        }
        private void MouseMoveEvent(object sender, MouseEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            vm.MouseMoved(e.GetPosition(this.Bitmap));
        }
        private void MouseWeelEvent(object sender, MouseWheelEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            vm.MouseWheelMoved(e.Delta, e.GetPosition(this));
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void ReadWriteTB_TextChanged(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            if (NumberTextBox.Text == "")
                vm.TextboxIterations = 100;
            else vm.TextboxIterations = Convert.ToInt32(NumberTextBox.Text);
        }
        private void MouseLeftDownEvent(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            vm.MouseLeftButtonDown(e.GetPosition(this));
        }
        private void MouseLeftUpEvent(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            vm.MouseLeftButtonUp(e.GetPosition(this));
        }
    }
}
