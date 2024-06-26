﻿using org.ohdsi.cdm.framework.common.Builder;
using org.ohdsi.cdm.framework.common.Omop;
using System;
using System.Collections.Generic;
using System.Data;

namespace org.ohdsi.cdm.framework.common.DataReaders.v5.v53
{
    public class ConditionOccurrenceDataReader53 : IDataReader
    {
        private readonly IEnumerator<ConditionOccurrence> _enumerator;
        private readonly KeyMasterOffsetManager _offset;

        // A custom DataReader is implemented to prevent the need for the HashSet to be transformed to a DataTable for loading by SqlBulkCopy
        public ConditionOccurrenceDataReader53(List<ConditionOccurrence> batch, KeyMasterOffsetManager o)
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
            //get { return 16; }
            get { return 15; }
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
                    return _enumerator.Current.EndDate;
                case 5:
                    return _enumerator.Current.EndDate;
                case 6:
                    return _enumerator.Current.TypeConceptId;
                case 7:
                    return _enumerator.Current.StopReason;
                case 8:
                    return _enumerator.Current.ProviderId == 0 ? null : _enumerator.Current.ProviderId;
                case 9:
                    if (_enumerator.Current.VisitOccurrenceId.HasValue)
                    {
                        if (_offset.GetKeyOffset(_enumerator.Current.PersonId).VisitOccurrenceIdChanged)
                            return _offset.GetId(_enumerator.Current.PersonId,
                                _enumerator.Current.VisitOccurrenceId.Value);
                        return _enumerator.Current.VisitOccurrenceId.Value;
                    }

                    return null;
                case 10:
                    if (_enumerator.Current.VisitDetailId.HasValue)
                    {
                        if (_offset.GetKeyOffset(_enumerator.Current.PersonId).VisitDetailIdChanged)
                            return _offset.GetId(_enumerator.Current.PersonId,
                                _enumerator.Current.VisitDetailId.Value);
                        return _enumerator.Current.VisitDetailId;
                    }

                    return null;
                case 11:
                    //return _enumerator.Current.StatusConceptId;
                    return null;
                case 12:
                    return _enumerator.Current.SourceValue;
                case 13:
                    return _enumerator.Current.SourceConceptId;
                case 14:
                    return _enumerator.Current.StatusSourceValue;
                default:
                    throw new NotImplementedException();
            }
        }

        public string GetName(int i)
        {
            switch (i)
            {
                //case 0: return "condition_occurrence_id";
                case 0: return "person_id";
                case 1: return "condition_concept_id";
                case 2: return "condition_start_date";
                case 3: return "condition_start_datetime";
                case 4: return "condition_end_date";
                case 5: return "condition_end_datetime";
                case 6: return "condition_type_concept_id";
                case 7: return "stop_reason";
                case 8: return "provider_id";
                case 9: return "visit_occurrence_id";
                case 10: return "visit_detail_id";
                case 11: return "condition_status_concept_id";
                case 12: return "condition_source_value";
                case 13: return "condition_source_concept_id";
                case 14: return "condition_status_source_value";
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
                    return typeof(DateTime?);
                case 5:
                    return typeof(DateTime);
                case 6:
                    return typeof(int?);
                case 7:
                    return typeof(string);
                case 8:
                    return typeof(long?);
                case 9:
                    return typeof(long?);
                case 10:
                    return typeof(long?);
                case 11:
                    return typeof(int);
                case 12:
                    return typeof(string);
                case 13:
                    return typeof(int);
                case 14:
                    return typeof(string);
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
