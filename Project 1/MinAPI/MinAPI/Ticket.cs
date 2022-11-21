using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tickets
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public double Amount { get; set; }
        public string Discription { get; set; }
        public DateTime DoC { get; set; }
        public int EmployeeId { get; set; }
        public DateTime DoE { get; set; }
        public int ManagerId { get; set; }
        public bool Pending { get; set; }
        public bool Approved { get; set; }

        public Ticket() { }

        public Ticket(int id, string type, double amount, string discription, DateTime doC, int employeeId, DateTime doE, int managerId, bool pending, bool approved)
        {
            this.Id = id;
            this.Type = type;
            this.Amount = amount;
            this.Discription = discription;
            this.DoC = doC;
            this.EmployeeId = employeeId;
            this.DoE = doE;
            this.ManagerId = managerId;
            this.Pending = pending;
            this.Approved = approved;
        }
    }
}
