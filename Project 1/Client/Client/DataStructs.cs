namespace DataStructs
{
    public class LoginStruct
    {
        public string username { get; set; }
        public string password { get; set; }

        public LoginStruct(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }

    public class EmployeeStruct
    {
        public string username { get; set; }
        public string password { get; set; }
        public int manager_id { get; set; }

        public EmployeeStruct(string username, string password, int manager_id)
        {
            this.username = username;
            this.password = password;
            this.manager_id = manager_id;
        }
    }

    public class TicketStruct
    {
        public int e_id { get; set; }
        public int m_id { get; set; }
        public string type { get; set; }
        public double amount { get; set; }
        public string discription { get; set; }

        public TicketStruct(int e_id, int m_id, string type, double amount, string discription)
        {
            this.e_id = e_id;
            this.m_id = m_id;
            this.type = type;
            this.amount = amount;
            this.discription = discription;
        }
    }
}
