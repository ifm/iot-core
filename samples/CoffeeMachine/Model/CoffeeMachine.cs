namespace CoffeeMachine.Model
{
    using Newtonsoft.Json;

    public class CoffeeMachine
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CoffeeMachine"/>.
        /// </summary>
        public CoffeeMachine()
        {
            this.WaterTank = new WaterTank();
        }

        /// <summary>
        /// Gets the <see cref="WaterTank"/> of this <see cref="CoffeeMachine"/>.
        /// </summary>
        public WaterTank WaterTank { get; }

        /// <summary>
        /// Creates an instance of <see cref="Capuccino"/>.
        /// </summary>
        /// <param name="addSugar">True if the created capuccino should have sugar, false if not.</param>
        /// <returns>A newly created instance of <see cref="Capuccino"/>.</returns>
        public Capuccino CreateCapuccino(bool addSugar)
        {
            uint liquidAmount = 5;

            this.WaterTank.DecreaseWaterLevel(liquidAmount);
            return new Capuccino() {  HasSugar = addSugar, LiquidAmount = liquidAmount};
        }

        /// <summary>
        /// Creates an instance of <see cref="LatteMachiatto"/>.
        /// </summary>
        /// <returns>A newly created instance of <see cref="LatteMachiatto"/>.</returns>
        public LatteMachiatto CreateLatteMacchiato()
        {
            uint liquidAmount = 10;
            this.WaterTank.DecreaseWaterLevel(liquidAmount);
            return new LatteMachiatto() { LiquidAmount = liquidAmount };
        }

        /// <summary>
        /// Creates an instance of <see cref="LatteMachiatto"/>.
        /// </summary>
        /// <returns>A newly created instance of <see cref="LatteMachiatto"/>.</returns>
        public LatteMachiatto CreateSpecialLatteMachiatto(bool sugar, uint amount)
        {
            this.WaterTank.DecreaseWaterLevel(amount);
            return new LatteMachiatto() { HasSugar = sugar, LiquidAmount = amount };
        }
    }

    public class SpecialLatteMacciatoParameters
    {
        [JsonProperty(PropertyName = "add_sugar", Required = Required.Always)]
        public bool AddSugar { get; set; }

        [JsonProperty(PropertyName = "liquid_amount", Required = Required.Always)]
        public uint Amount { get; set; }
    }
}
