using Data;
using Microsoft.AspNetCore.Mvc;
using Users;
using Tickets;
using System;
using System.Security.Cryptography.X509Certificates;
using DataStructs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string connValue = builder.Configuration.GetValue<string>("ConnectionString:ProjectDB");
SqlRepository repo = new SqlRepository(connValue);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/manager/login", (LoginStruct data) =>
{
    Manager? manager = repo.ManagerLogin(data.username, data.password);
    if (manager == null)
    {
        manager = new Manager();
        manager.Id = -1;
        manager.Name = "failed";
        manager.Password = "failed";
    }
    return Results.Created($"/manager/login/{manager.Id}", manager);
});

app.MapPost("/employee/login", (LoginStruct data) =>
{
    Employee? employee = repo.EmployeeLogin(data.username, data.password);
    if (employee == null)
    {
        employee = new Employee();
        employee.Id = -1;
        employee.Name = "failed";
        employee.Password = "failed";
        employee.Manager_ID = -1;
    }
    return Results.Created($"/employee/login/{employee.Id}", employee);
});

app.MapPost("/employee/create", (EmployeeStruct data) =>
{
    Employee? employee = repo.CreateEmployee(data.username, data.password, data.manager_id);
    if (employee == null)
    {
        employee = new Employee();
        employee.Id = -1;
        employee.Name = "failed";
        employee.Password = "failed";
        employee.Manager_ID = -1;
    }
    return Results.Created($"/employee/create/{employee.Id}", employee);
});

app.MapGet("/manager/tickets/pending/{id}", (int id) =>
{
    return repo.GetManagerPendingTickets(id);
});

app.MapGet("/employee/tickets/resolved/{id}", (int id) =>
{
    return repo.GetEmployeeResolvedTickets(id);
});
app.MapGet("/employee/tickets/pending/{id}", (int id) =>
{
    return repo.GetEmployeePendingTickets(id);
});
app.MapGet("/employee/tickets/all/{id}", (int id) =>
{
    return repo.GetAllEmployeeTickets(id);
});

app.MapGet("/manager/name/{id}", (int id) =>
{
    Manager? manager = repo.ManagerLoginId(id);
    if (manager == null)
    {
        manager = new Manager();
        manager.Id = -1;
        manager.Name = "ERROR";
        manager.Password = "ERROR";
    }
    return manager.Name;
});

app.MapGet("/employee/name/{id}", (int id) =>
{
    Employee? employee = repo.EmployeeLoginId(id);
    if (employee == null)
    {
        employee = new Employee();
        employee.Id = -1;
        employee.Name = "ERROR";
        employee.Password = "ERROR";
        employee.Manager_ID = -1;
    }
    return employee.Name;
});

app.MapPost("/employee/tickets/create", (TicketStruct data) =>
{
    repo.CreateTicket(data.type, data.amount, data.discription, data.e_id, data.m_id);
    return Results.Created($"/employee/tickets/create", data.e_id);
});

app.MapPost("/manager/tickets/approve", (Ticket t) =>
{
    repo.ApproveTicket(t.Id);
});

app.MapPost("/manager/tickets/deny", (Ticket t) =>
{
    repo.DenyTicket(t.Id);
});

app.Run();