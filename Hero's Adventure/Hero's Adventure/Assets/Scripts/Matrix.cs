using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matrix
{
    private int n;
    private int m = 0;
    private int currentPosIndexI, currentPosIndexJ;
    private int[,] matrix;
    public Matrix() { }
    public Matrix(int n,int m)
    {
        this.n = n;
        this.m = m;
        currentPosIndexI = 0;
        currentPosIndexJ = 0;
        int k = 1;
        matrix = new int[n,m];
        for(int i = 0; i < n; i++)
        {
            for(int j = 0; j < m; j++)
            {
                matrix[i, j] = k;
                k++;
            }
        }
    }
    public int GetN()
    {
        return n;
    }
    public int GetM()
    {
        return m;
    }
    public int[,] GetMatrix()
    {
        return matrix;
    }
    public int GetCurrentIndex()
    {
        return matrix[currentPosIndexI, currentPosIndexJ];
    }
    public void SetCurrentIndex(int index)
    {
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                if(matrix[i,j] == index)
                {
                    currentPosIndexI = i;
                    currentPosIndexJ = j;
                }
            }
        }
    }
    public void MoveLeft()
    {
        currentPosIndexJ--;
        if(currentPosIndexJ < 0 && currentPosIndexI == 0)
        {
            currentPosIndexJ = m - 1;
            currentPosIndexI = n - 1;
        }
        if(currentPosIndexJ < 0 && currentPosIndexI != 0)
        {
            currentPosIndexJ = m - 1;
            currentPosIndexI -= 1;
        }
    }
    public void MoveRight()
    {
        currentPosIndexJ++;
        if(currentPosIndexJ == m && currentPosIndexI != n - 1)
        {
            currentPosIndexJ = 0;
            currentPosIndexI += 1;
        }
        if(currentPosIndexJ == m && currentPosIndexI == n - 1)
        {
            currentPosIndexI = 0;
            currentPosIndexJ = 0;
        }
    }
    public void MoveUp()
    {
        currentPosIndexI--;
        if(currentPosIndexI < 0)
        {
            currentPosIndexI = n - 1;
        }
    }

    public void MoveDown()
    {
        currentPosIndexI++;
        if(currentPosIndexI == n)
        {
            currentPosIndexI = 0;
        }
    }
}
