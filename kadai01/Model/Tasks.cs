namespace kadai01.Model;

public record Tasks
(
    int TASK_ID,
    string TASK_NAME,
    string ASSIGNEE,
    DateTime? DUE_DATE,
    string STATUS,
    DateTime CREATE_DATETIME,
    DateTime UPDATE_DATETIME
);
