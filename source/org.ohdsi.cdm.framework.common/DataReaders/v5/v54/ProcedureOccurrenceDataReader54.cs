using org.ohdsi.cdm.framework.common.Builder;
using org.ohdsi.cdm.framework.common.Omop;
using System;
using System.Collections.Generic;
using System.Data;

namespace org.ohdsi.cdm.framework.common.DataReaders.v5.v54
{
    public class ProcedureOccurrenceDataReader54 : IDataReader
    {
        private readonly IEnumerator<ProcedureOccurrence> _enumerator;
        private readonly KeyMasterOffsetManager _offset;

        // A custom DataReader is implemented to prevent the need for the HashSet to be transformed to a DataTable for loading by SqlBulkCopy
        public ProcedureOccurrenceDataReader54(List<ProcedureOccurrence> batch, KeyMasterOffsetManager o)
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
            get { return 15; }
        }

        public object GetValue(int i)
        {
            if (_enumerator.Current == null) return null;

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
                    return _enumerator.Current.TypeConceptId;
                case 5:
                    return _enumerator.Current.ModifierConceptId;
                case 6:
                    return _enumerator.Current.Quantity;
                case 7:
                    return _enumerator.Current.ProviderId == 0 ? null : _enumerator.Current.ProviderId;
                case 8:
                    if (_enumerator.Current.VisitOccurrenceId.HasValue)
                    {
                        if (_offset.GetKeyOffset(_enumerator.Current.PersonId).VisitOccurrenceIdChanged)
                            return _offset.GetId(_enumerator.Current.PersonId,
                                _enumerator.Current.VisitOccurrenceId.Value);

                        return _enumerator.Current.VisitOccurrenceId.Value;
                    }

                    return null;
                case 9:
                    if (_enumerator.Current.VisitDetailId.HasValue)
                    {

                        if (_offset.GetKeyOffset(_enumerator.Current.PersonId).VisitDetailIdChanged)
                            return _offset.GetId(_enumerator.Current.PersonId,
                                _enumerator.Current.VisitDetailId.Value);

                        return _enumerator.Current.VisitDetailId;
                    }

                    return null;
                case 10:
                    return _enumerator.Current.SourceValue;
                case 11:
                    return _enumerator.Current.SourceConceptId;
                case 12:
                    return _enumerator.Current.QualifierSourceValue;
                case 13:
                    return _enumerator.Current.StartDate;           //end date = start date
                case 14:   
                    return _enumerator.Current.StartDate;           //end date = start date

                default:
                    throw new NotImplementedException();
            }
        }

        public string GetName(int i)
        {
            switch (i)
            {
                //case 0: return "procedure_occurrence_id";
                case 0: return "person_id";
                case 1: return "procedure_concept_id";
                case 2: return "procedure_date";
                case 3: return "procedure_datetime";
                case 4: return "procedure_type_concept_id";
                case 5: return "modifier_concept_id";
                case 6: return "quantity";
                case 7: return "provider_id";
                case 8: return "visit_occurrence_id";
                case 9: return "visit_detail_id";
                case 10: return "procedure_source_value";
                case 11: return "procedure_source_concept_id";
                case 12: return "modifier_source_value";
                case 13: return "procedure_end_date";
                case 14: return "procedure_end_datetime";

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
                    return typeof(int?);
                case 5:
                    return typeof(int);
                case 6:
                    return typeof(int?);
                case 7:
                    return typeof(long?);
                case 8:
                    return typeof(long?);
                case 9:
                    return typeof(long?);
                case 10:
                    return typeof(string);
                case 11:
                    return typeof(int);
                case 12:
                    return typeof(string);
                case 13:
                    return typeof(DateTime);
                case 14:
                    return typeof(DateTime);

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
