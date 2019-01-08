using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMP
{
    public class BSpline
    {
        public double[] U; // knots (Uzly)
        public int p; // order (Řád)
        public BSpline(params double[] knots)
        {
            U = knots;
            p = 0;
            for (int i = 1; i < U.Length; i++)
            {
                if (U[i] == U[i - 1])
                    p++;
                else
                    break;
            }
        }

        private int End => U.Length - p - 1;

        public int FindSpan(double u)
        {
            if (u == U[End])
                return End - 1;
            int low = p, high = End;
            int mid = (low + high) / 2;
            while (u < U[mid] || u >= U[mid + 1])
            {
                if (u < U[mid]) high = mid;
                else low = mid;
                mid = (low + high) / 2;
            }
            return mid;
        }

        public double[] BasisFuns(int i, double u)
        {
            double left(int j)
            {
                return u - U[i + 1 - j];
            }

            double right(int j)
            {
                return U[i + j] - u;
            }

            double[] N = new double[p + 1];
            N[0] = 1.0;
            for (int j = 1; j <= p; j++)
            {
                double saved = 0.0;
                for (int k = 0; k < j; k++)
                {
                    var temp = N[k] / (right(k + 1) + left(j - k));
                    N[k] = saved + right(k + 1) * temp;
                    saved = left(j - k) * temp;
                }
                N[j] = saved;
            }
            return N;
        }

        const double tol = 1E-6; 
        IEnumerable<double> Values(int i, int n=25)
        {
            double h = (U[i + 1] - U[i]) / n;
            if (h == 0)
                yield break;
            double value = U[i];
            while (value < U[i + 1]-tol)
            {
                yield return value;
                value += h;
            }
            yield return U[i + 1];
        }
      
        public class Point
        {
            public double x, y;
        }
        public Point[][] Bases
        {
            get
            {
                List<Stack<Point>> L = new List<Stack<Point>>();
                int start = -p;
                for (int i = 0; i + p + 1 < U.Length; i++)
                {
                    L.Add(new Stack<Point>());
                    if (Values(i).Any())
                    {
                        for (int j = 0; j <= p; j++)
                            if(L[start + j].Any())
                                L[start + j].Pop();
                        foreach (var u in Values(i))
                        {
                            var v = BasisFuns(i, u);
                            for (int j = 0; j <= p; j++)                            
                                L[start + j].Push(new Point { x = u, y = v[j] });                            
                        }                                                
                    }
                    start++;
                }
                return L.Select(a => a.Reverse().ToArray()).ToArray();
            }
        }

    }
}

    