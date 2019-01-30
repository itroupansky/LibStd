 using System;

namespace LibStd.Common.Audit
{
    public class AuditModel
    {
        public long UserId { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public long? Item_ID { get; set; }
        public string Item_Logged { get; set; }
        public string Description { get; set; }
        public string Data { get; set; }
        public string Prev_Value { get; set; }
        public string New_Value { get; set; }
        public bool? IsEncrypted { get; set; }
        public string EncryptVal { get; set; }
        public string IVSALT { get; set; }
        public long? MainRecordID { get; set; }
        public string MainRecordType { get; set; }
        public string FieldName { get; set; }
        public string Operation { get; set; }
    }
}
