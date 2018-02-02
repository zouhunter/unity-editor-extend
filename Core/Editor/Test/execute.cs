using System;

public class execute {
    [STAThread]
    static void Main(string[] arg)
    {
        int i = 10000;
        while (i>0)
        {
            Console.WriteLine(i--);
        }
    }
}
