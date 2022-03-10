using System;
using System.ComponentModel.DataAnnotations;

namespace Kremis.Domain.Entities
{
    public abstract class BaseEntity<TEntityKey>
    {
        [Key]
        public TEntityKey Id { get; set; }
        public DateTime? CreationDate { get; set; }
        public string CreationUser { get; set; }
        public DateTime? LastModificationDate { get; set; }
        public string LastModificationUser { get; set; }
        public bool IsCanceled { get; set; }
        public string CancelationReason { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
