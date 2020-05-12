using LORA.Computations;
using System.Collections.Generic;
using System.Windows;

namespace LORA.ui
{
    /// <summary>
    /// Interaction logic for ProbabilityofMultipleReceiversinSubzone.xaml
    /// </summary>
    public partial class UIProbabilityofMultipleReceiversinSubzone : Window
    {
        public class Record
        {
            public double m { get; set; }
            public double d { get; set; }
            public double T { get; set; }
            public double Pr0 { get; set; }
            public double pr1 { get; set; }
            public double Prg2 { get; set; }
        }

        public UIProbabilityofMultipleReceiversinSubzone()
        {
            InitializeComponent();
            double T = 5;
            double t = 3;
            List<Record> Lis = new List<Record>();
            
            for (double i = 3; i <= 20; i+=1)
            {
                MultipleReciverProblem x = new MultipleReciverProblem(i, T, t);
                Lis.Add(new Record() { m = i, T = T, d = t, Pr0 = x.Pr0, pr1 = x.Pr1, Prg2 = x.Prg2 });
            }
            /*
            double i = 4.09;
            MultipleReciverProblem x = new MultipleReciverProblem(i, T, t);
            Lis.Add(new Record() { m = i, T = T, d = t, Pr0 = x.Pr0, pr1 = x.Pr1, Prg2 = x.Prg2 });*/
            dg_date.ItemsSource = Lis;
        }
    }
}
