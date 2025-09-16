using PTP.Query;
using PTP.Server;

namespace PTP.DB
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("REPL ---------------------(type 'exit' to quit)");

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();

                if (input == null) continue;

                input = input.Trim();

                if (input.ToLower() == "exit")
                    break;

                string output = Evaluate(input);

                Console.WriteLine(output);
            }
        }

        private static string Evaluate(string input)
        {
            PTPDB db = new PTPDB("ptpdb.db");

            IPlan plan = db.planner().CreatePlan(input, db.NewTx());

            return plan.ToString();
        }
    }
}
