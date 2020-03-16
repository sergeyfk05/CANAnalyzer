using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANAnalyzer.Models.Databases
{
    public class TraceContext : DbContext
    {
        public TraceContext() : base("DefaultConnection")
        {
        }
        public DbSet<TraceModel> Traces { get; set; }
    }
}
