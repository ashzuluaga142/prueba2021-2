using System;

namespace Prueba2021_2.Common.Models
{
   public class Todo
    {
        public DateTime createdTime { get; set; }
        public string TaskDescription { get; set; }

        public bool IsCompleted { get; set; }
    }
}
