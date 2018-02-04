using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANHandler
{
    public class AISRoT : AISField
    {
        public override double GetValue(AISData aisData, out FieldValueState valueState)
        {
            valueState = FieldValueState.Valid;
            long value = AISEncoding.GetSigned(aisData.AISBytes, BitOffset, BitLength);
            return (value * Math.Abs(value)) / (4.733 * 4.733);
        }

        public override string ToString(AISData aisData)
        {
            long value = AISEncoding.GetSigned(aisData.AISBytes, BitOffset, BitLength);
            switch (value)
            {
                case -128: return "Data not available";
                case -127: return "Turning left at >5deg/30s (No TI available)";
                case 127: return "Turning right at >5deg/30s (No TI available)";
                default:
                    return ((value * Math.Abs(value)) / (4.733 * 4.733)).ToString();
            }
        }
    }

} // namespace
