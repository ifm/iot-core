namespace CoffeeMachine.Model
{
    public class LevelEventArgs
    {
        public LevelEventArgs(uint level)
        {
            this.Level = level;
        }

        public uint Level { get; set; }
    }
}