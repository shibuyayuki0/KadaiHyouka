using System;
using System.Collections.Generic;
using System.Text;

namespace kadai02
{
    public class TaskModel
    {
        public int TASK_ID { get; set; }
        public required string TASK_NAME { get; set; }
        public required string ASSIGNEE { get; set; }
        public DateTime DUE_DATE { get; set; }
        public required string STATUS { get; set; }
        public DateTime CREATE_DATETIME { get; set; }
        public DateTime UPDATE_DATETIME { get; set; }
    }
}
