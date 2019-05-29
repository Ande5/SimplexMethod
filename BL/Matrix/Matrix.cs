﻿using System;

namespace BL.Matrix
{
    public class Matrix
    {
        static readonly Random rnd = new Random();

        decimal[,] _array;
        int row, column;

        public int Row => row;
        public int Column => column;

        public void Random()
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    _array[i, j] = rnd.Next(10);
                }
            }
        }

        public void Random(int min, int max)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    _array[i, j] = rnd.Next(min, max);
                }
            }
        }

        public Matrix(int row, int colunm)
        {
            this.row = row;
            this.column = colunm;
            _array = new Decimal[row, column];
        }

        public Matrix Transpose()
        {
            Matrix m = new Matrix(column, row);

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    m._array[j, i] = _array[i, j];
                }
            }

            return m;
        }

        public void TransposeMyself()
        {
            _array = Transpose()._array;
        }

        public Matrix Inverse()
        {
            Decimal det = Determinant();
            if (det == 0)
            {
                throw new Exception("Матрица вырождена");
            }

            Matrix m = new Matrix(row, column);

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    m._array[i, j] = Cofactor(_array, i, j) / det;
                }
            }

            return m.Transpose();
        }

        public Decimal Determinant()
        {
            if (column != row)
            {
                throw new Exception("Расчет определителя невозможен");
            }
            return Determinant(_array);
        }

        private Decimal Determinant(Decimal[,] array)
        {
            int n = (int)Math.Sqrt(array.Length);

            if (n == 1)
            {
                return array[0, 0];
            }

            Decimal det = 0;

            for (int k = 0; k < n; k++)
            {
                det += array[0, k] * Cofactor(array, 0, k);
            }

            return det;
        }

        private Decimal Cofactor(Decimal[,] array, int row, int column) 
            => Convert.ToDecimal(Math.Pow(-1, column + row)) * Determinant(Minor(array, row, column));

        private Decimal[,] Minor(Decimal[,] array, int row, int column)
        {
            int n = (int)Math.Sqrt(array.Length);
            Decimal[,] minor = new Decimal[n - 1, n - 1];

            int _i = 0;
            for (int i = 0; i < n; i++)
            {
                if (i == row)
                {
                    continue;
                }
                int _j = 0;
                for (int j = 0; j < n; j++)
                {
                    if (j == column)
                    {
                        continue;
                    }
                    minor[_i, _j] = array[i, j];
                    _j++;
                }
                _i++;
            }
            return minor;
        }

        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            if (m1.row != m2.row || m1.column != m2.column)
            {
                throw new Exception("Сложение невозможно");
            }

            Matrix m = new Matrix(m1.row, m1.column);

            for (int i = 0; i < m1.row; i++)
            {
                for (int j = 0; j < m1.column; j++)
                {
                    m._array[i, j] = m1._array[i, j] + m2._array[i, j];
                }
            }

            return m;
        }

        public static Matrix operator -(Matrix m1, Matrix m2)
        {
            if (m1.row != m2.row || m1.column != m2.column)
            {
                throw new Exception("Вычитание невозможно");
            }

            Matrix m = new Matrix(m1.row, m1.column);

            for (int i = 0; i < m1.row; i++)
            {
                for (int j = 0; j < m1.column; j++)
                {
                    m._array[i, j] = m1._array[i, j] - m2._array[i, j];
                }
            }

            return m;
        }

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            if (m1.column != m2.row)
            {
                throw new Exception("Умножение невозможно");
            }

            Matrix m = new Matrix(m1.row, m2.column);

            for (int i = 0; i < m1.row; i++)
            {
                for (int j = 0; j < m2.column; j++)
                {
                    decimal sum = 0;

                    for (int k = 0; k < m1.column; k++)
                    {
                        sum += m1._array[i, k] * m2._array[k, j];
                    }

                    m._array[i, j] = sum;
                }
            }

            return m;
        }

        public override string ToString()
        {
            string str = "";

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    str += _array[i, j] + "\t";
                }
                str += "\n";
            }

            return str;
        }


    }
}
