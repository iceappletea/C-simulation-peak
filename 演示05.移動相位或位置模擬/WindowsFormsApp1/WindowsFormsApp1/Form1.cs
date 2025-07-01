using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScottPlot;
using ScottPlot.Plottables;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        //private FormsPlot formsPlot1; // Declare the FormsPlot control  
        double[] ts;
        double[] ys1, ys2, ys3, ysTotal;
        double[] ys21, ys22, ysTotal2;
        double wt, eps;
        int freq = 400;
        int dt = 0;
        double lambda = 0.85; // Wavelength in meters
        double h = 0.01; // Distance between the sources in meters
        double nh;
        double k; // Wave number (2 * pi / lambda)

        double r1, r2, r3; // Distances from the sources to the point of observation    
        double r21, r22;

        private void button2_Click_1(object sender, EventArgs e)
        {
            Application.Exit(); // Close the application 
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            // Create a new plot    
            //var plt = formsPlot1.Plot;  

            // Generate X-axis data    
            ts = Generate.Range(0, 10, 0.1);
            ys1 = new double[ts.Length];
            ys2 = new double[ts.Length];
            ys3 = new double[ts.Length];
            ysTotal = new double[ts.Length];

            ys21 = new double[ts.Length];
            ys22 = new double[ts.Length];
            ysTotal2 = new double[ts.Length];

            timer1.Interval = 300; // Set the timer interval to 1 second (1000 milliseconds)      
            timer1.Enabled = true; // Start the timer  
        }

        public Form1()
        {
            InitializeComponent();
            k = 2 * Math.PI / lambda; // Wave number
            this.WindowState = FormWindowState.Maximized;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            formsPlot1.Plot.Clear(); // Clear the previous plot data
            formsPlot2.Plot.Clear();
            eps = Math.PI * hScrollBar1.Value / 180.0; // Update the scrollbar value to reflect the current time step  
            wt = 2.0 * Math.PI * freq * dt / 20000.0;

            nh = hScrollBar2.Value * h; // Convert the scrollbar value to meters
            label3.Text = "Distance: " + nh.ToString("F2") + " m"; // Display the current distance in label3
            //Console.WriteLine($"nh = {nh} m");
            r1 = Math.Sqrt(Math.Pow(0 - 0, 2) + Math.Pow(2 - nh, 2));
            r2 = Math.Sqrt(Math.Pow(-Math.Sqrt(3) - 0, 2) + Math.Pow(-1 - nh, 2));
            r3 = Math.Sqrt(Math.Pow(Math.Sqrt(3) - 0, 2) + Math.Pow(-1 - nh, 2));
            //Console.WriteLine($"r1 = {r1}, r2 = {r2}, r3 = {r3}");
            r21 = Math.Sqrt(Math.Pow(-2 - nh, 2) + Math.Pow(0 - 0, 2));
            r22 = Math.Sqrt(Math.Pow(2 - nh, 2) + Math.Pow(0 - 0, 2));

            for (int i = 0; i < ts.Length; i++)
            {
                ys1[i] = Math.Cos(k*r1 + ts[i] - wt);
                ys2[i] = Math.Cos(k*r2 + ts[i] - wt + 2.0*Math.PI/3.0);
                ys3[i] = Math.Cos(k*r3 + ts[i] - wt + 4.0 * Math.PI/3.0 + eps);
                ysTotal[i] = ys1[i] + ys2[i] + ys3[i];

                ys21[i] = Math.Cos(k * r21 + ts[i] - wt);
                ys22[i] = Math.Cos(k * r22 + ts[i] - wt + Math.PI + eps);
                ysTotal2[i] = ys21[i] + ys22[i];
                //Console.WriteLine($"ts[{i}] = {ts[i]}, ys1[{i}] = {ys1[i]}, ys2[{i}] = {ys2[i]}, ys3[{i}] = {ys3[i]}, ysTotal[{i}] = {ysTotal[i]}");
            }

            label2.Text = "Shift: " + hScrollBar1.Value.ToString("F2"); // Display the current value of the scrollbar in label2

            label1.Text =  "3 waves Interference Peak: "+ysTotal.Max().ToString("F2");
            label4.Text = "2 waves Interference Peak: " + ysTotal2.Max().ToString("F2");
            // Add data to the plot    
            formsPlot1.Plot.Add.Scatter(ts, ys1);
            formsPlot1.Plot.Add.Scatter(ts, ys2);
            formsPlot1.Plot.Add.Scatter(ts, ys3);
            formsPlot1.Plot.Add.Scatter(ts, ysTotal);

            var sp1 = formsPlot2.Plot.Add.Scatter(ts, ys21);
            var sp2 = formsPlot2.Plot.Add.Scatter(ts, ys22);
            var sp3 = formsPlot2.Plot.Add.Scatter(ts, ysTotal2);
            sp3.Color = ScottPlot.Colors.Red;

            // Set plot title and axis labels
            formsPlot1.Plot.Title("Three-Waves Interferences");
            formsPlot1.Plot.XLabel("Time");
            formsPlot1.Plot.YLabel("Amplitude");

            formsPlot2.Plot.Title("Two-Waves Interferences");
            formsPlot2.Plot.XLabel("Time");
            formsPlot2.Plot.YLabel("Amplitude");

            formsPlot1.Plot.Axes.SetLimits(0, 10, -3, 3);
            formsPlot2.Plot.Axes.SetLimits(0, 10, -3, 3);

            // Show legend    
            formsPlot1.Plot.ShowLegend();
            formsPlot2.Plot.ShowLegend();

            // Refresh the plot to display changes
            formsPlot1.Refresh();
            formsPlot2.Refresh();

            dt++; // Increment the time step for the next iteration
        }
    }
}