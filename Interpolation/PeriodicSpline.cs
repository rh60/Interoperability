using System;
using System.Linq;
using vec = MathNet.Numerics.LinearAlgebra.Vector<double>;
using mat = MathNet.Numerics.LinearAlgebra.Matrix<double>;
using MathNet.Numerics.LinearAlgebra;
using MMP.Algebra.DoubleDouble;

namespace MMP.Interpolation
{
    public class PeriodicSpline
    {        
        vec h;
        Func<double, double>[,] S;

        public PeriodicSpline(vec t, params vec[] x)
        {            
            var n = t.Count - 1;
            var nx = x.Length;
            var X = mat.Build.DenseOfColumnVectors(x);

            var h0 = t.Diff();
            var h = vec.Build.Dense(t.Count);
            h0.CopySubVectorTo(h, 0, 0, n); h[n] = h0[0];
            var h1 = h.SubVector(1, n);
            this.h = h.SubVector(0, n); ;
            var A = mat.Build.SparseOfDiagonalVector(h0 + h1);
            A.SetDiag(h1, 1); A[0, n - 1] = h0[0];
            A += A.Transpose();

            X = X.Stack(X.SubMatrix(1, 1, 0, X.ColumnCount));
            var dX = X.Diff();
            dX = mat.Build.DenseOfColumnVectors(dX.EnumerateColumns().Select(par => par.PointwiseDivide(h)));
            var B = 3 * dX.Diff();
            var M = A.Solve(B);
            M = M.SubMatrix(M.RowCount - 1, 1, 0, M.ColumnCount).Stack(M);

            //Console.WriteLine(M);

            S = new Func<double, double>[n,nx];


            for (int j = 0; j < nx; j++)
            {
                var a2 = M.Column(j);

                var a0 = X.Column(j);
                
                for (int i = 0; i < n; i++)
                {
                    var a3 = (a2[i + 1] - a2[i]) / h[i] / 3;
                    var a1 = (a0[i + 1] - a0[i]) / h[i]-(2*a2[i]+a2[i+1])*h[i]/3;
                    S[i,j] = Horner.CreateEvaluator(a3,a2[i],a1, a0[i]);
                }
            }
        }

        public double[][] Interpolate(int n)
        {
            int N = n*h.Count;
            int ndim = S.GetLength(1);
            var R = new double[ndim][];
            for (int dim = 0; dim < ndim; dim++)
            {
                R[dim] = new double[N]; 
            }
            int pos = 0;
            for (int i = 0; i < h.Count; i++)
            {
                double s = 0;
                double dh = h[i] / n;
                for (int j = 0; j < n; j++)
                {                    
                    for (int dim = 0; dim < ndim; dim++)
                    {
                        R[dim][pos] = S[i, dim](s);
                    }
                    pos++; s += dh;
                }
            }
            return R;
        }           

    }
}
