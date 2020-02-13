using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;

namespace ToDoAppProject
{
    class Program
    {
        static string ConnectionStr = " server = INSTRUCTORIT; database = TODOAPP; user ID = ProfileUser; password = ProfileUser2019";
        static string sqlCommand;
        static int selection;

        static List<Task> Tasks = new List<Task>();

      
        //INTERFACE
        static void mainInterface()
        {

            Console.WriteLine("Select an action : \n" +
                "1. Add a task \n" +
                "2. Check today's tasks \n" +
                "3. Check Next 7 Days tasks \n" +
                "4. Check tasks without deadline\n" +
                "5. Check tasks done\n" +
                "6. Close");
            selection = int.Parse(Console.ReadLine());
           

        }


        static void Main(string[] args)
        {
            bool keepRun = true;

            
            //LOAD ALL TASKS
            using (SqlConnection myConection = new SqlConnection(ConnectionStr))
            {
                myConection.Open();
                sqlCommand = "SELECT * FROM TASKS";
                SqlCommand myCommand = new SqlCommand(sqlCommand, myConection);
                SqlDataReader myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    Task readingTasks = new Task();
                    readingTasks.title = myReader["Title"].ToString();
                    readingTasks.description = myReader["Description"].ToString();
                    readingTasks.dueDate = Convert.ToDateTime(myReader["DueDate"]);
                    Tasks.Add(readingTasks);
                }
                myReader.Close();
                myConection.Close();
 

                Console.WriteLine("To-Do LIST: \n");
                mainInterface();
                

                while (keepRun)
                {
                    if (selection == 0 || selection > 6)
                    {
                        Console.WriteLine("Please select an option between 1 to 5: ");
                        mainInterface();
                    }
                    //ADD A TASK
                    if (selection == 1)
                    {
                        Task addingTask = new Task();
                        string sqlInsertCommand;
                        try
                        {
                            Console.WriteLine("Insert a Title : ");
                            addingTask.title = Console.ReadLine();
                            Console.WriteLine("Description:  ");
                            addingTask.description = Console.ReadLine();
                            Console.WriteLine("Due date (YYYY-DD-MM): ");
                            addingTask.dueDate = Convert.ToDateTime(Console.ReadLine());
                            Tasks.Add(addingTask);

                            using (SqlConnection myConn = new SqlConnection(ConnectionStr))
                            {
                                myConn.Open();
                                sqlInsertCommand = "INSERT INTO TASKS" + "(Title, Description, DueDate) VALUES ('" + addingTask.title + "','" + addingTask.description + "','" + addingTask.dueDate + "');";
                                SqlCommand myInsertCommand = new SqlCommand(sqlInsertCommand, myConn);
                                myInsertCommand.ExecuteNonQuery();
                                myConn.Close();

                                Console.WriteLine("Add new task sucessful");
                                mainInterface();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }

                    if (selection == 2)
                    {
                        
                        try
                        {
                            DateTime dateToday = DateTime.Now;

                            int i;
                            List<Task> todayTask = new List<Task>();
                            Task tempTask = new Task();
                            string sqlTodayCommand;
                            using (SqlConnection myConnection = new SqlConnection(ConnectionStr))//connection with database
                            {
                                myConnection.Open();
                                sqlTodayCommand = "SELECT * FROM TASKS";//sql command
                                SqlCommand myTodayCommand = new SqlCommand(sqlTodayCommand, myConnection);//connection with sql 
                                SqlDataReader myTodayReader = myTodayCommand.ExecuteReader();//to select data

                                while (myTodayReader.Read())//while read, the program reads as an array
                                {
                                    Task readingTask = new Task();
                                    readingTask.title = myTodayReader["Title"].ToString();
                                    readingTask.description = myTodayReader["Description"].ToString();
                                    readingTask.dueDate = Convert.ToDateTime(myTodayReader["DueDate"]);

                                    if (readingTask.dueDate.Day == dateToday.Day)
                                    {
                                        todayTask.Add(readingTask);
                                    }

                                }
                                for (i = 0; i < todayTask.Count; i++)
                                {
                                    tempTask = todayTask[i];
                                    Console.WriteLine("Title:\n" + tempTask.title + "\n Description:\n" + tempTask.description + "\n Due Date:\n" + tempTask.dueDate +"\n --------------------------");
                                }

                                myReader.Close();
                                myConnection.Close();
                                mainInterface();
                            }
                      



                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                    }

                    if (selection == 3)
                    {

                        try
                        {
                            int j;
                            Task tempTask = new Task();

                            List<Task> tasks7days = new List<Task>();


                            var query = from nextTasks in Tasks
                                        where (nextTasks.dueDate.Day > DateTime.Now.Day) && (nextTasks.dueDate.Day < (DateTime.Now.Day + 7))
                                        select nextTasks;
                            tasks7days = query.ToList<Task>();


                            for (j = 0; j < tasks7days.Count; j++)
                            {
                                tempTask = tasks7days[j];
                                Console.WriteLine("Title:\n" + tempTask.title + "\n Description:\n" + tempTask.description + "\n Due Date:\n" + tempTask.dueDate+"\n --------------------------");
                            }
                            mainInterface();

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    if (selection == 4)
                    {
                        try
                        {
                            int k;
                            Task tempTask = new Task();

                            List<Task> tasksNoDate = new List<Task>();


                            var query = from nextTasks in Tasks
                                        where nextTasks.dueDate == null
                                        select nextTasks;
                            tasksNoDate = query.ToList<Task>();


                            for (k = 0; k < tasksNoDate.Count; k++)
                            {
                                tempTask = tasksNoDate[k];
                                Console.WriteLine("Title:\n" + tempTask.title + "\n Description:\n" + tempTask.description + "\n Due Date:\n" + tempTask.dueDate+ "\n --------------------------");
                            }
                            mainInterface();


                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }


                    }
                    

                    //TASK DONE
                    if (selection == 5)
                    {
                        int l;
                        Task tempTask = new Task();
                        string doneTitle;
                        string sqlDeleteCommand;
                        List<Task> tasksDone = new List<Task>();
                        Console.WriteLine("Title of task done: \n");
                        doneTitle = Console.ReadLine();
                        var query = from doneTasks in Tasks
                                    where doneTasks.dueDate.Day == DateTime.Now.Day && doneTasks.title == doneTitle
                                    select doneTasks;
                        tasksDone = query.ToList<Task>();
                       
                        for (l = 0; l < tasksDone.Count; l++)
                        {
                            tempTask = tasksDone[l];
                            if (tempTask.title == doneTitle)
                            {
                                Tasks.Remove(tempTask);
                                using (SqlConnection myConn = new SqlConnection(ConnectionStr))
                                {
                                    myConn.Open();
                                    sqlDeleteCommand = "DELETE FROM TASKS WHERE Title = '" + tempTask.title + "' AND DueDate ='" + tempTask.dueDate + "';";
                                    SqlCommand myDeleteCommand = new SqlCommand(sqlDeleteCommand, myConn);
                                    myDeleteCommand.ExecuteNonQuery();
                                    myConn.Close();

                                }
                                Console.WriteLine("Task with title=" + tempTask.title + " and Due date = " + tempTask.dueDate + " is done and removed!");
                            }
                        }
                        mainInterface();
                    }
                    if (selection == 6)
                        keepRun = false;



                }

                
            }
        }
    }
}
