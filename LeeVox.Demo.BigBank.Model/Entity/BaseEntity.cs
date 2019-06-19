using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace LeeVox.Demo.BigBank.Model
{
    public abstract class BaseEntity : IEntity
    {
        [Key]
        [Column(Order = 0)]
        public int Id {get; set;}
        
        [IgnoreDataMember]
        [Column(Order = int.MaxValue - 2)]
        public DateTime? __Created {get; set;}

        [IgnoreDataMember]
        [Column(Order = int.MaxValue - 1)]
        public DateTime? __Deleted {get; set;}

        [IgnoreDataMember]
        [Column(Order = int.MaxValue)]
        public DateTime? __Updated {get; set;}
    }
}
