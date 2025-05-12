namespace Lab3
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        //[STAThread]
        static void Main()
        {
            Model model = new Model(3);

            model.StartModel();
        }

    }

}
 