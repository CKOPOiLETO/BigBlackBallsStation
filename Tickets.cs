//------------------------------------------------------------------------------
// <auto-generated>
//    Этот код был создан из шаблона.
//
//    Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//    Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BigBusStation
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tickets
    {
        public Tickets()
        {
            this.Payments = new HashSet<Payments>();
        }
    
        public int Id { get; set; }
        public int ScheduleID { get; set; }
        public int PassengerID { get; set; }
        public System.DateTime PurchaseDate { get; set; }
        public decimal Price { get; set; }
    
        public virtual Passangers Passangers { get; set; }
        public virtual ICollection<Payments> Payments { get; set; }
        public virtual Schedules Schedules { get; set; }
    }
}
