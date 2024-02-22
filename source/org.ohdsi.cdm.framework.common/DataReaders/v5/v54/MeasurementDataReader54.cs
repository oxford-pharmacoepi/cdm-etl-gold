using org.ohdsi.cdm.framework.common.Builder;
using org.ohdsi.cdm.framework.common.Omop;
using System;
using System.Collections.Generic;
using System.Data;

namespace org.ohdsi.cdm.framework.common.DataReaders.v5.v54
{
    public class MeasurementDataReader54 : IDataReader
    {
        private readonly IEnumerator<Measurement> _enumerator;
        private readonly KeyMasterOffsetManager _offset;

        // A custom DataReader is implemented to prevent the need for the HashSet to be transformed to a DataTable for loading by SqlBulkCopy
        public MeasurementDataReader54(List<Measurement> batch, KeyMasterOffsetManager o)
        {
            _enumerator = batch?.GetEnumerator();
            _offset = o;
        }

        public bool Read()
        {
            return _enumerator.MoveNext();
        }

        public int FieldCount
        {
            get { return 22; }
        }

        // is this called only because the datatype specific methods are not implemented?  
        // probably performance to be gained by not passing object back?
        public object GetValue(int i)
        {

            switch (i)
            {
                //case 0:
                //    return _offset.GetId(_enumerator.Current.PersonId, _enumerator.Current.Id);
                case 0:
                    return _enumerator.Current.PersonId;
                case 1:
                    return _enumerator.Current.ConceptId;
                case 2:
                    return _enumerator.Current.StartDate;
                case 3:
                    return _enumerator.Current.StartDate;
                case 4:
                    return _enumerator.Current.Time;
                case 5:
                    return _enumerator.Current.TypeConceptId;
                case 6:
                    return _enumerator.Current.OperatorConceptId;
                case 7:
                    return _enumerator.Current.ValueAsNumber;
                case 8:
                    return _enumerator.Current.ValueAsConceptId;
                case 9:
                    return _enumerator.Current.UnitConceptId;
                case 10:
                    return _enumerator.Current.RangeLow;
                case 11:
                    return _enumerator.Current.RangeHigh;
                case 12:
                    return _enumerator.Current.ProviderId == 0 ? null : _enumerator.Current.ProviderId;
                case 13:
                    if (_enumerator.Current.VisitOccurrenceId.HasValue)
                        return _enumerator.Current.VisitOccurrenceId.Value;
                    else
                        return null;

                case 14:
                    if (_enumerator.Current.VisitDetailId.HasValue)
                        return _enumerator.Current.VisitDetailId;
                    else
                        return null;
                case 15:
                    return _enumerator.Current.SourceValue;
                case 16:
                    return _enumerator.Current.SourceConceptId;
                case 17:
                    return _enumerator.Current.UnitSourceValue;
                case 18:
                    //return _enumerator.Current.UnitSourceConceptId;
                    if (String.IsNullOrEmpty(_enumerator.Current.UnitSourceValue))
                        return null;
                    else
                        return _enumerator.Current.UnitSourceConceptId;
                case 19:
                    return _enumerator.Current.ValueSourceValue;
                case 20:
                    //return _enumerator.Current.MeasurementEventId;
                    return null;
                case 21:
                    //return _enumerator.Current.MeasEventFieldConceptId;
                    return null;

                default:
                    throw new NotImplementedException();
            }
        }

        public string GetName(int i)
        {
            switch (i)
            {
                //case 0: return "measurement_id";
                case 0: return "person_id";
                case 1: return "measurement_concept_id";
                case 2: return "measurement_date";
                case 3: return "measurement_datetime";
                case 4: return "measurement_time";
                case 5: return "measurement_type_concept_id";
                case 6: return "operator_concept_id";
                case 7: return "value_as_number";
                case 8: return "value_as_concept_id";
                case 9: return "unit_concept_id";
                case 10: return "range_low";
                case 11: return "range_high";
                case 12: return "provider_id";
                case 13: return "visit_occurrence_id";
                case 14: return "visit_detail_id";
                case 15: return "measurement_source_value";
                case 16: return "measurement_source_concept_id";
                case 17: return "unit_source_value";
                case 18: return "unit_source_concept_id";
                case 19: return "value_source_value";
                case 20: return "measurement_event_id";
                case 21: return "meas_event_field_concept_id";

                default:
                    throw new NotImplementedException();
            }
        }

        #region implementationn not required for SqlBulkCopy

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public bool IsClosed
        {
            get { throw new NotImplementedException(); }
        }

        public int Depth
        {
            get { throw new NotImplementedException(); }
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public int RecordsAffected
        {
            get { throw new NotImplementedException(); }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool GetBoolean(int i)
        {
            return (bool)GetValue(i);
        }

        public byte GetByte(int i)
        {
            return (byte)GetValue(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            return (char)GetValue(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            return (DateTime)GetValue(i);
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)GetValue(i);
        }

        public double GetDouble(int i)
        {
            return Convert.ToDouble(GetValue(i));
        }

        public Type GetFieldType(int i)
        {
            switch (i)
            {
                //case 0:
                //    return typeof(long);
                case 0:
                    return typeof(long);
                case 1:
                    return typeof(int);
                case 2:
                    return typeof(DateTime);
                case 3:
                    return typeof(DateTime);
                case 4:
                    return typeof(string);
                case 5:
                    return typeof(int?);
                case 6:
                    return typeof(int);
                case 7:
                    return typeof(decimal?);
                case 8:
                    return typeof(int);
                case 9:
                    return typeof(int);
                case 10:
                    return typeof(decimal?);
                case 11:
                    return typeof(decimal?);
                case 12:
                    return typeof(long?);
                case 13:
                    return typeof(long?);
                case 14:
                    return typeof(long?);
                case 15:
                    return typeof(string);
                case 16:
                    return typeof(int);
                case 17:
                    return typeof(string);
                case 18:
                    return typeof(int);
                case 19: 
                    return typeof(string);
                case 20: 
                    return typeof(long);
                case 21: 
                    return typeof(int);

                default:
                    throw new NotImplementedException();
            }
        }

        public float GetFloat(int i)
        {
            return (float)GetValue(i);
        }

        public Guid GetGuid(int i)
        {
            return (Guid)GetValue(i);
        }

        public short GetInt16(int i)
        {
            return (short)GetValue(i);
        }

        public int GetInt32(int i)
        {
            return (int)GetValue(i);
        }

        public long GetInt64(int i)
        {
            return Convert.ToInt64(GetValue(i));
        }

        public int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            return (string)GetValue(i);
        }

        public int GetValues(object[] values)
        {
            var cnt = 0;
            for (var i = 0; i < FieldCount; i++)
            {
                values[i] = GetValue(i);
                cnt++;
            }

            return cnt;
        }

        public bool IsDBNull(int i)
        {
            return GetValue(i) == null;
        }

        public object this[string name]
        {
            get { throw new NotImplementedException(); }
        }

        public object this[int i]
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
