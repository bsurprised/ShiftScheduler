
namespace Onyx.ShiftScheduler.Core.App
{
    public class RuleSet
    {
        public int InitialState { get; set; }

        public int[] AcceptingStates { get; set; }

        public int[,] Tuples { get; set; }
    }
}
