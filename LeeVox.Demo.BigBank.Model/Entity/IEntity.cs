using System;

namespace LeeVox.Demo.BigBank.Model
{
    public interface IEntity
    {
        int Id {get; set;}
        
        DateTime? __Created {get; set;}
        DateTime? __Deleted {get; set;}
        DateTime? __Updated {get; set;}
    }
}
