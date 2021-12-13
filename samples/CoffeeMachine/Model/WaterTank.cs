namespace CoffeeMachine.Model
{
    using System;

    public class WaterTank
    {
        /// <summary>
        /// The current level of water in the tank.
        /// </summary>
        public uint Level { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="WaterTank"/>.
        /// </summary>
        public WaterTank()
        {
            this.Level = 100;
        }

        public event EventHandler<LevelEventArgs> LevelChanged;

        /// <summary>
        /// Decreases the water level in the tank.
        /// </summary>
        /// <param name="amount">The amount that should be subtracted.</param>
        public void DecreaseWaterLevel(uint amount)
        {
            if (this.Level < amount)
            {
                throw new InvalidOperationException($"Not enough water. Current level is {this.Level}. Required level for operation is {amount}. Please fill tank.");
            }

            this.Level -= amount;

            this.RaiseLevelChanged(this.Level);
        }

        /// <summary>
        /// Fills the watertank.
        /// </summary>
        /// <param name="amount">The amount to be added.</param>
        public void FillWaterTank(uint amount = 100)
        {
            this.Level += amount;
        }

        /// <summary>
        /// Raises the <see cref="LevelChanged"/> event.
        /// </summary>
        /// <param name="level"></param>
        protected void RaiseLevelChanged(uint level)
        {
            this.LevelChanged?.Invoke(this, new LevelEventArgs(level));
        }
    }
}