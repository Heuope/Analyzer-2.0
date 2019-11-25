using Microsoft.Win32;
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

namespace Minecraft
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static public List<string> codeList;
        Box box = new Box();
        private string _filePath;

        public MainWindow()
        {
            InitializeComponent();
        }
      
        private void Load(object sender, RoutedEventArgs e)
        {            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                _filePath = openFileDialog.FileName;

                box.FirstMetric.Clear();
                box.SecondMetric.Clear();
                box.ThirdMetric.Clear();

                box = Chepin.NotMain(_filePath);

                foreach (var item in box.FirstMetric.Keys)
                {                    
                    ComboBoxChepinSpen.Items.Add(item);
                    ComboBoxChepinIO.Items.Add(item);
                }
            }
        }        

        private void ChepinSpen_Loaded(object sender, RoutedEventArgs e)
        {
                   
        }

        private void ComboBoxChepinSpen_Selected(object sender, RoutedEventArgs e)
        {
            var spenMetrics = new List<SpenMetric>();

            spenMetrics.Add(new SpenMetric(box.FirstMetric[ComboBoxChepinSpen.SelectedItem.ToString()]));

            ChepinSpen.ItemsSource = spenMetrics;
        }

        private void ComboBoxChepinIO_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string funName = ComboBoxChepinIO.SelectedItem.ToString();

            int i = 0;
            double sum = 0;
            foreach (var item in box.SecondMetric[funName].Values)
            {
                string temp = "";
                int cnt = 0;
                foreach (var key in item.Keys)
                {
                    cnt++;
                    temp += key + ",\n";
                }
                
                switch (i)
                {
                    case 0:
                        _1.Text = temp;
                        _9.Text = cnt.ToString();
                        sum += cnt * 1;
                        break;
                    case 1:
                        _2.Text = temp;
                        _10.Text = cnt.ToString();
                        sum += cnt * 2;
                        break;
                    case 2:
                        _3.Text = temp;
                        _11.Text = cnt.ToString();
                        sum += cnt * 3;
                        break;
                    case 3:
                        sum += cnt * 0.5;
                        _4.Text = temp;
                        _12.Text = cnt.ToString();
                        break;                                            
                    default:
                        break;
                }
                i++;
            }
            i = 0;
            _17.Text = sum.ToString();
            sum = 0;
            foreach (var item in box.ThirdMetric[funName].Values)
            {
                string temp = "";
                int cnt = 0;
                foreach (var key in item.Keys)
                {
                    cnt++;
                    temp += key + ",\n";
                }

                switch (i)
                {
                    case 0:
                        sum += cnt * 1;
                        _5.Text = temp;
                        _13.Text = cnt.ToString();
                        break;
                    case 1:
                        sum += cnt * 2;
                        _6.Text = temp;
                        _14.Text = cnt.ToString();
                        break;
                    case 2:
                        sum += cnt * 3;
                        _7.Text = temp;
                        _15.Text = cnt.ToString();
                        break;
                    case 3:
                        sum += cnt * 0.5;
                        _8.Text = temp;
                        _16.Text = cnt.ToString();
                        break;
                    default:
                        break;
                }
                i++;
                _18.Text = sum.ToString();
            }
        }      
    }
}
