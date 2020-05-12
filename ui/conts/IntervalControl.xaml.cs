using LORA.Modules;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace LORA.ui.conts
{
    /// <summary>
    /// Interaction logic for IntervalControl.xaml
    /// </summary>
    public partial class IntervalControl : UserControl
    {
        public int MaximuzedTo = 10;
        public IntervalControl(Sensor node)
        {
            InitializeComponent();

            // add the lable of:
            Stack_intervals.Children.Add(new Label() { Content = "Node:"+node.ID, HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center, VerticalContentAlignment = System.Windows.VerticalAlignment.Center, Height = 24, Width = 80 });

            for (int i = 0; i < node.DutyCycleString.Count-1; i++)
            {
                try
                {
                    if(i==0)
                    {
                        int start = node.DutyCycleString[i]; 
                        int end = node.DutyCycleString[i + 1];
                        double width = end - start;

                        Border lbl_intialize = new Border();
                        lbl_intialize.BorderThickness = new System.Windows.Thickness(1, 1, 1, 0);
                        lbl_intialize.BorderBrush = Brushes.Black;
                        lbl_intialize.Height = 24;
                        lbl_intialize.Width = start * MaximuzedTo;
                        lbl_intialize.BorderThickness = new System.Windows.Thickness(0, 0, 0, 1);
                        lbl_intialize.BorderBrush = Brushes.Black;


                        Border lbl = new Border();
                        lbl.BorderThickness = new System.Windows.Thickness(1, 1, 1, 0);
                        lbl.BorderBrush = Brushes.Black;
                        lbl.Height = 24;
                        lbl.Width = width * MaximuzedTo;
                        //lbl.Background = Brushes.Gold;
                      //  lbl.ToolTip = new Label() { Content = "["+start.ToString()+","+end.ToString()+"]" };

                        Stack_intervals.Children.Add(lbl_intialize);
                        Stack_intervals.Children.Add(lbl);
                    }
                    else
                    {
                        int start = node.DutyCycleString[i];
                        int end = node.DutyCycleString[i + 1];
                        double width = end - start;
                        Border lbl = new Border();
                        lbl.Height = 24;
                        lbl.Width = width * MaximuzedTo;
                       // lbl.ToolTip = new Label() { Content = "[" + start.ToString() + "," + end.ToString() + "]" };
                        if (i % 2 == 0)
                        {
                           // lbl.Background = Brushes.Gold;
                            lbl.BorderThickness = new System.Windows.Thickness(0, 1, 1, 0);
                            lbl.BorderBrush = Brushes.Black;
                        }
                        else
                        {
                           // lbl.Background = Brushes.Tan;
                            lbl.BorderThickness = new System.Windows.Thickness(0, 0, 1, 1);
                            lbl.BorderBrush = Brushes.Black;
                        }
                        Stack_intervals.Children.Add(lbl);
                       
                    }
                }
                catch
                {

                }
            }


        }



        public IntervalControl(int blockSize,int IntervalsCounts) 
        {
            InitializeComponent();
            int x = IntervalsCounts / blockSize;
            // add the lable of:
            Stack_intervals.Children.Add(new Label() { Content = "", HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center, VerticalContentAlignment = System.Windows.VerticalAlignment.Center, Height = 24, Width = 80 });

            for (int i = 0; i <= IntervalsCounts; i++)
            {
                Label lbl = new Label();
                lbl.Width = blockSize * MaximuzedTo;
                lbl.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                lbl.Content = (i * blockSize);
                Stack_intervals.Children.Add(lbl);
            }

           
        }
    }
}
