using System;
using Project1;

namespace Project1.Tickets {
    public class Ticket {
        //Ticket Fields
        bool status; //Pending = false, Resolved = true;
        bool approved;
        string discription;
        double amount;

        //Constructor
        public Ticket(string discription, double amount) {
            this.status = false;
            this.approved = false;
            this.discription = discription;
            this.amount = amount;
        }

        //Methods
    }
}