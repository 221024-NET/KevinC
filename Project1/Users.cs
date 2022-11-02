using System;
using Project1;

namespace Project1.Users {
    public class User {
        //Generic Fields for all Users
        public string name;
        public string password;

        //Generic Methods for all Users
        public void SetName(string name) {
            this.name = name;
        }

        public string GetName() {
            return this.name;
        }

        public void SetPassword(string password) {
            this.password = password;
        }

        public string GetPassword() {
            return this.password;
        }

        public bool IsPassword(string password) {
            return this.password == password;
        }
    }

    public class Employee:User {
        //Employees need a Manager
        public Manager manager;

        //Constructor(s)
        public Employee(string name, string password, Manager manager) {
            this.name = name;
            this.password = password;
            this.manager = manager;
        }

        public Employee(string name, string password) {
            this.name = name;
            this.password = password;
            this.manager = null;
        }

    }

    public class Manager:User {
        //A Manager needs Employees
        List<Employee> employees;

        //Constructor(s)
        public Manager(string name, string password, List<Employee> employees) {
            this.name = name;
            this.password = password;
            this.employees = employees;
        }

        public Manager(string name, string password) {
            this.name = name;
            this.password = password;
            this.employees = null;
        }

        //Manager Methods
        public int AddEmployee(Employee e) {
            //Check if employee is not currently in the list
            if (!employees.Contains(e)) {
                //Add the employee
                employees.Add(e);
                return 0;
            }

            //The employee is currently in the list
            return -1;
        }

        public int RemoveEmployee(Employee e) {
            //Check if that employee is in the list
            if (employees.Contains(e)) {
                //Remove that employee
                employees.Remove(e);
            }

            //That employee wasn't in the list
            return -1;
        }
    }
}