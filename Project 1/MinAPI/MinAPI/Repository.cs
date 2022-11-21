using System;
using System.Data;
using System.Text;
using Tickets;
using Users;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using System.Data.SqlClient;

namespace Data
{
    public interface IRepository
    {

    }

    public class SqlRepository : IRepository
    {
        public string ConnectionString;

        public SqlRepository() { }
        public SqlRepository(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        //--- Employee Methods ---
        public bool EmployeeExists(string? name)
        {
            if (name == null)
            {
                return false;
            }
            bool found = false;

            using SqlConnection connection = new SqlConnection(this.ConnectionString);
            connection.Open();

            string qry1 = @"SELECT Name FROM Project1.Employees WHERE Name = @name;";

            using SqlCommand cmd1 = new SqlCommand(qry1, connection);

            cmd1.Parameters.AddWithValue("@name", name);

            using SqlDataReader reader = cmd1.ExecuteReader();

            while (reader.Read())
            {
                string n = reader.GetString(0);
                if (n == name)
                {
                    found = true;
                }
            }

            connection.Close();

            return found;
        }

        public Employee? CreateEmployee(string name, string password, int managerId)
        {
            if (EmployeeExists(name))
            {
                return null;
            }
            if (ManagerExists(name))
            {
                return null;
            }

            Employee employee = new Employee(name, password, managerId);
            employee.Id = 0;

            using SqlConnection connection = new SqlConnection(this.ConnectionString);
            connection.Open();

            string qry1 = @"INSERT INTO Project1.Employees (Name, Password, Manager_ID) VALUES (@name, @password, @managerId);";

            using SqlCommand cmd1 = new SqlCommand(qry1, connection);

            cmd1.Parameters.AddWithValue("@name", name);
            cmd1.Parameters.AddWithValue("@password", password);
            cmd1.Parameters.AddWithValue("@managerId", managerId);

            cmd1.ExecuteNonQuery();

            string qry2 = @"SELECT ID FROM Project1.Employees WHERE Name = @name AND Password = @password;";

            using SqlCommand cmd2 = new SqlCommand(qry2, connection);

            cmd2.Parameters.AddWithValue("@name", name);
            cmd2.Parameters.AddWithValue("@password", password);

            using SqlDataReader reader = cmd2.ExecuteReader();

            while (reader.Read())
            {
                int E_id = reader.GetInt32(0);
                employee.Id = E_id;
            }

            connection.Close();


            if (employee.Id == 0)
            {
                return null;
            }
            return employee;
        }

        public Employee? EmployeeLogin(string name, string? password)
        {
            if (!EmployeeExists(name))
            {
                return null;
            }
            else if (password == null)
            {
                Console.WriteLine("LOGIN: FAILED. You did not enter a valid password.");
                return null;
            }

            Employee? employee = null;

            using SqlConnection connection = new SqlConnection(this.ConnectionString);
            connection.Open();

            string qry1 = @"SELECT Name, Password, Manager_ID, ID FROM Project1.Employees WHERE Name = @name AND Password = @password;";

            using SqlCommand cmd1 = new SqlCommand(qry1, connection);

            cmd1.Parameters.AddWithValue("@name", name);
            cmd1.Parameters.AddWithValue("@password", password);

            using SqlDataReader reader = cmd1.ExecuteReader();

            while (reader.Read())
            {
                string E_name = reader.GetString(0);
                string E_password = reader.GetString(1);
                if (E_name == name && E_password == password)
                {
                    int managerId = reader.GetInt32(2);
                    int id = reader.GetInt32(3);
                    employee = new Employee(name, password, managerId);
                    employee.Id = id;
                    connection.Close();
                    return employee;
                }
            }
            connection.Close();

            Console.WriteLine("LOGIN: FAILED. Your password was incorrect.");

            return null;
        }

        public Employee? EmployeeLoginId(int id)
        {
            Employee? employee = null;

            using SqlConnection connection = new SqlConnection(this.ConnectionString);
            connection.Open();

            string qry1 = @"SELECT Name, Password, Manager_ID, ID FROM Project1.Employees WHERE ID = @id;";

            using SqlCommand cmd1 = new SqlCommand(qry1, connection);

            cmd1.Parameters.AddWithValue("@id", id);

            using SqlDataReader reader = cmd1.ExecuteReader();

            while (reader.Read())
            {
                string E_name = reader.GetString(0);
                string E_password = reader.GetString(1);
                int M_id = reader.GetInt32(2);
                int E_id = reader.GetInt32(3);
                if (E_id == id)
                {
                    employee = new Employee(E_name, E_password, M_id);
                    employee.Id = id;
                    connection.Close();
                    return employee;
                }
            }
            connection.Close();

            Console.WriteLine("LOGIN: FAILED. Your password was incorrect.");

            return null;
        }

        public void CreateTicket(string type, double amount, string discription, int E_id, int M_id)
        {
            using SqlConnection connection = new SqlConnection(this.ConnectionString);
            connection.Open();

            string qry1 = @"INSERT INTO Project1.Tickets (Type, Amount, Discription, Employee_ID, Manager_ID) VALUES (@type, @amount, @discription, @e_id, @m_id);";

            using SqlCommand cmd1 = new SqlCommand(qry1, connection);

            cmd1.Parameters.AddWithValue("@type", type);
            cmd1.Parameters.AddWithValue("@amount", amount);
            cmd1.Parameters.AddWithValue("@discription", discription);
            cmd1.Parameters.AddWithValue("@e_id", E_id);
            cmd1.Parameters.AddWithValue("@m_id", M_id);

            cmd1.ExecuteNonQuery();

            connection.Close();
        }

        //--- Manager Methods ---
        public bool ManagerExists(string? name)
        {
            if (name == null)
            {
                return false;
            }
            bool found = false;

            using SqlConnection connection = new SqlConnection(this.ConnectionString);
            connection.Open();

            string qry1 = @"SELECT Name FROM Project1.Managers WHERE Name = @name;";

            using SqlCommand cmd1 = new SqlCommand(qry1, connection);

            cmd1.Parameters.AddWithValue("@name", name);

            using SqlDataReader reader = cmd1.ExecuteReader();

            while (reader.Read())
            {
                string n = reader.GetString(0);
                if (n == name)
                {
                    found = true;
                }
            }

            connection.Close();

            return found;
        }

        public Manager? CreateManager(string name, string password)
        {
            if (ManagerExists(name))
            {
                return null;
            }
            if (EmployeeExists(name))
            {
                return null;
            }

            Manager manager = new Manager(name, password);
            manager.Id = 0;

            using SqlConnection connection = new SqlConnection(this.ConnectionString);
            connection.Open();

            string qry1 = @"INSERT INTO Project1.Managers (Name, Password) VALUES (@name, @password);";

            using SqlCommand cmd1 = new SqlCommand(qry1, connection);

            cmd1.Parameters.AddWithValue("@name", name);
            cmd1.Parameters.AddWithValue("@password", password);

            cmd1.ExecuteNonQuery();

            string qry2 = @"SELECT ID FROM Project1.Managers WHERE Name = @name AND Password = @password;";

            using SqlCommand cmd2 = new SqlCommand(qry2, connection);

            cmd2.Parameters.AddWithValue("@name", name);
            cmd2.Parameters.AddWithValue("@password", password);

            using SqlDataReader reader = cmd2.ExecuteReader();

            while (reader.Read())
            {
                int M_id = reader.GetInt32(0);
                manager.Id = M_id;
            }

            connection.Close();


            if (manager.Id == 0)
            {
                return null;
            }
            return manager;
        }

        public Manager? ManagerLogin(string name, string? password)
        {
            if (!ManagerExists(name))
            {
                return null;
            }
            else if (password == null)
            {
                Console.WriteLine("LOGIN: FAILED. You did not enter a valid password.");
                return null;
            }

            Manager? manager = null;

            using SqlConnection connection = new SqlConnection(this.ConnectionString);
            connection.Open();

            string qry1 = @"SELECT Name, Password, ID FROM Project1.Managers WHERE Name = @name AND Password = @password;";

            using SqlCommand cmd1 = new SqlCommand(qry1, connection);

            cmd1.Parameters.AddWithValue("@name", name);
            cmd1.Parameters.AddWithValue("@password", password);

            using SqlDataReader reader = cmd1.ExecuteReader();

            while (reader.Read())
            {
                string M_name = reader.GetString(0);
                string M_password = reader.GetString(1);
                if (M_name == name && M_password == password)
                {
                    int id = reader.GetInt32(2);
                    manager = new Manager(name, password);
                    manager.Id = id;
                    connection.Close();
                    return manager;
                }
            }
            connection.Close();

            Console.WriteLine("LOGIN: FAILED. Your password was incorrect.");

            return null;
        }

        public Manager? ManagerLoginId(int id)
        {
            Manager? manager = null;

            using SqlConnection connection = new SqlConnection(this.ConnectionString);
            connection.Open();

            string qry1 = @"SELECT Name, Password, ID FROM Project1.Managers WHERE ID = @id;";

            using SqlCommand cmd1 = new SqlCommand(qry1, connection);

            cmd1.Parameters.AddWithValue("@id", id);

            using SqlDataReader reader = cmd1.ExecuteReader();

            while (reader.Read())
            {
                string M_name = reader.GetString(0);
                string M_password = reader.GetString(1);
                int M_id = reader.GetInt32(2);
                if (M_id == id)
                {
                    manager = new Manager(M_name, M_password);
                    manager.Id = id;
                    connection.Close();
                    return manager;
                }
            }
            connection.Close();

            Console.WriteLine("LOGIN: FAILED. Your password was incorrect.");

            return null;
        }
        public List<Employee> GetMyEmployees(int id)
        {
            List<Employee> employees = new List<Employee>();

            using SqlConnection connection = new SqlConnection(this.ConnectionString);
            connection.Open();

            string qry1 = @"SELECT Name, ID FROM Project1.Employees WHERE Manager_ID = @id;";

            using SqlCommand cmd1 = new SqlCommand(qry1, connection);

            cmd1.Parameters.AddWithValue("@id", id);

            using SqlDataReader reader = cmd1.ExecuteReader();

            while (reader.Read())
            {
                string E_name = reader.GetString(0);
                string E_password = "ACCESS_DENIED";
                int E_id = reader.GetInt32(1);
                Employee E_temp = new Employee(E_name, E_password, id);
                E_temp.Id = E_id;
                employees.Add(E_temp);
            }

            connection.Close();

            return employees;
        }

        public void ApproveTicket(int id)
        {
            using SqlConnection connection = new SqlConnection(this.ConnectionString);
            connection.Open();

            string qry1 = @"UPDATE Project1.Tickets SET Pending = 1, Approved = 0 WHERE ID = @id";

            SqlCommand cmd1 = new SqlCommand(qry1, connection);

            cmd1.Parameters.AddWithValue("@id", id);

            cmd1.ExecuteNonQuery();

            connection.Close();
        }

        public void DenyTicket(int id)
        {
            using SqlConnection connection = new SqlConnection(this.ConnectionString);
            connection.Open();

            string qry1 = @"UPDATE Project1.Tickets SET Pending = 1, Approved = 1 WHERE ID = @id";

            SqlCommand cmd1 = new SqlCommand(qry1, connection);

            cmd1.Parameters.AddWithValue("@id", id);

            cmd1.ExecuteNonQuery();

            connection.Close();
        }

        //--- Ticket Methods ---
        public List<Ticket> GetEmployeePendingTickets(int id)
        {
            List<Ticket> tickets = new List<Ticket>();

            using SqlConnection connection = new SqlConnection(this.ConnectionString);
            connection.Open();

            //                     0   1     2       3            4    5            6    7           8        9       
            string qry1 = @"SELECT ID, Type, Amount, Discription, DoC, Employee_ID, DoE, Manager_ID, Pending, Approved FROM Project1.Tickets WHERE Employee_ID = @id AND Pending = 0;";

            using SqlCommand cmd1 = new SqlCommand(qry1, connection);

            cmd1.Parameters.AddWithValue("@id", id);

            using SqlDataReader reader = cmd1.ExecuteReader();

            while (reader.Read())
            {
                int T_id = reader.GetInt32(0);
                string T_type = reader.GetString(1);
                double T_amount = (double) reader.GetDecimal(2);
                string T_discription = reader.GetString(3);
                DateTime T_doc = reader.GetDateTime(4);
                int T_E_id = reader.GetInt32(5);
                DateTime T_doe = reader.GetDateTime(6);
                int T_M_id = reader.GetInt32(7);
                int tmp = reader.GetInt32(8);
                bool T_pending = true;
                if (tmp != 0)
                {
                    T_pending = false;
                }
                tmp = reader.GetInt32(9);
                bool T_approved = true;
                if (tmp != 0)
                {
                    T_approved = false;
                }
                tickets.Add(new(T_id, T_type, T_amount, T_discription, T_doc, T_E_id, T_doe, T_M_id, T_pending, T_approved));
            }

            connection.Close();

            return tickets;
        }

        public List<Ticket> GetEmployeeResolvedTickets(int id)
        {
            List<Ticket> tickets = new List<Ticket>();

            using SqlConnection connection = new SqlConnection(this.ConnectionString);
            connection.Open();

            //                     0   1     2       3            4    5            6    7           8        9       
            string qry1 = @"SELECT ID, Type, Amount, Discription, DoC, Employee_ID, DoE, Manager_ID, Pending, Approved FROM Project1.Tickets WHERE Employee_ID = @id AND Pending = 1;";

            using SqlCommand cmd1 = new SqlCommand(qry1, connection);

            cmd1.Parameters.AddWithValue("@id", id);

            using SqlDataReader reader = cmd1.ExecuteReader();

            while (reader.Read())
            {
                int T_id = reader.GetInt32(0);
                string T_type = reader.GetString(1);
                double T_amount = (double) reader.GetDecimal(2);
                string T_discription = reader.GetString(3);
                DateTime T_doc = reader.GetDateTime(4);
                int T_E_id = reader.GetInt32(5);
                DateTime T_doe = reader.GetDateTime(6);
                int T_M_id = reader.GetInt32(7);
                int tmp = reader.GetInt32(8);
                bool T_pending = true;
                if (tmp != 0)
                {
                    T_pending = false;
                }
                tmp = reader.GetInt32(9);
                bool T_approved = true;
                if (tmp != 0)
                {
                    T_approved = false;
                }
                tickets.Add(new(T_id, T_type, T_amount, T_discription, T_doc, T_E_id, T_doe, T_M_id, T_pending, T_approved));
            }

            connection.Close();

            return tickets;
        }

        public List<Ticket> GetAllEmployeeTickets(int id)
        {
            List<Ticket> tickets = new List<Ticket>();

            using SqlConnection connection = new SqlConnection(this.ConnectionString);
            connection.Open();

            //                     0   1     2       3            4    5            6    7           8        9       
            string qry1 = @"SELECT ID, Type, Amount, Discription, DoC, Employee_ID, DoE, Manager_ID, Pending, Approved FROM Project1.Tickets WHERE Employee_ID = @id;";

            using SqlCommand cmd1 = new SqlCommand(qry1, connection);

            cmd1.Parameters.AddWithValue("@id", id);

            using SqlDataReader reader = cmd1.ExecuteReader();

            while (reader.Read())
            {
                int T_id = reader.GetInt32(0);
                string T_type = reader.GetString(1);
                double T_amount = (double) reader.GetDecimal(2);
                string T_discription = reader.GetString(3);
                DateTime T_doc = reader.GetDateTime(4);
                int T_E_id = reader.GetInt32(5);
                DateTime T_doe = reader.GetDateTime(6);
                int T_M_id = reader.GetInt32(7);
                int tmp = reader.GetInt32(8);
                bool T_pending = true;
                if (tmp != 0)
                {
                    T_pending = false;
                }
                tmp = reader.GetInt32(9);
                bool T_approved = true;
                if (tmp != 0)
                {
                    T_approved = false;
                }
                tickets.Add(new(T_id, T_type, T_amount, T_discription, T_doc, T_E_id, T_doe, T_M_id, T_pending, T_approved));
            }

            connection.Close();

            return tickets;
        }

        public List<Ticket> GetManagerPendingTickets(int id)
        {
            List<Ticket> tickets = new List<Ticket>();

            using SqlConnection connection = new SqlConnection(this.ConnectionString);
            connection.Open();

            //                     0   1     2       3            4    5            6    7           8        9       
            string qry1 = @"SELECT ID, Type, Amount, Discription, DoC, Employee_ID, DoE, Manager_ID, Pending, Approved FROM Project1.Tickets WHERE Manager_ID = @id AND Pending = 0;";

            using SqlCommand cmd1 = new SqlCommand(qry1, connection);

            cmd1.Parameters.AddWithValue("@id", id);

            using SqlDataReader reader = cmd1.ExecuteReader();

            while (reader.Read())
            {
                int T_id = reader.GetInt32(0);
                string T_type = reader.GetString(1);
                double T_amount = (double) reader.GetDecimal(2);
                string T_discription = reader.GetString(3);
                DateTime T_doc = reader.GetDateTime(4);
                int T_E_id = reader.GetInt32(5);
                DateTime T_doe = reader.GetDateTime(6);
                int T_M_id = reader.GetInt32(7);
                int tmp = reader.GetInt32(8);
                bool T_pending = true;
                if (tmp != 0)
                {
                    T_pending = false;
                }
                tmp = reader.GetInt32(9);
                bool T_approved = true;
                if (tmp != 0)
                {
                    T_approved = false;
                }
                tickets.Add(new(T_id, T_type, T_amount, T_discription, T_doc, T_E_id, T_doe, T_M_id, T_pending, T_approved));
            }

            connection.Close();

            return tickets;
        }

        public List<Ticket> GetManagerResolvedTickets(int id)
        {
            List<Ticket> tickets = new List<Ticket>();

            using SqlConnection connection = new SqlConnection(this.ConnectionString);
            connection.Open();

            //                     0   1     2       3            4    5            6    7           8        9       
            string qry1 = @"SELECT ID, Type, Amount, Discription, DoC, Employee_ID, DoE, Manager_ID, Pending, Approved FROM Project1.Tickets WHERE Manager_ID = @id AND Pending = 1;";

            using SqlCommand cmd1 = new SqlCommand(qry1, connection);

            cmd1.Parameters.AddWithValue("@id", id);

            using SqlDataReader reader = cmd1.ExecuteReader();

            while (reader.Read())
            {
                int T_id = reader.GetInt32(0);
                string T_type = reader.GetString(1);
                double T_amount = reader.GetDouble(2);
                string T_discription = reader.GetString(3);
                DateTime T_doc = reader.GetDateTime(4);
                int T_E_id = reader.GetInt32(5);
                DateTime T_doe = reader.GetDateTime(6);
                int T_M_id = reader.GetInt32(7);
                int tmp = reader.GetInt32(8);
                bool T_pending = true;
                if (tmp != 0)
                {
                    T_pending = false;
                }
                tmp = reader.GetInt32(9);
                bool T_approved = true;
                if (tmp != 0)
                {
                    T_approved = false;
                }
                tickets.Add(new(T_id, T_type, T_amount, T_discription, T_doc, T_E_id, T_doe, T_M_id, T_pending, T_approved));
            }

            connection.Close();

            return tickets;
        }

        public List<Ticket> GetAllManagerTickets(int id)
        {
            List<Ticket> tickets = new List<Ticket>();

            using SqlConnection connection = new SqlConnection(this.ConnectionString);
            connection.Open();

            //                     0   1     2       3            4    5            6    7           8        9       
            string qry1 = @"SELECT ID, Type, Amount, Discription, DoC, Employee_ID, DoE, Manager_ID, Pending, Approved FROM Project1.Tickets WHERE Manager_ID = @id;";

            using SqlCommand cmd1 = new SqlCommand(qry1, connection);

            cmd1.Parameters.AddWithValue("@id", id);

            using SqlDataReader reader = cmd1.ExecuteReader();

            while (reader.Read())
            {
                int T_id = reader.GetInt32(0);
                string T_type = reader.GetString(1);
                double T_amount = reader.GetDouble(2);
                string T_discription = reader.GetString(3);
                DateTime T_doc = reader.GetDateTime(4);
                int T_E_id = reader.GetInt32(5);
                DateTime T_doe = reader.GetDateTime(6);
                int T_M_id = reader.GetInt32(7);
                int tmp = reader.GetInt32(8);
                bool T_pending = true;
                if (tmp != 0)
                {
                    T_pending = false;
                }
                tmp = reader.GetInt32(9);
                bool T_approved = true;
                if (tmp != 0)
                {
                    T_approved = false;
                }
                tickets.Add(new(T_id, T_type, T_amount, T_discription, T_doc, T_E_id, T_doe, T_M_id, T_pending, T_approved));
            }

            connection.Close();

            return tickets;
        }

    }
}

