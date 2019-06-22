using System.ComponentModel.DataAnnotations;

namespace LeeVox.Demo.BigBank.Model
{
    public class Currency : BaseEntity, IEntity
    {
        [StringLength(5, MinimumLength = 3)]
        public string Name {get; set;}

        public string Description {get; set;}
    }
}
