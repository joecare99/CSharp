using ILGPU;
using ILGPU.Runtime;
using System;
using System.Collections.Generic;

public class RMatrixKernels
{
    public static void AdditionKernel(Index2D index, ArrayView2D<int, Stride2D.DenseX> matrixA, ArrayView2D<int, Stride2D.DenseX> matrixB, ArrayView2D<int, Stride2D.DenseX> result)
    {
        result[index] = matrixA[index] + matrixB[index];
    }

    public static void ScalarMultiplyKernel(
        Index2D index,
        ArrayView2D<int, Stride2D.DenseX> aView, int scalar,
        ArrayView2D<int, Stride2D.DenseX> cView)
    {
        cView[index] = aView[index] * scalar;
    }
}

public static class RMatrixExtensions
{
    public static Action<Index2D, ArrayView2D<int, Stride2D.DenseX>, ArrayView2D<int, Stride2D.DenseX>, ArrayView2D<int, Stride2D.DenseX>> BuildAdditionKernel(this Accelerator accelerator_)
    {
        return accelerator_.LoadAutoGroupedStreamKernel<Index2D, ArrayView2D<int, Stride2D.DenseX>, ArrayView2D<int, Stride2D.DenseX>, ArrayView2D<int, Stride2D.DenseX>>(RMatrixKernels.AdditionKernel);
    }
    public static Action<Index2D, ArrayView2D<int, Stride2D.DenseX>, int, ArrayView2D<int, Stride2D.DenseX>> BuildSkalarMultiplyKernel(this Accelerator accelerator_)
    {
        return accelerator_.LoadAutoGroupedStreamKernel<Index2D, ArrayView2D<int, Stride2D.DenseX>, int, ArrayView2D<int, Stride2D.DenseX>>(RMatrixKernels.ScalarMultiplyKernel);
    }
}

public class RMatrix
{

    private int[,] matrix_;
    public int Rows { get; private set; }
    public int Cols { get; private set; }

    private static Accelerator accelerator_;

    static RMatrix()
    {
        var context_ = Context.CreateDefault();
        accelerator_ = context_.GetPreferredDevice(preferCPU: true).CreateAccelerator(context_);
    }

    public RMatrix(int rows, int cols)
    {
        Rows = rows;
        Cols = cols;
        matrix_ = new int[rows, cols];
    }

    public RMatrix(int[,] arr)
    {
        Rows = arr.GetLength(0);
        Cols = arr.GetLength(1);
        matrix_ = new int[Rows, Cols];
        Array.Copy(arr, matrix_, arr.Length);
    }

    public static RMatrix operator +(RMatrix a, RMatrix b)
    {
        if (a.Rows != b.Rows || a.Cols != b.Cols)
            throw new ArgumentException("Matrix dimensions must match for addition.");

        var deviceMatrixA = accelerator_.Allocate2DDenseX<int>(new Index2D(a.Rows, a.Cols));
        var deviceMatrixB = accelerator_.Allocate2DDenseX<int>(new Index2D(a.Rows, a.Cols));
        var deviceResult = accelerator_.Allocate2DDenseX<int>(new Index2D(a.Rows, a.Cols));

        deviceMatrixA.CopyFromCPU(a.matrix_);
        deviceMatrixB.CopyFromCPU(b.matrix_);

        //TODO: 
        var kernel = accelerator_.BuildAdditionKernel();

        kernel((a.Rows, a.Cols), deviceMatrixA.View, deviceMatrixB.View, deviceResult.View);

        accelerator_.Synchronize();

        int[,] hostResult = new int[a.Rows, a.Cols];
        deviceResult.CopyToCPU(hostResult);

        deviceMatrixA.Dispose();
        deviceMatrixB.Dispose();
        deviceResult.Dispose();

        return new RMatrix(hostResult);
    }

    public static RMatrix operator *(RMatrix a, int scalar)
    {
        var deviceMatrixA = accelerator_.Allocate2DDenseX<int>(new Index2D(a.Rows, a.Cols));
        var deviceResult = accelerator_.Allocate2DDenseX<int>(new Index2D(a.Rows, a.Cols));

        deviceMatrixA.CopyFromCPU(a.matrix_);

        //TODO: 
        var kernel = accelerator_.BuildSkalarMultiplyKernel();

        kernel((a.Rows, a.Cols), deviceMatrixA.View, scalar, deviceResult.View);

        accelerator_.Synchronize();

        int[,] hostResult = new int[a.Rows, a.Cols];
        deviceResult.CopyToCPU(hostResult);

        deviceMatrixA.Dispose();
        deviceResult.Dispose();

        return new RMatrix(hostResult);
    }


    public void Show()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                Console.Write(matrix_[i, j] + "\t");
            }
            Console.WriteLine();
        }
    }
}

public static class Program
{
    static void Main(string[] arg)
    {
        int[,] hostMatrixA = {
                        { 1, 2, 3 },
                        { 4, 5, 6 },
                        { 7, 8, 9 }
                    };
        int[,] hostMatrixB = {
                        { 9, 8, 7 },
                        { 6, 5, 4 },
                        { 3, 2, 1 }
                    };

        RMatrix a = new RMatrix(hostMatrixA);
        RMatrix b = new RMatrix(hostMatrixB);

        RMatrix c = a + b;
        RMatrix d = a * 10;

        Console.WriteLine("Matrix A + B:");
        c.Show();
        Console.WriteLine("\nMatrix A * 10:");
        d.Show();
    }
}