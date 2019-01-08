using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var pm = new PlotModel
            {
                Title = "B-splines",
                //PlotType = PlotType.Cartesian,
                Background = OxyColors.White
            };

            var bs = new MMP.BSpline(0, 0, 0, 1, 2, 3, 4, 4, 5, 5, 5);

            foreach (var B in bs.Bases)
            {
                var ls = new LineSeries();
                foreach (var P in B)
                    ls.Points.Add(new DataPoint(P.x, P.y));
                pm.Series.Add(ls);               
            }
                                               
            this.plotView1.Model = pm;         

            var exporter = new PngExporter { Width = 600, Height = 400, Background = OxyColors.White };
            exporter.ExportToFile(pm, "fig1.png");

        }
    }
}
