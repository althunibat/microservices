using Framework.Api;
using MessagePack.Resolvers;

namespace SampleApi {
    public class Program {
        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args) => ProgramBase<Startup>.Run(args);
    }
}