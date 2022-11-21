using System;

namespace Users
{
    public class Manager
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        public Manager() { }
        public Manager(string name, string password)
        {
            this.Name = name;
            this.Password = password;
        }
    }

    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int Manager_ID { get; set; }

        public Employee() { }

        public Employee(string name, string password, int manager_ID)
        {
            this.Name = name;
            this.Password = password;
            this.Manager_ID = manager_ID;
        }
    }
}