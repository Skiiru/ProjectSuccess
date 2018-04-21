
namespace ProjectSuccessWPF
{
    public class WorkDuration
    {
        public double Estimated { get; set; }
        public double Spent { get; set; }
        public double Overtime { get; set; }

        public WorkDuration(double estimated, double spent)
        {
            Estimated = estimated;
            Spent = spent;
            Overtime = Spent - Estimated;
        }

        public WorkDuration() : this(0, 0) { }

        public double TotalDuration()
        {
            return Estimated > Spent ? Estimated : Spent;
        }

        public override string ToString()
        {
            return TotalDuration().ToString();
        }

        public void ReCalculateOvertime()
        {
            Overtime = Spent - Estimated;
        }

    }
}
