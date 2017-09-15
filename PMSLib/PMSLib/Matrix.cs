using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using mat=MathNet.Numerics.LinearAlgebra;

namespace PMSLib
{
    [ClassInterface(ClassInterfaceType.None)]
    public class Matrix : IMatrix
    {
        public object Mult(object VBinput1, object VBinput2)
        {
            try
            {
                mat.Matrix<double> myMatrix1 = loadIntoMatrix(VBinput1);
                mat.Matrix<double> myMatrix2 = loadIntoMatrix(VBinput2);

                mat.Matrix<double> myOutputMatrix = myMatrix1.Multiply(myMatrix2);
                return myOutputMatrix.ToArray();
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

        public object Inv(object VBinput)
        {
            try
            {
                mat.Matrix<double> myMatrix = loadIntoMatrix(VBinput);
                return myMatrix.Inverse().ToArray(); //myMatrix.Determinant() == 0 ? null : myMatrix.Inverse().ToArray();
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

        private mat.Matrix<double> loadIntoMatrix(object VBinput)
        {
            if (VBinput == null) return null;
            var tempVBinput = (Array)VBinput;
            if (tempVBinput.Rank == 2)
            {
                mat.Matrix<double> myMatrix = mat.CreateMatrix.Dense<double>(tempVBinput.GetUpperBound(0) - tempVBinput.GetLowerBound(0) + 1, tempVBinput.GetUpperBound(1) - tempVBinput.GetLowerBound(1) + 1, 0);
                int ligne = 0;
                int colonne = 0;
                for (int i = tempVBinput.GetLowerBound(0); i <= tempVBinput.GetUpperBound(0); i++)
                {
                    colonne = 0;
                    for (int j = tempVBinput.GetLowerBound(1); j <= tempVBinput.GetUpperBound(1); j++)
                    {
                        double s;
                        if(Double.TryParse(tempVBinput.GetValue(i, j).ToString(), out s)) myMatrix.At(ligne,colonne, s);
                        colonne++;
                    }
                    ligne++;
                }

                return myMatrix;
            }
            else if (tempVBinput.Rank == 1)
            {
                mat.Vector<double> myVector = mat.CreateVector.Dense<double>(tempVBinput.GetUpperBound(0) - tempVBinput.GetLowerBound(0) + 1, 0);
                int ligne = 0;
                for (int i = tempVBinput.GetLowerBound(0); i <= tempVBinput.GetUpperBound(0); i++)
                {
                    double s;
                    if(Double.TryParse(tempVBinput.GetValue(i).ToString(), out s)) myVector.At(ligne, s);
                    ligne++;
                }

                return myVector.ToColumnMatrix();
            }
            else
            { return null; }

        }
    }
}
