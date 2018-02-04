using CANDefinitions;
using CANHandler;
using OpenSkipperApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANMessages
{
    public class N2kMessage
    {
        #region Properties

        private PGNDefn _definition;
        private N2kFrame _frame;

        public PGNDefn Definition
        {
            get { return _definition;  }
        }

        public N2kFrame Frame
        {
            get { return _frame; }
            private set { _frame = value; }
        }

        #endregion

        public N2kMessage(int pgn)
        {
            _definition = GetPGNDefn(pgn);

            if (Definition == null)
            {
                throw new ArgumentException("Could not find the definition for PGN " + pgn);
            }
        }

        public N2kMessage(int pgn, int priority, int source, int destination)
        {
            _definition = GetPGNDefn(pgn);

            if (Definition == null)
            {
                throw new ArgumentException("Could not find the definition for PGN " + pgn);
            }

            _frame = _definition.CreateMsg(priority.ToByte(), source.ToByte(), destination.ToByte(), Definition);
        }

        public N2kMessage(N2kFrame frame)
        {
            if (frame == null || frame.Defn == null)
            {
                throw new ArgumentException("A valid Frame was not provided");
            }

            _frame = frame;
            _definition = frame.Defn;
        }

        public void CreateFrame(int priority, int source, int destination)
        {
            _frame = _definition.CreateMsg(priority.ToByte(), source.ToByte(), destination.ToByte(), Definition);
        }

        #region Get Value

        public double GetDblField(string fieldName, out FieldValueState state)
        {
            var field = _frame.Defn.GetFieldByName(fieldName) as N2kDblField;
            return field.GetValue(_frame.Data, out state);
        }
        public double GetDblField(string fieldName)
        {
            FieldValueState state;
            return GetDblField(fieldName, out state);
        }

        public double GetUIntField(string fieldName, out FieldValueState state)
        {
            var field = _frame.Defn.GetFieldByName(fieldName) as N2kUIntField;
            return field.GetValue(_frame.Data, out state);
        }
        public double GetUIntField(string fieldName)
        {
            FieldValueState state;
            return GetUIntField(fieldName, out state);
        }

        public double GetUDblField(string fieldName, out FieldValueState state)
        {
            var field = _frame.Defn.GetFieldByName(fieldName) as N2kUDblField;
            return field.GetValue(_frame.Data, out state);
        }
        public double GetUDblField(string fieldName)
        {
            FieldValueState state;
            return GetUDblField(fieldName, out state);
        }

        #endregion

        #region Set Value

        public void SetDblField(string fieldName, double value, out FieldValueState state)
        {
            var field = _frame.Defn.GetFieldByName(fieldName) as N2kDblField;
            var bytes = field.SetValue(value, out state);
            bytes.CopyTo(_frame.Data, field.ByteOffset);
        }
        public void SetDblField(string fieldName, double value)
        {
            FieldValueState state;
            SetDblField(fieldName, value, out state);
        }

        public void SetUIntField(string fieldName, int value, out FieldValueState state)
        {
            var field = _frame.Defn.GetFieldByName(fieldName) as N2kUIntField;
            var bytes = field.SetValue(value, out state);
            bytes.CopyTo(_frame.Data, field.ByteOffset);
        }
        public void SetUIntField(string fieldName, int value)
        {
            FieldValueState state;
            SetUIntField(fieldName, value, out state);
        }

        public void SetUIntField(string fieldName, uint value, out FieldValueState state)
        {
            SetUIntField(fieldName, (int)value, out state);
        }
        public void SetUIntField(string fieldName, uint value)
        {
            FieldValueState state;
            SetUIntField(fieldName, value, out state);
        }

        public void SetUDblField(string fieldName, double value, out FieldValueState state)
        {
            var field = _frame.Defn.GetFieldByName(fieldName) as N2kUDblField;
            var bytes = field.SetValue(value, out state);
            bytes.CopyTo(_frame.Data, field.ByteOffset);
        }
        public void SetUDblField(string fieldName, double value)
        {
            FieldValueState state;
            SetUDblField(fieldName, value, out state);
        }

        #endregion

        /// <summary>
        /// Looks up the PGN Definition based on the PGN value
        /// </summary>
        /// <param name="pgn">PGN to lookup</param>
        /// <param name="data">????</param>
        /// <returns></returns>
        private PGNDefn GetPGNDefn(int pgn)
        {
            if (Definitions.PGNDefnCol == null)
            {
                Definitions.LoadPGNDefns(string.Empty);
            }

            uint PGN = (uint)pgn;
            int pgnIndex;
            if (!Definitions.PGNDefnCol.PGNDictionary.TryGetValue(PGN, out pgnIndex))
            {
                // No matching PGN definition; return
                return null;
            }

            PGNDefn pgnDef = Definitions.PGNDefnCol.PGNDefns[pgnIndex];

            // Is this a multiple definition PGN? (If so, it must be a fast packet PGN?)
            if (!pgnDef.HasMultipleDefinitions)
            {
                // Only one matching definition available; return it
                return Definitions.PGNDefnCol.PGNDefns[pgnIndex];
            }

            // Return the first match in the list
            return Definitions.PGNDefnCol.PGNDefns[0];
        }

    } // class

} // namespace
