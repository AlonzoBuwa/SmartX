namespace SmartX.Api.Models
{
    public class SensorReading
    {
        public double Value { get; set; }

        public SensorReading(double value)
        {
            Value = value;
        }

        // Operator overloading
        public static SensorReading operator +(
            SensorReading a,
            SensorReading b)
        {
            return new SensorReading(a.Value + b.Value);
        }

        public static SensorReading operator -(
            SensorReading a,
            SensorReading b)
        {
            return new SensorReading(a.Value - b.Value);
        }

        public static bool operator >(
            SensorReading a,
            SensorReading b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(
            SensorReading a,
            SensorReading b)
        {
            return a.Value < b.Value;
        }

        public override string ToString()
        {
            return Value.ToString("F2");
        }
    }
}