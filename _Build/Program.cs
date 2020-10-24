using FlubuCore.Scripting;

namespace BuildScript
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var engine = new FlubuEngine();

            engine.RunScript<BuildScript>(args);
        }
    }
}
